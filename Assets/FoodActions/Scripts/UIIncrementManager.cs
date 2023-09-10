using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UIIncrementManager : MonoBehaviour
{
    [SerializeField] TMP_Text m_textField;

    [SerializeField] UnityEvent<TMP_Text> m_incrementEvent;
    [SerializeField] UnityEvent<TMP_Text> m_decrementEvent;

    int m_incrementState = 0;
    float m_incrementTimer = 0.0f;
    [SerializeField] float m_incrementSplit = 0.1f;
    [SerializeField] float m_initialIncrementWait = 0.5f;

    private void Update()
    {
        if(m_incrementState == 1)
        {
            if(IncrementTimer())
            {
                IncrementValue();
            }
        }

        if (m_incrementState == -1)
        {
            if (IncrementTimer())
            {
                DecrementValue();
            }
        }
    }

    public void BeginIncrement()
    {
        m_incrementState = 1;
        m_incrementTimer = -m_initialIncrementWait;
        IncrementValue();
    }

    public void BeginDecrement()
    {
        m_incrementState = -1;
        m_incrementTimer = -m_initialIncrementWait;
        DecrementValue();
    }

    public void StopIncrement()
    {
        m_incrementState = 0;
    }

    bool IncrementTimer()
    {
        m_incrementTimer += Time.unscaledDeltaTime;
        if(m_incrementTimer > m_incrementSplit)
        {
            m_incrementTimer -= m_incrementSplit;
            return true;
        }
        return false;
    }

    void IncrementValue()
    {
        m_incrementEvent.Invoke(m_textField);
    }

    void DecrementValue()
    {
        m_decrementEvent.Invoke(m_textField);
    }
}
