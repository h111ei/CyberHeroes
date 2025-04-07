using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class ChangeTextOnTap : MonoBehaviour
{
    [System.Serializable]
    public class DialogueBlock
    {
        public string[] lines;
        public string animationSequenceName;
        public bool waitForAnimation = false;
    }

    public TextMeshProUGUI myText;
    public DialogueBlock[] dialogueBlocks;
    public Intro animationManager;

    private int currentBlockIndex = 0;
    private int currentLineIndex = 0;
    private bool isAnimating = false;

    private void Start()
    {
        ShowCurrentLine();
    }

    public void OnPointerClick()
    {
        if (isAnimating) return;

        // Если есть еще строки в текущем блоке
        if (currentLineIndex + 1 < dialogueBlocks[currentBlockIndex].lines.Length)
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
        else
        {
            // Запускаем анимацию для этого блока
            if (!string.IsNullOrEmpty(dialogueBlocks[currentBlockIndex].animationSequenceName))
            {
                StartCoroutine(PlayBlockAnimation());
            }

            // Переходим к следующему блоку
            if (currentBlockIndex + 1 < dialogueBlocks.Length)
            {
                currentBlockIndex++;
                currentLineIndex = 0;

                // Если не нужно ждать анимацию, сразу показываем следующую строку
                if (!dialogueBlocks[currentBlockIndex - 1].waitForAnimation)
                {
                    ShowCurrentLine();
                }
            }
        }
    }

    private IEnumerator PlayBlockAnimation()
    {
        isAnimating = true;
        string sequenceName = dialogueBlocks[currentBlockIndex].animationSequenceName;

        // Запускаем анимацию
        animationManager.PlaySequence(sequenceName);

        // Ждем завершения анимации (если нужно)
        if (dialogueBlocks[currentBlockIndex].waitForAnimation)
        {
            // Здесь нужно реализовать ожидание завершения анимации
            // Можно добавить событие в Intro или использовать корутину с примерным временем
            yield return new WaitForSeconds(GetAnimationDuration(sequenceName));
        }

        isAnimating = false;
    }

    private float GetAnimationDuration(string sequenceName)
    {
        // Здесь можно реализовать получение длительности анимации
        // Например, хранить длительности в словаре или вычислять сумму длительностей шагов
        return 2f; // Примерное значение
    }

    private void ShowCurrentLine()
    {
        myText.GetComponent<QuestionDisplay>().DisplayQuestion(
            dialogueBlocks[currentBlockIndex].lines[currentLineIndex]);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnPointerClick();
        }
    }
}
