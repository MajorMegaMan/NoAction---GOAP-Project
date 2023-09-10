using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBB.GOAP
{
    public interface IGOAPAction
    {
        public string actionName { get; }

        // condition
        bool CheckCondition(GOAPWorldState worldState);

        // effects. returns an int just so the user might want to add some additional information when effects are applied.
        int AddEffects(GOAPWorldState worldState);

        float GetWeight();
    }
}
