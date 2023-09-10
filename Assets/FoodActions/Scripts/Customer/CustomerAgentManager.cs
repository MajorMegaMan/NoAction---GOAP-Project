using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

public class CustomerAgentManager : MonoBehaviour
{
    [SerializeField] TableManager m_tableManager;
    [SerializeField] CustomerAgent m_agentPrefab;

    [SerializeField] Transform m_spawnPosition;

    [SerializeField] Transform m_orderPosition;
    [SerializeField] float m_lineSeperation = 1.0f;

    Queue<CustomerAgent> m_nothingCustomers = new Queue<CustomerAgent>();

    CustomerAgent m_orderingCustomer = null;
    Queue<CustomerAgent> m_inLineCustomers = new Queue<CustomerAgent>();

    List<CustomerAgent> m_sittingCustomers = new List<CustomerAgent>();

    Dictionary<PlateState, CustomerAgent> m_waitingCustomers = new Dictionary<PlateState, CustomerAgent>();

    HashSet<CustomerAgent> m_allAgents = new HashSet<CustomerAgent>();

    bool m_customerWaitingForOrder = false;
    public bool customerWaitingToOrder { get { return m_customerWaitingForOrder; } }

    int m_customerCount = 0;

    [SerializeField] UnityEvent<CustomerAgent> m_finishEatingEvent;
    public UnityEvent<CustomerAgent> finishEatingEvent { get { return m_finishEatingEvent; } }
    public int customerCount { get { return m_customerCount; } }

    // Update is called once per frame
    void Update()
    {
        while (m_nothingCustomers.Count > 0)
        {
            QueueToLine(m_nothingCustomers.Dequeue());
        }

        if (m_orderingCustomer == null)
        {
            PushLine();
        }
    }

    public void SpawnCustomer()
    {
        var agent = Instantiate(m_agentPrefab);
        agent.Warp(m_spawnPosition.position);
        agent.finishEatingEvent.AddListener(LeaveSitting);
        agent.leaveEvent.AddListener(DestroyAgent);
        m_nothingCustomers.Enqueue(agent);


        m_allAgents.Add(agent);
        m_customerCount++;
    }

    void PushLine()
    {
        if(m_inLineCustomers.Count > 0)
        {
            m_orderingCustomer = m_inLineCustomers.Dequeue();
        }
        else
        {
            return;
        }

        if(m_orderingCustomer != null)
        {
            m_orderingCustomer.BehaviourOrderMeal(m_orderPosition.position, OrderCustomerIsReady);
        }

        int linePos = 1;
        foreach(var agent in m_inLineCustomers)
        {
            agent.BehaviourWaitInLine(FindLinePosition(linePos++));
        }
    }

    void OrderCustomerIsReady()
    {
        m_customerWaitingForOrder = true;
    }

    void QueueToLine(CustomerAgent agent)
    {
        m_inLineCustomers.Enqueue(agent);
        agent.BehaviourWaitInLine(FindLinePosition(m_inLineCustomers.Count + 1));
    }

    Vector3 FindLinePosition(int count)
    {
        return m_orderPosition.position - m_orderPosition.forward * (count * m_lineSeperation);
    }

    void DebugLeave()
    {
        if(m_orderingCustomer != null)
        {
            m_orderingCustomer.BehaviourLeave(m_spawnPosition.position);
            m_orderingCustomer = null;
        }
        else
        {
            DebugLeaveSitting();
        }
    }

    public void AssignOrderingCustomerToEatingPosition(EatingPosition eatPosition, PlateState orderedPlate)
    {
        if (eatPosition != null && m_orderingCustomer != null)
        {
            m_orderingCustomer.BehaviourWaitForFood(eatPosition, orderedPlate);
            m_sittingCustomers.Add(m_orderingCustomer);

            m_waitingCustomers.Add(orderedPlate, m_orderingCustomer);

            eatPosition.deliverEvent.AddListener(GivePlateToCustomerAtEatPosition);
            m_orderingCustomer = null;
            m_customerWaitingForOrder = false;
        }
    }

    void GivePlateToCustomerAtEatPosition(EatingPosition eatPos)
    {
        var orderedPlate = eatPos.currentItem as PlateState;

        if(m_waitingCustomers.TryGetValue(orderedPlate, out var agent))
        {
            agent.BehaviourEatFood();
            m_waitingCustomers.Remove(orderedPlate);
        }
    }

    public bool SetPlateToCustomerLocation(PlateState plate)
    {
        if (m_waitingCustomers.TryGetValue(plate, out var agent))
        {
            plate.SetDeliveryPoint(agent.lastEatPosition);
            return true;
        }
        return false;
    }

    void DebugLeaveSitting()
    {
        if(m_sittingCustomers.Count > 0)
        {
            int i = m_sittingCustomers.Count - 1;
            var agent = m_sittingCustomers[i];

            LeaveSitting(agent);
        }
    }

    public void LeaveSitting(CustomerAgent agent)
    {
        if(m_sittingCustomers.Remove(agent))
        {
            agent.BehaviourLeave(m_spawnPosition.position);
            m_waitingCustomers.Remove(agent.plate);

            agent.lastEatPosition.deliverEvent.RemoveListener(GivePlateToCustomerAtEatPosition);
            //m_tableManager.RestoreEatingPosition(agent.lastEatPosition);

            m_finishEatingEvent.Invoke(agent);
        }
    }

    public void DestroyAgent(CustomerAgent agent)
    {
        m_allAgents.Remove(agent);
        Destroy(agent.gameObject);
        m_customerCount--;
    }

    public CustomerAgent GetRandomCustomer()
    {
        int rand = Random.Range(0, m_customerCount);
        int i = 0;
        foreach( CustomerAgent agent in m_allAgents)
        {
            if(i ==  rand)
            {
                return agent;
            }
            i++;
        }
        return null;
    }
}
