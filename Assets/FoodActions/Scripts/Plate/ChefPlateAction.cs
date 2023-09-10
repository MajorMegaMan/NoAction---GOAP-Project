using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefPlateAction : MonoBehaviour, IGOAPAction, IGOAPAgentActionMethods
{
    [SerializeField] ChefActionTarget m_plate;

    [SerializeField] ChefAgentAction m_plateAction;

    public string actionName { get { return m_plateAction.actionName; } }
    public bool shouldMoveTo { get { return m_plateAction.shouldMoveTo; } }
    public float beginRange { get { return m_plateAction.beginRange; } }
    public float completeTime { get { return m_plateAction.completeTime; } }
    public float effectTime { get { return m_plateAction.effectTime; } }
    public string beginAnimID { get { return m_plateAction.beginAnimID; } }
    public string completeAnimID { get { return m_plateAction.completeAnimID; } }

    public bool InBeginRange(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_plateAction.InBeginRange(goapAgent, actionTarget);
    }

    public Vector3 CalcBeginPosition(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_plateAction.CalcBeginPosition(goapAgent, actionTarget);
    }

    public IGOAPActionTarget FindActionTarget()
    {
        if(m_plate.tablePosition == null)
        {
            return m_plate;
        }
        return m_plate.tablePosition;
    }

    #region GOAPAction
    public bool CheckCondition(GOAPWorldState worldState)
    {
        return m_plateAction.CheckCondition(worldState);
    }

    public int AddEffects(GOAPWorldState worldState)
    {
        return m_plateAction.AddEffects(worldState);
    }

    public float GetWeight()
    {
        return m_plateAction.GetWeight();
    }
    #endregion // GOAPAction

    public void PerformAgentEffects(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {

    }

    public bool PlayBeginAnim(Animator animator)
    {
        return m_plateAction.PlayBeginAnim(animator);
    }

    public bool PlayCompleteAnim(Animator animator)
    {
        return m_plateAction.PlayCompleteAnim(animator);
    }
}
