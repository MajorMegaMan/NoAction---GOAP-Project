using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefRecipe : MonoBehaviour, IGOAPGoal
{
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_plateState;

    [SerializeField] List<RecipePlateAction> m_plateActions;

    [SerializeField] int m_maxDepth = 10;

    [Header("Allocation")]
    [SerializeField] int m_averageActionCount = 4;
    [SerializeField] int m_averageDepthCount = 5;

    public GOAPWorldState plateRecipe { get { return m_plateState; } }
    public int actionCount { get { return m_plateActions.Count; } }

    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    public int AllocatePlanningStates()
    {
        int count = (int)Mathf.Pow(m_averageActionCount, m_averageDepthCount);
        GOAPPlanner<RecipePlateAction>.ExpandPlanningPool(m_plateState, count);
        return count;
    }

    public Queue<RecipePlateAction> FindRecipePlan(GOAPWorldState plateState)
    {
        return GOAPPlanner<RecipePlateAction>.GetActionPlan(plateState, this, m_plateActions, m_maxDepth);
    }

    public bool CheckWorldState(GOAPWorldState worldState)
    {
        return m_plateState.Compare(worldState);
    }
}
