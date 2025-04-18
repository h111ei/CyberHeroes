using DG.Tweening;
using UnityEngine;

public class CanvasAnimationUtils
{
    public static void AnimateCanvasBlackScreen(RectTransform blackScreen, Vector2 targetPosition, float duration, System.Action onComplete = null)
    {
        if (blackScreen == null)
        {
            Debug.LogError("Black screen RectTransform is null!");
            return;
        }

        DOTween.Sequence()
            .Append(blackScreen.DOAnchorPos(targetPosition, duration).SetEase(Ease.Linear))
            .AppendCallback(() => onComplete?.Invoke());
    }

    public static Vector2 GetOffscreenPosition(RectTransform blackScreen, Vector2 direction)
    {
        Rect rect = blackScreen.rect;
        Vector2 screenSize = new Vector2(rect.width, rect.height);

        // –ассчитываем смещение, достаточное чтобы увести панель за пределы экрана
        Vector2 offset = new Vector2(
            screenSize.x * direction.x,
            screenSize.y * direction.y
        );

        return blackScreen.anchoredPosition + offset;
    }
}
