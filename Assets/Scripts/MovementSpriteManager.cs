using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

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
    [Tooltip("Horizontal offset per jump")]
    [SerializeField] private float _xOffset = 100f;
    [Tooltip("Vertical offset per jump")]
    [SerializeField] private float _yOffset = 100f;
    [SerializeField] private float _jumpDuration = 0.3f;

    [SerializeField] private Transform _catTransform;

    private CatState _state = CatState.Idle;
    private Coroutine _idleRoutine;
    private Vector3 _pos;
    private int _lastDir = 0;

    private void Awake()
    {
        _pos = _catTransform.position;
        StartIdle();
    }

    /// <summary>
    /// Triggers a jump in the given direction (-1=left, +1=right).
    /// Diagonal if changing direction, straight up if same direction twice.
    /// </summary>
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
