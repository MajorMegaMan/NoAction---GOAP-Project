using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class DebugAgentPlateActions : MonoBehaviour
{
    [SerializeField] ChefAgent m_agent;

    [SerializeField] PlateState m_targetPlate;

    Queue<IGOAPAction> m_actionPlan;

    [SerializeField][RequireInterface(typeof(IGOAPAgentActionMethods))] List<Object> m_actionObjects = new List<Object>();
    List<IGOAPAction> m_actions;

    private void Awake()
    {
        m_actions = new List<IGOAPAction>();
        foreach (var actionObject in m_actionObjects)
        {
            m_actions.Add(actionObject as IGOAPAction);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            FindAndBeginActionPlan();
        }
    }

    public bool FindAndBeginActionPlan()
    {
        if (FindActionPlan(m_agent.GetCombinedGlobalWorldState(m_targetPlate.GetWorldState())))
        {
            m_agent.movementMachine.SetActionPlan(GetActionPlan());
            m_agent.movementMachine.Begin();
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool FindActionPlan(GOAPWorldState worldState)
    {
        // Need to have some way of seacrching for a goal.
        var plateAction = m_targetPlate.PopAction();
        if (plateAction != null)
        {
            plateAction.SetGoalForAgentState(worldState);
            m_actionPlan = GOAPPlanner<IGOAPAction>.GetActionPlan(worldState, plateAction, m_actions);
            return true;
        }
        else
        {
            // Goal is null
            return false;
        }
    }

    public Queue<IGOAPAgentActionMethods> GetActionPlan()
    {
        var plan = new Queue<IGOAPAgentActionMethods>();
        foreach (var action in m_actionPlan)
        {
            plan.Enqueue(action as IGOAPAgentActionMethods);
        }
        return plan;
    }
}
