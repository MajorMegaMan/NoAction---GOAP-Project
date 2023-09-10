using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomerMoveTarget : MonoBehaviour, IGOAPActionTarget
{
    public string targetName { get { return name; } }

    public void PerfromActionOnTarget(IGOAPAgentElements agent, IGOAPAgentActionMethods action)
    {
        var agentAction = action as AgentAction<ChefWorldStateEnum>;
        if (agentAction == null)
        {
            return;
        }
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}