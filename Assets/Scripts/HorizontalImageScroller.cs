using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Linq;
using TMPro;

public class HorizontalImageScroller : MonoBehaviour
{
    [System.Serializable]
    public class FileData
    {
        public Sprite image;
        [TextArea(3, 5)] public string text;
        public bool shouldBeDeleted;
    }

    public SimpleScrollSnap scrollSnap;
    public Button deleteButton;
    public float deleteAnimationDuration = 0.5f;
    public GameObject panelPrefab; // ������ ������ � Image � �������� TextMeshProUGUI
    public FileData[] filesData;

    private bool isDeleting = false;
    private int correctDeletions = 0;
    private int incorrectDeletions = 0;
    private Coroutine currentDeletionCoroutine;

    public GameObject ErrorPanel;
    public GameObject WinPanel;
    public Intro Sequence;
    void Start()
    {
        InitializeScrollSnap();
        deleteButton.onClick.AddListener(DeleteCurrentFile);
    }

    void InitializeScrollSnap()
    {
        // ������� ������������ ���������
        foreach (Transform child in scrollSnap.Content)
        {
            Destroy(child.gameObject);
        }

        // �������� ����� ��������� �� �������
        foreach (var file in filesData)
        {
            CreateFileElement(file);
        }

        scrollSnap.Setup();
    }


    GameObject CreateFileElement(FileData file)
    {
        if (panelPrefab == null)
        {
            Debug.LogError("Panel prefab is not assigned!");
            return null;
        }

        // ������� ��������� �������
        GameObject fileObject = Instantiate(panelPrefab, scrollSnap.Content);
        fileObject.name = "File";

        // ��������� Image
        Image img = fileObject.GetComponentInChildren<Image>();

        if (img != null)
        {
            img.sprite = file.image;
            img.preserveAspect = true;
            img.color = Color.white;
        }

        // ������� TextMeshProUGUI � �������� ��������
        TextMeshProUGUI tmpText = fileObject.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = file.text;
            tmpText.color = Color.black;
            tmpText.fontSize = 24;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableWordWrapping = true;
        }

        // ��������� RectTransform (���� �����)
        RectTransform rt = fileObject.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.sizeDelta = new Vector2(300, 400);
        }

        return fileObject;
    }

    public void DeleteCurrentFile()
    {
        if (isDeleting || scrollSnap.Panels.Length == 0) return;

        int currentIndex = scrollSnap.CenteredPanel;
        if (currentIndex < 0 || currentIndex >= scrollSnap.Panels.Length) return;

        RectTransform fileToDelete = scrollSnap.Panels[currentIndex];
        FileData currentFileData = filesData[currentIndex];

        bool shouldDelete = CheckForKeywords(currentFileData.text);

        if (shouldDelete == currentFileData.shouldBeDeleted)
        {
            incorrectDeletions++;
            Debug.Log("������! ������������ ��������. ������: " + incorrectDeletions);
            ErrorPanel.gameObject.SetActive(true);

        }
        else if (shouldDelete)
        {
            correctDeletions++;
        }

        if (currentDeletionCoroutine != null)
        {
            StopCoroutine(currentDeletionCoroutine);
        }
        currentDeletionCoroutine = StartCoroutine(DeleteWithAnimation(fileToDelete, currentIndex, shouldDelete));
    }


    //��������� � �������
    public void CloseErrorPanel()
    {
        ErrorPanel.gameObject.SetActive(false);
    }

    public void PlaySequence()
    {
        Sequence.PlaySequence("Ending");
    }

    bool CheckForKeywords(string text)
    {
        string[] keywords = { "�����", "malware", "�����", "������", "�������" };
        return keywords.Any(keyword => text.ToLower().Contains(keyword.ToLower()));
    }

    private IEnumerator DeleteWithAnimation(RectTransform fileToDelete, int index, bool correctDeletion)
    {
        isDeleting = true;

        // ��������� ������ �� ���������� ����� ���������
        TextMeshProUGUI tmpText = fileToDelete.GetComponentInChildren<TextMeshProUGUI>();
        string originalText = tmpText != null ? tmpText.text : "";

        if (tmpText != null)
        {
            tmpText.text = "<color=#ff0000>��������...</color>";
        }

        // �������� ��� ����������� ����������
        Graphic[] graphics = fileToDelete.GetComponentsInChildren<Graphic>();
        Color[] originalColors = graphics.Select(g => g.color).ToArray();
        Vector3 originalScale = fileToDelete.localScale;

        // ��������
        float elapsed = 0f;
        while (elapsed < deleteAnimationDuration)
        {
            float progress = elapsed / deleteAnimationDuration;

            // ���������������
            fileToDelete.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

            // ��������� ������������
            for (int i = 0; i < graphics.Length; i++)
            {
                if (graphics[i] != null)
                {
                    Color newColor = originalColors[i];
                    newColor.a = Mathf.Lerp(1, 0, progress);
                    graphics[i].color = newColor;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // �������� �� ������
        filesData = filesData.Where((_, i) => i != index).ToArray();

        // �������� �� ScrollSnap
        scrollSnap.Remove(index);

        // �������� ������
        if (filesData.All(f => !f.shouldBeDeleted))
        {
            Debug.Log("������! ��� ������� ����� �������!");
            WinPanel.SetActive(true);
            deleteButton.interactable = false;
        }

        isDeleting = false;
        currentDeletionCoroutine = null;
    }
}
