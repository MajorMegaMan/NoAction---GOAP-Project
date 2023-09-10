using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefRecipe : MonoBehaviour, IGOAPGoal
{
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_plateState;

    [SerializeField] List<RecipePlateAction> m_plateActions;

    public GOAPWorldState plateRecipe { get { return m_plateState; } }
    public int actionCount { get { return m_plateActions.Count; } }

    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    public Queue<RecipePlateAction> FindRecipePlan(GOAPWorldState plateState)
    {
        return GOAPPlanner<RecipePlateAction>.GetActionPlan(plateState, this, m_plateActions);
    }

    public bool CheckWorldState(GOAPWorldState worldState)
    {
        return m_plateState.Compare(worldState);
    }
}
