using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefActionTarget : MonoBehaviour, IGOAPActionTarget
{
    [SerializeField] List<ActionEffectPair> m_actionEffects;
    Dictionary<AgentAction<ChefWorldStateEnum>, UnityEvent<IGOAPAgentElements>> m_actionEffectDictionary;

    //public IGOAPAction currentAction { get { return null; } } // Return null for now. But the action being performed on it would be cool.

    public EatingPosition tablePosition = null;

    public string targetName { get { return name; } }

    [System.Serializable]
    struct ActionEffectPair
    {
        [SerializeField][RequireInterface(typeof(IGOAPAction))] internal Object m_actionObject;
        [SerializeField] internal UnityEvent<IGOAPAgentElements> m_onPerformActionEvent;

        public AgentAction<ChefWorldStateEnum> action { get { return m_actionObject as AgentAction<ChefWorldStateEnum>; } }
    }

    private void Awake()
    {
        m_actionEffectDictionary = new Dictionary<AgentAction<ChefWorldStateEnum>, UnityEvent<IGOAPAgentElements>>();
        foreach (var pair in m_actionEffects)
        {
            m_actionEffectDictionary.Add(pair.action, pair.m_onPerformActionEvent);
        }
    }

    private void OnEnable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentAction<ChefWorldStateEnum>.RegisterActionTarget(this, pair.action);
        }
    }

    private void OnDisable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentAction<ChefWorldStateEnum>.DeregisterActionTarget(this, pair.action);
        }
    }

    public void PerfromActionOnTarget(IGOAPAgentElements agent, IGOAPAgentActionMethods action)
    {
        var agentAction = action as AgentAction<ChefWorldStateEnum>;
        if (agentAction == null)
        {
            return;
        }

        if (m_actionEffectDictionary.TryGetValue(agentAction, out var performEvent))
        {
            performEvent.Invoke(agent);
        }
    }

    public bool ContainsAction(AgentAction<ChefWorldStateEnum> action)
    {
        return m_actionEffectDictionary.ContainsKey(action);
    }

    public void SetPositionToAgent(IGOAPAgentElements agent)
    {
        transform.position = agent.GetPosition();
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
