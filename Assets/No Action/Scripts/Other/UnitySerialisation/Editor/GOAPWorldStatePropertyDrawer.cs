using BBB.GOAP;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(BBB.GOAP.GOAPWorldState), true)]
public class GOAPWorldStatePropertyDrawer : PropertyDrawer
{
    bool m_valueFoldout = false;
    bool m_controlFoldout = false;

    ReorderableList m_userControllerList;

    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        if (property.isExpanded)
        {
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }
        var infoPosition = position;
        infoPosition.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(infoPosition, label);

        //var infoPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        infoPosition.y += EditorGUIUtility.singleLineHeight;

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        DrawProp(infoPosition, property);

        var controlsRect = position;
        controlsRect.y += GetListHeight(property);
        controlsRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.indentLevel++;

        m_controlFoldout = EditorGUI.Foldout(controlsRect, m_controlFoldout, "Controls");
        if (m_controlFoldout)
        {
            EditorGUI.indentLevel++;
            controlsRect.y += EditorGUIUtility.singleLineHeight;

            DrawAddingProp(controlsRect, property);
            controlsRect.y += EditorGUIUtility.singleLineHeight * 2;

            var buttonRect = controlsRect;
            buttonRect.x += 30; // 2 indents.
            if (GUI.Button(buttonRect, "CLEAR ALL VALUES"))
            {
                ClearAllGOAPValues(property.FindPropertyRelative("m_goapValueList"));
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetListHeight(property) + GetButtonsHeight();
    }

    float GetListHeight(SerializedProperty property)
    {
        var listProperty = property.FindPropertyRelative("m_goapValueList");

        float height = 1;
        if(m_valueFoldout)
        {
            height += listProperty.arraySize;
        }
        height = Mathf.Max(height, 2);
        //return EditorGUIUtility.singleLineHeight * (height);

        EnableReorderableList(property, listProperty);

        height = EditorGUIUtility.singleLineHeight * 2;
        if (m_valueFoldout)
        {
            height = m_userControllerList.GetHeight() + EditorGUIUtility.singleLineHeight;
        }

        return height;
    }

    float GetButtonsHeight()
    {
        float height = EditorGUIUtility.singleLineHeight;
        if(m_controlFoldout)
        {
            height *= 5;
        }

        return height;
    }

    void EnableReorderableList(SerializedProperty property, SerializedProperty listProperty)
    {
        if (m_userControllerList == null)
        {
            m_userControllerList = new ReorderableList(listProperty.GetUnderlyingValue() as IList, typeof(GOAPValue), true, false, true, true);
            m_userControllerList.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                var element = listProperty.GetArrayElementAtIndex(index);

                var keyPosition = position;
                keyPosition.width /= 2;
                var valuePosition = keyPosition;
                valuePosition.x += valuePosition.width;

                var keyProperty = element.FindPropertyRelative("m_key");
                EditorGUI.LabelField(keyPosition, keyProperty.GetUnderlyingValue().ToString());
                //GOAPValuePropertyDrawer.DrawGOAPKeyField(keyPosition, element, GUIContent.none);
                GOAPValuePropertyDrawer.DrawGOAPValueField(valuePosition, element);
            };

            m_userControllerList.onAddCallback = (ReorderableList list) =>
            {
                AddGOAPValue(listProperty, property.FindPropertyRelative("m_editorValueToAdd"));
            };
        }
    }

    void DrawProp(Rect position, SerializedProperty property)
    {
        EditorGUI.indentLevel++;
        var listProperty = property.FindPropertyRelative("m_goapValueList");

        position.width -= EditorGUIUtility.singleLineHeight;
        position.height = EditorGUIUtility.singleLineHeight;

        string countString = "Count : ";
        countString += listProperty.arraySize;
        m_valueFoldout = EditorGUI.Foldout(position, m_valueFoldout, countString);
        position.y += EditorGUIUtility.singleLineHeight;
        if (!m_valueFoldout)
        {
            EditorGUI.indentLevel--;
            return;
        }

        Rect removeButtonPos = position;
        removeButtonPos.width = EditorGUIUtility.singleLineHeight;
        removeButtonPos.x = position.x + position.width;

        Rect labelPos = position;
        Rect elementPos = position;
        labelPos.width /= 2;
        elementPos.width /= 2;
        elementPos.x += elementPos.width;

        EditorGUI.indentLevel++;
        EnableReorderableList(property, listProperty);
        m_userControllerList.DoList(position);

        //for (int i = 0; i < listProperty.arraySize - 1; i++)
        //{
        //    var goapValueProperty = listProperty.GetArrayElementAtIndex(i);
        //    var keyProperty = goapValueProperty.FindPropertyRelative("m_key");
        //
        //    EditorGUI.LabelField(labelPos, keyProperty.GetUnderlyingValue().ToString());
        //
        //    GOAPValuePropertyDrawer.DrawGOAPValueField(elementPos, goapValueProperty);
        //
        //    labelPos.y += EditorGUIUtility.singleLineHeight;
        //    elementPos.y += EditorGUIUtility.singleLineHeight;
        //
        //    if (GUI.Button(removeButtonPos, "-"))
        //    {
        //        RemoveAt(listProperty, i);
        //        i--;
        //    }
        //    removeButtonPos.y += EditorGUIUtility.singleLineHeight;
        //}
        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    static void DrawAddingProp(Rect position, SerializedProperty property)
    {
        var toAddProperty = property.FindPropertyRelative("m_editorValueToAdd");

        GOAPValuePropertyDrawer.DrawGOAPKeyField(position, toAddProperty);
        position.y += EditorGUIUtility.singleLineHeight;
        GOAPValuePropertyDrawer.DrawGOAPValueField(position, toAddProperty);
        position.y += EditorGUIUtility.singleLineHeight;
    }

    static void AddGOAPValue(SerializedProperty listProperty, SerializedProperty goapValueProperty)
    {
        var targetKey = goapValueProperty.FindPropertyRelative("m_key").GetUnderlyingValue();
        for (int i = 0; i < listProperty.arraySize; ++i)
        {
            var currentGoapValueProperty = listProperty.GetArrayElementAtIndex(i);
            if(currentGoapValueProperty.FindPropertyRelative("m_key").GetUnderlyingValue().Equals(targetKey))
            {
                // List already contains key
                return;
            }
        }

        listProperty.arraySize++;
        listProperty.serializedObject.ApplyModifiedProperties();

        int index = listProperty.arraySize - 1;

        //listProperty.InsertArrayElementAtIndex(index);
        var newGoapValueProperty = listProperty.GetArrayElementAtIndex(index);

        //var goapValue = newGoapValueProperty.GetUnderlyingValue() as GOAPValue;
        //goapValue.Copy(goapValueProperty.GetUnderlyingValue() as GOAPValue);
        //
        //newGoapValueProperty.serializedObject.ApplyModifiedProperties();

        GOAPValuePropertyDrawer.CopyGOAPValue(goapValueProperty, newGoapValueProperty);
    }

    void RemoveAt(SerializedProperty listProperty, int index)
    {
        if (index < listProperty.arraySize)
        {
            listProperty.DeleteArrayElementAtIndex(index);
        }
    }

    static void ClearAllGOAPValues(SerializedProperty listProperty)
    {
        while (listProperty.arraySize > 0)
        {
            listProperty.DeleteArrayElementAtIndex(0);
        }
    }

    public static void DrawGUI(Rect position, SerializedProperty property, GUIContent label, ref bool valueFoldout, ref ReorderableList userControllerList, ref bool controlFoldout)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        if (property.isExpanded)
        {
            //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        }
        var infoPosition = position;
        infoPosition.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.LabelField(infoPosition, label);

        //var infoPosition = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        infoPosition.y += EditorGUIUtility.singleLineHeight;

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects

        // Draw fields - pass GUIContent.none to each so they are drawn without labels
        DrawProp(infoPosition, property, ref valueFoldout, userControllerList);

        var controlsRect = position;
        controlsRect.y += GetListHeight(property, valueFoldout, ref userControllerList);
        controlsRect.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.indentLevel++;

        controlFoldout = EditorGUI.Foldout(controlsRect, controlFoldout, "Controls");
        if (controlFoldout)
        {
            EditorGUI.indentLevel++;
            controlsRect.y += EditorGUIUtility.singleLineHeight;

            DrawAddingProp(controlsRect, property);
            controlsRect.y += EditorGUIUtility.singleLineHeight * 2;

            var buttonRect = controlsRect;
            buttonRect.x += 30; // 2 indents.
            if (GUI.Button(buttonRect, "CLEAR ALL VALUES"))
            {
                ClearAllGOAPValues(property.FindPropertyRelative("m_goapValueList"));
            }
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    static void DrawProp(Rect position, SerializedProperty property, ref bool m_valueFoldout, ReorderableList userControllerList)
    {
        EditorGUI.indentLevel++;
        var listProperty = property.FindPropertyRelative("m_goapValueList");

        position.width -= EditorGUIUtility.singleLineHeight;
        position.height = EditorGUIUtility.singleLineHeight;

        string countString = "Count : ";
        countString += listProperty.arraySize;
        m_valueFoldout = EditorGUI.Foldout(position, m_valueFoldout, countString);
        position.y += EditorGUIUtility.singleLineHeight;
        if (!m_valueFoldout)
        {
            EditorGUI.indentLevel--;
            return;
        }

        Rect removeButtonPos = position;
        removeButtonPos.width = EditorGUIUtility.singleLineHeight;
        removeButtonPos.x = position.x + position.width;

        Rect labelPos = position;
        Rect elementPos = position;
        labelPos.width /= 2;
        elementPos.width /= 2;
        elementPos.x += elementPos.width;

        EditorGUI.indentLevel++;
        EnableReorderableList(property, listProperty, ref userControllerList);
        userControllerList.DoList(position);

        EditorGUI.indentLevel--;
        EditorGUI.indentLevel--;
    }

    static float GetListHeight(SerializedProperty property, bool valueFoldout, ref ReorderableList userControllerList)
    {
        var listProperty = property.FindPropertyRelative("m_goapValueList");

        float height = 1;
        if (valueFoldout)
        {
            height += listProperty.arraySize;
        }
        height = Mathf.Max(height, 2);
        //return EditorGUIUtility.singleLineHeight * (height);

        EnableReorderableList(property, listProperty, ref userControllerList);

        height = EditorGUIUtility.singleLineHeight * 2;
        if (valueFoldout)
        {
            height = userControllerList.GetHeight() + EditorGUIUtility.singleLineHeight;
        }

        return height;
    }

    static void EnableReorderableList(SerializedProperty property, SerializedProperty listProperty, ref ReorderableList userControllerList)
    {
        if (userControllerList == null)
        {
            userControllerList = new ReorderableList(listProperty.GetUnderlyingValue() as IList, typeof(GOAPValue), true, false, true, true);
            userControllerList.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
            {
                var element = listProperty.GetArrayElementAtIndex(index);

                var keyPosition = position;
                keyPosition.width /= 2;
                var valuePosition = keyPosition;
                valuePosition.x += valuePosition.width;

                var keyProperty = element.FindPropertyRelative("m_key");
                EditorGUI.LabelField(keyPosition, keyProperty.GetUnderlyingValue().ToString());
                //GOAPValuePropertyDrawer.DrawGOAPKeyField(keyPosition, element, GUIContent.none);
                GOAPValuePropertyDrawer.DrawGOAPValueField(valuePosition, element);
            };

            userControllerList.onAddCallback = (ReorderableList list) =>
            {
                AddGOAPValue(listProperty, property.FindPropertyRelative("m_editorValueToAdd"));
                Debug.Log("Added");
            };
        }
    }

    public static float GetPropertyHeight(SerializedProperty property, bool valueFoldout, ref ReorderableList userControllerList, bool controlFoldout)
    {
        return GetListHeight(property, valueFoldout, ref userControllerList) + GetButtonsHeight(controlFoldout);
    }

    static float GetButtonsHeight(bool controlFoldout)
    {
        float height = EditorGUIUtility.singleLineHeight;
        if (controlFoldout)
        {
            height *= 5;
        }

        return height;
    }
}
