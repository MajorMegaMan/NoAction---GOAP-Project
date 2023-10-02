using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EatingPosition))]
public class EatPositionEditor : Editor
{
    static Vector3 _drawSize = Vector3.one;
    static Color _originColour = Color.blue;
    static Color _sitLocationColour = Color.red;
    static Color _foodColour = Color.yellow;

    // draw lines between a chosen game object
    // and a selection of added game objects

    static void DrawEatPositionWires(EatingPosition eatPosition, Color originColour, Color sitColour, Color foodColour, Vector3 drawSize)
    {
        Transform transform = eatPosition.transform;

        // Set matrix to origin
        Handles.matrix = transform.localToWorldMatrix;

        Handles.color = originColour;

        // draw origin
        Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.identity, 1.0f, EventType.Repaint);
        Handles.DrawWireCube(Vector3.zero, drawSize);

        // set matrix to foodPosition
        Matrix4x4 matrix = Matrix4x4.TRS(eatPosition.foodPosition, Quaternion.identity, Vector3.one);
        Handles.matrix = matrix * Matrix4x4.Rotate(transform.rotation) * Matrix4x4.Rotate(Quaternion.Euler(eatPosition.localFoodEuler));

        Handles.color = foodColour;

        // Draw food Position
        Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.identity, 1.0f, EventType.Repaint);
        Handles.DrawWireCube(Vector3.zero, drawSize);

        // Set Matrix to si Position
        matrix = Matrix4x4.TRS(eatPosition.sitPosition, Quaternion.identity, Vector3.one);
        Handles.matrix = matrix * Matrix4x4.Rotate(transform.rotation) * Matrix4x4.Rotate(Quaternion.Euler(eatPosition.localSitEuler));

        Handles.color = sitColour;

        // Draw sit position
        Handles.ArrowHandleCap(0, Vector3.zero, Quaternion.identity, 1.0f, EventType.Repaint);
        Handles.DrawWireCube(Vector3.zero, drawSize);
    }

    [DrawGizmo(GizmoType.InSelectionHierarchy)]
    static void DrawHandles(EatingPosition eatPosition, GizmoType gizmoType)
    {
        DrawEatPositionWires(eatPosition, _originColour, _sitLocationColour, _foodColour, _drawSize);
    }
}