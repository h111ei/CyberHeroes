using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRestrictor : MonoBehaviour
{
    public float leftMargin = 200f;
    public float rightMargin = 200f;
    private ScrollRect scrollRect;
    private RectTransform content;

    void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        content = scrollRect.content;
    }

    void Update()
    {
        // Ограничиваем позицию скролла
        float maxX = -leftMargin;
        float minX = -(content.sizeDelta.x - rightMargin - scrollRect.viewport.rect.width);

        float clampedX = Mathf.Clamp(content.anchoredPosition.x, minX, maxX);

        if (!Mathf.Approximately(content.anchoredPosition.x, clampedX))
        {
            content.anchoredPosition = new Vector2(clampedX, content.anchoredPosition.y);
        }
    }
}