using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Quiz Level Data")]
public class QuizLevelData : LevelData
{
    public string questionText;
    public string[] answers;
    public int correctAnswerIndex;
    public string explanationText;
    public string explanationTextCorrect;
}