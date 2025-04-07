using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Shake : MonoBehaviour
{
    [Header("Shake Settings")]
    public float shakeStrength = 0.2f;
    public float shakeDuration = 1f;
    public int shakeVibrato = 5;

    private Vector3 startPosition;
    private Sequence shakeSequence;
    private bool isQuitting = false;

    private void Start()
    {
        startPosition = transform.position;
        ShakeContinuously();
    }

    private void ShakeContinuously()
    {
        // Останавливаем предыдущую последовательность, если она есть
        StopShake();

        shakeSequence = DOTween.Sequence();

        // Устанавливаем ссылку на объект для автоматического уничтожения твина
        shakeSequence.SetLink(gameObject);

        for (int i = 0; i < shakeVibrato; i++)
        {
            if (!this || !transform) return; // Проверка на уничтожение объекта

            Vector3 randomDirection = Random.insideUnitSphere.normalized * shakeStrength;
            Vector3 targetPosition = startPosition + randomDirection;

            shakeSequence.Append(transform
                .DOMove(targetPosition, shakeDuration / shakeVibrato)
                .SetEase(Ease.InOutSine));
        }

        // Проверка перед добавлением завершающего движения
        if (this && transform)
        {
            shakeSequence.Append(transform
                .DOMove(startPosition, shakeDuration / shakeVibrato)
                .SetEase(Ease.InOutSine));

            shakeSequence.SetLoops(-1);
        }
    }

    private void StopShake()
    {
        if (shakeSequence != null && shakeSequence.IsActive())
        {
            shakeSequence.Kill();
        }
        shakeSequence = null;
    }

    private void OnApplicationQuit()
    {
        isQuitting = true;
    }

    private void OnDisable()
    {
        StopShake();
    }

    private void OnDestroy()
    {
        // Не пытаемся останавливать твины при выходе из приложения
        if (!isQuitting)
        {
            StopShake();
        }
    }
}

