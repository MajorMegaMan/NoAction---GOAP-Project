using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionData<TKey>
{
    [SerializeField] string m_actionName = "New Action";

    public List<ActionCondition<TKey>> m_conditions;
    public List<ActionEffect<TKey>> m_effects;

    public string actionName { get { return m_actionName; } }

    #region GOAPAction
    public bool CheckCondition(GOAPWorldState worldState)
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

    public int AddEffects(GOAPWorldState worldState)
    {
        foreach (var effect in m_effects)
        {
            if (worldState.GetObjectValue(effect.value.key.id, out var worldValue))
            {
                effect.Apply(ref worldValue);
                worldState.SetValue(effect.value.key.id, worldValue);
            }
        }
        return 0;
    }

    public float GetWeight()
    {
        return 1.0f;
    }
    #endregion // GOAPAction
}
