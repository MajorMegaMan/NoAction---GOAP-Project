using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

public class PlanningExplorerWindowSettingsInspector : EditorWindow
{
    [MenuItem("No Action/Planner Window Settings")]
    public static void ShowExample()
    {
        PlanningExplorerWindowSettingsInspector wnd = GetWindow<PlanningExplorerWindowSettingsInspector>();
        wnd.titleContent = new GUIContent("Planner Window Settings");
    }

    private void OnGUI()
    {
        Rect currentPosition = position;
        currentPosition.x = 0;
        currentPosition.y = 0;
        PlanningExplorerWindowSettings.instance.DrawGUI(currentPosition);
    }
}
