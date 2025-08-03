using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class GameOverZone2D : MonoBehaviour
{
    [Tooltip("Drag your CalculatingScoreTimerManager here")]
    public CalculatingScoreTimerManager gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.TriggerGameOver();
        }
    }
}
