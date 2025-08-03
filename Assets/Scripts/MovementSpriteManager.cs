using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovementSpriteManager : MonoBehaviour
{
    public enum CatState { Idle, JumpLeft, JumpRight }

    [Header("Cat Image & Sprites")]
    [SerializeField] private Image _catImage;
    [SerializeField] private Sprite[] _idleSprites;
    [SerializeField] private Sprite _jumpLeftSprite;
    [SerializeField] private Sprite _jumpRightSprite;

    [Header("Idle Animation")]
    [SerializeField] private float _idleFrameDelay = 0.5f;

    [Header("Jump Settings")]
    [SerializeField] private float _xOffset = 100f;
    [SerializeField] private float _yOffset = 100f;
    [SerializeField] private float _jumpDuration = 0.3f;

    [Header("Fall Settings")]
    [SerializeField] private float _fallDistance = 150f;

    [SerializeField] private Transform _catTransform;

    private CatState _state = CatState.Idle;
    private Coroutine _idleRoutine;
    private Vector3 _pos;
    private int _lastDir = 0;

    public float CurrentY => _catTransform.position.y;

    private void Awake()
    {
        _pos = _catTransform.position;
        StartIdle();
    }

    public void Jump(int dir, TweenCallback onComplete)
    {
        if (_state != CatState.Idle) return;
        StopIdle();

        _state = dir < 0 ? CatState.JumpLeft : CatState.JumpRight;
        _catImage.sprite = dir < 0 ? _jumpLeftSprite : _jumpRightSprite;

        float horiz = (dir == _lastDir && _lastDir != 0) ? 0f : _xOffset * dir;
        Vector3 target = _pos + new Vector3(horiz, _yOffset, 0f);

        _catTransform
            .DOMove(target, _jumpDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _pos = target;
                _state = CatState.Idle;
                StartIdle();
                onComplete?.Invoke();
            });

        _lastDir = dir;
    }

    public void Fall(TweenCallback onComplete)
    {
        if (_state != CatState.Idle) return;
        StopIdle();

        _catImage.sprite = _jumpLeftSprite; // or a dedicated fall sprite

        Vector3 fallTarget = _catTransform.position + Vector3.down * _fallDistance;

        _catTransform
            .DOMove(fallTarget, _jumpDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                _pos = fallTarget;
                _state = CatState.Idle;
                StartIdle();
                onComplete?.Invoke();
            });
    }

    private void StartIdle()
    {
        if (_idleSprites.Length == 0 || _idleRoutine != null) return;
        _idleRoutine = StartCoroutine(IdleCycle());
    }

    private void StopIdle()
    {
        if (_idleRoutine == null) return;
        StopCoroutine(_idleRoutine);
        _idleRoutine = null;
    }

    private IEnumerator IdleCycle()
    {
        int i = 0;
        while (true)
        {
            _catImage.sprite = _idleSprites[i];
            i = (i + 1) % _idleSprites.Length;
            yield return new WaitForSeconds(_idleFrameDelay);
        }
    }
}
