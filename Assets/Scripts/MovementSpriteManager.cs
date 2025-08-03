using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MovementSpriteManager : MonoBehaviour
{
    public enum CatState { Idle, Moving }

    [Header("Cat Image & Sprites")]
    [SerializeField] private Image _catImage;
    [SerializeField] private Sprite[] _idleSprites;
    [SerializeField] private Sprite _jumpLeftSprite;
    [SerializeField] private Sprite _jumpRightSprite;

    [Header("Idle Animation")]
    [SerializeField] private float _idleFrameDelay = 0.5f;

    [Header("Grid Settings")]
    [Tooltip("AnchoredPosition of bottom-left cell")]
    [SerializeField] private Vector2 gridOrigin = Vector2.zero;
    [Tooltip("Cell width in UI units")]
    [SerializeField] private float cellWidth = 100f;
    [Tooltip("Cell height in UI units")]
    [SerializeField] private float cellHeight = 100f;
    [Tooltip("Tween duration for jumps")]
    [SerializeField] private float moveDuration = 0.3f;

    [SerializeField] private RectTransform _catTransform;

    private Coroutine _idleRoutine;
    private CatState _state = CatState.Idle;

    private int _gridX = 0;
    private int _gridY = 0;
    private int _lastDir = 0; 

    private void Awake()
    {
 
        _gridX = 0;
        _gridY = 0;
        _lastDir = 0;
        _catTransform.anchoredPosition = gridOrigin;
        StartIdle();
    }

    public void Jump(int dir, TweenCallback onComplete)
    {
        if (_state != CatState.Idle) return;
        StopIdle();
        _state = CatState.Moving;

 
        _catImage.sprite = dir < 0 ? _jumpLeftSprite : _jumpRightSprite;

        // movement logic:
        if (dir > 0)
        {
            // goes right diagonally
            _gridX += 1;
            _gridY += 1;
        }
        else // dir < 0
        {
            // goes left diagonally
            if (_lastDir > 0)
            {
                _gridX -= 1;
                _gridY += 1;
            }
            else
            { // goes upwards
                _gridY += 1;
            }
        }

        _gridX = Mathf.Clamp(_gridX, 0, 1);

        Vector2 target = gridOrigin
                       + new Vector2(_gridX * cellWidth, _gridY * cellHeight);

        _catTransform
            .DOAnchorPos(target, moveDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _state = CatState.Idle;
                StartIdle();
                _lastDir = dir;
                onComplete?.Invoke();
            });
    }

    public void Fall(TweenCallback onComplete)
    {
        if (_state != CatState.Idle) return;
        StopIdle();
        _state = CatState.Moving;

        _gridY = Mathf.Max(0, _gridY - 1);
        _catImage.sprite = _jumpLeftSprite;

        Vector2 target = gridOrigin
                       + new Vector2(_gridX * cellWidth, _gridY * cellHeight);

        _catTransform
            .DOAnchorPos(target, moveDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                _state = CatState.Idle;
                StartIdle();
                onComplete?.Invoke();
            });
    }

    public void ResetPosition()
    {
        StopIdle();
        _gridX = 0;
        _gridY = 0;
        _lastDir = 0;
        _state = CatState.Idle;
        _catTransform.anchoredPosition = gridOrigin;
        StartIdle();
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

    private System.Collections.IEnumerator IdleCycle()
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
