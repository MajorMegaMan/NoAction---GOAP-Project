using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class CustomerAgent : MonoBehaviour
{
    [SerializeField] NavMeshAgent m_navAgent;
    [SerializeField] CustomerStatus m_status = 0;

    [SerializeField] float m_arriveRadius = 0.2f;

    [SerializeField] EatingPosition m_eatPosition;
    [SerializeField] float m_slideSpeed = 5.0f;
    [SerializeField] float m_lookSpeed = 1.0f;

    Vector3 m_targetWalkPosition = Vector3.zero;

    StateMachine<CustomerAgent> m_walkStateMachine;

    [SerializeField] float m_eatSpeed = 1.0f;
    [SerializeField] UnityEvent<CustomerAgent> m_finishEatingEvent;
    [SerializeField] UnityEvent<CustomerAgent> m_leaveEvent;

    PlateState m_targetPlate;
    bool m_foodWaiting = false;

    public PlateState plate { get { return m_targetPlate; } }
    public EatingPosition lastEatPosition { get { return m_eatPosition; } }
    public UnityEvent<CustomerAgent> finishEatingEvent { get { return m_finishEatingEvent; } }
    public bool foodWaiting { get { return m_foodWaiting; } }

    public UnityEvent<CustomerAgent> leaveEvent { get { return m_leaveEvent; } }

    [SerializeField] Vector3 debug_position;
    [SerializeField] UnityEvent<Vector3> debug_event;
    [SerializeField] float debug_float = 0.0f;

    [Header("Animation")]
    [SerializeField] Animator m_anim;
    [SerializeField] float m_targetWalkSpeed = 5.0f;
    [SerializeField] AnimationCurve m_movementCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField] Utility.AnimationStateID m_walkAnimStateID;
    [SerializeField] Utility.AnimationStateID m_eatAnimStateID;
    [SerializeField] Utility.AnimationID m_walkSpeedAnimParamID;
    [SerializeField] Utility.AnimationID m_walkOverDriveAnimParamID;

    [SerializeField] Utility.SmoothFloat m_smoothMoveSpeed;

    public delegate void ActionDelegate();
    ActionDelegate m_walkAction = Empty;
    ActionDelegate m_arriveAction = Empty;

    [SerializeField] SitMachine m_sitMachine;
    [SerializeField] SlideMachine m_slideMachine;
    [SerializeField] LookMachine m_lookMachine;

    static IState<CustomerAgent> _nothingState = new NothingState();
    static IState<CustomerAgent> _waitInLineState = new WaitInLineState();
    static IState<CustomerAgent> _orderingState = new OrderMealState();
    static IState<CustomerAgent> _waitingForFoodState = new WaitForFoodState();
    IState<CustomerAgent> m_eatingState = new EatingState();
    static IState<CustomerAgent> _leavingState = new LeavingState();

    enum CustomerStatus
    {
        Nothing,
        WaitingInLine,
        Ordering,
        WaitingForFood,
        EatingFood,
        Leaving,
    }

    private void Awake()
    {
        m_walkAnimStateID.InitHashID();
        m_walkSpeedAnimParamID.InitHashID();
        m_walkOverDriveAnimParamID.InitHashID();
        m_eatAnimStateID.InitHashID();

        m_walkStateMachine = new StateMachine<CustomerAgent>();
        m_walkStateMachine.InitialiseState(this, _nothingState);
    }

    [System.Serializable]
    class SitMachine
    {
        [SerializeField] float m_weight = 1.0f;
        [SerializeField] Utility.SmoothFloat m_smoothWeight = new Utility.SmoothFloat();
        [SerializeField] int m_layer = 0;

        delegate void SubStateDelegate(Animator anim);
        SubStateDelegate m_subStateAction;

        bool m_isSitting = false;

        public bool isSitting { get { return m_isSitting; } }

        public SitMachine()
        {
            m_subStateAction = Empty;
        }

        public void SetLayerWeight(Animator anim)
        {
            anim.SetLayerWeight(m_layer, m_smoothWeight.value);
        }

        void SmoothToSit(Animator anim)
        {
            float t = m_smoothWeight.Smooth(m_weight);

            if (t > 0.9999f && Mathf.Abs(m_smoothWeight.velocity) < 0.0001f)
            {
                anim.SetLayerWeight(m_layer, 1.0f);
                m_subStateAction = Empty;
            }

            anim.SetLayerWeight(m_layer, t);
        }

        void SmoothToStand(Animator anim)
        {
            float t = m_smoothWeight.Smooth(0.0f);

            if (t < 0.0001f && Mathf.Abs(m_smoothWeight.velocity) < 0.0001f)
            {
                anim.SetLayerWeight(m_layer, 0.0f);
                m_subStateAction = Empty;
            }

            anim.SetLayerWeight(m_layer, t);
        }

        static void Empty(Animator anim)
        {
            
        }

        public void Invoke(Animator anim)
        {
            m_subStateAction.Invoke(anim);
        }

        public void Sit()
        {
            m_subStateAction = SmoothToSit;
            m_isSitting = true;
        }

        public void Stand()
        {
            m_subStateAction = SmoothToStand;
            m_isSitting = false;
        }
    }

    [System.Serializable]
    class SlideMachine
    {
        [SerializeField] Vector3 m_start = Vector3.zero;
        [SerializeField] Vector3 m_target = Vector3.zero;

        [SerializeField] float m_lerp = 0.0f;
        [SerializeField] float m_timeScale = 0.0f;

        delegate void SubStateDelegate(CustomerAgent agent);
        SubStateDelegate m_subStateAction;

        public delegate void ArriveAction();
        ArriveAction m_arriveAction;

        public SlideMachine()
        {
            m_subStateAction = Empty;
            m_arriveAction = Empty;
        }

        void SmoothSlide(CustomerAgent agent)
        {
            m_lerp += Time.deltaTime * m_timeScale;
            if(m_lerp > 1.0f)
            {
                agent.transform.position = m_target;
                m_subStateAction = Empty;
                m_arriveAction.Invoke();
                m_arriveAction = Empty;
            }
            else
            {
                agent.transform.position = Vector3.Lerp(m_start, m_target, m_lerp);
            }
        }

        static void Empty(CustomerAgent agent)
        {

        }

        static void Empty()
        {

        }

        public void Invoke(CustomerAgent agent)
        {
            m_subStateAction.Invoke(agent);
        }

        public void Slide(CustomerAgent agent, Vector3 target, float time)
        {
            m_subStateAction = SmoothSlide;
            m_start = agent.transform.position;
            m_target = target;
            m_timeScale = (m_start - m_target).magnitude * time;
            if(m_timeScale != 0.0f)
            {
                m_timeScale = 1.0f / m_timeScale;
                m_lerp = 0.0f;
            }
            else
            {
                m_timeScale = 1.0f;
                m_lerp = 1.0f;
            }
        }

        public void Cease()
        {
            m_arriveAction = Empty;
            m_subStateAction = Empty;
        }

        public void AddArriveAction(ArriveAction action)
        {
            m_arriveAction += action;
        }

        public void ClearArriveAction()
        {
            m_arriveAction = Empty;
        }
    }

    [System.Serializable]
    class LookMachine
    {
        [SerializeField] Quaternion m_start = Quaternion.identity;
        [SerializeField] Quaternion m_target = Quaternion.identity;

        [SerializeField] float m_lerp = 0.0f;
        [SerializeField] float m_timeScale = 0.0f;

        delegate void SubStateDelegate(CustomerAgent agent);
        SubStateDelegate m_subStateAction;

        public delegate void ArriveAction();
        ArriveAction m_arriveAction;

        public LookMachine()
        {
            m_subStateAction = Empty;
            m_arriveAction = Empty;
        }

        void Smooth(CustomerAgent agent)
        {
            m_lerp += Time.deltaTime * m_timeScale;
            if (m_lerp > 1.0f)
            {
                agent.transform.rotation = m_target;
                m_subStateAction = Empty;
                m_arriveAction.Invoke();
                m_arriveAction = Empty;
            }
            else
            {
                agent.transform.rotation = Quaternion.Lerp(m_start, m_target, m_lerp);
            }
        }

        static void Empty(CustomerAgent agent)
        {

        }

        static void Empty()
        {

        }

        public void Invoke(CustomerAgent agent)
        {
            m_subStateAction.Invoke(agent);
        }

        public void Look(CustomerAgent agent, Quaternion target, float time)
        {
            m_subStateAction = Smooth;
            m_start = agent.transform.rotation;
            m_target = target;
            m_timeScale = time;
            if (m_timeScale != 0.0f)
            {
                m_timeScale = 1.0f / m_timeScale;
                m_lerp = 0.0f;
            }
            else
            {
                m_timeScale = 1.0f;
                m_lerp = 1.0f;
            }
        }

        public void Cease()
        {
            m_arriveAction = Empty;
            m_subStateAction = Empty;
        }

        public void AddArriveAction(ArriveAction action)
        {
            m_arriveAction += action;
        }

        public void ClearArriveAction()
        {
            m_arriveAction = Empty;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_walkStateMachine.Invoke(this);
        m_walkAction.Invoke();
        m_sitMachine.Invoke(m_anim);
        m_slideMachine.Invoke(this);
        m_lookMachine.Invoke(this);
    }

    static void Empty()
    {

    }

    #region StateSetters
    public void BehaviourWaitInLine(Vector3 position)
    {
        m_targetWalkPosition = position;
        m_walkStateMachine.SetState(this, _waitInLineState);
    }

    public void BehaviourOrderMeal(Vector3 position, ActionDelegate arriveAction)
    {
        m_targetWalkPosition = position;
        m_walkStateMachine.SetState(this, _orderingState);
        AddArriveAction(arriveAction);
    }

    public void BehaviourWaitForFood(EatingPosition eatPosition, PlateState orderedPlate)
    {
        m_eatPosition = eatPosition;
        m_eatPosition.hasAssignedAgent = true;
        m_targetWalkPosition = eatPosition.GetPosition();
        m_targetPlate = orderedPlate;
        m_foodWaiting = true;
        m_walkStateMachine.SetState(this, _waitingForFoodState);
    }

    public void BehaviourEatFood()
    {
        m_walkStateMachine.SetState(this, m_eatingState);
        m_foodWaiting = false;
    }

    public void BehaviourLeave(Vector3 position)
    {
        m_targetWalkPosition = position;
        m_walkStateMachine.SetState(this, _leavingState);
    }

    public void BehaviourDoNothing()
    {
        m_walkStateMachine.SetState(this, _nothingState);
    }
    #endregion // StateSetters

    #region Navigation
    void WalkToTargetPosition()
    {
        WalkTo(m_targetWalkPosition);
    }

    void WalkTo(Vector3 position)
    {
        m_navAgent.SetDestination(position);
        m_walkAnimStateID.CrossFade(m_anim);
        m_walkAction = UpdateWalkAnim;
    }

    void Arrive()
    {
        m_walkAction = SlowToStop;
        m_arriveAction.Invoke();
        ClearArriveAction();
    }

    bool InArriveRadius()
    {
        if (!m_navAgent.pathPending)
        {
            return m_navAgent.remainingDistance < m_arriveRadius;
        }

        return false;
    }

    void AddArriveAction(ActionDelegate action)
    {
        m_arriveAction += action;
    }

    void ClearArriveAction()
    {
        m_arriveAction = Empty;
    }

    void EnableNavAgent()
    {
        m_navAgent.enabled = true;
    }

    void DisableNavAgent()
    {
        m_navAgent.enabled = false;
    }

    public void Warp(Vector3 position)
    {
        m_navAgent.Warp(position);
    }
    #endregion // Navigation

    #region Animation
    void UpdateWalkAnim()
    {
        if (InArriveRadius())
        {
            Arrive();
            return;
        }

        float t = m_navAgent.velocity.magnitude / m_targetWalkSpeed;
        m_smoothMoveSpeed.Smooth(t);
        t = m_movementCurve.Evaluate(m_smoothMoveSpeed.value);
        if (t < 1.0f)
        {
            m_anim.SetFloat(m_walkSpeedAnimParamID.id, t);
            m_anim.SetFloat(m_walkOverDriveAnimParamID.id, 1);
        }
        else
        {
            m_anim.SetFloat(m_walkSpeedAnimParamID.id, 1);
            m_anim.SetFloat(m_walkOverDriveAnimParamID.id, t + 1);
        }
    }

    void SlowToStop()
    {
        m_smoothMoveSpeed.Smooth(0);
        float t = m_movementCurve.Evaluate(m_smoothMoveSpeed.value);

        if (t < 0.0001f && Mathf.Abs(m_smoothMoveSpeed.velocity) < 0.0001f)
        {
            m_walkAction = Empty;
            m_smoothMoveSpeed.value = 0.0f;
            m_smoothMoveSpeed.velocity = 0.0f;
            m_anim.SetFloat(m_walkSpeedAnimParamID.id, 0);
            m_anim.SetFloat(m_walkOverDriveAnimParamID.id, 1);
        }

        if (t < 1.0f)
        {
            m_anim.SetFloat(m_walkSpeedAnimParamID.id, t);
            m_anim.SetFloat(m_walkOverDriveAnimParamID.id, 1);
        }
        else
        {
            m_anim.SetFloat(m_walkSpeedAnimParamID.id, 1);
            m_anim.SetFloat(m_walkOverDriveAnimParamID.id, t + 1);
        }
    }

    void SlideToEatPosition()
    {
        m_slideMachine.Slide(this, m_eatPosition.sitPosition, m_slideSpeed);
        m_lookMachine.Look(this, m_eatPosition.sitRotation, m_lookSpeed);
    }

    void SlideToTablePosition()
    {
        m_slideMachine.Slide(this, m_eatPosition.GetPosition(), m_slideSpeed);
        m_lookMachine.Look(this, m_eatPosition.transform.rotation, m_lookSpeed);
    }
    #endregion // Animation

    void InvokeLeaveArrive()
    {
        m_leaveEvent.Invoke(this);
    }

    #region States
    class NothingState : IState<CustomerAgent>
    {
        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.Nothing;
            if(agent.m_navAgent.enabled)
                agent.m_navAgent.SetPath(new NavMeshPath());
            agent.m_walkAnimStateID.CrossFade(agent.m_anim);
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {

        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {

        }
    }

    class WaitInLineState : IState<CustomerAgent>
    {
        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.WaitingInLine;

            // Go to line position
            agent.WalkToTargetPosition();
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {

        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {

        }
    }

    class OrderMealState : IState<CustomerAgent>
    {
        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.Ordering;

            // Go to line position
            agent.WalkToTargetPosition();
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {

        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {

        }
    }

    class WaitForFoodState : IState<CustomerAgent>
    {
        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.WaitingForFood;

            // Go to line position
            agent.WalkToTargetPosition();
            agent.AddArriveAction(agent.m_sitMachine.Sit);
            agent.AddArriveAction(agent.DisableNavAgent);
            agent.AddArriveAction(agent.SlideToEatPosition);


            // Walk to table position.

            // walk/Slide to Eat Position.

            // Sit
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {
            agent.ClearArriveAction();
        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {

        }
    }

    class EatingState : IState<CustomerAgent>
    {
        float m_time = 0.0f;

        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.EatingFood;
            //agent.m_sitMachine.Sit();
            agent.m_eatAnimStateID.CrossFade(agent.m_anim);
            m_time = 0.0f;
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {
            //agent.m_sitMachine.Stand();
        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {
            m_time += Time.deltaTime * agent.m_eatSpeed;
            agent.debug_float = m_time;
            if (m_time > 1.0f)
            {
                //agent.BehaviourDoNothing();
                agent.lastEatPosition.hasAssignedAgent = false;
                agent.m_finishEatingEvent.Invoke(agent);
            }
        }
    }

    class LeavingState : IState<CustomerAgent>
    {
        void IState<CustomerAgent>.Enter(CustomerAgent agent)
        {
            agent.m_status = CustomerStatus.Leaving;

            if(agent.m_sitMachine.isSitting)
            {
                agent.m_sitMachine.Stand();
                agent.SlideToTablePosition();
                agent.m_slideMachine.AddArriveAction(agent.EnableNavAgent);
                agent.m_slideMachine.AddArriveAction(agent.WalkToTargetPosition);
            }
            else
            {
                agent.EnableNavAgent();
                agent.WalkToTargetPosition();
            }

            agent.AddArriveAction(agent.InvokeLeaveArrive);

            // Go to line position
            //agent.WalkToTargetPosition();
        }

        void IState<CustomerAgent>.Exit(CustomerAgent agent)
        {
            agent.ClearArriveAction();
            agent.m_slideMachine.Cease();
        }

        void IState<CustomerAgent>.Invoke(CustomerAgent agent)
        {

        }
    }
    #endregion // States
}