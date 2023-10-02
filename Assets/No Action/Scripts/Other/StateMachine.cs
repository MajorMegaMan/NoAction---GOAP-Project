using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<T>
{
    IState<T> m_currentState = null;

    public void InitialiseState(T owner, IState<T> initialState)
    {
        m_currentState = initialState;
        m_currentState.Enter(owner);
    }

    public void SetState(T owner, IState<T> nextState)
    {
        m_currentState.Exit(owner);
        m_currentState = nextState;
        m_currentState.Enter(owner);
    }

    public void Invoke(T owner)
    {
        m_currentState.Invoke(owner);
    }
}

public interface IState<T>
{
    void Enter(T owner);
    void Exit(T owner);
    void Invoke(T owner);
}

public class PackagedStateMachine<TOwner>
{
    TOwner m_owner = default;
    StateMachine<TOwner> m_stateMachine;
    IState<TOwner>[] m_states;

    int m_currentIndex = -1;

    // current index can be used duiring the exit call to find the next state that the machine is going to.
    public int currentIndex { get { return m_currentIndex; } }

    public PackagedStateMachine(TOwner owner, IState<TOwner>[] states)
    {
        m_stateMachine = new StateMachine<TOwner>();
        m_owner = owner;
        SetStates(states);
    }

    void SetStates(IState<TOwner>[] states)
    {
        m_states = new IState<TOwner>[states.Length];
        for (int i = 0; i < states.Length; i++)
        {
            m_states[i] = states[i];
        }
    }

    public void InitialiseState(int index = 0)
    {
        index = index % m_states.Length;
        m_currentIndex = index;
        m_stateMachine.InitialiseState(m_owner, m_states[index]);
    }

    public void ChangeToState(int index)
    {
        index = index % m_states.Length;
        m_currentIndex = index;
        m_stateMachine.SetState(m_owner, m_states[index]);
    }

    public void Invoke()
    {
        m_stateMachine.Invoke(m_owner);
    }

    public IState<TOwner> GetStateObject(int index)
    {
        index = index % m_states.Length;
        return m_states[index];
    }
}