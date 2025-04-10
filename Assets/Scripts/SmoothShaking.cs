using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothShaking : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float shakeIntensity = 0.1f;
    [SerializeField] private float shakeSpeed = 10f;
    [SerializeField] private float rotationShakeAmount = 1f;

    [Header("Axes")]
    [SerializeField] private bool shakeX = true;
    [SerializeField] private bool shakeY = true;
    [SerializeField] private bool shakeZ = true;
    [SerializeField] private bool rotateShake = false;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private float seed;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        seed = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // Position shake
        float x = shakeX ? (Mathf.PerlinNoise(seed, Time.time * shakeSpeed) * 2 - 1) * shakeIntensity : 0f;
        float y = shakeY ? (Mathf.PerlinNoise(seed + 1, Time.time * shakeSpeed) * 2 - 1) * shakeIntensity : 0f;
        float z = shakeZ ? (Mathf.PerlinNoise(seed + 2, Time.time * shakeSpeed) * 2 - 1) * shakeIntensity : 0f;

        transform.localPosition = originalPosition + new Vector3(x, y, z);

        // Rotation shake
        if (rotateShake)
        {
            float rx = (Mathf.PerlinNoise(seed + 3, Time.time * shakeSpeed) * 2 - 1) * rotationShakeAmount;
            float ry = (Mathf.PerlinNoise(seed + 4, Time.time * shakeSpeed) * 2 - 1) * rotationShakeAmount;
            float rz = (Mathf.PerlinNoise(seed + 5, Time.time * shakeSpeed) * 2 - 1) * rotationShakeAmount;

            transform.localRotation = originalRotation * Quaternion.Euler(rx, ry, rz);
        }
    }

    private void OnDisable()
    {
        // Reset position when disabled
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

    // Public methods to control shake parameters
    public void SetShakeIntensity(float intensity)
    {
        shakeIntensity = intensity;
    }

    public void SetShakeSpeed(float speed)
    {
        shakeSpeed = speed;
    }
}
