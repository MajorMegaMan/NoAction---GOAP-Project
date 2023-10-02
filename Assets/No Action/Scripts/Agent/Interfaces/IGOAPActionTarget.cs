using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGOAPActionTarget
{
    public string targetName { get; }

    void PerfromActionOnTarget(IGOAPAgentElements agent, IGOAPAgentActionMethods action);

    Vector3 GetPosition();
}
