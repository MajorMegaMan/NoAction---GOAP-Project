using BBB.GOAP;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PlanningExplorerWindow : BasePlanningExplorerWindow<RecipePlateAction>
{
    const string myTitle = "Recipe Planner";

    static Color _elementTint = new Color(0.0f, 0.0f, 0.0f, 0.2f);

    [MenuItem("No Action/" + myTitle)]
    public static void ShowExample()
    {
        PlanningExplorerWindow wnd = GetWindow<PlanningExplorerWindow>();
        wnd.titleContent = new GUIContent(myTitle);
    }

    public override void DrawSelectedWorldState(Rect position, PlanningStateReadValues planningState)
    {
        var currentPosition = position;

        float worldStateHeight = DrawWorldState(currentPosition, planningState.worldState);
        currentPosition.y += worldStateHeight;

        //DrawActionsLabel(currentPosition, planningState.worldState);
    }

    protected override GOAPWorldState InitialiseWindowWorldState()
    {
        GOAPWorldState windowWorldState = new GOAPWorldState();
        windowWorldState.AddValue((int)ChefWorldStateEnum.Plate_BunCount, 0);
        windowWorldState.AddValue((int)ChefWorldStateEnum.Plate_LettuceCount, 0);
        windowWorldState.AddValue((int)ChefWorldStateEnum.Plate_TomatoCount, 0);
        windowWorldState.AddValue((int)ChefWorldStateEnum.Plate_BeefCount, 0);
        windowWorldState.AddValue((int)ChefWorldStateEnum.Plate_TotalCount, 0);
        return windowWorldState;
    }

    float DrawWorldState(Rect position, GOAPWorldState worldState)
    {
        var currentPosition = position;
        currentPosition.height = EditorGUIUtility.singleLineHeight;

        var valuePosition = currentPosition;
        valuePosition.width *= 0.5f;
        valuePosition.x = valuePosition.width;

        bool isTinted = false;

        var keys = worldState.Keys;
        foreach (var key in keys)
        {
            if (isTinted)
            {
                EditorGUI.DrawRect(currentPosition, _elementTint);
            }

            EditorGUI.LabelField(currentPosition, ((ChefWorldStateEnum)key).ToString());
            worldState.GetValue<int>(key, out var value);
            EditorGUI.LabelField(valuePosition, value.ToString());

            valuePosition.y += currentPosition.height;
            currentPosition.y += currentPosition.height;

            isTinted = !isTinted;
        }

        return keys.Length * currentPosition.height;
    }
}
