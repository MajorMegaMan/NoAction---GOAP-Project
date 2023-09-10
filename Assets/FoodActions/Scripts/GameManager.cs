using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] SimpleFollow m_cameraPoint;
    [SerializeField] ChefAgentManager m_chefManager;
    [SerializeField] CustomerAgentManager m_customerManager;

    [SerializeField] int m_initialSpawnCount = 1;

    [SerializeField] int m_minCustomerCount = 1;

    int m_currentCamIndex = -1;

    [SerializeField] bool m_spawning = true;
    [SerializeField] bool m_rushEnabled = true;

    [SerializeField] bool m_rushing = true;
    [SerializeField] float m_rushIntervalTime = 60.0f;
    [SerializeField] float m_rushLengthTime = 10.0f;
    [SerializeField] int m_rushCount = 3;
    [SerializeField] float m_nextRushSpawn = 0.0f;

    float m_rushControlTimer = 0.0f;

    [SerializeField] int m_softMaxCustomers = 20;

    [SerializeField] SimpleTimeControl m_timeControl;
    [SerializeField] AnimationCurve m_timeCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    ChefAgent m_currentFocusChef;
    [SerializeField] TMP_Text m_cameraChefNameText;
    [SerializeField] TMP_Text m_cameraFocusGoalText;
    [SerializeField] TMP_Text m_cameraFocusActionText;

    public bool spawnEnabled { get { return m_spawning; } set { m_spawning = value; } }
    public int minCustomerCount { get { return m_minCustomerCount; } set { m_minCustomerCount = value; } }
    public int softMaxCustomers { get { return m_softMaxCustomers; } set { m_softMaxCustomers = value; } }

    public float rushIntervalTime { get { return m_rushIntervalTime; } set { m_rushIntervalTime = value; } }
    public float rushLengthTime { get { return m_rushLengthTime; } set { m_rushLengthTime = value; } }
    public int rushCount { get { return m_rushCount; } set { m_rushCount = value; } }
    public bool rushEnabled { get { return m_rushEnabled; } set { m_rushEnabled = value; } }
    public bool isRushing { get { return m_rushing; } }

    public float rushControlCurrentTimer { get { return m_rushControlTimer; } }

    private void Start()
    {
        for (int i = 0; i < m_initialSpawnCount; i++)
        {
            m_chefManager.SpawnChef();
        }
        IncreaseCamIndex();
    }

    private void Update()
    {
        while(m_spawning && m_customerManager.customerCount < m_minCustomerCount)
        {
            TrySpawnCustomer();
        }

        if (m_spawning && m_rushEnabled)
        {
            if (m_rushing)
            {
                m_rushControlTimer += Time.deltaTime;
                if (m_rushControlTimer > m_nextRushSpawn)
                {
                    m_nextRushSpawn += (m_rushLengthTime / m_rushCount);
                    TrySpawnCustomer();
                }
                if (m_rushControlTimer > m_rushLengthTime)
                {
                    m_rushControlTimer = 0.0f;
                    m_rushing = false;
                }
            }
            else
            {
                m_rushControlTimer += Time.deltaTime;
                if (m_rushControlTimer > m_rushIntervalTime)
                {
                    m_rushControlTimer = 0.0f;
                    m_rushing = true;
                    m_nextRushSpawn = (m_rushLengthTime / m_rushCount);
                }
            }
        }
    }

    void TrySpawnCustomer()
    {
        if(m_customerManager.customerCount < m_softMaxCustomers)
        {
            m_customerManager.SpawnCustomer();
        }
        else
        {
            int diff = m_customerManager.customerCount - m_softMaxCustomers;
            int rand = Random.Range(0, diff);
            if(rand == 0)
            {
                m_customerManager.SpawnCustomer();
            }
        }
    }

    public void SpawnCustomer()
    {
        m_customerManager.SpawnCustomer();
    }

    public void SpawnChef()
    {
        var chef = m_chefManager.SpawnChef();
        chef.onDestroyEvent.AddListener(OnChefDestroy);
    }

    public void DestroyNextChef()
    {
        m_chefManager.MarkNextReadyAgentForDespawn();
    }

    void OnChefDestroy(ChefAgent chef)
    {
        if(chef == m_cameraPoint.targetFollow)
        {
            IncreaseCamIndex();
        }
    }

    public void IncreaseCamIndex()
    {
        var agents = m_chefManager.allAgents;
        if(agents.Length != 0)
        {
            m_currentCamIndex++;
            m_currentCamIndex = m_currentCamIndex % agents.Length;
            SetChefToCameraFocus(agents[m_currentCamIndex]);
        }
        else
        {
            m_currentCamIndex = -1;
            SetChefToCameraFocus(null);
        }
    }

    public void DecreaseCamIndex()
    {
        var agents = m_chefManager.allAgents;
        if (agents.Length != 0)
        {
            m_currentCamIndex--;
            if (m_currentCamIndex < 0)
            {
                m_currentCamIndex = agents.Length - 1;
            }
            SetChefToCameraFocus(agents[m_currentCamIndex]);
        }
        else
        {
            m_currentCamIndex = -1;
            SetChefToCameraFocus(null);
        }
    }

    void SetChefToCameraFocus(ChefAgent chef)
    {
        if(m_currentFocusChef != null)
        {
            m_currentFocusChef.movementMachine.popActionEvent.RemoveListener(UpdateCameraActionText);
            m_currentFocusChef.movementMachine.beginPlanEvent.RemoveListener(UpdateCameraGoalText);
            m_currentFocusChef.movementMachine.completePlanEvent.RemoveListener(UpdateCameraGoalText);
        }
        m_currentFocusChef = chef;
        m_cameraPoint.targetFollow = chef.transform;
        if(chef != null)
        {
            m_cameraChefNameText.SetText(chef.name);
            UpdateCameraActionText(chef);
            UpdateCameraGoalText(chef);
            chef.movementMachine.popActionEvent.AddListener(UpdateCameraActionText);
            chef.movementMachine.beginPlanEvent.AddListener(UpdateCameraGoalText);
            chef.movementMachine.completePlanEvent.AddListener(UpdateCameraGoalText);
        }
        else
        {
            m_cameraChefNameText.SetText("No Chef");
            m_cameraFocusActionText.SetText("No Action");
            m_cameraFocusGoalText.SetText("No Goal");
        }
    }

    void UpdateCameraActionText(ChefAgent chef, IGOAPAgentActionMethods action)
    {
        UpdateCameraActionText(chef);
    }

    void UpdateCameraActionText(ChefAgent chef)
    {
        string text = "";
        if(chef.currentAction == null)
        {
            m_cameraFocusActionText.SetText("No Action");
            return;
        }
        AddActionText(ref text, chef.currentAction);
        var actionPlan = chef.actionPlan;
        if(actionPlan != null)
        {
            foreach(var action in actionPlan)
            {
                AddActionText(ref text, action);
            }
        }
        //text += chef.currentAction.actionName;
        m_cameraFocusActionText.SetText(text);
    }

    void AddActionText(ref string text, IGOAPAction action)
    {
        if(action != null)
        {
            text += action.actionName + ", ";
        }
    }

    void AddActionText(ref string text, IGOAPAgentActionMethods action)
    {
        if (action != null)
        {
            text += action.actionName;
        }
    }

    void UpdateCameraGoalText(ChefAgent chef)
    {
        string text = "";
        if (chef.currentGoal == null)
        {
            m_cameraFocusGoalText.SetText("No Goal");
            return;
        }
        text += chef.currentGoal.goalName;
        m_cameraFocusGoalText.SetText(text);
    }

    public void SetTimeScale(float timeScale)
    {
        m_timeControl.SetTimeScale(m_timeCurve.Evaluate(timeScale));
    }
}
