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

        // ���� ���� ��� ������ � ������� �����
        if (currentLineIndex + 1 < dialogueBlocks[currentBlockIndex].lines.Length)
        {
            currentLineIndex++;
            ShowCurrentLine();
        }
        else
        {
            // ��������� �������� ��� ����� �����
            if (!string.IsNullOrEmpty(dialogueBlocks[currentBlockIndex].animationSequenceName))
            {
                StartCoroutine(PlayBlockAnimation());
            }

            // ��������� � ���������� �����
            if (currentBlockIndex + 1 < dialogueBlocks.Length)
            {
                currentBlockIndex++;
                currentLineIndex = 0;

                // ���� �� ����� ����� ��������, ����� ���������� ��������� ������
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

        // ��������� ��������
        animationManager.PlaySequence(sequenceName);

        // ���� ���������� �������� (���� �����)
        if (dialogueBlocks[currentBlockIndex].waitForAnimation)
        {
            // ����� ����� ����������� �������� ���������� ��������
            // ����� �������� ������� � Intro ��� ������������ �������� � ��������� ��������
            yield return new WaitForSeconds(GetAnimationDuration(sequenceName));
        }

        isAnimating = false;
    }

    private float GetAnimationDuration(string sequenceName)
    {
        // ����� ����� ����������� ��������� ������������ ��������
        // ��������, ������� ������������ � ������� ��� ��������� ����� ������������� �����
        return 2f; // ��������� ��������
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
