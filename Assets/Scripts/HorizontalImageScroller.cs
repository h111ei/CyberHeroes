using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HorizontalImageScroller : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float snapSpeed = 10f;
    public float threshold = 0.01f;

    private RectTransform contentPanel;
    private GameObject[] children;
    private int selectedIndex = 0;

    private void Start()
    {
        contentPanel = scrollRect.content;
        UpdateChildren();

        // �������� ������ ������� �� ���������
        if (children.Length > 0)
            SelectChild(selectedIndex);
    }

    private void Update()
    {
        // ��������� ������ A/D
        if (Input.GetKeyDown(KeyCode.D))
        {
            MoveToNext();
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            MoveToPrevious();
        }

        // ������� ����������� � ���������� ��������
        SnapToSelected();
    }

    private void UpdateChildren()
    {
        children = new GameObject[contentPanel.childCount];
        for (int i = 0; i < contentPanel.childCount; i++)
        {
            children[i] = contentPanel.GetChild(i).gameObject;
        }
    }

    private void MoveToNext()
    {
        if (selectedIndex < children.Length - 1)
        {
            selectedIndex++;
            SelectChild(selectedIndex);
        }
    }

    private void MoveToPrevious()
    {
        if (selectedIndex > 0)
        {
            selectedIndex--;
            SelectChild(selectedIndex);
        }
    }

    private void SelectChild(int index)
    {
        selectedIndex = Mathf.Clamp(index, 0, children.Length - 1);
        // ����� �������� ���������� ���������, ��������, �������� ����
    }

    private void SnapToSelected()
    {
        if (children == null || children.Length == 0) return;

        // �������� ������� ���������� ������� ������������ Content
        RectTransform selectedChild = children[selectedIndex].GetComponent<RectTransform>();
        Vector2 contentPos = (Vector2)scrollRect.transform.InverseTransformPoint(contentPanel.position);
        Vector2 childPos = (Vector2)scrollRect.transform.InverseTransformPoint(selectedChild.position);
        Vector2 targetPos = contentPos - childPos;

        // ���������� ������ �� �����������
        targetPos.y = contentPanel.anchoredPosition.y;

        // ������� �����������
        contentPanel.anchoredPosition = Vector2.Lerp(
            contentPanel.anchoredPosition,
            targetPos,
            snapSpeed * Time.deltaTime
        );

        // ���� ����� ������, �� "�������������"
        if (Vector2.Distance(contentPanel.anchoredPosition, targetPos) < threshold)
        {
            contentPanel.anchoredPosition = targetPos;
        }
    }
}
