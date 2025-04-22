using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuizMinigame : MonoBehaviour, IMinigame
{
    public MinigameType Type => MinigameType.Quiz;

    [Header("Config")]
    [SerializeField] private QuizConfig config;

    [Header("UI References")]
    [SerializeField] private Image questionImageField;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI explanationTextUI;
    [SerializeField] private TextMeshProUGUI correctTextUI;
    [SerializeField] private GameObject nextPanel;
    [SerializeField] private QuestionDisplay questionDisplay;

    private int currentLevelIndex;
    private bool isGameActive;
    private QuizLevelData currentLevel => (QuizLevelData)config.levels[currentLevelIndex];

    public void Initialize()
    {
        currentLevelIndex = 0;
        isGameActive = false;
        gameOverPanel.SetActive(false);
        nextPanel.SetActive(false);
    }

    public void StartGame()
    {
        if (config.levels.Count == 0)
        {
            Debug.LogError("No levels configured for Quiz minigame!");
            return;
        }

        isGameActive = true;
        LoadQuestion(currentLevelIndex);
        MinigameEvents.TriggerMinigameStart(this);
    }

    private IEnumerator LoadQuestionCoroutine(int questionIndex)
    {
        if (questionIndex >= config.levels.Count) yield break;

        questionImageField.sprite = currentLevel.levelImage;
        questionDisplay.DisplayQuestion(currentLevel.questionText);

        yield return new WaitUntil(() => questionDisplay.typeCoroutine == null);

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentLevel.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }
    }

    private void LoadQuestion(int questionIndex)
    {
        foreach (var button in answerButtons)
            button.gameObject.SetActive(false);

        StartCoroutine(LoadQuestionCoroutine(questionIndex));
    }

    private void OnAnswerSelected(int answerIndex)
    {
        if (!isGameActive) return;

        if (answerIndex == currentLevel.correctAnswerIndex)
        {
            currentLevelIndex++;
            nextPanel.SetActive(true);
            correctTextUI.text = currentLevel.explanationTextCorrect;
        }
        else
        {
            EndGame(false);
        }
    }

    public void EndGame(bool isWin)
    {
        isGameActive = false;
        gameOverPanel.SetActive(!isWin);
        explanationTextUI.text = currentLevel.explanationText;
        MinigameEvents.TriggerMinigameEnd(isWin);
    }

    public void Restart()
    {
        Cleanup();
        Initialize();
        StartGame();
    }

    public void Cleanup()
    {
        StopAllCoroutines();
        foreach (var button in answerButtons)
        {
            button.gameObject.SetActive(false);
            button.onClick.RemoveAllListeners();
        }
    }

    // Called from UI button
    public void ProceedToNextQuestion()
    {
        nextPanel.SetActive(false);

        if (currentLevelIndex >= config.levels.Count)
        {
            EndGame(true);
        }
        else
        {
            LoadQuestion(currentLevelIndex);
        }
    }
}