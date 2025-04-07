using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCube : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    private Transform originalParent;

    public bool isCorrect = false; //  ���������� � Inspector ��� ���������� ������
    public WallGame gameManager; //  ���������� GameManager � Inspector

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        originalPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("OnBeginDrag");
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(transform.root); //  ����� ���� ������ ������ ��������� �� ����� ��������������
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / gameManager.canvas.scaleFactor; //  ��������� scaleFactor Canvas
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

     
        // ���������, ���� ������ ���� ���������� � TargetPanel
        if (rectTransform.anchoredPosition == gameManager.targetPanel.anchoredPosition)
        {
          
            Debug.Log(transform.parent);
            Debug.Log(gameManager.targetPanel.transform);
            if (isCorrect)
            {
                gameManager.CorrectButtonDragged(this);
                // rectTransform.anchoredPosition = originalPosition;
            }
            else
            {
                gameManager.IncorrectButtonDragged();
                
            }
        }
        else
        {
            ReturnToOriginalPosition();
        }
    }

    public void ReturnToOriginalPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
        transform.SetParent(originalParent); //  ���������� � �������� ������������ �������
    }
}
