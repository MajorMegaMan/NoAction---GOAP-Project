using System.Collections.Generic;
using System.Linq;

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

        public int searchStateCount { get { return m_statesToSearch.Count; } }
        public GOAPPlanningWorldState<TAction> topSearchState { get { return m_statesToSearch.Peek(); } }
        public GOAPPlanningWorldState<TAction>[] goals { get { return m_goalStates.ToArray(); } }
        public bool foundGoal { get { return m_currentGoal != null; } }

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
                // Continue searching
                foreach (var action in actions)
                {
                    if (planningState.ProcessActionedWorldState(action, out var actionedWorldState))
                    {
                        m_activatedSeachStates.Add(actionedWorldState);
                        m_statesToSearch.Enqueue(actionedWorldState);
                    }
                }
            }
        }

        // Sets up variables for the planner to begin.
        public void Begin(GOAPWorldState currentWorldState)
        {
            m_goalStates.Clear();
            m_statesToSearch.Clear();

            GOAPPlanningWorldState<TAction> startPlanningState = GOAPPlanningWorldState<TAction>.ActivateNextPlanningState();
            startPlanningState.AddCopyValues(currentWorldState);//, goal.GetKeys());
            m_startPlanningState = startPlanningState;
            m_currentGoal = startPlanningState;
            m_goalDepth = int.MaxValue;

            m_statesToSearch.Enqueue(startPlanningState);
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
    }
}
