using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBB.GOAP
{
    public class GOAPValue
    {
        GOAPKey m_key;
        object m_value;

        public GOAPKey key { get { return m_key; } }

        public GOAPValue(int key)
        {
            m_key = new GOAPKey(key, 0);
        }

        public GOAPValue(GOAPKey key)
        {
            m_key = key;
        }

        public GOAPValue(GOAPValue other)
        {
            Copy(other);
        }

        public void SetKey(GOAPKey key)
        {
            m_key = key;
        }

        public GOAPKey GetKey()
        {
            return m_key;
        }

        public void SetValue<T>(T value)
        {
            m_value = value;
        }

        public object GetObjectValue()
        {
            return m_value;
        }

        public T GetValue<T>()
        {
            return (T)m_value;
        }

        public void Copy(GOAPValue other)
        {
            m_key = other.m_key;
            m_value = other.m_value;
        }

        public bool Compare(GOAPValue other)
        {
            // TODO: Figure out how to check if object types are nullable or not.
            // It's not ideal that there are additional checks to determine if objects are null. The equality operator needs to be called for reference types.
            // This might be too big of an issue and it's very worth considering that only value types should be allowed.
            // Most data types used by GOAP should be relatively easy to check as there are a very large amount of comparisons being made. Idealy most will be booleans or integers.
            // If the value is a reference type. Users will easily be able to change values mid planning stage, and this could cause catastophic undefined behaviour.
            if(m_value == null)
            {
                return other.m_value == null;
            }

            return m_value.Equals(other.m_value);
        }
    }
}
