using BBB.GOAP;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActionCollection<TAction> where TAction : class, BBB.GOAP.IGOAPAction
{
    [SerializeField][RequireInterface(typeof(IGOAPAction))] List<Object> m_actionObjects = new List<Object>();
    HashSet<TAction> m_actions = new HashSet<TAction>();
    HashSet<IGOAPAction> m_baseActions = new HashSet<IGOAPAction>();

    public void ValidateCollection()
    {
        m_actions.Clear();
        m_baseActions.Clear();
        for (int i = 0; i < m_actionObjects.Count; i++)
        {
            var action = m_actionObjects[i] as TAction;
            m_actions.Add(action);
            m_baseActions.Add(action);
        }
    }

    public HashSet<TAction> GetActions()
    {
        return m_actions;
    }

    public HashSet<IGOAPAction> GetBaseActions()
    {
        return m_baseActions;
    }
}

public interface IActionCollectionHolder<TAction> where TAction : class, BBB.GOAP.IGOAPAction
{
    public ActionCollection<TAction> GetActionCollection();
}
