using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Chef/Chef Goal", fileName = "New Chef Goal")]
public class ChefGoal : ScriptableObject, IGOAPGoal
{
    public List<ActionCondition<ChefWorldStateEnum>> m_conditions;

    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    public bool CheckWorldState(GOAPWorldState worldState)
    {
        foreach (var condition in m_conditions)
        {
            if (worldState.GetObjectValue(condition.value.key.id, out var worldValue))
            {
                if (!condition.Compare(worldValue))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}