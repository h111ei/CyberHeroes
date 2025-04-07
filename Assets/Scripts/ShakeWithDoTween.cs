using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeWithDoTween : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeDuration = 0.5f;
    public float shakeStrength = 10f;
    public int vibrato = 10;
    public float randomness = 90f;
    public bool snapping = false;
    public bool fadeOut = true;

    private RectTransform rectTransform;
    private Tween shakeTween;
    private bool isQuitting = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartShake();
    }

    void OnDisable()
    {
        StopShake();
    }

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    void OnDestroy()
    {
        StopShake();
    }

    void StartShake()
    {
        // Останавливаем предыдущую тряску, если она была
        StopShake();

        // Создаем новый твин для тряски
        shakeTween = rectTransform.DOShakeAnchorPos(
            duration: shakeDuration,
            strength: shakeStrength,
            vibrato: vibrato,
            randomness: randomness,
            snapping: snapping,
            fadeOut: fadeOut)
            .SetLoops(-1, LoopType.Restart) // Бесконечное повторение
            .SetLink(gameObject); // Автоматически уничтожает твин при уничтожении объекта
    }

    void StopShake()
    {
        // Проверяем, что приложение не закрывается и твин существует
        if (!isQuitting && shakeTween != null && shakeTween.IsActive())
        {
            shakeTween.Kill();
        }
        shakeTween = null;
    }
}
