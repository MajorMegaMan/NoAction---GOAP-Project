using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChefGlobalWorldState : MonoBehaviour
{
    [SerializeField] SerialisedGOAPWorldState<ChefWorldStateEnum> m_worldState;

    public SerialisedGOAPWorldState<ChefWorldStateEnum> globalWorldState { get { return m_worldState; } }
}
