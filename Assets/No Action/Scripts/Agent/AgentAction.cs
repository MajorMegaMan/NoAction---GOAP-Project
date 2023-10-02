using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AgentAction<TKey> : ScriptableObject, IGOAPAction, IGOAPAgentActionMethods
{
    [SerializeField] string m_actionName = "New Action";

    public List<ActionCondition<TKey>> m_conditions;
    public List<ActionEffect<TKey>> m_effects;

    [SerializeField] AgentActionData m_agentData;

    public string actionName { get { return m_actionName; } }
    public bool shouldMoveTo { get { return m_agentData.beginRange != 0.0f; } }
    public float beginRange { get { return m_agentData.beginRange; } }
    public float completeTime { get { return m_agentData.completeTime; } }
    public float effectTime { get { return m_agentData.effectTime; } }
    public string beginAnimID { get { return m_agentData.beginAnimID; } }
    public string completeAnimID { get { return m_agentData.completeAnimID; } }

    static Dictionary<AgentAction<TKey>, List<IGOAPActionTarget>> _regisiteredActionTargets = new Dictionary<AgentAction<TKey>, List<IGOAPActionTarget>>();

    #region AgentAction
    public bool InBeginRange(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_agentData.InBeginRange(goapAgent, actionTarget);
    }

    public Vector3 CalcBeginPosition(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {
        return m_agentData.CalcBeginPosition(goapAgent, actionTarget);
    }

    public bool PlayBeginAnim(Animator animator)
    {
        return m_agentData.PlayBeginAnim(animator);
    }

    public bool PlayCompleteAnim(Animator animator)
    {
        return m_agentData.PlayCompleteAnim(animator);
    }

    public IGOAPActionTarget FindActionTarget()
    {
        if (_regisiteredActionTargets.TryGetValue(this, out var targetList))
        {
            // find an action Target for this action.
            for (int i = 0; i < targetList.Count; i++)
            {
                // This is a dumb return the first element. Was meant to have some sorting based on distance or something.
                return targetList[i];
            }
        }

        return null;
    }

    public void PerformAgentEffects(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget)
    {

    }
    #endregion // AgentAction

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

    #region Register
    public static void RegisterActionTarget(IGOAPActionTarget actionTarget, AgentAction<TKey> action)
    {
        if (_regisiteredActionTargets.TryGetValue(action, out var registerList))
        {
            registerList.Add(actionTarget);
        }
        else
        {
            registerList = new List<IGOAPActionTarget>();
            registerList.Add(actionTarget);
            _regisiteredActionTargets.Add(action, registerList);
        }
    }

    public static void DeregisterActionTarget(IGOAPActionTarget actionTarget, AgentAction<TKey> action)
    {
        if (_regisiteredActionTargets.TryGetValue(action, out var registerList))
        {
            registerList.Remove(actionTarget);
        }
    }

    public static void RegisterActionTargets(IGOAPActionTarget actionTarget, List<AgentAction<TKey>> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (_regisiteredActionTargets.TryGetValue(actions[i], out var registerList))
            {
                registerList.Add(actionTarget);
            }
            else
            {
                registerList = new List<IGOAPActionTarget>();
                registerList.Add(actionTarget);
                _regisiteredActionTargets.Add(actions[i], registerList);
            }
        }
    }

    public static void DeregisterActionTargets(IGOAPActionTarget actionTarget, List<AgentAction<TKey>> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (_regisiteredActionTargets.TryGetValue(actions[i], out var registerList))
            {
                registerList.Remove(actionTarget);
            }
        }
    }
    #endregion // Register
}
