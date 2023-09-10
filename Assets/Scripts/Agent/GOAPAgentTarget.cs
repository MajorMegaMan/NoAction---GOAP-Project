using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GOAPAgentTarget : MonoBehaviour, IGOAPActionTarget
{
    [SerializeField] List<ActionEffectPair> m_actionEffects;
    Dictionary<AgentAction<TestEnum>, UnityEvent<IGOAPAgentElements>> m_actionEffectDictionary;

    public string targetName { get { return name; } }

    [System.Serializable]
    struct ActionEffectPair
    {
        [SerializeField] internal AgentAction<TestEnum> m_action;
        [SerializeField] internal UnityEvent<IGOAPAgentElements> m_onPerformActionEvent;
    }

    private void Awake()
    {
        m_actionEffectDictionary = new Dictionary<AgentAction<TestEnum>, UnityEvent<IGOAPAgentElements>>();
        foreach(var pair in m_actionEffects)
        {
            m_actionEffectDictionary.Add(pair.m_action, pair.m_onPerformActionEvent);
        }
    }

    private void OnEnable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentAction<TestEnum>.RegisterActionTarget(this, pair.m_action);
        }
    }

    private void OnDisable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentAction<TestEnum>.DeregisterActionTarget(this, pair.m_action);
        }
    }

    public void PerfromActionOnTarget(IGOAPAgentElements agent, IGOAPAgentActionMethods action)
    {
        var agentAction = action as AgentAction<TestEnum>;
        if (agentAction == null)
        {
            return;
        }

        if (m_actionEffectDictionary.TryGetValue(agentAction, out var performEvent))
        {
            performEvent.Invoke(agent);
        }
    }

    public bool ContainsAction(AgentAction<TestEnum> action)
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
