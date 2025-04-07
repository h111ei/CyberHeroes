using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class Shake : MonoBehaviour
{
    public float shakeStrength = 0.2f;
    public float shakeDuration = 1f;
    public int shakeVibrato = 5;

    private Vector3 startPosition;
    private Sequence shakeSequence;

    private void Start()
    {
        startPosition = transform.position;
        ShakeContinuously();
    }

    private void ShakeContinuously()
    {
        shakeSequence = DOTween.Sequence();
        for (int i = 0; i < shakeVibrato; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere.normalized * shakeStrength;
            Vector3 targetPosition = startPosition + randomDirection;
            // Добавляем плавную интерполяцию
            shakeSequence.Append(transform.DOMove(targetPosition, shakeDuration / shakeVibrato).SetEase(Ease.InOutSine));
        }
        // Возвращение в исходную позицию плавно
        shakeSequence.Append(transform.DOMove(startPosition, shakeDuration / shakeVibrato).SetEase(Ease.InOutSine));
        shakeSequence.SetLoops(-1);
    }
}

