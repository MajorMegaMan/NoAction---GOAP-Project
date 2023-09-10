using System.Collections.Generic;
using System.Linq;

namespace BBB.GOAP
{
    /*

        public static class GOAPPlanner<TAction> where TAction : IGOAPAction
    {
        static List<GOAPPlanningWorldState<TAction>> _goalStates = new List<GOAPPlanningWorldState<TAction>>();
        static GOAPPlanningWorldState<TAction> _currentGoal = null;
        static GOAPPlanningWorldState<TAction> _startPlanningState = null;
        static int _goalDepth = int.MaxValue;

        static Queue<GOAPPlanningWorldState<TAction>> _statesToSearch = new Queue<GOAPPlanningWorldState<TAction>>();

        public static Queue<TAction> GetActionPlan(GOAPWorldState currentWorldState, IGOAPGoal goal, ICollection<TAction> actions, int depthLimit = 10)
        {
            _goalStates.Clear();
            _statesToSearch.Clear();

            GOAPPlanningWorldState<TAction> startPlanningState = GOAPPlanningWorldState<TAction>.ActivateNextPlanningState();
            startPlanningState.AddCopyValues(currentWorldState);//, goal.GetKeys());
            _startPlanningState = startPlanningState;
            _currentGoal = startPlanningState;
            _goalDepth = int.MaxValue;

            _statesToSearch.Enqueue(startPlanningState);
            while (_statesToSearch.Count > 0)
            {
                var currentPlanningState = _statesToSearch.Dequeue();
                ProcessPlanningState(currentPlanningState, goal, actions, depthLimit);
            }

            if(_currentGoal == null)
            {
                // no goal found.
                GOAPPlanningWorldState<TAction>.DeactivateAll();
                return null;
            }

            Stack<TAction> actionPlanStack = new Stack<TAction>();
            while(_currentGoal.parent != null)
            {
                actionPlanStack.Push(_currentGoal.action);
                _currentGoal = _currentGoal.parent;
            }

            GOAPPlanningWorldState<TAction>.DeactivateAll();

            Queue<TAction> actionPlanQueue = new Queue<TAction>();
            while(actionPlanStack.Count > 0)
            {
                actionPlanQueue.Enqueue(actionPlanStack.Pop());
            }
            return actionPlanQueue;
        }

        static void ProcessPlanningState(GOAPPlanningWorldState<TAction> planningState, IGOAPGoal goal, ICollection<TAction> actions, int depthLimit)
        {
            if(goal.CheckWorldState(planningState.worldState))
            {
                // This is the goal state.
                _goalStates.Add(planningState);
                if(planningState.weight < _currentGoal.weight || _currentGoal == _startPlanningState)
                {
                    _currentGoal = planningState;
                    _goalDepth = planningState.depth;
                }
            }
            else if(planningState.depth > depthLimit || planningState.depth > _goalDepth)
            {
                // stop branching this planning node.
            }
            else
            {
                // Continue searching
                foreach (var action in actions)
                {
                    if(planningState.ProcessActionedWorldState(action, out var actionedWorldState))
                    {
                        _statesToSearch.Enqueue(actionedWorldState);
                    }
                }
            }
        }

        // Expands the pool to target size.
        public static void ExpandPlanningPool(GOAPWorldState worldState, int count)
        {
            GOAPPlanningWorldState<TAction>.ExpandPlanningPool(worldState, count);
        }
    }



     */

    public static class GOAPPlanner<TAction> where TAction : IGOAPAction
    {
        static GOAPPlannerObject<TAction> _plannerObject = new GOAPPlannerObject<TAction>();

        public static Queue<TAction> GetActionPlan(GOAPWorldState currentWorldState, IGOAPGoal goal, ICollection<TAction> actions, int depthLimit = 10)
        {
            return _plannerObject.GetActionPlan(currentWorldState, goal, actions, depthLimit);
        }

        // Expands the pool to target size.
        public static void ExpandPlanningPool(GOAPWorldState worldState, int count)
        {
            GOAPPlanningWorldState<TAction>.ExpandPlanningPool(worldState, count);
        }
    }
}
