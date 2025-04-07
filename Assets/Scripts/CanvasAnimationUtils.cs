using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAnimationUtils
{
    public static void AnimateCanvasBlackScreen(RectTransform blackScreen, Vector2 targetPosition, float duration, System.Action onComplete = null) //transition animation
    {
        if (blackScreen == null)
        {
            Debug.LogError("Black screen RectTransform is null!");
            return;
        }

        DOTween.Sequence()
            .Append(blackScreen.DOAnchorPos(targetPosition, duration))
            .AppendCallback(() =>
            {
                onComplete?.Invoke();
            });
    }

    public static Vector2 GetCanvasEdgePosition(RectTransform canvas, RectTransform blackScreen, Vector2 direction) //get full screen
    {
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();
        if (canvasRect == null)
        {
            Debug.LogError("Canvas doesn't have a RectTransform component!");
            return Vector2.zero;
        }

        Vector2 canvasSize = canvasRect.sizeDelta;
        return blackScreen.anchoredPosition + (direction.normalized * canvasSize.magnitude);
    }
}
