using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpItemAction : MonoBehaviour, IGOAPAction, IGOAPAgentActionMethods
{
    [SerializeField] ChefActionTarget m_item;
    [SerializeField] EatingPosition m_pickUpLocation;

    [SerializeField] string m_actionName;
    [SerializeField] float m_beginRange;
    [SerializeField] float m_completeTime;
    [SerializeField] float m_effectTime;
    [SerializeField] string m_beginAnimID;
    [SerializeField] string m_completeAnimID;

    [SerializeField] float m_beginRangeError = 0.01f;

    public string actionName { get { return m_actionName; } }
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

    public IGOAPActionTarget FindActionTarget()
    {
        return m_pickUpLocation;
    }

    #region GOAPAction
    public bool CheckCondition(GOAPWorldState worldState)
    {
        worldState.GetValue<FoodEnum>((int)ChefWorldStateEnum.Chef_HoldItem, out var item);
        worldState.GetValue<Object>((int)ChefWorldStateEnum.Chef_HoldRef, out var holdRef);
        return item == FoodEnum.None && holdRef == null;
    }

    public int AddEffects(GOAPWorldState worldState)
    {
        //worldState.SetValue<FoodEnum>((int)ChefWorldStateEnum.Chef_HoldItem, FoodEnum.Plate);
        worldState.SetValue<int>((int)ChefWorldStateEnum.Chef_HoldItem, (int)FoodEnum.Plate);
        worldState.SetValue<Object>((int)ChefWorldStateEnum.Chef_HoldRef, m_item);
        return 0;
    }

    public float GetWeight()
    {
        return 1.0f;
    }
    #endregion // GOAPAction

    public void PerformAgentEffects(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        var chef = ((ChefAgent)goapAgent);

        m_item.tablePosition.RemoveItem();
        chef.SetItemToHoldPosition(m_item);
        chef.SetArmsToHold();
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

    public void SetPickUpPoint(EatingPosition pickUpLocation)
    {
        m_pickUpLocation = pickUpLocation;
    }
}
