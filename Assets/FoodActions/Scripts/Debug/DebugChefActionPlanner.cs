using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChefActionPlanner : MonoBehaviour
{
    [SerializeField] ChefAgent m_agent;

    [SerializeField][RequireInterface(typeof(IGOAPAgentActionMethods))] List<Object> m_actionObjects = new List<Object>();
    List<IGOAPAction> m_actions;

    Queue<IGOAPAction> m_actionPlan;

    [SerializeField]
    [RequireInterface(typeof(IGOAPGoal))] Object m_goalObject;

    [SerializeField]
    [RequireInterface(typeof(IGOAPAgentActionMethods))] List<Object> debug_actionObjects;

    private void Awake()
    {
        m_actions = new List<IGOAPAction>();
        foreach(var actionObject in m_actionObjects)
        {
            m_actions.Add(actionObject as IGOAPAction);
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            FindActionPlan(m_agent.GetCombinedGlobalWorldState(new GOAPWorldState()));
            m_agent.movementMachine.SetActionPlan(GetActionPlan());
            m_agent.movementMachine.Begin();
        }
    }

    IGOAPGoal FindGoal()
    {
        // Need to have some way of seacrching for a goal.
        return m_goalObject as IGOAPGoal;
    }

    public void FindActionPlan(GOAPWorldState worldState)
    {
        m_actionPlan = GOAPPlanner<IGOAPAction>.GetActionPlan(worldState, FindGoal(), m_actions);
    }

    public Queue<IGOAPAgentActionMethods> GetActionPlan()
    {
        var plan = new Queue<IGOAPAgentActionMethods>();
        foreach(var action in m_actionPlan)
        {
            plan.Enqueue(action as IGOAPAgentActionMethods);
        }
        return plan;
    }
}
