using System.Collections.Generic;
using System.Linq;

namespace BBB.GOAP
{
    // Temp class used for planning.
    class GOAPPlanningWorldState<TAction> where TAction : IGOAPAction
    {
        GOAPWorldState m_worldState = new GOAPWorldState();

        public GOAPPlanningWorldState<TAction> parent = null;
        public TAction action = default;
        public int depth = 0;
        public float weight = 0;

        public GOAPWorldState worldState { get { return m_worldState; } }

        private GOAPPlanningWorldState()
        {

        }

        static ObjectPool<GOAPPlanningWorldState<TAction>> _plannerStatesPool;
        static Dictionary<GOAPPlanningWorldState<TAction>, GOAPPlanningWorldState<TAction>> _activePlannerStates;

        static GOAPPlanningWorldState()
        {
            _plannerStatesPool = new ObjectPool<GOAPPlanningWorldState<TAction>>(16, CreatePlanningState, OnActivatePlanningState, OnDeactivatePlanningState);
            _activePlannerStates = new Dictionary<GOAPPlanningWorldState<TAction>, GOAPPlanningWorldState<TAction>>();
        }

        static GOAPPlanningWorldState<TAction> CreatePlanningState()
        {
            return new GOAPPlanningWorldState<TAction>();
        }

        static void OnActivatePlanningState(ref GOAPPlanningWorldState<TAction> planningState)
        {
            // initialise ready for use.
            planningState.Clear();
            planningState.parent = null;
            planningState.action = default;
            planningState.depth = 0;
            planningState.weight = 0;

            _activePlannerStates.Add(planningState, planningState);
        }

        static void OnDeactivatePlanningState(ref GOAPPlanningWorldState<TAction> planningState)
        {
            _activePlannerStates.Remove(planningState);
        }

        public static GOAPPlanningWorldState<TAction> ActivateNextPlanningState()
        {
            _plannerStatesPool.ActivateNext(out GOAPPlanningWorldState<TAction> planningState);
            while (planningState == null)
            {
                _plannerStatesPool.ExtendPool(1);
                _plannerStatesPool.ActivateNext(out planningState);
            }

            return planningState;
        }

        public static void DeactivatePlanningState(GOAPPlanningWorldState<TAction> planningState)
        {
            _plannerStatesPool.DeactivateObject(planningState);
        }

        public static void DeactivateAll()
        {
            var activeStates = _activePlannerStates.Keys.ToArray();
            foreach (var state in activeStates)
            {
                _plannerStatesPool.DeactivateObject(state);
            }
            _activePlannerStates.Clear();
        }

        public static void ExpandPlanningPool(GOAPWorldState worldState, int count)
        {
            // Only increase count if diff actually would increase the size.
            int diff = count - _plannerStatesPool.maxCapacity;
            if (diff > 0)
            {
                _plannerStatesPool.ExtendPool(diff);
                for (int i = 0; i < _plannerStatesPool.maxCapacity; i++)
                {
                    _plannerStatesPool.ActivateNext(out var planningState);
                    planningState.CreateEmpty(worldState);
                }
                DeactivateAll();
            }
        }

        public void AddCopyValues(GOAPWorldState target)
        {
            foreach (var value in target.m_values)
            {
                AddCopyValue(value);
            }
        }

        public void AddCopyValues(GOAPWorldState target, List<GOAPKey> keys)
        {
            foreach (var key in keys)
            {
                if (target.GetGOAPValue(key.id, out var targetValue))
                {
                    if (m_worldState.GetGOAPValue(key.id, out var value))
                    {
                        value.Copy(targetValue);
                    }
                    else
                    {
                        value = new GOAPValue(targetValue);
                        m_worldState.AddGOAPValue(value);
                    }
                }
            }
        }

        public void CreateEmpty(GOAPWorldState target)
        {
            foreach (var value in target.m_values)
            {
                var newValue = new GOAPValue(value.key);
                m_worldState.AddGOAPValue(newValue);
            }
        }

        public void AddCopyValue(GOAPValue value)
        {
            if (m_worldState.AddGOAPValue(new GOAPValue(value)))
            {
                // Value was added. No reason to recopy.
            }
            else
            {
                // Value already exists. Copy the value.
                m_worldState.GetGOAPValue(value, out var thisValue);
                thisValue.Copy(value);
            }
        }

        // Fills world state with empty values
        public void FillEmpty(GOAPWorldState target)
        {
            foreach (var value in target.m_values)
            {
                m_worldState.AddGOAPValue(new GOAPValue(value.key));
            }
        }

        // Fills world state with empty values
        public void FillEmpty(List<GOAPKey> keys)
        {
            foreach (var key in keys)
            {
                m_worldState.AddGOAPValue(new GOAPValue(key));
            }
        }

        public void Clear()
        {

        }

        public bool ProcessActionedWorldState(TAction action, out GOAPPlanningWorldState<TAction> actionedWorldState)
        {
            if (!action.CheckCondition(m_worldState))
            {
                // Action can not be performed.
                actionedWorldState = null;
                return false;
            }

            actionedWorldState = ActivateNextPlanningState();
            actionedWorldState.AddCopyValues(m_worldState);

            actionedWorldState.parent = this;
            actionedWorldState.action = action;
            actionedWorldState.depth = depth + 1;
            actionedWorldState.weight = weight + action.GetWeight();

            action.AddEffects(actionedWorldState.m_worldState);

            return true;
        }
    }
}
