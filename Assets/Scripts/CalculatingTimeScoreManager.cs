using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class CalculatingScoreTimerManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI equationText;
    [SerializeField] private Button leftBtn;
    [SerializeField] private TextMeshProUGUI leftTxt;
    [SerializeField] private Button rightBtn;
    [SerializeField] private TextMeshProUGUI rightTxt;
    [SerializeField] private TextMeshProUGUI questionTimerTxt;
    [SerializeField] private TextMeshProUGUI roundTimerTxt;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;


    [Header("Timers")]
    [Tooltip("Seconds per question")]
    [SerializeField] private float timePerQuestion = 5f;
    [Tooltip("Total round duration in seconds")]
    [SerializeField] private float roundDuration = 60f;

    [Header("Movement References")]
    [SerializeField] private TowerMoving _tower;
    [SerializeField] private MovementSpriteManager _cat;

    private int _answer;
    private float _qTimer;
    private float _rTimer;
    private bool _locked;
    private int _score = 0;

    private void Start()
    {
        leftBtn.onClick.AddListener(() => OnAnswered(-1));
        rightBtn.onClick.AddListener(() => OnAnswered(1));

        gameOverPanel.SetActive(false);
        _score = 0;
        UpdateScoreUI();

        // Start timers and first question
        _rTimer = roundDuration;
        StartNextQuestion();
    }

    private void Update()
    {
        if (_locked) return;

        _qTimer -= Time.deltaTime;
        _rTimer -= Time.deltaTime;

        questionTimerTxt.text = Mathf.CeilToInt(_qTimer).ToString();
        roundTimerTxt.text = Mathf.CeilToInt(_rTimer).ToString();

        if (_qTimer <= 0f)
        {
            // question time expired: new question
            StartNextQuestion();
        }

        if (_rTimer <= 0f)
        {
            // round over > game over
            TriggerGameOver();
        }
    }

    private void OnAnswered(int dir)
    {
        if (_locked) return;
        _locked = true;

        leftBtn.interactable = false;
        rightBtn.interactable = false;

        bool correct = (dir < 0 && int.Parse(leftTxt.text) == _answer)
                    || (dir > 0 && int.Parse(rightTxt.text) == _answer);

        if (correct)
        {
            _score++;
            UpdateScoreUI();

            _cat.Jump(dir, () =>
            {
                DOVirtual.DelayedCall(0.5f, () =>
                    _tower.PanDown(StartNextQuestion)
                );
            });
        }
        else
        {
            _cat.Fall(() =>
            {
                DOVirtual.DelayedCall(0.5f, StartNextQuestion);
            });
        }
    }

    private void StartNextQuestion()
    {
        if (_rTimer <= 0f)
        {
            TriggerGameOver();
            return;
        }

        _locked = false;
        leftBtn.interactable = true;
        rightBtn.interactable = true;

        // reset question timer
        _qTimer = timePerQuestion;

        GenerateQuestion();
    }

    private void GenerateQuestion()
    {
        int a = Random.Range(1, 15);
        int b = Random.Range(1, 15);

        string op;
        switch (Random.Range(0, 3))
        {
            case 0:
                _answer = a + b;
                op = "+";
                break;
            case 1:
                _answer = a - b;
                op = "-";
                break;
            case 2:
                _answer = a * b;
                op = "×";
                break;
            default:
                _answer = a + b;
                op = "+";
                break;
        }

        equationText.text = $"{a} {op} {b}";

        int delta = Random.Range(1, 4);
        int wrong = _answer + (Random.value > 0.5f ? delta : -delta);

        if (Random.value > 0.5f)
        {
            leftTxt.text = _answer.ToString();
            rightTxt.text = wrong.ToString();
        }
        else
        {
            leftTxt.text = wrong.ToString();
            rightTxt.text = _answer.ToString();
        }
    }

    public void TriggerGameOver()
    {
        _locked = true;
        gameOverPanel.SetActive(true);
        equationText.text = "Game Over!";

        if (finalScoreText != null)
            finalScoreText.text = $"Final Score: {_score}";
    }


    private void UpdateScoreUI()
    {
        scoreText.text = $"Score: {_score}";
    }

    public void ResetScore()
    {
        _score = 0;
        UpdateScoreUI();
    }

    public void ResetTimer()
    {
        _qTimer = timePerQuestion;
        _rTimer = roundDuration;
        questionTimerTxt.text = Mathf.CeilToInt(_qTimer).ToString();
        roundTimerTxt.text = Mathf.CeilToInt(_rTimer).ToString();
    }

    public void EnableQuestionCycle()
    {
        _locked = false;
        leftBtn.interactable = true;
        rightBtn.interactable = true;
        StartNextQuestion();
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false);

        if (finalScoreText != null)
            finalScoreText.text = "";
    }


}
