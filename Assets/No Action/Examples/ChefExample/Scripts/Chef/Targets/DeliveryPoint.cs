using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class DeliveryPoint : ChefActionTarget
{
    [SerializeField] Vector3 m_deliverPosition = Vector3.zero;
    [SerializeField] Vector3 m_deliverRotation = Vector3.zero;

    [SerializeField] Vector3 m_drawSize = Vector3.one;
    [SerializeField] Color m_agentLocationColour = Color.blue;
    [SerializeField] Color m_deliverColour = Color.yellow;

    public void SetItemToDeliveryPoint(Transform itemTransform)
    {
        var parent = itemTransform.parent;
        itemTransform.parent = transform;
        itemTransform.SetLocalPositionAndRotation(m_deliverPosition, Quaternion.Euler(m_deliverRotation));
        itemTransform.parent = parent;
    }

    private void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = m_agentLocationColour;
        Utility.DrawColourCube(Vector3.zero, Vector3.one);

        Matrix4x4 matrix = Matrix4x4.TRS(m_deliverPosition, Quaternion.Euler(m_deliverRotation), m_drawSize);
        Gizmos.matrix *= matrix;

        Gizmos.color = m_deliverColour;
        Utility.DrawColourCube(Vector3.zero, Vector3.one);
    }
}
