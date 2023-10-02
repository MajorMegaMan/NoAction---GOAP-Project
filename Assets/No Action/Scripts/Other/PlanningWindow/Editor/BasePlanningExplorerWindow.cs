using BBB.GOAP;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public abstract class BasePlanningExplorerWindow<TAction> : EditorWindow where TAction : class, IGOAPAction
{
    Object m_actionHolder;
    Object m_goal;

    bool m_isInitialised = false;
    bool m_isSearching = false;
    bool m_stepping = false;
    int m_stepPerFrame = 30;

    GOAPPlannerObject<TAction> m_plannerObject;

    GOAPWorldState m_windowWorldState;

    int m_depthLimit = 5;

    int m_searchAllocation = 65536;

    PlanningStateReadValues m_top = null;
    PlanningStateReadValues m_focused = null;
    Dictionary<GOAPPlanningWorldState<TAction>, PlanningStateReadValues> m_foldouts;

    GUIStyle m_leafStyle;

    ScrollHelper m_treeScroller = new ScrollHelper();

    Queue<int> m_averageSearchQueue = new Queue<int>();
    int m_averageSeacrhCount = 0;

    public IActionCollectionHolder<TAction> actionCollectionHolder { get { return m_actionHolder as IActionCollectionHolder<TAction>; } }
    public HashSet<TAction> actionSet { get { if (m_actionHolder != null) return actionCollectionHolder.GetActionCollection().GetActions(); else return null; } }

    protected virtual void OnGUI()
    {
        m_leafStyle = new GUIStyle(GUI.skin.button);

        var currentPosition = position;
        currentPosition.x = 0;
        currentPosition.y = 0;

        currentPosition.height = EditorGUIUtility.singleLineHeight;

        m_actionHolder = EditorGUI.ObjectField(currentPosition, "Actions", m_actionHolder, typeof(IActionCollectionHolder<TAction>), true);

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        m_goal = EditorGUI.ObjectField(currentPosition, "Goal", m_goal, typeof(IGOAPGoal), true);

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        m_depthLimit = EditorGUI.IntField(currentPosition, "Depth Limit", m_depthLimit);

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        m_searchAllocation = EditorGUI.IntField(currentPosition, "Search Allocation", m_searchAllocation);

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        m_stepping = EditorGUI.Toggle(currentPosition, "Stepping", m_stepping);

        currentPosition.y += EditorGUIUtility.singleLineHeight;
        m_stepPerFrame = EditorGUI.IntField(currentPosition, "StepPerFrame", m_stepPerFrame);
        m_stepPerFrame = Mathf.Max(m_stepPerFrame, 1);

        if (m_stepping && m_isSearching)
        {
            PauseSearching();
        }

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        if (!m_isInitialised)
        {
            if (GUI.Button(currentPosition, "Start Search"))
            {
                StartSearching();
            }

            currentPosition.y += EditorGUIUtility.singleLineHeight;
        }
        else
        {
            if (m_isSearching)
            {
                if (GUI.Button(currentPosition, "Pause Search"))
                {
                    PauseSearching();
                }
            }
            else
            {
                if (GUI.Button(currentPosition, "Resume Search"))
                {
                    StartSearching();
                }
            }

            currentPosition.y += EditorGUIUtility.singleLineHeight;

            if (m_stepping)
            {
                if (GUI.Button(currentPosition, "Step"))
                {
                    Step();
                }
                currentPosition.y += EditorGUIUtility.singleLineHeight;
            }

            if (GUI.Button(currentPosition, "End Search"))
            {
                StopSearching();
            }
        }

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        if (GUI.Button(currentPosition, "Allocate"))
        {
            AllocateSearchSpace(m_searchAllocation);
            //int thing = 9;
        }

        currentPosition.y += EditorGUIUtility.singleLineHeight;

        if (GUI.Button(currentPosition, "Clear"))
        {
            StopSearching();
            m_top = null;
            m_focused = null;
        }
        currentPosition.y += EditorGUIUtility.singleLineHeight;

        Rect statesArea = currentPosition;
        statesArea.width *= 0.5f;
        statesArea.height = position.height - currentPosition.y;

        statesArea.width -= 30;
        statesArea.x += 20;
        statesArea.height -= 40;
        statesArea.y += 20;

        Rect treeWindowArea = statesArea;
        treeWindowArea.x += treeWindowArea.width + 20;

        Rect treeViewArea = treeWindowArea;
        treeViewArea.y = 0;
        treeViewArea.x = 0;
        treeViewArea.height = GetTreeHeight() + EditorGUIUtility.singleLineHeight;

        EditorGUI.DrawRect(statesArea, PlanningExplorerWindowSettings.instance.stateViewColour);
        EditorGUI.DrawRect(treeWindowArea, PlanningExplorerWindowSettings.instance.treeBranchAreaColour);

        DrawWorldStateArea(statesArea);

        m_treeScroller.Begin(treeWindowArea, treeViewArea);
        PrintLabelTree(treeViewArea);
        m_treeScroller.End();

        currentPosition.y = position.height - EditorGUIUtility.singleLineHeight;

        PrintStats(currentPosition);
    }

    private void Update()
    {
        Search();
    }

    private void OnEnable()
    {
        m_plannerObject = new GOAPPlannerObject<TAction>();
        m_foldouts = new Dictionary<GOAPPlanningWorldState<TAction>, PlanningStateReadValues>();
        m_windowWorldState = InitialiseWindowWorldState();

        LoadWindowData();
    }

    private void OnDisable()
    {
        SaveWindowData();
        m_plannerObject.End();
    }

    void LoadWindowData()
    {
        //m_worldStateContainer = ScriptableObject.CreateInstance<WindowDataContainer>();

        //m_worldStateProperty = new SerializedObject(m_worldStateContainer).FindProperty("m_worldState");
    }

    void SaveWindowData()
    {
        //DestroyImmediate(m_worldStateContainer);
    }

    void SetFocused(PlanningStateReadValues planningState)
    {
        if (m_focused != null)
        {
            FocusParentTree(m_focused, false);
        }
        m_focused = planningState;
        if (m_focused != null)
        {
            UnFoldParentTree(planningState);
            FocusParentTree(m_focused, true);
        }
    }

    void UnFoldParentTree(PlanningStateReadValues planningState)
    {
        var current = planningState.parent;
        while(current != null)
        {
            current.foldout = true;
            current = current.parent;
        }
    }

    #region WorldStateMethods
    protected abstract GOAPWorldState InitialiseWindowWorldState();

    void DrawWorldStateArea(Rect position)
    {
        if (m_focused != null)
        {
            DrawSelectedWorldState(position, m_focused);
            position.y += EditorGUIUtility.singleLineHeight * 8;

            position.y += DrawActionsLabel(position, m_focused);
            DrawWorldStateParentLine(position, m_focused);
        }
    }

    public abstract void DrawSelectedWorldState(Rect position, PlanningStateReadValues planningState);

    float DrawActionsLabel(Rect position, PlanningStateReadValues planningState)
    {
        GOAPWorldState worldState = planningState.worldState;

        string successString = "";
        string failString = "";

        float totalHeight;

        if (planningState.childLevel < m_depthLimit)
        {
            // This will have Children
            // Grab the children as actions, so that we can use them for buttons to be able to focus their states.
            foreach (var planState in planningState.children)
            {
                successString += planState.actionName + ", ";
            }

            // Grab the failed actions and display them
            var actions = actionSet;
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    if (!action.CheckCondition(worldState))
                    {
                        failString += action.actionName + ", ";
                    }
                }
            }

            position.height = EditorGUIUtility.singleLineHeight;
            totalHeight = position.height;
            EditorGUI.LabelField(position, "Actions");

            //position.y += position.height;
            //EditorGUI.LabelField(position, "Ready  : " + successString);

            float successHeight = DrawSuccessActionsAsButtons(position, planningState.children, 3);
            position.y += successHeight;
            totalHeight += successHeight;

            //position.y += position.height;
            EditorGUI.LabelField(position, "Unable : " + failString);
            totalHeight += position.height;
        }
        else
        {
            // This should not have children
            var actions = actionSet;
            if (actions != null)
            {
                // Check actions that could be performed and actions that can't be performed.
                foreach (var action in actions)
                {
                    if (action.CheckCondition(worldState))
                    {
                        successString += action.actionName + ", ";
                    }
                    else
                    {
                        failString += action.actionName + ", ";
                    }
                }
            }

            position.height = EditorGUIUtility.singleLineHeight;
            EditorGUI.LabelField(position, "Actions");
            position.y += position.height;
            EditorGUI.LabelField(position, "Ready  : " + successString);
            position.y += position.height;
            EditorGUI.LabelField(position, "Unable : " + failString);

            totalHeight = position.height * 3;
        }

        return totalHeight;
    }

    float DrawSuccessActionsAsButtons(Rect position, ICollection<PlanningStateReadValues> actionedPlanningStates, int buttonsPerRow)
    {
        var buttonPos = position;
        buttonPos.height = EditorGUIUtility.singleLineHeight;
        buttonPos.width = buttonPos.width / buttonsPerRow;

        float totalHeight = buttonPos.height;
        int i = 0;
        foreach (var state in actionedPlanningStates)
        {
            if(i >= buttonsPerRow)
            {
                i = 0;
                buttonPos.y += buttonPos.height;
                totalHeight += buttonPos.height;
                buttonPos.x = position.x;
            }

            if (GUI.Button(buttonPos, state.actionName))
            {
                SetFocused(state);
            }

            buttonPos.x += buttonPos.width;
            i++;
        }

        return totalHeight;
    }

    void DrawWorldStateParentLine(Rect position, PlanningStateReadValues planningState)
    {
        var current = planningState.parent;

        var buttonPos = position;
        buttonPos.height = EditorGUIUtility.singleLineHeight;

        while(current != null)
        {
            if(GUI.Button(buttonPos, current.planName))
            {
                SetFocused(current);
            }
            buttonPos.y += buttonPos.height;
            current = current.parent;
        }
    }

    #endregion // WorldStateMethods

    #region SearchMethods
    void AllocateSearchSpace(int count)
    {
        m_plannerObject.ExpandPlanningPool(m_windowWorldState, count);
    }

    void StartSearching()
    {
        if (m_goal == null)
        {
            Debug.LogWarning("No goal has been set.");
            return;
        }

        if (!m_isInitialised)
        {
            m_plannerObject.Begin(m_windowWorldState);
            m_isInitialised = true;

            m_foldouts.Clear();

            var actionCollection = ((IActionCollectionHolder<TAction>)m_actionHolder).GetActionCollection();
            actionCollection.ValidateCollection();

            var actions = actionCollection.GetActions();
            m_top = new PlanningStateReadValues(m_plannerObject.topSearchState, actions);
            m_foldouts.Add(m_plannerObject.topSearchState, m_top);
            m_focused = m_top;
            FocusParentTree(m_top, true);

            m_averageSeacrhCount = 0;
            m_averageSearchQueue.Clear();
            for(int i = 0; i < 10; i++)
            {
                m_averageSearchQueue.Enqueue(0);
            }
        }

        m_isSearching = true;
    }

    void FoundGoal()
    {
        var goals = m_plannerObject.goals;
        if(goals.Length > 0)
        {
            var first = goals[0];
            if (m_foldouts.TryGetValue(first, out var goalState))
            {
                GoalParentTree(goalState, true);
            }
        }
        else
        {
            Debug.LogWarning("Goal was not found. Depth may be too shallow or no path of actions could be found to reach the target worldstate.");
        }

        StopSearching();
    }

    void StopSearching()
    {
        m_isSearching = false;
        m_plannerObject.End();
        m_isInitialised = false;
    }

    void PauseSearching()
    {
        Debug.LogWarning("Pause.");
        m_isSearching = false;
    }

    void Search()
    {
        if (m_isSearching && !m_stepping)
        {
            Debug.Log("Searching...");
            for(int i = 0; i < m_stepPerFrame && m_plannerObject.searchStateCount > 0; i++)
            {
                Step();
            }
            Repaint();
        }
    }

    void Step()
    {
        int prevSearchCount = m_plannerObject.searchStateCount;

        var actions = ((IActionCollectionHolder<TAction>)m_actionHolder).GetActionCollection().GetActions();
        m_plannerObject.Step(m_goal as IGOAPGoal, actions, m_depthLimit);
        if (m_plannerObject.searchStateCount > 0)
        {
            AddPlanningState(m_plannerObject.topSearchState, actions);

            int nextSearchCount = m_plannerObject.searchStateCount;
            int diff = nextSearchCount - prevSearchCount;

            if (m_plannerObject.topSearchState.depth >= m_depthLimit)// && diff == 0)
            {
                return;
            }
            int averageSeacrhCount = 0;
            if (m_averageSearchQueue.Count > 0)
            {
                //m_averageSearchQueue.Dequeue();
                m_averageSearchQueue.Enqueue(diff);

                foreach (int count in m_averageSearchQueue)
                {
                    averageSeacrhCount += count;
                }
                averageSeacrhCount /= m_averageSearchQueue.Count;

                m_averageSeacrhCount = averageSeacrhCount;
            }
        }
        else
        {
            FoundGoal();
        }
    }

    void AddPlanningState(GOAPPlanningWorldState<TAction> planningState, ICollection<TAction> actions)
    {
        var readValues = new PlanningStateReadValues(planningState, actions);
        m_foldouts.Add(planningState, readValues);
        if (m_foldouts.TryGetValue(planningState.parent, out PlanningStateReadValues parentReadValues))
        {
            parentReadValues.RemoveFailedAction(planningState.action);
            parentReadValues.children.Add(readValues);
            readValues.childLevel = parentReadValues.childLevel + 1;
            readValues.parent = parentReadValues;
        }
    }
    #endregion // SearchMethods

    #region TreeMethods
    void PrintLabelTree(Rect position)
    {
        if (m_top == null)
        {
            EditorGUI.Foldout(position, false, "Start a New Search");
            return;
        }

        var labelPos = position;
        labelPos.height = EditorGUIUtility.singleLineHeight;
        labelPos.width = EditorGUIUtility.labelWidth;
        Stack<PlanningStateReadValues> toPrint = new Stack<PlanningStateReadValues>();
        Stack<PlanningStateReadValues> toAdd = new Stack<PlanningStateReadValues>();
        toPrint.Push(m_top);
        while (toPrint.Count > 0)
        {
            var next = toPrint.Pop();
            if (next.foldout)
            {
                foreach (var child in next.children)
                {
                    toAdd.Push(child);
                }

                while (toAdd.Count > 0)
                {
                    toPrint.Push(toAdd.Pop());
                }
            }
            labelPos.x = position.x + (next.childLevel * 20); // indent to the child level
            labelPos.y += EditorGUIUtility.singleLineHeight;

            if (next.DoFoldoutButton(labelPos, m_leafStyle))
            {
                SetFocused(next);
            }
        }
    }

    float GetTreeHeight()
    {
        if (m_top == null)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        float height = 0.0f;
        var labelPos = position;
        Stack<PlanningStateReadValues> toPrint = new Stack<PlanningStateReadValues>();
        Stack<PlanningStateReadValues> toAdd = new Stack<PlanningStateReadValues>();
        toPrint.Push(m_top);
        while (toPrint.Count > 0)
        {
            var next = toPrint.Pop();
            if (next.foldout)
            {
                foreach (var child in next.children)
                {
                    toAdd.Push(child);
                }

                while (toAdd.Count > 0)
                {
                    toPrint.Push(toAdd.Pop());
                }
            }
            height += EditorGUIUtility.singleLineHeight;
        }

        return height;
    }

    void FocusParentTree(PlanningStateReadValues planningState, bool isFocused)
    {
        var current = planningState;

        current.isFocusState = isFocused;
        current.SetLeafColour();
        if (planningState.isFocusState)
        {
            planningState.drawColour = PlanningExplorerWindowSettings.instance.focusedColour;
        }
        current = current.parent;

        while (current != null)
        {
            current.isFocusState = isFocused;
            current.SetLeafColour();
            current = current.parent;
        }
    }

    void GoalParentTree(PlanningStateReadValues planningState, bool isGoal)
    {
        var current = planningState;
        while (current != null)
        {
            current.isGoalState = isGoal;
            current.SetLeafColour();
            current = current.parent;
        }
    }

    void PrintStats(Rect position)
    {
        int currentSearchCount = m_plannerObject.searchStateCount;
        int currentProcessedStates = m_foldouts.Count;

        string msg = "Search Count : ";

        msg += currentSearchCount + " / " + currentProcessedStates;
        msg += "-- Average Search : " + m_averageSeacrhCount;

        msg += " -- Pruned Count : " + m_plannerObject.prunedCount;

        EditorGUI.LabelField(position, msg);
    }

    #endregion // TreeMethods

    public class PlanningStateReadValues
    {
        public PlanningStateReadValues parent;
        public GOAPPlanningWorldState<TAction> planningState;
        public List<PlanningStateReadValues> children = new List<PlanningStateReadValues>();
        public HashSet<TAction> unsearchedActions = new HashSet<TAction>();
        public bool foldout = false;

        public int childLevel = 0;

        public bool isFocusState = false;
        public bool isGoalState = false;
        public Color drawColour = Color.white;

        public GOAPWorldState worldState { get { return planningState.worldState; } }
        public bool isPruned { get { return planningState.twinState != null; } }

        public string planName
        {
            get
            {
                if (planningState.action != null)
                {
                    return childLevel + ". " + planningState.action.actionName;
                }
                else
                {
                    return "No Action";
                }
            }
        }

        public string actionName
        {
            get
            {
                if (planningState.action != null)
                {
                    return planningState.action.actionName;
                }
                else
                {
                    return "No Action";
                }
            }
        }

        public PlanningStateReadValues(GOAPPlanningWorldState<TAction> planningState, ICollection<TAction> actions)
        {
            this.planningState = planningState;
            foreach (var action in actions)
            {
                unsearchedActions.Add(action);
            }

            SetLeafColour();
        }

        public bool DoFoldoutButton(Rect position, string msg, GUIStyle style)
        {
            var oldColor = GUI.backgroundColor;
            GUI.backgroundColor = drawColour;

            string buttonPrint = planName;
            if (msg.Length > 0)
            {
                buttonPrint += " - " + msg;
            }
            foldout = EditorGUI.Foldout(position, foldout, "");

            position.width -= 20;
            position.x += 20;
            bool press = GUI.Button(position, buttonPrint, style);

            GUI.backgroundColor = oldColor;

            return press;
        }

        public bool DoFoldoutButton(Rect position, GUIStyle style)
        {
            return DoFoldoutButton(position, "", style);
        }

        public void RemoveFailedAction(TAction action)
        {
            unsearchedActions.Remove(action);
        }

        public float CalcHeight()
        {
            float height = EditorGUIUtility.singleLineHeight;
            if (foldout)
            {
                foreach (PlanningStateReadValues readValues in children)
                {
                    height += CalcHeight();
                }
            }

            return height;
        }

        public void SetLeafColour()
        {
            if (isFocusState)
            {
                drawColour = PlanningExplorerWindowSettings.instance.focusedParentColour;
            }
            else if (isGoalState)
            {
                drawColour = PlanningExplorerWindowSettings.instance.goalColour;
            }
            else if (isPruned)
            {
                drawColour = PlanningExplorerWindowSettings.instance.pruneColour;
            }
            else
            {
                drawColour = Color.white;
            }
        }
    }

    public class ScrollHelper
    {
        // The position on of the scrolling viewport
        public Vector2 scrollPosition = Vector2.zero;

        public void Begin(Rect position, Rect scrollView)
        {
            // An absolute-positioned example: We make a scrollview that has a really large client
            // rect and put it in a small rect on the screen.
            scrollPosition = GUI.BeginScrollView(position, scrollPosition, scrollView);
        }

        public void End()
        {
            // End the scroll view that we began above.
            GUI.EndScrollView();
        }
    }
}

