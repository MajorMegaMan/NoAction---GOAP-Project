using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Animations;

public class ActionBillboard : MonoBehaviour
{
    [SerializeField] PositionConstraint m_constraint;
    ConstraintSource m_source;
    [SerializeField] TMP_Text m_text;

    ChefAgent m_agent;

    public void SetChef(ChefAgent agent)
    {
        m_agent = agent;
        AddFollowTarget(m_agent.billboardPosition);
    }

    void AddFollowTarget(Transform target)
    {
        transform.position = target.position;

        m_source.sourceTransform = target;
        m_source.weight = 1.0f;
        m_constraint.AddSource(m_source);
        m_constraint.constraintActive = true;
    }

    public void SetText(ChefAgent chef, IGOAPAgentActionMethods action)
    {
        if (action != null)
        {
            m_text.SetText(action.actionName);
        }
        else
        {
            m_text.SetText("No Action");
        }
    }

    public void AddListenerToChef()
    {
        m_agent.movementMachine.popActionEvent.AddListener(SetText);
        m_agent.onDestroyEvent.AddListener(DestroyBillboard);
    }

    public void RemoveListenerFromChef()
    {
        m_agent.movementMachine.popActionEvent.RemoveListener(SetText);
        m_agent.onDestroyEvent.RemoveListener(DestroyBillboard);
    }

    public void DestroyBillboard(ChefAgent chef)
    {
        Destroy(gameObject);
    }
}
