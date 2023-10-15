using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefActionTarget : MonoBehaviour, IGOAPActionTarget
{
    [SerializeField] UnityEvent<IGOAPAgentElements> m_onPerformActionEvent;
    [SerializeField] List<ActionEffectPair> m_actionEffects;
    Dictionary<IGOAPAgentActionMethods, UnityEvent<IGOAPAgentElements>> m_actionEffectDictionary;

    //public IGOAPAction currentAction { get { return null; } } // Return null for now. But the action being performed on it would be cool.

    public EatingPosition tablePosition = null;

    public string targetName { get { return name; } }

    [System.Serializable]
    struct ActionEffectPair
    {
        [SerializeField][RequireInterface(typeof(IGOAPAction))] internal Object m_actionObject;
        [SerializeField] internal UnityEvent<IGOAPAgentElements> m_onPerformActionEvent;

        public IGOAPAgentActionMethods action { get { return m_actionObject as IGOAPAgentActionMethods; } }
    }

    private void Awake()
    {
        m_actionEffectDictionary = new Dictionary<IGOAPAgentActionMethods, UnityEvent<IGOAPAgentElements>>();
        foreach (var pair in m_actionEffects)
        {
            m_actionEffectDictionary.Add(pair.action, pair.m_onPerformActionEvent);
        }
    }

    private void OnEnable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentActionRegister.RegisterActionTarget(this, pair.action);
        }
    }

    private void OnDisable()
    {
        foreach (var pair in m_actionEffects)
        {
            AgentActionRegister.DeregisterActionTarget(this, pair.action);
        }
    }

    public void PerfromActionOnTarget(IGOAPAgentElements agent, IGOAPAgentActionMethods action)
    {
        m_onPerformActionEvent.Invoke(agent);

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
