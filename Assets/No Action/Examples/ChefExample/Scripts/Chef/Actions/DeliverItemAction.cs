using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliverItemAction : MonoBehaviour, IGOAPAction, IGOAPAgentActionMethods, IGOAPGoal
{
    [SerializeField] string m_actionName = "New Action";

    [SerializeField] PlateState m_plate;
    [SerializeField] EatingPosition m_deliveryLocation;

    [SerializeField] AgentActionData m_agentActionData;

    [SerializeField] ChefWorldStateEnum m_deliverBool = ChefWorldStateEnum.Plate_Delivered;

    public string actionName { get { return m_actionName; } }
    public bool shouldMoveTo { get { return m_agentActionData.beginRange != 0.0f; } }
    public float beginRange { get { return m_agentActionData.beginRange; } }
    public float completeTime { get { return m_agentActionData.completeTime; } }
    public float effectTime { get { return m_agentActionData.effectTime; } }
    public string beginAnimID { get { return m_agentActionData.beginAnimID; } }
    public string completeAnimID { get { return m_agentActionData.completeAnimID; } }

    public EatingPosition deliveryLocation { get { return m_deliveryLocation; } }

    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    public bool InBeginRange(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_agentActionData.InBeginRange(goapAgent, actionTarget);
    }

    public Vector3 CalcBeginPosition(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_agentActionData.CalcBeginPosition(goapAgent, actionTarget);
    }

    public IGOAPActionTarget FindActionTarget()
    {
        return m_deliveryLocation;
    }

    #region GOAPAction
    public bool CheckCondition(GOAPWorldState worldState)
    {
        worldState.GetValue<FoodEnum>((int)ChefWorldStateEnum.Chef_HoldItem, out var item);
        worldState.GetValue<Object>((int)ChefWorldStateEnum.Chef_HoldRef, out var holdRef);
        return item == FoodEnum.Plate && holdRef == m_plate;
    }

    public int AddEffects(GOAPWorldState worldState)
    {
        //worldState.SetValue<FoodEnum>((int)ChefWorldStateEnum.Chef_HoldItem, FoodEnum.None);
        worldState.SetValue<int>((int)ChefWorldStateEnum.Chef_HoldItem, (int)FoodEnum.None);
        worldState.SetValue<Object>((int)ChefWorldStateEnum.Chef_HoldRef, null);
        worldState.SetValue<bool>((int)m_deliverBool, true);
        return 0;
    }

    public float GetWeight()
    {
        return 1.0f;
    }
    #endregion // GOAPAction

    public void PerformAgentEffects(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        //m_deliveryLocation.SetItemToEatPoint(actionTarget as ChefActionTarget);
        m_deliveryLocation.SetItemToEatPoint(m_plate);
        m_plate.SetPickUpPoint(m_deliveryLocation);

        // Force deliver bool to be false so that the plate can be delivered elsewhere.
        m_plate.GetWorldState().SetValue<bool>((int)m_deliverBool, false);

        var chef = ((ChefAgent)goapAgent);
        chef.SetArmsToNeutral();
    }

    public bool PlayBeginAnim(Animator animator)
    {
        return m_agentActionData.PlayBeginAnim(animator);
    }

    public bool PlayCompleteAnim(Animator animator)
    {
        return m_agentActionData.PlayCompleteAnim(animator);
    }

    public bool CheckWorldState(GOAPWorldState worldState)
    {
        return worldState.GetValue<bool>((int)m_deliverBool, out bool deliverValue) && deliverValue;
    }

    public void SetDeliveryPoint(EatingPosition deliveryPoint)
    {
        m_deliveryLocation = deliveryPoint;
    }
}
