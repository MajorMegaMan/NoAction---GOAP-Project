using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[System.Serializable]
public class AgentMoveMachine<TOwner, TKey> : IGOAPAgentElements where TOwner : IGOAPAgentElements
{
    TOwner m_owner;
    GOAPWorldState m_agentWorldState;

    IGOAPActionTarget m_currentActionTarget;
    Queue<IGOAPAgentActionMethods> m_actionPlan;
    IGOAPAgentActionMethods m_currentAction;
    float m_actionTimer = 0.0f;
    float m_actionTime = 0.0f;
    float m_actionEffectTime = 0.0f;
    bool m_performedEffects = false;

    [Header("Movement var")]
    [SerializeField] NavMeshAgent m_navAgent;

    [SerializeField] float m_arriveRadius = 1.0f;

    StateMachine<AgentMoveMachine<TOwner, TKey>> m_moveStateMachine;
    MoveToState m_moveState;
    PerformActionState m_actionState;
    static EmptyState _emptyState = new EmptyState();

    public delegate Vector3 PositionGetter();
    PositionGetter m_positionGetter;

    [SerializeField] Animator m_anim;
    [SerializeField] string m_movementStateID = "Movement";
    [SerializeField] string m_movementAnimSpeedID = "MoveSpeed";
    [SerializeField] string m_movementAnimOverdriveID = "MoveOverSpeed";
    [SerializeField] AnimationCurve m_movementCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
    [SerializeField] float m_targetWalkSpeed = 1.0f;

    [Header("Events")]
    [SerializeField] UnityEvent<TOwner> m_beginPlanEvent;
    [SerializeField] UnityEvent<TOwner, IGOAPAgentActionMethods> m_popActionEvent;
    [SerializeField] UnityEvent<TOwner, IGOAPActionTarget, IGOAPAgentActionMethods> m_completeActionEvent;
    [SerializeField] UnityEvent<TOwner> m_completePlanEvent;

    public IGOAPAction currentAction { get { return m_currentAction as IGOAPAction; } }
    public Animator animator { get { return m_anim; } }

    public Queue<IGOAPAgentActionMethods> actionPlan { get { return m_actionPlan; } }

    public UnityEvent<TOwner> beginPlanEvent { get { return m_beginPlanEvent; } }
    public UnityEvent<TOwner, IGOAPAgentActionMethods> popActionEvent { get { return m_popActionEvent; } }
    public UnityEvent<TOwner, IGOAPActionTarget, IGOAPAgentActionMethods> completeActionEvent { get { return m_completeActionEvent; } }
    public UnityEvent<TOwner> completePlanEvent { get { return m_completePlanEvent; } }

    public void Init(TOwner owner, GOAPWorldState agentWorldState, PositionGetter positionGetter)
    {
        m_owner = owner;
        m_agentWorldState = agentWorldState;

        m_moveStateMachine = new StateMachine<AgentMoveMachine<TOwner, TKey>>();
        m_moveState = new MoveToState();
        m_actionState = new PerformActionState();
        m_moveStateMachine.InitialiseState(this, _emptyState);

        m_positionGetter = positionGetter;
    }

    public Vector3 GetPosition()
    {
        return m_positionGetter.Invoke();
    }

    public void Update()
    {
        m_moveStateMachine.Invoke(this);
    }

    public void Begin()
    {
        PopAction();
        SetToAction();
        m_beginPlanEvent.Invoke(m_owner);
    }

    public void Stop()
    {
        ClearActions();
        SetToEmpty();
    }

    #region MoveStateMachine
    void SetTargetMovePosition(Vector3 position)
    {
        m_navAgent.SetDestination(position);
    }

    void SetToMove()
    {
        m_moveStateMachine.SetState(this, m_moveState);
        m_anim.CrossFade(m_movementStateID, 0.1f);
    }

    void SetToAction()
    {
        if (m_currentAction == null)
        {
            // No Plan found.
            SetToEmpty();
            return;
        }
        m_moveStateMachine.SetState(this, m_actionState);
    }

    void SetToEmpty()
    {
        m_moveStateMachine.SetState(this, _emptyState);
    }

    void MoveTo(Vector3 position)
    {
        SetTargetMovePosition(position);
        SetToMove();
    }

    void UpdateMove()
    {
        float t = m_navAgent.velocity.magnitude / m_targetWalkSpeed;
        t = m_movementCurve.Evaluate(t);
        if(t < 1.0f)
        {
            m_anim.SetFloat(m_movementAnimSpeedID, t);
            m_anim.SetFloat(m_movementAnimOverdriveID, 1);
        }
        else
        {
            m_anim.SetFloat(m_movementAnimSpeedID, 1);
            m_anim.SetFloat(m_movementAnimOverdriveID, t + 1);
        }
    }
    #endregion // MoveStateMachine

    #region Actions
    public void SetActionPlan(Queue<IGOAPAgentActionMethods> actionPlan)
    {
        m_actionPlan = actionPlan;
    }

    bool HasArrived()
    {
        if (m_navAgent.pathPending)
        {
            return false;
        }
        return m_navAgent.remainingDistance <= m_arriveRadius;
    }

    void Arrive()
    {
        // Should decide behaviour.

        // for now do empty action.
        SetToAction();
    }

    void BeginCurrentAction()
    {
        m_currentActionTarget = m_currentAction.FindActionTarget();

        if (m_currentAction.InBeginRange(this, m_currentActionTarget))
        {
            // Play animations
            m_actionTime = m_currentAction.completeTime;
            m_actionTimer = 0.0f;
            m_actionEffectTime = m_currentAction.effectTime;
            m_performedEffects = false;

            if(!m_currentAction.PlayBeginAnim(m_anim))
            {
                // There is no begin animation/ Play Idle instead
                //m_anim.CrossFade(m_movementStateID, 0.1f);
                m_anim.SetFloat(m_movementAnimSpeedID, 0);
                m_anim.SetFloat(m_movementAnimOverdriveID, 1);
            }
        }
        else
        {
            // Not in range yet. Move To action position.
            MoveTo(m_currentAction.CalcBeginPosition(this, m_currentActionTarget));
        }
    }

    void ProcessCurrentAction()
    {
        m_actionTimer += Time.deltaTime;
        if (!m_performedEffects && m_actionTimer > m_actionEffectTime)
        {
            m_currentAction.AddEffects(m_agentWorldState);
            if(m_currentActionTarget != null)
            {
                m_currentAction.PerformAgentEffects(m_owner, m_currentActionTarget);
                m_currentActionTarget.PerfromActionOnTarget(m_owner, m_currentAction);
            }
            m_performedEffects = true;
        }
        if (m_actionTimer > m_actionTime)
        {
            // Play complete animation
            if(!m_currentAction.PlayCompleteAnim(m_anim))
            {
                // There is no complete animation/ Play Idle instead
                m_anim.CrossFade(m_movementStateID, 0.1f);
                m_anim.SetFloat(m_movementAnimSpeedID, 0);
                m_anim.SetFloat(m_movementAnimOverdriveID, 1);
            }
            EndAction();
        }
    }

    void EndAction()
    {
        m_completeActionEvent.Invoke(m_owner, m_currentActionTarget, m_currentAction);

        // Action has ended.
        PopAction();
        SetToAction();
    }

    void PopAction()
    {
        if (m_actionPlan.Count > 0)
        {
            m_currentAction = m_actionPlan.Dequeue();
            m_popActionEvent.Invoke(m_owner, m_currentAction);
        }
        else
        {
            // Plan is complete
            m_currentAction = null;
            m_completePlanEvent.Invoke(m_owner);
        }
    }

    void ClearActions()
    {
        m_actionPlan.Clear();
        m_currentAction = null;
        //PopAction();
    }
    #endregion // Actions

    class MoveToState : IState<AgentMoveMachine<TOwner, TKey>>
    {
        void IState<AgentMoveMachine<TOwner, TKey>>.Enter(AgentMoveMachine<TOwner, TKey> owner)
        {
            owner.m_navAgent.isStopped = false;
        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Exit(AgentMoveMachine<TOwner, TKey> owner)
        {
            owner.m_navAgent.isStopped = true;
        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Invoke(AgentMoveMachine<TOwner, TKey> owner)
        {
            owner.UpdateMove();
            if (owner.HasArrived())
            {
                owner.Arrive();
            }
            //owner.UpdateMove();
        }
    }

    class PerformActionState : IState<AgentMoveMachine<TOwner, TKey>>
    {
        void IState<AgentMoveMachine<TOwner, TKey>>.Enter(AgentMoveMachine<TOwner, TKey> owner)
        {
            owner.BeginCurrentAction();
        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Exit(AgentMoveMachine<TOwner, TKey> owner)
        {

        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Invoke(AgentMoveMachine<TOwner, TKey> owner)
        {
            owner.ProcessCurrentAction();
        }
    }

    class EmptyState : IState<AgentMoveMachine<TOwner, TKey>>
    {
        void IState<AgentMoveMachine<TOwner, TKey>>.Enter(AgentMoveMachine<TOwner, TKey> owner)
        {

        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Exit(AgentMoveMachine<TOwner, TKey> owner)
        {

        }

        void IState<AgentMoveMachine<TOwner, TKey>>.Invoke(AgentMoveMachine<TOwner, TKey> owner)
        {

        }
    }
}
