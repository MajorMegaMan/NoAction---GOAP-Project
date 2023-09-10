using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGOAPAgentElements
{
    public IGOAPAction currentAction { get; }

    public Vector3 GetPosition();
}
