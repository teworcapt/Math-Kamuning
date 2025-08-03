using UnityEngine;
using TMPro;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI feedbackText;
    public GameObject finalPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalTimerText;

    private void Start()
    {
        feedbackText.gameObject.SetActive(false);
        finalPanel.SetActive(false);
    }

    public void ShowQuestion(string question)
    {
        questionText.text = question;
    }

    public void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.transform.localScale = Vector3.zero;
        feedbackText.gameObject.SetActive(true);

        // Animate "pop" effect
        feedbackText.transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.OutBack);
        feedbackText.DOFade(0f, 1f)
            .SetDelay(1f)
            .OnComplete(() => {
                feedbackText.gameObject.SetActive(false);
                feedbackText.color = new Color(color.r, color.g, color.b, 1f); // reset alpha
            });
    }

    public void ShowFinalScore(int score, float timer)
    {
        finalPanel.SetActive(true);
        finalScoreText.text = $"Score: {score}";
        finalTimerText.text = $"Time Left: {timer:F1}s";
    }
}
