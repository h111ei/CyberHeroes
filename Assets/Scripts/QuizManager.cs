using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

[System.Serializable]
public class QuizQuestion
{
    public string questionText;
    public Sprite questionImage;
    public string[] answers;
    public int correctAnswerIndex;
    public string explanationText;
    public string explanationTextCorrect;
}

public class QuizManager : BaseGameManager
{
    [Header("Quiz Settings")]
    public List<QuizQuestion> questions;
    public QuestionDisplay questionDisplay;

    [Header("UI References")]
    public Image questionImageField;
    public Button[] answerButtons;
    public GameObject gameOverPanel;
    public TextMeshProUGUI explanationTextUI;
    public TextMeshProUGUI correctTextUI;
    public GameObject nextPanel;
    public GameObject winPanel;
    public Image anotherBlackPanel;

    [Header("Audio")]
    public AudioSource hi;
    public AudioClip brass;

    public override void StartGame()
    {
        currentLevelIndex = 0;
        isGameOver = false;
        gameOverPanel.SetActive(false);
        LoadQuestion(currentLevelIndex);
    }

    public override void MoveToNextLevel()
    {
        nextPanel.SetActive(false);
        if (currentLevelIndex >= questions.Count)
        {
            PlaySequence("SecondLevelTrans");
            StopBackgroundMusic();
        }
        else
        {
            LoadQuestion(currentLevelIndex);
        }
    }

    public override void RestartGame()
    {
        gameOverPanel.SetActive(false);
        currentLevelIndex = 0;
        LoadQuestion(currentLevelIndex);
    }

    public override void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        explanationTextUI.text = questions[currentLevelIndex].explanationText;
    }

    private IEnumerator LoadQuestionCoroutine(int questionIndex)
    {
        if (questionIndex < questions.Count)
        {
            QuizQuestion currentQuestion = questions[questionIndex];

            questionImageField.sprite = currentQuestion.questionImage;
            questionDisplay.DisplayQuestion(currentQuestion.questionText);

            yield return new WaitUntil(() => questionDisplay.typeCoroutine == null);

            for (int i = 0; i < answerButtons.Length; i++)
            {
                int buttonIndex = i;
                answerButtons[i].gameObject.SetActive(true);
                answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.answers[i];
                answerButtons[i].onClick.RemoveAllListeners();
                answerButtons[i].onClick.AddListener(() => AnswerButtonClicked(buttonIndex));
            }
        }
    }

    private void LoadQuestion(int questionIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }
        StartCoroutine(LoadQuestionCoroutine(questionIndex));
    }

    private void AnswerButtonClicked(int answerIndex)
    {
        if (answerIndex == questions[currentLevelIndex].correctAnswerIndex)
        {
            currentLevelIndex++;
            nextPanel.SetActive(true);
            correctTextUI.text = questions[currentLevelIndex - 1].explanationTextCorrect;
        }
        else
        {
            GameOver();
        }
    }
}