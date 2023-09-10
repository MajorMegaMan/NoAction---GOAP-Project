using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace BBB.GOAP
{
    public interface IGOAPGoal
    {
        public string goalName { get; }
        bool CheckWorldState(GOAPWorldState worldState);
    }
}
