using UnityEngine;

/// <summary>
/// Эффект тряски камеры
/// </summary>
public class CameraShakeEffect : MonoBehaviour
{
    private Vector3 originalPosition;
    private float shakeTimer = 0f;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0f;

    void Start()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimer > 0f)
        {
            // Случайная позиция в радиусе интенсивности
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;
            shakeOffset.z = 0; // Не двигаем камеру по Z в 2D
            
            transform.localPosition = originalPosition + shakeOffset;

            shakeTimer -= Time.deltaTime;

            if (shakeTimer <= 0f)
            {
                // Вернуть камеру в исходную позицию
                transform.localPosition = originalPosition;
            }
        }
    }

    public void Shake(float duration, float intensity)
    {
        shakeTimer = duration;
        shakeDuration = duration;
        shakeIntensity = intensity;
    }
}
