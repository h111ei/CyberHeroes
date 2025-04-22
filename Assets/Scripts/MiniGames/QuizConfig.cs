using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Minigames/Quiz Config")]
public class QuizConfig : MinigameConfig
{
    public float timePerQuestion = 30f;
    public Color correctAnswerColor = Color.green;
    public Color wrongAnswerColor = Color.red;
}