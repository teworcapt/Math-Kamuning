using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;

public class TowerMoving : MonoBehaviour
{
    [Header("Separate Containers")]
    [SerializeField] private RectTransform stairsContainer;
    [SerializeField] private RectTransform railsContainer;

    [Header("Prefabs & Initial Count")]
    [SerializeField] private GameObject stairPrefab;
    [SerializeField] private GameObject railPrefab;
    [SerializeField] private int initialCount = 12;

    [Header("Spacing & Speed")]
    [SerializeField] private float stepSpacing = 100f;
    [SerializeField] private float moveDuration = 0.4f;

    private List<RectTransform> _stairs = new List<RectTransform>();
    private List<RectTransform> _rails = new List<RectTransform>();

    private void Start()
    {
        // Spawn initial stack
        float y = 0f;
        for (int i = 0; i < initialCount; i++)
        {
            _stairs.Add(Spawn(stairPrefab, stairsContainer, y));
            _rails.Add(Spawn(railPrefab, railsContainer, y));
            y += stepSpacing;
        }
    }

    private RectTransform Spawn(GameObject prefab, RectTransform parent, float y)
    {
        var rt = Instantiate(prefab, parent).GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, y);
        return rt;
    }

    /// <summary>
    /// Moves all existing stairs/rails down by one stepSpacing, then spawns one new pair at the new top.
    /// </summary>
    public void MoveUp(Action onComplete = null)
    {
        // 1) Animate everyone down
        var seq = DOTween.Sequence();
        foreach (var stair in _stairs)
            seq.Join(stair.DOAnchorPosY(stair.anchoredPosition.y - stepSpacing, moveDuration).SetEase(Ease.OutQuad));

        foreach (var rail in _rails)
            seq.Join(rail.DOAnchorPosY(rail.anchoredPosition.y - stepSpacing, moveDuration).SetEase(Ease.OutQuad));

        // 2) After movement, spawn a new top stair+rail
        seq.OnComplete(() =>
        {
            SpawnNextAtTop();
            onComplete?.Invoke();
        });
    }

    private void SpawnNextAtTop()
    {
        // find the current maximum Y among stairs
        float maxY = _stairs.Max(s => s.anchoredPosition.y);

        // new position is maxY + stepSpacing
        float newY = maxY + stepSpacing;

        // spawn and add to lists
        _stairs.Add(Spawn(stairPrefab, stairsContainer, newY));
        _rails.Add(Spawn(railPrefab, railsContainer, newY));

        // Remove oldest (bottom) if you only want a fixed running window
        var oldStair = _stairs[0];
        Destroy(oldStair.gameObject);
        _stairs.RemoveAt(0);

        var oldRail = _rails[0];
        Destroy(oldRail.gameObject);
        _rails.RemoveAt(0);

    }
}
