using BBB.GOAP.PlannerInternal;
using BBB.GOAP;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ChefAgent : MonoBehaviour, IGOAPAgentElements
{
    //[SerializeField] ChefGlobalWorldState m_globalWorldState;
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_agentWorldState;
    [SerializeField] AgentMoveMachine<ChefAgent, ChefWorldStateEnum> m_movementMachine;

    GOAPWorldState m_combinedState = new GOAPWorldState();

    public AgentMoveMachine<ChefAgent, ChefWorldStateEnum> movementMachine { get { return m_movementMachine; } }
    public Queue<IGOAPAgentActionMethods> actionPlan { get { return m_movementMachine.actionPlan; } }

    public IGOAPAction currentAction { get { return m_movementMachine.currentAction; } }

    [SerializeField] PlateState m_targetPlate;
    [SerializeField][RequireInterface(typeof(IGOAPAgentActionMethods))] List<Object> m_actionObjects = new List<Object>();
    HashSet<IGOAPAction> m_actions;
    //Queue<IGOAPAction> m_actionPlan;

    HashSet<IGOAPAction> m_plateUnionActions = new HashSet<IGOAPAction>();

    IGOAPGoal m_currentGoal;
    public IGOAPGoal currentGoal { get { return m_currentGoal; } }


    [SerializeField] Vector3 m_holdPosition = Vector3.zero;
    [SerializeField] Vector3 m_holdRotation = Vector3.zero;

    public string debug_animationID = "";

    [SerializeField][RequireInterface(typeof(IGOAPAgentActionMethods))] List<Object> debug_deliveryActions = new List<Object>();

    [SerializeField] TextBillboard m_billBoard;

    [SerializeField] int m_animHoldArmsLayer = 0;

    public UnityEvent<ChefAgent, IGOAPActionTarget, IGOAPAgentActionMethods> completeActionEvent { get { return m_movementMachine.completeActionEvent; } }
    public UnityEvent<ChefAgent> completePlanEvent { get { return m_movementMachine.completePlanEvent; } }

    [SerializeField] UnityEvent<ChefAgent> m_onDestroyEvent;
    public UnityEvent<ChefAgent> onDestroyEvent { get { return m_onDestroyEvent; } }

    [SerializeField] Transform m_billboardPosition;
    public Transform billboardPosition { get { return m_billboardPosition; } }

    private void Awake()
    {
        m_movementMachine.Init(this, m_combinedState, GetPosition);

        m_actions = new HashSet<IGOAPAction>();
        for (int i = 0; i < m_actionObjects.Count; i++)
        {
            m_actions.Add(m_actionObjects[i] as IGOAPAction);
        }
    }

    private void OnDestroy()
    {
        m_onDestroyEvent.Invoke(this);
    }

    // Update is called once per frame
    void Update()
    {
        m_movementMachine.Update();
    }

    public GOAPWorldState GetCombinedGlobalWorldState(GOAPWorldState plateState)
    {
        m_combinedState.Clear();
        m_combinedState.Combine(m_agentWorldState);
        m_combinedState.Combine(plateState);
        //m_combinedState.Combine(m_globalWorldState.globalWorldState);

        return m_combinedState;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public void SetActionPlan(Queue<IGOAPAgentActionMethods> actions)
    {
        m_movementMachine.SetActionPlan(actions);
    }

    public static bool FindAndBeginActionPlan(ChefAgent agent, PlateState targetPlate)
    {
        var actionPlan = agent.FindActionPlan(targetPlate);
        if (actionPlan != null && actionPlan.Count > 0)
        {
            targetPlate.PopAction();
            agent.movementMachine.SetActionPlan(ConvertActionPlan(actionPlan));
            agent.movementMachine.Begin();
            return true;
        }

        return false;
    }

    Queue<IGOAPAction> FindActionPlan(PlateState targetPlate)
    {
        m_targetPlate = targetPlate;

        var combinedWorldState = GetCombinedGlobalWorldState(targetPlate.GetWorldState());

        // Need to have some way of seacrching for a goal.
        var plateAction = targetPlate.PeekAction();
        if (plateAction != null)
        {
            UnionPlateActions(targetPlate);

            plateAction.SetGoalForAgentState(combinedWorldState);
            m_currentGoal = plateAction;
            return GOAPPlanner<IGOAPAction>.GetActionPlan(combinedWorldState, plateAction, m_plateUnionActions);
        }
        else
        {
            // Goal is null
            m_currentGoal = null;
            return null;
        }
    }

    public static void FindAndBeginDelivery(ChefAgent agent, PlateState targetPlate)
    {
        var actionPlan = agent.FindDeliveryPlan(targetPlate);
        if (actionPlan != null)
        {
            agent.movementMachine.SetActionPlan(ConvertActionPlan(actionPlan));
            agent.movementMachine.Begin();
        }
    }

    Queue<IGOAPAction> FindDeliveryPlan(PlateState targetPlate)
    {
        m_targetPlate = targetPlate;

        var combinedWorldState = GetCombinedGlobalWorldState(targetPlate.GetWorldState());

        //if (targetPlate.PlateIsFinished())
        //{
        //    UnionPlateDeliveryActions(targetPlate);
        //    return GOAPPlanner<IGOAPAction>.GetActionPlan(combinedWorldState, targetPlate.deliverAction, m_plateUnionActions);
        //}
        //
        //return null;

        UnionPlateDeliveryActions(targetPlate);
        m_currentGoal = targetPlate.deliverAction;
        return GOAPPlanner<IGOAPAction>.GetActionPlan(combinedWorldState, targetPlate.deliverAction, m_plateUnionActions);
    }

    static Queue<IGOAPAgentActionMethods> ConvertActionPlan(Queue<IGOAPAction> actionPlan)
    {
        var plan = new Queue<IGOAPAgentActionMethods>();
        foreach (var action in actionPlan)
        {
            plan.Enqueue(action as IGOAPAgentActionMethods);
        }
        return plan;
    }

    void UnionPlateActions(PlateState targetPlate)
    {
        m_plateUnionActions.Clear();
        m_plateUnionActions.UnionWith(m_actions);
        m_plateUnionActions.UnionWith(targetPlate.GetAgentPlateActions());
    }

    void UnionPlateDeliveryActions(PlateState targetPlate)
    {
        m_plateUnionActions.Clear();

        foreach (var action in debug_deliveryActions)
        {
            m_plateUnionActions.Add(action as IGOAPAction);
        }

        //m_plateUnionActions.UnionWith(m_actions);
        targetPlate.DeliveryActionsUnion(m_plateUnionActions);
    }

    public static void PlayDebugAnimation(ChefAgent agent)
    {
        agent.m_currentGoal = null;
        if (agent.debug_animationID.Length > 0)
        {
            agent.m_movementMachine.animator.CrossFade(agent.debug_animationID, 0.1f);
        }
    }

    public void SetItemToHoldPosition(ChefActionTarget target)
    {
        target.transform.parent = transform;
        target.transform.SetLocalPositionAndRotation(m_holdPosition, Quaternion.Euler(m_holdRotation));
    }

    public void SetBillboardCamera(Camera camera)
    {
        m_billBoard.SetCamera(camera);
    }

    public void ForceUnsafeAction(IGOAPAgentActionMethods action)
    {
        Queue<IGOAPAgentActionMethods> actionPlan = new Queue<IGOAPAgentActionMethods>();
        actionPlan.Enqueue(action);
        movementMachine.SetActionPlan(actionPlan);
        movementMachine.Begin();
    }

    public void SetArmsToHold()
    {
        m_movementMachine.animator.SetLayerWeight(m_animHoldArmsLayer, 1);
    }

    public void SetArmsToNeutral()
    {
        m_movementMachine.animator.SetLayerWeight(m_animHoldArmsLayer, 0);
    }
}
