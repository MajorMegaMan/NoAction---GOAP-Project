using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using BBB.GOAP;

[CustomPropertyDrawer(typeof(BBB.GOAP.GOAPValue), true)]
public class GOAPValuePropertyDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        if(property.isExpanded)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects
        var keyRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

        position.y += EditorGUIUtility.singleLineHeight;

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        GUIContent keyLabel = new GUIContent("Key");
        EditorGUI.PropertyField(keyRect, property.FindPropertyRelative("m_key"), keyLabel);

        DrawGOAPValueField(position, property);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight * 2;
    }

    public static void DrawGOAPKeyField(Rect position, SerializedProperty property)
    {
        GUIContent keyLabel = new GUIContent("Key");
        EditorGUI.PropertyField(position, property.FindPropertyRelative("m_key"), keyLabel);
    }

    public static void DrawGOAPKeyField(Rect position, SerializedProperty property, GUIContent content)
    {
        EditorGUI.PropertyField(position, property.FindPropertyRelative("m_key"), content);
    }

    public static void DrawGOAPValueField(Rect position, SerializedProperty property)
    {
        DrawGOAPValueField(position, property, GUIContent.none);
    }

    public static void DrawGOAPValueField(Rect position, SerializedProperty property, GUIContent content)
    {
        float halfWidth = (position.width / 2) - (EditorGUIUtility.singleLineHeight / 2);

        var amountRect = new Rect(position.x, position.y, halfWidth, EditorGUIUtility.singleLineHeight);
        var unitRect = new Rect(position.x + halfWidth + EditorGUIUtility.singleLineHeight, position.y, halfWidth, EditorGUIUtility.singleLineHeight);

        var valueTypeProperty = property.FindPropertyRelative("m_valueType");

        EditorGUI.PropertyField(amountRect, valueTypeProperty, content);
        var valueType = valueTypeProperty.GetUnderlyingValue();

        switch (valueType)
        {
            case SerialisedGOAPValue.ValueType.FLOAT:
                {
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("m_floatValue"), GUIContent.none);
                    break;
                }
            case SerialisedGOAPValue.ValueType.INT:
                {
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("m_intValue"), GUIContent.none);
                    break;
                }
            case SerialisedGOAPValue.ValueType.BOOLEAN:
                {
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("m_boolValue"), GUIContent.none);
                    break;
                }
            case SerialisedGOAPValue.ValueType.STRING:
                {
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("m_stringValue"), GUIContent.none);
                    break;
                }
            case SerialisedGOAPValue.ValueType.OBJECT:
                {
                    EditorGUI.PropertyField(unitRect, property.FindPropertyRelative("m_objectValue"), GUIContent.none);
                    break;
                }
        }
    }

    public static void CopyGOAPValue(SerializedProperty sourceProp, SerializedProperty fillProp)
    {
        fillProp.FindPropertyRelative("m_key"           ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_key"            ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_valueType"     ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_valueType"      ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_floatValue"    ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_floatValue"     ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_intValue"      ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_intValue"       ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_boolValue"     ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_boolValue"      ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_stringValue"   ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_stringValue"    ).GetUnderlyingValue());
        fillProp.FindPropertyRelative("m_objectValue"   ).SetUnderlyingValue(sourceProp.FindPropertyRelative("m_objectValue"    ).GetUnderlyingValue());

        var sourceGoapValue = sourceProp.GetUnderlyingValue() as GOAPValue;
        var fillGoapValue = fillProp.GetUnderlyingValue() as GOAPValue;
        fillGoapValue.Copy(sourceGoapValue);

        fillProp.serializedObject.ApplyModifiedProperties();
    }
}
