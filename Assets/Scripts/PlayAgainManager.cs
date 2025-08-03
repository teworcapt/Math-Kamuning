using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayAgainManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button playAgainButton;
    [SerializeField] private CalculatingScoreTimerManager gameManager;
    [SerializeField] private MovementSpriteManager catManager;
    [SerializeField] private TowerMoving tower;

    private void Start()
    {
        playAgainButton.onClick.AddListener(PlayAgain);
    }

    private void PlayAgain()
    {
        gameManager.HideGameOverPanel();
        gameManager.ResetScore();
        gameManager.ResetTimer();
        catManager.ResetPosition();
        tower.ResetTower();
        gameManager.EnableQuestionCycle();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

}
