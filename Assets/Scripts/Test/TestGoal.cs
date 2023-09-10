using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BBB.GOAP;

public abstract class TestGoal : IGOAPGoal
{
    [SerializeField] string m_goalName = "Goal";

    public string goalName { get { return m_goalName; } }

    public abstract bool CheckWorldState(GOAPWorldState worldState);

    public abstract List<GOAPKey> GetKeys();
}
