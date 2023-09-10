using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GOAP/Scriptable Goal", fileName = "New Goal")]
public class DynamicGoal : ScriptableObject, IGOAPGoal
{
    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }
    public List<ActionCondition<TestEnum>> m_conditions;

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
