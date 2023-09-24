using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[FilePath("BBB/NoAction/PlanningExplorer/PlanningWindowSettings.sav", FilePathAttribute.Location.PreferencesFolder)]
public class PlanningExplorerWindowSettings : ScriptableSingleton<PlanningExplorerWindowSettings>
{
    [SerializeField] Color m_goalColour = Color.green;
    [SerializeField] Color m_focusedColour = Color.yellow;
    [SerializeField] Color m_focusedParentColour = new Color(0.6f, 0.6f, 0.2f);

    [SerializeField] Color m_stateViewColour = new Color(0.2f, 0.2f, 0.2f);
    [SerializeField] Color m_treeBranchAreaColour = new Color(0.5f, 0.5f, 0.5f);

    public Color goalColour { get { return m_goalColour; } }
    public Color focusedColour { get { return m_focusedColour; } }
    public Color focusedParentColour { get { return m_focusedParentColour; } }

    public Color stateViewColour { get { return m_stateViewColour; } }
    public Color treeBranchAreaColour { get { return m_treeBranchAreaColour; } }

    public void DrawGUI(Rect position)
    {
        Rect currentPosition = position;
        currentPosition.height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();

        m_goalColour = EditorGUI.ColorField(currentPosition, "Goal Colour", m_goalColour);
        currentPosition.y += currentPosition.height;
        m_focusedColour = EditorGUI.ColorField(currentPosition, "Focused Colour", m_focusedColour);
        currentPosition.y += currentPosition.height;
        m_focusedParentColour = EditorGUI.ColorField(currentPosition, "Focused Parent Colour", m_focusedParentColour);
        currentPosition.y += currentPosition.height;

        m_stateViewColour = EditorGUI.ColorField(currentPosition, "State View Colour", m_stateViewColour);
        currentPosition.y += currentPosition.height;
        m_treeBranchAreaColour = EditorGUI.ColorField(currentPosition, "Tree Branch Colour", m_treeBranchAreaColour);
        currentPosition.y += currentPosition.height;

        if (EditorGUI.EndChangeCheck())
        {
            Save(true);
        }
    }
}
