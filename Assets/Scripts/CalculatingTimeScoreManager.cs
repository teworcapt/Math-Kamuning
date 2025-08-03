using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;

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

    [Header("Score UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    private int _score = 0;

    [Header("Timing Settings")]
    [SerializeField] private float timePerQuestion = 5f;
    [SerializeField] private float roundDuration = 60f;

    [Header("Movement References")]
    [SerializeField] private TowerMoving _tower;
    [SerializeField] private MovementSpriteManager _cat;

    private int _answer;
    private float _qTimer, _rTimer;
    private bool _locked;
    private int _pending;

    private void Start()
    {
        leftBtn.onClick.AddListener(() => OnAnswered(-1));
        rightBtn.onClick.AddListener(() => OnAnswered(1));

        _score = 0;
        UpdateScoreUI();

        _rTimer = roundDuration;
        NextQuestion();
    }

    private void Update()
    {
        if (_locked) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) OnAnswered(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) OnAnswered(1);

        _qTimer -= Time.deltaTime;
        _rTimer -= Time.deltaTime;

        questionTimerTxt.text = Mathf.CeilToInt(_qTimer).ToString();
        roundTimerTxt.text = Mathf.CeilToInt(_rTimer).ToString();

        if (_qTimer <= 0f) NextQuestion();           // just skip to next
        if (_rTimer <= 0f) EndRound();
    }

    private void NextQuestion()
    {
        _locked = false;
        leftBtn.interactable = true;
        rightBtn.interactable = true;

        _qTimer = timePerQuestion;

        // Generate question (with optional bit-shift)
        int a = UnityEngine.Random.Range(1, 11);
        int b = UnityEngine.Random.Range(1, 11);
        if (UnityEngine.Random.value > 0.5f)
        {
            int s = UnityEngine.Random.Range(1, 4);
            _answer = (a << s) + b;
            equationText.text = $"{a} << {s} + {b}";
        }
        else
        {
            _answer = a + b;
            equationText.text = $"{a} + {b}";
        }

        // Wrong answer
        int delta = UnityEngine.Random.Range(1, 5);
        int wrong = _answer + ((UnityEngine.Random.value > 0.5f) ? delta : -delta);

        // Randomize placement
        if (UnityEngine.Random.value > 0.5f)
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
        }

        // We always do exactly two animations: cat jump + tower scroll
        _pending = 2;

        _cat.Jump(dir, () => {
            OnAnimationComplete();         // cat done
            _tower.MoveUp(OnAnimationComplete); // then tower done
        });
    }

    private void OnAnimationComplete()
    {
        if (--_pending > 0) return;

        if (_rTimer > 0f)
            NextQuestion();
        else
            EndRound();

        _locked = false;
    }

    private void EndRound()
    {
        _locked = true;
        equationText.text = "Game Over!";
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {_score}";
    }
}
