using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CustomerAgent))]
public class CustomerFoodHider : MonoBehaviour
{
    CustomerAgent m_agent = null;

    private void Awake()
    {
        m_agent = GetComponent<CustomerAgent>();
    }

    // Very laxy implementation to get the component to hide food. Should mostly be fine.
    public void HideCurrentFood(CustomerAgent customerAgent)
    {
        customerAgent.plate.GetComponentInChildren<FoodActivator>().HideAll();
    }
}
