using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Audio Clips")]
    public AudioClip mainMenuClip;
    public AudioClip gameplayClip;
    public AudioClip buttonClickClip;

    private void Start()
    {
        if (AudioManager.Instance != null && mainMenuClip != null)
        {
            AudioManager.Instance.PlayMusic(mainMenuClip);
        }
    }

    public void PlayGame()
    {
        if (AudioManager.Instance != null)
        {
            if (gameplayClip != null)
                AudioManager.Instance.PlayMusic(gameplayClip);

            if (buttonClickClip != null)
                AudioManager.Instance.PlaySFX(buttonClickClip);
        }

        SceneManager.LoadScene("Gameplay");
    }

    public void QuitGame()
    {
        if (AudioManager.Instance != null && buttonClickClip != null)
        {
            AudioManager.Instance.PlaySFX(buttonClickClip);
        }

        Debug.Log("Quit Game");
        Application.Quit();
    }
}
