using UnityEngine;
using DG.Tweening;
using System;

public class TowerMoving : MonoBehaviour
{
    [Header("Tower Container (children move together)")]
    [SerializeField] private RectTransform towerContainer;

    [Header("Spawn Sub‐Containers")]
    [SerializeField] private RectTransform stairsContainer;
    [SerializeField] private RectTransform railsContainer;

    [Header("Prefabs & Initial Count")]
    [SerializeField] private GameObject stairPrefab;
    [SerializeField] private GameObject railPrefab;
    [SerializeField] private int initialCount = 12;

    [Header("Pan Settings")]
    [Tooltip("Vertical distance to pan on each correct answer")]
    [SerializeField] private float panDistance = 100f;
    [Tooltip("Duration of the pan animation in seconds")]
    [SerializeField] private float panDuration = 0.4f;

    private float _nextSpawnY;

    private void Start()
    {
        // build initial stack
        float y = 0f;
        for (int i = 0; i < initialCount; i++)
        {
            Spawn(stairPrefab, stairsContainer, y);
            Spawn(railPrefab, railsContainer, y);
            y += panDistance;
        }
        _nextSpawnY = y;
    }

    private void Spawn(GameObject prefab, RectTransform parent, float y)
    {
        var rt = Instantiate(prefab, parent).GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(0, y);
    }

    /// <summary>
    /// Pan the entire towerContainer down by panDistance, then spawn a new stair+rail at the top.
    /// </summary>
    public void PanDown(Action onComplete = null)
    {
        Vector2 down = Vector2.down * panDistance;
        towerContainer
            .DOAnchorPos(towerContainer.anchoredPosition + down, panDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {

                // spawn new at the next Y
                Spawn(stairPrefab, stairsContainer, _nextSpawnY);
                Spawn(railPrefab, railsContainer, _nextSpawnY);
                _nextSpawnY += panDistance;

                onComplete?.Invoke();
            });
    }
}
