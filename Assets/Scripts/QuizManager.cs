using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;


public class QuizManager : BaseGameManager
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public Sprite questionImage;
        public string[] answers;
        public int correctAnswerIndex;
        public string explanationText;
        public string explanationTextCorrect;
    }

    public List<Question> questions;

    public Image questionImageField;
    public Button[] answerButtons;
    public GameObject gameOverPanel;
    public QuestionDisplay questionDisplay;
    public TextMeshProUGUI explanationTextUI;
    public TextMeshProUGUI CorrectTextUI;
    public GameObject NextPanel;
    public GameObject WinPanel;
    public AudioSource hi;
    public AudioClip Brass;
    public Image AnotherBlackPanel;
    public GameObject Quiz;

    public override void StartGame()
    {
        gameOverPanel.SetActive(false);
        LoadQuestion(currentLevelIndex);
    }

    IEnumerator LoadQuestionCoroutine(int questionIndex)
    {
        if (questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];
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

    void LoadQuestion(int questionIndex)
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }
        StartCoroutine(LoadQuestionCoroutine(questionIndex));
    }

    void AnswerButtonClicked(int answerIndex)
    {
        if (answerIndex == questions[currentLevelIndex].correctAnswerIndex)
        {
            currentLevelIndex++;
            NextPanel.SetActive(true);
            CorrectTextUI.text = questions[currentLevelIndex - 1].explanationTextCorrect;
        }
        else
        {
            GameOver();
        }
    }

    public void NextPanelLoad()
    {
        NextPanel.SetActive(false);
        if (currentLevelIndex >= questions.Count)
        {
            AnimationManager.PlaySequence("SecondLevelTrans");
            HandleLevelCompletion();
        }
        else
        {
            LoadQuestion(currentLevelIndex);
        }
    }

    public override void GameOver()
    {
        isGameOver = true;
        gameOverPanel.SetActive(true);
        explanationTextUI.text = questions[currentLevelIndex].explanationText;
    }

    public override void RestartGame()
    {
        ResetGameState();
        gameOverPanel.SetActive(false);
        LoadQuestion(currentLevelIndex);
    }

    public override void MoveToNextLevel()
    {
        NextPanelLoad();
    }
}
