using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTimeControl : MonoBehaviour
{
    [SerializeField, Min(0.0f)] float m_timeScale = 1.0f;

    private void Awake()
    {
        Time.timeScale = m_timeScale;
    }

    private void OnValidate()
    {
        if(Application.isPlaying)
        {
            Time.timeScale = m_timeScale;
        }
    }

    public void SetTimeScale(float timeScale)
    {
        m_timeScale = timeScale;
        Time.timeScale = m_timeScale;
    }
}
