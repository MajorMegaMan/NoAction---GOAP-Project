using System.Collections.Generic;

namespace BBB.GOAP
{
    public class GOAPPlannerObject<TAction> where TAction : IGOAPAction
    {
        List<GOAPPlanningWorldState<TAction>> m_goalStates = new List<GOAPPlanningWorldState<TAction>>();
        GOAPPlanningWorldState<TAction> m_currentGoal = null;
        GOAPPlanningWorldState<TAction> m_startPlanningState = null;
        int m_goalDepth = int.MaxValue;

        Queue<GOAPPlanningWorldState<TAction>> m_statesToSearch = new Queue<GOAPPlanningWorldState<TAction>>();
        List<GOAPPlanningWorldState<TAction>> m_activatedSeachStates = new List<GOAPPlanningWorldState<TAction>>();

        Dictionary<GOAPPlanningWorldState<TAction>, GOAPPlanningWorldState<TAction>> m_uniqueStates = new Dictionary<GOAPPlanningWorldState<TAction>, GOAPPlanningWorldState<TAction>>(PlannerStateComparer.Instance);
        int m_prunedCount = 0;

        public int searchStateCount { get { return m_statesToSearch.Count; } }
        public GOAPPlanningWorldState<TAction> topSearchState { get { return m_statesToSearch.Peek(); } }
        public GOAPPlanningWorldState<TAction>[] goals { get { return m_goalStates.ToArray(); } }
        public bool foundGoal { get { return m_currentGoal != null; } }
        public int prunedCount { get { return m_prunedCount; } }

        public Queue<TAction> GetActionPlan(GOAPWorldState currentWorldState, IGOAPGoal goal, ICollection<TAction> actions, int depthLimit = 10)
        {
            Begin(currentWorldState);
            while (m_statesToSearch.Count > 0)
            {
                Step(goal, actions, depthLimit);
            }

            var actionPlan = ExtractActionPlan();
            DeactivateSearchStates();

            return actionPlan;
        }

        void ProcessPlanningState(GOAPPlanningWorldState<TAction> planningState, IGOAPGoal goal, ICollection<TAction> actions, int depthLimit)
        {
            if (goal.CheckWorldState(planningState.worldState))
            {
                // This is the goal state.
                m_goalStates.Add(planningState);
                if (planningState.weight < m_currentGoal.weight || m_currentGoal == m_startPlanningState)
                {
                    m_currentGoal = planningState;
                    m_goalDepth = planningState.depth;
                }
            }
            else if (planningState.depth > depthLimit || planningState.depth > m_goalDepth)
            {
                // stop branching this planning node.
            }
            else
            {
                // This is a branched planning State
                // Continue searching
                foreach (var action in actions)
                {
                    switch(planningState.ProcessActionedWorldStateWithPruning(action, out var actionedWorldState, m_uniqueStates))
                    {
                        case GOAPPlanningWorldState<TAction>.ActionCheck.Success:
                            {
                                m_activatedSeachStates.Add(actionedWorldState);
                                m_statesToSearch.Enqueue(actionedWorldState);
                                break;
                            }
                        case GOAPPlanningWorldState<TAction>.ActionCheck.Pruned:
                            {
                                // this node should be pruned. it is the same as an existing node.
                                // stop branching this planning node.
                                actionedWorldState.twinState = m_uniqueStates[actionedWorldState];
                                m_prunedCount++;

                                // Even though this was pruned. it is still an activated state. This is for editor inspection reasons, but I don't think I should keep a reference if it's unused.
                                m_activatedSeachStates.Add(actionedWorldState);
                                break;
                            }
                        case GOAPPlanningWorldState<TAction>.ActionCheck.BadCondition:
                            {
                                // Actioned world state needs to be deactivated
                                GOAPPlanningWorldState<TAction>.DeactivatePlanningState(actionedWorldState);
                                break;
                            }
                    }

                    //if (planningState.ProcessActionedWorldState(action, out var actionedWorldState))
                    //{
                    //    if (m_uniqueStates.ContainsKey(actionedWorldState))
                    //    {
                    //        // this node should be pruned. it is the same as an existing node.
                    //        // stop branching this planning node.
                    //        actionedWorldState.twinState = m_uniqueStates[actionedWorldState];
                    //        m_prunedCount++;
                    //    }
                    //    else
                    //    {
                    //        m_uniqueStates.Add(actionedWorldState, actionedWorldState);
                    //        m_statesToSearch.Enqueue(actionedWorldState);
                    //    }
                    //    m_activatedSeachStates.Add(actionedWorldState);
                    //}
                }
            }
        }

        // Sets up variables for the planner to begin.
        public void Begin(GOAPWorldState currentWorldState)
        {
            m_goalStates.Clear();
            m_statesToSearch.Clear();
            m_uniqueStates.Clear();

            GOAPPlanningWorldState<TAction> startPlanningState = GOAPPlanningWorldState<TAction>.ActivateNextPlanningState();
            startPlanningState.AddCopyValues(currentWorldState);//, goal.GetKeys());
            m_startPlanningState = startPlanningState;
            m_currentGoal = startPlanningState;
            m_goalDepth = int.MaxValue;

            m_statesToSearch.Enqueue(startPlanningState);

            m_prunedCount = 0;
        }

        // called during each planning step.
        public void Step(IGOAPGoal goal, ICollection<TAction> actions, int depthLimit)
        {
            var currentPlanningState = m_statesToSearch.Dequeue();
            ProcessPlanningState(currentPlanningState, goal, actions, depthLimit);
        }

        public bool ExtractActionPlanToStack(Stack<TAction> actionPlanStack)
        {
            if (m_currentGoal == null)
            {
                // no goal found.
                return false;
            }

            var currentNode = m_currentGoal;
            while (currentNode.parent != null)
            {
                actionPlanStack.Push(currentNode.action);
                currentNode = currentNode.parent;
            }

            return true;
        }

        public Stack<TAction> ExtractActionPlanStack()
        {
            Stack<TAction> actionPlanStack = new Stack<TAction>();
            ExtractActionPlanToStack(actionPlanStack);            
            return actionPlanStack;
        }

        public void End()
        {
            DeactivateSearchStates();
        }

        void DeactivateSearchStates()
        {
            foreach(var state in m_activatedSeachStates)
            {
                GOAPPlanningWorldState<TAction>.DeactivatePlanningState(state);
            }
            m_activatedSeachStates.Clear();
        }

        public Queue<TAction> ExtractActionPlan()
        {
            // Get a stack first.
            Stack<TAction> actionPlanStack = ExtractActionPlanStack();

            // Reverse the stack into a queue.
            Queue<TAction> actionPlanQueue = new Queue<TAction>();
            while (actionPlanStack.Count > 0)
            {
                actionPlanQueue.Enqueue(actionPlanStack.Pop());
            }
            return actionPlanQueue;
        }

        // Expands the pool to target size.
        public void ExpandPlanningPool(GOAPWorldState worldState, int count)
        {
            GOAPPlanningWorldState<TAction>.ExpandPlanningPool(worldState, count);
        }

        class PlannerStateComparer : IEqualityComparer<GOAPPlanningWorldState<TAction>>
        {

            public static PlannerStateComparer Instance = new PlannerStateComparer();

            Dictionary<TAction, int> m_lhsActionCounts = new Dictionary<TAction, int>();
            Dictionary<TAction, int> m_rhsActionCounts = new Dictionary<TAction, int>();

            PlannerStateComparer()
            {

            }

            bool IEqualityComparer<GOAPPlanningWorldState<TAction>>.Equals(BBB.GOAP.GOAPPlanningWorldState<TAction> x, BBB.GOAP.GOAPPlanningWorldState<TAction> y)
            {
                if(x.depth != y.depth)
                    return false;

                FillActionCounts(x, m_lhsActionCounts);
                FillActionCounts(y, m_rhsActionCounts);

                var keys = m_lhsActionCounts.Keys;

                foreach(var key in keys)
                {
                    if(m_rhsActionCounts.TryGetValue(key, out int value))
                    {
                        if (m_lhsActionCounts[key] != value)
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                return true;
            }

            int IEqualityComparer<GOAPPlanningWorldState<TAction>>.GetHashCode(BBB.GOAP.GOAPPlanningWorldState<TAction> obj)
            {
                FillActionCounts(obj, m_lhsActionCounts);

                var keys = m_lhsActionCounts.Keys;

                int hash = 0;
                foreach (var key in keys)
                {
                    hash |= key.GetHashCode();
                }

                return hash;
            }

            void FillActionCounts(GOAPPlanningWorldState<TAction> planningState, Dictionary<TAction, int> actionCounts)
            {
                actionCounts.Clear();

                var current = planningState;
                while(current.parent != null)
                {
                    if(actionCounts.TryAdd(current.action, 0))
                    {

                    }
                    else
                    {
                        actionCounts[current.action]++;
                    }
                    current = current.parent;
                }
            }
        }
    }
}
