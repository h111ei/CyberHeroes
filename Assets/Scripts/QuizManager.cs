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
    
    private int currentQuestionIndex = 0;

    public Image questionImageField;
    public Button[] answerButtons;
    public GameObject losePanel;
    public QuestionDisplay question;

    public GameObject winPanel;

    void Start()
    {
        losePanel.SetActive(false);
        LoadQuestion(currentQuestionIndex);
    }
    IEnumerator LoadQuestionCoroutine(int questionIndex)
    {
        if (questionIndex < questions.Count)
        {
            Question currentQuestion = questions[questionIndex];

            questionImageField.sprite = currentQuestion.questionImage;
            question.DisplayQuestion(currentQuestion.questionText);

            // Ждем завершения анимации текста
            yield return new WaitUntil(() => question.typeCoroutine == null);


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
        if (answerIndex == questions[currentQuestionIndex].correctAnswerIndex)
        {
            currentQuestionIndex++;

            winPanel.SetActive(true);
            winPanel.GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestionIndex - 1].explanationTextCorrect;

        }
        else
        {
            GameOver();
        }
    }


    public void WinPanelLoad()
    {
        HandleCompletion(
            currentQuestionIndex,
            questions.Count,
            () => LoadQuestion(currentQuestionIndex),
            false,
            () =>
            {
                if (winPanel != null)
                {   
                    winPanel.SetActive(false);
                }
            }
        );
    }

    void GameOver()
    {
        losePanel.SetActive(true);
        losePanel.GetComponentInChildren<TextMeshProUGUI>().text = questions[currentQuestionIndex].explanationText;
    }

    public void RestartGame()
    {
        losePanel.SetActive(false);
        currentQuestionIndex = 0;
        LoadQuestion(currentQuestionIndex);
    }
}