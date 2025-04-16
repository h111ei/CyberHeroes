using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class QuestionDisplay : MonoBehaviour
{
    private TextMeshProUGUI questionTextField;
    public float typeSpeed = 0.05f;
    private string fullQuestionText;
    public Coroutine typeCoroutine; // �����������

    private void Start()
    {
        questionTextField = GetComponent<TextMeshProUGUI>();
    }
    public void DisplayQuestion(string questionText)
    {
        fullQuestionText = questionText;
        // ���������� ���������� �����������, ���� ��� ��������
        if (typeCoroutine != null)
        {
            StopCoroutine(typeCoroutine);
        }

        typeCoroutine = StartCoroutine(TypeText());

    }

    IEnumerator TypeText()
    {
        if (questionTextField != null)
        {
            questionTextField.text = "";

            foreach (char letter in fullQuestionText.ToCharArray())
            {
                questionTextField.text += letter;
                yield return new WaitForSeconds(typeSpeed);
            }

            typeCoroutine = null;
        }

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // ���������, �������� �� ��������
            if (typeCoroutine != null)
            {
                // ��������� ��������
                StopCoroutine(typeCoroutine);
                // ������� ������ �����
                questionTextField.text = fullQuestionText;
                typeCoroutine = null;
            }
        }
    }
}
