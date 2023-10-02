using System.Collections.Generic;

namespace BBB.GOAP
{
    public class GOAPWorldState
    {
        internal HashSet<GOAPValue> m_values;

        // whenever a GOAPValue is added to a world state by the user. It will also add a default value that can be easily accessed if only looking for a key.
        // This will help convert strings into keys to be ables to index into the hashset.
        static Dictionary<int, GOAPValue> _allKeyDefaults;

        public int Count { get { return m_values.Count; } }
        public int[] Keys
        {
            get
            {
                var keys = new int[Count];
                int i = 0;
                foreach (var value in m_values)
                {
                    keys[i++] = value.key.id;
                }
                return keys;
            }
        }

        public GOAPWorldState()
        {
            m_values = new HashSet<GOAPValue>(KeyComparer.comparer);
        }

        static GOAPWorldState()
        {
            _allKeyDefaults = new Dictionary<int, GOAPValue>();
        }

        class KeyComparer : IEqualityComparer<GOAPValue>
        {
            static KeyComparer _comparer;
            public static KeyComparer comparer { get { return _comparer; } }

            static KeyComparer()
            {
                _comparer = new KeyComparer();
            }

            bool IEqualityComparer<GOAPValue>.Equals(GOAPValue x, GOAPValue y)
            {
                return x.key.id == y.key.id;
            }

            int IEqualityComparer<GOAPValue>.GetHashCode(GOAPValue obj)
            {
                return obj.key.id.GetHashCode();
            }
        }

        static bool GetKeyDefault(int key, out GOAPValue keyDefault)
        {
            return _allKeyDefaults.TryGetValue(key, out keyDefault);
        }

        public bool AddValue<T>(int key, T value)
        {
            var goapValue = new GOAPValue(key);
            goapValue.SetValue(value);
            return AddGOAPValue(goapValue);
        }

        public bool GetObjectValue(int key, out object value)
        {
            GetKeyDefault(key, out var pairKey);
            bool success = m_values.TryGetValue(pairKey, out var pairValue);
            if (success)
            {
                value = pairValue.GetObjectValue();
            }
            else
            {
                value = default;
            }
            return success;
        }

        public bool GetValue<T>(int key, out T value)
        {
            GetKeyDefault(key, out var pairKey);
            bool success = m_values.TryGetValue(pairKey, out var pairValue);
            if (success)
            {
                value = pairValue.GetValue<T>();
            }
            else
            {
                value = default;
            }
            return success;
        }

        public bool SetValue<T>(int key, T value)
        {
            GetKeyDefault(key, out var pairKey);
            bool success = m_values.TryGetValue(pairKey, out var pairValue);
            if (success)
            {
                pairValue.SetValue(value);
            }
            return success;
        }

        public bool GetGOAPValue(int key, out GOAPValue value)
        {
            GetKeyDefault(key, out var pairKey);
            return GetGOAPValue(pairKey, out value);
        }

        public bool GetGOAPValue(GOAPValue keyValue, out GOAPValue value)
        {
            bool success = m_values.TryGetValue(keyValue, out var pairValue);
            if (success)
            {
                value = pairValue;
            }
            else
            {
                value = null;
            }
            return success;
        }

        public bool AddGOAPValue(GOAPValue value)
        {
            bool success = m_values.Add(value);
            if (success)
            {
                // Ensure that all goap keys can be found.
                if (!_allKeyDefaults.ContainsKey(value.key.id))
                {
                    _allKeyDefaults.Add(value.key.id, new GOAPValue(value.key.id));
                }
            }

            return success;
        }

        public bool RemoveGOAPValue(GOAPValue value)
        {
            bool success = m_values.Remove(value);
            // No need to remove from all list as that is a "global" variable. Other world states may have values with those keys.
            return success;
        }

        public bool Contains(int key)
        {
            GetKeyDefault(key, out var pairKey);
            return m_values.Contains(pairKey);
        }

        public void Clear()
        {
            m_values.Clear();
        }

        public bool Compare(GOAPWorldState other)
        {
            foreach (var pair in m_values)
            {
                if(other.GetGOAPValue(pair.key.id, out var otherGOAPValue))
                {
                    if (!otherGOAPValue.Compare(pair))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                //if(other.GetObjectValue(pair.key.id, out var otherValue))
                //{
                //    if(!otherValue.Equals(pair.GetObjectValue()))
                //    //if(otherValue != pair.GetObjectValue())
                //    {
                //        return false;
                //    }
                //}
                //else
                //{
                //    return false;
                //}
            }

            return true;
        }
    }

    namespace PlannerInternal
    {
        public static class GOAPWorldStateExtension
        {
            // Uses UnionWith to add values not present in this WorldState. Worldstates that are combined will Share GOAPValue references.
            public static void Combine(this GOAPWorldState thisState, GOAPWorldState otherState)
            {
                thisState.m_values.UnionWith(otherState.m_values);
            }

            // only copies values found in this worldState and the target worldState. Does not add values to this worldState.
            // Only sets values, does not change sharing access. If values are shared, they are still shared. If not, they will not.
            public static void CopyValues(this GOAPWorldState thisState, GOAPWorldState target)
            {
                foreach (var value in thisState.m_values)
                {
                    if (target.m_values.TryGetValue(value, out var copyValue))
                    {
                        value.Copy(copyValue);
                    }
                }
            }

            public static void AddCopyValues(this GOAPWorldState thisState, GOAPWorldState target)
            {
                foreach (var value in target.m_values)
                {
                    if(thisState.AddGOAPValue(new GOAPValue(value)))
                    {
                        // Value was added. No reason to recopy.
                    }
                    else
                    {
                        // Value already exists. Copy the value.
                        thisState.GetGOAPValue(value, out var thisValue);
                        thisValue.Copy(value);
                    }
                }
            }

            // Fills world state with empty values
            public static void FillEmpty(this GOAPWorldState thisState, GOAPWorldState target)
            {
                foreach (var value in target.m_values)
                {
                    thisState.AddGOAPValue(new GOAPValue(value.key));
                }
            }

            // Fills world state with empty values
            public static void FillEmpty(this GOAPWorldState thisState, List<GOAPKey> keys)
            {
                foreach (var key in keys)
                {
                    thisState.AddGOAPValue(new GOAPValue(key));
                }
            }

            // Creates a clone worldState. Clones do not Share references with the target.
            public static void Clone(this GOAPWorldState thisState, GOAPWorldState target)
            {
                thisState.m_values.Clear();
                foreach (var value in target.m_values)
                {
                    thisState.AddGOAPValue(new GOAPValue(value));
                }
            }
        }
    }
}
