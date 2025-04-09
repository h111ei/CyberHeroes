using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDropController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private GameObject _panelPrefab;
    [SerializeField] private Transform _spawnParent;
    [SerializeField] private Vector2 _spawnPosition;
    [SerializeField] private Transform _panelsContainer;
    [SerializeField] private int _panelIndex;
    [SerializeField] private float _fadeDuration = 0.3f;

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private bool _isLocked = false;
    private Vector2 _dragOffset;
    private TextMeshProUGUI _textComponent;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _canvasGroup = GetComponent<CanvasGroup>();
        _textComponent = GetComponentInChildren<TextMeshProUGUI>();

        if (_spawnParent == null) _spawnParent = transform.parent;
        if (_panelsContainer == null) _panelsContainer = _spawnParent;

        if (_spawnPosition == Vector2.zero)
            _spawnPosition = _rectTransform.anchoredPosition;

        UpdateTextFromManager();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _dragOffset);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        Vector2 localPointer;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform.parent as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPointer))
        {
            _rectTransform.anchoredPosition = localPointer - _dragOffset;
        }
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isLocked) return;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        var dropZone = eventData.pointerCurrentRaycast.gameObject?.GetComponent<DropZone>();
        bool droppedInZone = dropZone != null &&
                           WallGameManager.Instance.CanAttachToDropZone(dropZone, _panelIndex);

        if (droppedInZone)
        {
            StartCoroutine(FadeAndDestroy(dropZone.gameObject));
            WallGameManager.Instance.MoveToNextLevel();
            UpdateAllUnlockedPanels();
            SpawnNewPanel();
        }
        else
        {
            ReturnToSpawnPosition();
        }
    }

    private IEnumerator FadeAndDestroy(GameObject dropZone)
    {
        _isLocked = true;

        CanvasGroup dropZoneCanvasGroup = dropZone.GetComponent<CanvasGroup>();
        if (dropZoneCanvasGroup == null) dropZoneCanvasGroup = dropZone.AddComponent<CanvasGroup>();

        float elapsed = 0f;
        while (elapsed < _fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / _fadeDuration);
            _canvasGroup.alpha = alpha;
            dropZoneCanvasGroup.alpha = alpha;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(dropZone);
        Destroy(gameObject);
    }


    private void LockPanelToDropZone(Transform dropZone)
    {
        _isLocked = true;
        _rectTransform.SetParent(dropZone);
        _rectTransform.anchoredPosition = Vector2.zero;
        _canvasGroup.blocksRaycasts = true;
    }

    private void SpawnNewPanel()
    {
        if (_panelPrefab == null) return;

        GameObject newPanel = Instantiate(_panelPrefab, _panelPrefab.transform.parent);
        newPanel.GetComponent<RectTransform>().SetParent(_spawnParent);
        newPanel.GetComponent<RectTransform>().anchoredPosition = _spawnPosition;
        var controller = newPanel.GetComponent<DragAndDropController>();
        controller._panelIndex = this._panelIndex;
        controller.UpdateTextFromManager();

    }

    private void ReturnToSpawnPosition()
    {
        _rectTransform.SetParent(_spawnParent);
        _rectTransform.anchoredPosition = _spawnPosition;
    }

    private void UpdateAllUnlockedPanels()
    {
        foreach (Transform child in _panelsContainer)
        {
            var panel = child.GetComponent<DragAndDropController>();
            if (panel != null)
            {
                panel.UpdateTextFromManager();
            }
        }
    }

    public void UpdateTextFromManager()
    {
        if (_textComponent != null && WallGameManager.Instance != null)
        {
            _textComponent.text = WallGameManager.Instance.GetTextForPanel(_panelIndex);
        }
    }
}
