using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DanielLochner.Assets.SimpleScrollSnap;
using System.Linq;
using TMPro;

public class HorizontalImageScroller : BaseGameManager
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
    public GameObject panelPrefab;
    public FileData[] filesData;
    public float paddingSize = 200f; // Размер отступов по краям

    private bool isDeleting = false;
    private int incorrectDeletions = 0;
    private Coroutine currentDeletionCoroutine;

    public GameObject ErrorPanel;
    public GameObject WinPanel;
    public Button FinishGameButton;

    void Start()
    {
        InitializeScrollSnap();
        deleteButton.onClick.AddListener(DeleteCurrentFile);
    }
    
    void InitializeScrollSnap()
    {
        // Clean up existing elements
        foreach (Transform child in scrollSnap.Content)
        {
            Destroy(child.gameObject);
        }

        // Создаем левый отступ
        CreatePaddingElement(paddingSize);

        // Создаем основные элементы
        foreach (var file in filesData)
        {
            CreateFileElement(file);
        }

        // Создаем правый отступ
        CreatePaddingElement(paddingSize);

        scrollSnap.Setup();

        // Добавляем задержку перед установкой позиции
        StartCoroutine(SetInitialPosition());
    }
    IEnumerator SetInitialPosition()
    {
        // Ждем завершения кадра, чтобы все элементы успели инициализироваться
        yield return new WaitForEndOfFrame();

        // Устанавливаем начальную позицию на первый реальный элемент (индекс 1, так как 0 - это отступ)
        scrollSnap.GoToPanel(1);

        // Альтернативный вариант - центрировать вручную
        // scrollSnap.Content.anchoredPosition = new Vector2(-(paddingSize + 150), 0);
    }
    void CreatePaddingElement(float width)
    {
        GameObject padding = new GameObject("Padding", typeof(RectTransform));
        padding.transform.SetParent(scrollSnap.Content);

        RectTransform rt = padding.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(width, 0);
        rt.localScale = Vector3.one;

        // Отключаем Raycast Target чтобы не мешал взаимодействию
        CanvasRenderer cr = padding.AddComponent<CanvasRenderer>();
        cr.cullTransparentMesh = true;
    }

    GameObject CreateFileElement(FileData file)
    {
        if (panelPrefab == null)
        {
            Debug.LogError("Panel prefab is not assigned!");
            return null;
        }

        GameObject fileObject = Instantiate(panelPrefab, scrollSnap.Content);
        fileObject.name = "File";

        Image img = fileObject.GetComponentInChildren<Image>();
        if (img != null)
        {
            img.sprite = file.image;
            img.preserveAspect = true;
            img.color = Color.white;
        }
        else
        {
            img.sprite = null;
            Debug.Log("Hi");
        }

        TextMeshProUGUI tmpText = fileObject.GetComponentInChildren<TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = file.text;
            tmpText.color = Color.black;
            tmpText.fontSize = 24;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.enableWordWrapping = true;
        }

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

        // Учитываем что первый элемент - это отступ
        int currentIndex = scrollSnap.CenteredPanel - 1;
        if (currentIndex < 0 || currentIndex >= filesData.Length) return;

        RectTransform fileToDelete = scrollSnap.Panels[currentIndex + 1]; // +1 потому что первый элемент отступ
        FileData currentFileData = filesData[currentIndex];

        bool shouldDelete = CheckForKeywords(currentFileData.text);

        if (shouldDelete == currentFileData.shouldBeDeleted)
        {
            incorrectDeletions++;
            Debug.Log("Ошибка! Неправильное удаление. Ошибок: " + incorrectDeletions);
            ErrorPanel.gameObject.SetActive(true);
            return;
        }

        if (currentDeletionCoroutine != null)
        {
            StopCoroutine(currentDeletionCoroutine);
        }
        currentDeletionCoroutine = StartCoroutine(DeleteWithAnimation(fileToDelete, currentIndex, shouldDelete));
    }

    public void CloseErrorPanel()
    {
        ErrorPanel.gameObject.SetActive(false);
    }

    bool CheckForKeywords(string text)
    {
        string[] keywords = { "вирус", "malware", "троян", "опасно", "удалить" };
        return keywords.Any(keyword => text.ToLower().Contains(keyword.ToLower()));
    }

    private IEnumerator DeleteWithAnimation(RectTransform fileToDelete, int index, bool correctDeletion)
    {
        isDeleting = true;

        TextMeshProUGUI tmpText = fileToDelete.GetComponentInChildren<TextMeshProUGUI>();
        string originalText = tmpText != null ? tmpText.text : "";

        if (tmpText != null)
        {
            tmpText.text = "<color=#ff0000>Удаление...</color>";
        }

        Graphic[] graphics = fileToDelete.GetComponentsInChildren<Graphic>();
        Color[] originalColors = graphics.Select(g => g.color).ToArray();
        Vector3 originalScale = fileToDelete.localScale;

        float elapsed = 0f;
        while (elapsed < deleteAnimationDuration)
        {
            float progress = elapsed / deleteAnimationDuration;

            fileToDelete.localScale = Vector3.Lerp(originalScale, Vector3.zero, progress);

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

        // Удаляем данные и обновляем массив
        filesData = filesData.Where((_, i) => i != index).ToArray();

        // Удаляем элемент из scrollSnap (учитываем что первый элемент - отступ)
        scrollSnap.Remove(index + 1);

        // Проверяем условие победы
        if (filesData.All(f => !f.shouldBeDeleted))
        {
            WinPanel.SetActive(true);
            FinishGameButton.onClick.AddListener(PlaySequence);
            deleteButton.interactable = false;
        }

        isDeleting = false;
        currentDeletionCoroutine = null;
    }
}