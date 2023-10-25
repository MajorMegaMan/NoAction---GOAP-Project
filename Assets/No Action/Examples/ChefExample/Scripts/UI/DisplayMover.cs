using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMover : MonoBehaviour
{
    [SerializeField] RectTransform m_targetTransform;

    Vector2 m_origin = Vector2.zero;
    [SerializeField] Vector2 m_targetPosition = Vector2.zero;

    [SerializeField, Range(0.0f, 1.0f)] float m_currentValue = 0.0f;

    [SerializeField] AnimationCurve m_moveCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    bool m_atOrigin = true;
    bool m_isMoving = false;

    delegate void MoveAction();
    MoveAction m_moveDelegate = null;

    private void Awake()
    {
        m_origin = m_targetTransform.anchoredPosition;
        if (m_moveDelegate == null)
        {
            m_moveDelegate = ProgressMoveToOrigin;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_moveDelegate.Invoke();
    }

    public void MoveToOrigin()
    {
        enabled = true;
        m_moveDelegate = ProgressMoveToOrigin;
        m_atOrigin = true;
        m_isMoving = true;
    }

    public void MoveToTarget()
    {
        enabled = true;
        m_moveDelegate = ProgressMoveToTarget;
        m_atOrigin = false;
        m_isMoving = true;
    }

    void Lerp()
    {
        m_targetTransform.anchoredPosition = Vector2.Lerp(m_origin, m_targetPosition, m_moveCurve.Evaluate(m_currentValue));
    }

    void ProgressMoveToTarget()
    {
        m_currentValue += Time.unscaledDeltaTime;
        if(m_currentValue > 1.0f)
        {
            m_currentValue = 1.0f;
            enabled = false;
            m_isMoving = false;
        }
        Lerp();
    }

    void ProgressMoveToOrigin()
    {
        m_currentValue -= Time.unscaledDeltaTime;
        if (m_currentValue < 0.0f)
        {
            m_currentValue = 0.0f;
            enabled = false;
            m_isMoving = false;
        }
        Lerp();
    }

    public void Toggle()
    {
        if(m_atOrigin)
        {
            MoveToTarget();
        }
        else
        {
            MoveToOrigin();
        }
    }
}
