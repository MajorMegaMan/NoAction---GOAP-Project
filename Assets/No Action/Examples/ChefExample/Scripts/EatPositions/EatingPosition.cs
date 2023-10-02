using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EatingPosition : ChefActionTarget
{
    [SerializeField] Vector3 m_sitPosition = Vector3.zero;
    [SerializeField] Vector3 m_sitRotation = Vector3.zero;

    [SerializeField] Vector3 m_foodPosition = Vector3.zero;
    [SerializeField] Vector3 m_foodRotation = Vector3.zero;

    public Vector3 sitPosition { get { return transform.TransformPoint(m_sitPosition); } }
    public Quaternion sitRotation { get { return transform.rotation * Quaternion.Euler(m_sitRotation); } }
    public Vector3 localSitEuler { get { return m_sitRotation; } }
    public Vector3 localSitPosition { get { return m_sitPosition; } }
    public Quaternion localSitRotation { get { return Quaternion.Euler(m_sitRotation); } }

    public Vector3 foodPosition { get { return transform.TransformPoint(m_foodPosition); } }
    public Quaternion foodRotation { get { return transform.rotation * Quaternion.Euler(m_foodRotation); } }
    public Vector3 localFoodEuler { get { return m_foodRotation; } }
    public Vector3 localFoodPosition { get { return m_foodPosition; } }
    public Quaternion localFoodRotation { get { return Quaternion.Euler(m_foodRotation); } }

    [SerializeField] UnityEvent<EatingPosition> m_deliverEvent;
    [SerializeField] UnityEvent<EatingPosition> m_pickUpEvent;

    public UnityEvent<EatingPosition> deliverEvent { get { return m_deliverEvent; } }
    public UnityEvent<EatingPosition> pickUpEventEvent { get { return m_pickUpEvent; } }

    public TableManager table;

    ChefActionTarget m_currentItem;
    public bool hasAssignedAgent = false;

    public ChefActionTarget currentItem { get { return m_currentItem; } }

    public void SetItemToEatPoint(ChefActionTarget item)
    {
        AlignItemToEatPoint(item);

        m_currentItem = item;
        m_currentItem.tablePosition = this;

        m_deliverEvent.Invoke(this);
    }

    public void AlignItemToEatPoint(ChefActionTarget item)
    {
        var itemTransform = item.transform;
        //var parent = itemTransform.parent;
        itemTransform.parent = transform;
        itemTransform.SetLocalPositionAndRotation(m_foodPosition, Quaternion.Euler(m_foodRotation));
        //itemTransform.parent = parent;
        itemTransform.parent = null;
    }

    public void RemoveItem()
    {
        if(m_currentItem != null)
        {
            m_currentItem.tablePosition = null;
            m_currentItem = null;
        }

        m_pickUpEvent.Invoke(this);
    }
}
