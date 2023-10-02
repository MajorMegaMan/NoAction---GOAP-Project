using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is intended to only be used if a IGOAPAction is also inherited. Not ideal but that's how the system works for the moment.
public interface IGOAPAgentActionMethods
{
    public string actionName { get; }

    float beginRange { get; }
    float completeTime { get; }
    float effectTime { get; }

    bool InBeginRange(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget);

    Vector3 CalcBeginPosition(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget);

    IGOAPActionTarget FindActionTarget();

    bool CheckCondition(GOAPWorldState worldState);

    int AddEffects(GOAPWorldState worldState);

    float GetWeight();

    void PerformAgentEffects(IGOAPAgentElements goapAgent, IGOAPActionTarget actionTarget);

    bool PlayBeginAnim(Animator animator);

    bool PlayCompleteAnim(Animator animator);
}
