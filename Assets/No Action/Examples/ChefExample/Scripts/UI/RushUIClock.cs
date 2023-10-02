using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RushUIClock : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager;
    [SerializeField] Slider m_slider;

    [SerializeField] RectTransform m_clock;
    [SerializeField] Vector3 m_originPos;
    [SerializeField] Vector2 m_shakeSpeed = Vector2.one;
    [SerializeField] Vector2 m_shakeStrength = Vector2.one;

    [SerializeField] Utility.SmoothVector3 m_smoothShake;

    private void Awake()
    {
        m_originPos = m_clock.transform.position;
    }

    private void Update()
    {
        SetSliderValue();
        Shake();
    }

    void SetSliderValue()
    {
        float rushScale;
        if (!m_gameManager.isRushing)
        {
            rushScale = Mathf.InverseLerp(0, m_gameManager.rushIntervalTime, m_gameManager.rushControlCurrentTimer);
        }
        else
        {
            rushScale = 1 - Mathf.InverseLerp(0, m_gameManager.rushLengthTime, m_gameManager.rushControlCurrentTimer);
        }
        m_slider.value = rushScale;
    }

    void Shake()
    {
        Vector3 shake = Vector2.zero;
        if (m_gameManager.isRushing)
        {
            float noiseX = Mathf.PerlinNoise(Time.time * m_shakeSpeed.x, m_gameManager.rushControlCurrentTimer * m_shakeSpeed.x);
            float noiseY = Mathf.PerlinNoise(Time.time * m_shakeSpeed.y, m_gameManager.rushControlCurrentTimer * m_shakeSpeed.y);

            shake.x = noiseX;
            shake.y = noiseY;

            shake = (shake * 2) - Vector3.one;

            shake.x *= m_shakeStrength.x;
            shake.y *= m_shakeStrength.y;

            shake.z = 0.0f;
        }
        shake = m_smoothShake.Smooth(shake);

        m_clock.position = m_originPos + shake;
    }
}
