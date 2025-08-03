using TMPro;
using UnityEngine;

public class StairAnswerDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI leftText;
    [SerializeField] private TextMeshProUGUI rightText;

    public int LeftValue { get; private set; }
    public int RightValue { get; private set; }

    public int CorrectValue { get; private set; }
    public bool IsCorrectLeft => LeftValue == CorrectValue;

    public void SetValues(int correctAnswer, int wrongAnswer, bool correctOnLeft)
    {
        CorrectValue = correctAnswer;

        if (correctOnLeft)
        {
            LeftValue = correctAnswer;
            RightValue = wrongAnswer;
        }
        else
        {
            LeftValue = wrongAnswer;
            RightValue = correctAnswer;
        }

        leftText.text = LeftValue.ToString();
        rightText.text = RightValue.ToString();
    }

    public bool IsAnswerCorrect(bool isLeft) => 
        (isLeft && IsCorrectLeft) || (!isLeft && !IsCorrectLeft);
}
