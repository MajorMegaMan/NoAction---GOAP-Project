using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AgentActionData<TKey> : ActionData<TKey>
{
    [SerializeField] float m_beginRange = 0.0f;
    [SerializeField] float m_beginRangeError = 0.01f;

    [SerializeField] float m_completeTime = 1.0f;
    [SerializeField] float m_effectTime = 0.5f;

    [SerializeField] string m_beginAnimID;
    [SerializeField] string m_completeAnimID;

    public bool shouldMoveTo { get { return m_beginRange != 0.0f; } }
    public float beginRange { get { return m_beginRange; } }
    public float completeTime { get { return m_completeTime; } }
    public float effectTime { get { return m_effectTime; } }
    public string beginAnimID { get { return m_beginAnimID; } }
    public string completeAnimID { get { return m_completeAnimID; } }

    public bool InBeginRange(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        if (!shouldMoveTo)
        {
            return true;
        }

        float range = (goapAgent.GetPosition() - actionTarget.GetPosition()).magnitude;
        return range - m_beginRangeError <= m_beginRange;
    }

    public Vector3 CalcBeginPosition(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        var targetPosition = actionTarget.GetPosition();
        var toAgent = (goapAgent.GetPosition() - targetPosition).normalized;
        return targetPosition + toAgent * m_beginRange;
    }

    public bool PlayBeginAnim(Animator animator)
    {
        if (beginAnimID.Length > 0)
        {
            animator.CrossFade(beginAnimID, 0.1f);
            return true;
        }
        return false;
    }

    public bool PlayCompleteAnim(Animator animator)
    {
        if (completeAnimID.Length > 0)
        {
            animator.CrossFade(completeAnimID, 0.1f);
            return true;
        }
        return false;
    }
}
