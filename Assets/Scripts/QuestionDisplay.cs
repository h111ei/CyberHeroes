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
    public AudioSource PIZ;
    public AudioClip Clava;
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
            PIZ.Stop();
        }

        typeCoroutine = StartCoroutine(TypeText());

        if (gameObject.name != "TaskTextPanel")
        {
            PIZ.PlayOneShot(Clava, 5f);
        }
    }

    IEnumerator TypeText()
    {
        questionTextField.text = "";

        foreach (char letter in fullQuestionText.ToCharArray())
        {
            questionTextField.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }

        typeCoroutine = null;
        if (gameObject.name != "TaskTextPanel")
        {
            PIZ.Stop();
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
                if (gameObject.name != "f")
                {
                    PIZ.Stop();
                }
                // ������� ������ �����
                questionTextField.text = fullQuestionText;
                typeCoroutine = null;
            }
        }
    }
}
