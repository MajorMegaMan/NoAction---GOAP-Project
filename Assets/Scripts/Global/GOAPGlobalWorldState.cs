using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPGlobalWorldState : MonoBehaviour
{
    [SerializeField] SerialisedGOAPWorldState<TestEnum> m_worldState;

    public SerialisedGOAPWorldState<TestEnum> globalWorldState { get { return m_worldState; } }
}
