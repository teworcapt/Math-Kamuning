using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class TowerMoving : MonoBehaviour
{
    [Header("Tower Container (children all move together)")]
    [SerializeField] private RectTransform towerContainer;

    [Header("Spawn Sub‐Containers")]
    [SerializeField] private RectTransform stairsContainer;
    [SerializeField] private RectTransform railsContainer;

    [Header("Prefabs & Initial Count")]
    [SerializeField] private GameObject stairPrefab;
    [SerializeField] private GameObject railPrefab;
    [SerializeField] private int initialCount = 12;

    [Header("Grid & Pan Settings")]
    [Tooltip("The same cellHeight used by the cat")]
    [SerializeField] private float cellHeight = 100f;
    [Tooltip("How long the pan takes")]
    [SerializeField] private float panDuration = 0.4f;

    private float _nextSpawnY;

    private void Start()
    {
        float y = 0f;
        for (int i = 0; i < initialCount; i++)
        {
            Spawn(stairPrefab, stairsContainer, y);
            Spawn(railPrefab, railsContainer, y);
            y += cellHeight;
        }
        _nextSpawnY = y;
    }

    private void Spawn(GameObject prefab, RectTransform parent, float y)
    {
        var rt = Instantiate(prefab, parent).GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, y);
    }
    
    public void PanDown(Action onComplete = null)
    {
        Vector2 down = Vector2.down * cellHeight;
        towerContainer
            .DOAnchorPos(towerContainer.anchoredPosition + down, panDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {

                Spawn(stairPrefab, stairsContainer, _nextSpawnY);
                Spawn(railPrefab, railsContainer, _nextSpawnY);
                _nextSpawnY += cellHeight;

                onComplete?.Invoke();
            });
    }

    public void ResetTower()
    {
        foreach (Transform child in stairsContainer)
            Destroy(child.gameObject);
        foreach (Transform child in railsContainer)
            Destroy(child.gameObject);

        towerContainer.anchoredPosition = Vector2.zero;

        float y = 0f;
        for (int i = 0; i < initialCount; i++)
        {
            Spawn(stairPrefab, stairsContainer, y);
            Spawn(railPrefab, railsContainer, y);
            y += cellHeight;
        }
        _nextSpawnY = y;
    }

}
