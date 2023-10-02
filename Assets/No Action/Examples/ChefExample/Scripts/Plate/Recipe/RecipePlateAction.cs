using BBB.GOAP;
using BBB.GOAP.PlannerInternal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chef/Recipe Action", fileName = "New Recipe Action")]
public class RecipePlateAction : ScriptableObject, IGOAPAction, IGOAPGoal
{
    // This is not a Chef Agent aciton. This action is used to plan what actions need to be taken for the plate to match it's goal state.
    // Rather than a chef searching for a specific path of actions to the fufilled plate, the agent will instead use this action as it's goal.
    // After the chef has succeeded in this action the plate, the chef will then perfom the next RecipeAction until the plate is filled.
    // It's split up like this in order to reduce the complexity of the planning by splitting it into more managable parts.
    [SerializeField] ActionData<ChefWorldStateEnum> m_actionData;

    public string actionName { get { return m_actionData.actionName; } }

    GOAPWorldState m_agentCopystate = new GOAPWorldState();

    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    #region GOAPAction
    public bool CheckCondition(GOAPWorldState worldState)
    {
        return m_actionData.CheckCondition(worldState);
    }

    public int AddEffects(GOAPWorldState worldState)
    {
        return m_actionData.AddEffects(worldState);
    }

    public float GetWeight()
    {
        return m_actionData.GetWeight();
    }
    #endregion // GOAPAction

    public void SetGoalForAgentState(GOAPWorldState agentState)
    {
        // Copy values from agent state.
        m_agentCopystate.Clear();
        m_agentCopystate.FillEmpty(agentState);
        m_agentCopystate.CopyValues(agentState);

        // Add effects to agent state. This can now used as the goal for the specified agent.
        AddEffects(m_agentCopystate);
    }

    // Should call SetGoalForAgentState before using this goal in planning methods.
    public bool CheckWorldState(GOAPWorldState worldState)
    {
        return m_agentCopystate.Compare(worldState);
    }
}
