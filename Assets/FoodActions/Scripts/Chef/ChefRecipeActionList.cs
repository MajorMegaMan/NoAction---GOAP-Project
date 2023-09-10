using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefRecipeActionList : MonoBehaviour
{
    [SerializeField] List<AgentAction<ChefWorldStateEnum>> m_actionObjects = new List<AgentAction<ChefWorldStateEnum>>();
    List<AgentAction<ChefWorldStateEnum>> m_actions;

    void JustifyActionList()
    {
         var actions = new List<AgentAction<ChefWorldStateEnum>>();
        foreach (var actionObject in m_actionObjects)
        {
            actions.Add(actionObject);
        }
        m_actions = actions;
    }

    private void Awake()
    {
        JustifyActionList();
    }
}
