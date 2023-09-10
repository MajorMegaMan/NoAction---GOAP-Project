using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EatingPosition : ChefActionTarget
{
    [SerializeField] Vector3 m_sitPosition = Vector3.zero;
    [SerializeField] Vector3 m_sitRotation = Vector3.zero;

    [SerializeField] Vector3 m_foodPosition = Vector3.zero;
    [SerializeField] Vector3 m_foodRotation = Vector3.zero;

    [SerializeField] Vector3 m_drawSize = Vector3.one;
    [SerializeField] Color m_originColour = Color.blue;
    [SerializeField] Color m_sitLocationColour = Color.blue;
    [SerializeField] Color m_foodColour = Color.yellow;

    public Vector3 sitPosition { get { return transform.TransformPoint(m_sitPosition); } }
    public Quaternion sitRotation { get { return transform.rotation * Quaternion.Euler(m_sitRotation); } }

    public Vector3 foodPosition { get { return transform.TransformPoint(m_foodPosition); } }
    public Quaternion foodRotation { get { return transform.rotation * Quaternion.Euler(m_foodRotation); } }

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

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = m_originColour;
        Utility.DrawColourCube(Vector3.zero, Vector3.one);
        Handles.ArrowHandleCap(0, transform.position, transform.rotation, 1.0f, EventType.Repaint);

        Matrix4x4 matrix = Matrix4x4.TRS(m_foodPosition, Quaternion.Euler(m_foodRotation), m_drawSize);
        Gizmos.matrix *= matrix;

        Gizmos.color = m_foodColour;
        Utility.DrawColourCube(Vector3.zero, Vector3.one);
        Handles.ArrowHandleCap(0, foodPosition, foodRotation, 1.0f, EventType.Repaint);

        Gizmos.color = m_sitLocationColour;
        matrix = Matrix4x4.TRS(m_sitPosition, Quaternion.Euler(m_sitRotation), m_drawSize);
        Gizmos.matrix = transform.localToWorldMatrix * matrix;
        Utility.DrawColourCube(Vector3.zero, Vector3.one);
        Handles.ArrowHandleCap(0, sitPosition, sitRotation, 1.0f, EventType.Repaint);
    }
}
