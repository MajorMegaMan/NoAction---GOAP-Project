using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManagerUppers : MonoBehaviour
{
    [SerializeField] GameManager m_gameManager;

    [SerializeField] UnityEvent m_startEvent;

    private void Start()
    {
        m_startEvent.Invoke();
    }

    public void IncrementCustomerMin(TMP_Text textField)
    {
        m_gameManager.minCustomerCount++;
        ApplyMinCustomerToText(textField);
    }

    public void DecrementCustomerMin(TMP_Text textField)
    {
        m_gameManager.minCustomerCount--;
        ApplyMinCustomerToText(textField);
    }

    public void ApplyMinCustomerToText(TMP_Text textField)
    {
        textField.SetText(m_gameManager.minCustomerCount.ToString());
    }

    public void SetRushInterval(float value)
    {
        m_gameManager.rushIntervalTime = value;
    }

    public void ApplyRushIntervalToText(TMP_Text textField)
    {
        textField.SetText(m_gameManager.rushIntervalTime.ToString(".0#"));
    }

    public void AlignSliderToRushInterval(Slider slider)
    {
        slider.value = m_gameManager.rushIntervalTime;
    }

    public void SetRushLength(float value)
    {
        m_gameManager.rushLengthTime = value;
    }

    public void ApplyRushLengthToText(TMP_Text textField)
    {
        textField.SetText(m_gameManager.rushLengthTime.ToString(".0#"));
    }

    public void AlignSliderToRushLength(Slider slider)
    {
        slider.value = m_gameManager.rushLengthTime;
    }

    public void IncrementRushCount(TMP_Text textField)
    {
        m_gameManager.rushCount++;
        ApplyRushCountToText(textField);
    }

    public void DecrementRushCount(TMP_Text textField)
    {
        m_gameManager.rushCount = Mathf.Max(1, m_gameManager.rushCount - 1);
        ApplyRushCountToText(textField);
    }

    public void ApplyRushCountToText(TMP_Text textField)
    {
        textField.SetText(m_gameManager.rushCount.ToString());
    }

    public void IncrementSoftMax(TMP_Text textField)
    {
        m_gameManager.softMaxCustomers++;
        ApplySoftMaxToText(textField);
    }

    public void DecrementSoftMax(TMP_Text textField)
    {
        m_gameManager.softMaxCustomers = Mathf.Max(1, m_gameManager.softMaxCustomers - 1);
        ApplySoftMaxToText(textField);
    }

    public void ApplySoftMaxToText(TMP_Text textField)
    {
        textField.SetText(m_gameManager.softMaxCustomers.ToString());
    }
}
