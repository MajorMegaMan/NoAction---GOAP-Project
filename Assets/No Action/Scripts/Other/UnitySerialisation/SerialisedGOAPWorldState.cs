using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BBB.GOAP;

using ValueType = SerialisedGOAPValue.ValueType;

[System.Serializable]
public class SerialisedGOAPWorldState<KeyType> : GOAPWorldState, ISerializationCallbackReceiver
{
    [SerializeField] bool m_hasCustomEnum = false;

    [SerializeField] List<SerialisedGOAPValue<KeyType>> m_goapValueList;

    [SerializeField] SerialisedGOAPValue<KeyType> m_editorValueToAdd;// only used in the editor for inspector shenanigins. This is the value that will be used when a value is added into the world state.

    public SerialisedGOAPWorldState() : base()
    {
        m_goapValueList = new List<SerialisedGOAPValue<KeyType>>();
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        foreach (var goapValue in m_goapValueList)
        {
            goapValue.m_hasCustomEnum = m_hasCustomEnum;
        }
        m_editorValueToAdd.m_hasCustomEnum = m_hasCustomEnum;
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        Queue<SerialisedGOAPValue<KeyType>> toAdd = new Queue<SerialisedGOAPValue<KeyType>>();
        for (int i = 0; i < m_goapValueList.Count; i++)
        {
            toAdd.Enqueue(m_goapValueList[i]);
        }

        Clear(); // Should clear? May need to only remove elements that shouldn't exist.

        while (toAdd.Count > 0)
        {
            var value = toAdd.Dequeue();
            if (AddGOAPValue(value))
            {

            }
            else
            {

            }
        }
    }
}

[System.Serializable]
public class SerialisedGOAPValue<KeyType> : GOAPValue, ISerializationCallbackReceiver
{
    [SerializeField] internal KeyType m_key;

    [SerializeField] ValueType m_valueType = 0;

    public ValueType valueType => m_valueType;
    [SerializeField] float m_floatValue = 0;
    [SerializeField] int m_intValue = 0;
    [SerializeField] bool m_boolValue = false;
    [SerializeField] string m_stringValue = "";
    [SerializeField] Object m_objectValue = null;

    [SerializeField] internal bool m_hasCustomEnum = false;

    public SerialisedGOAPValue(int key) : base(key)
    {

    }

    public SerialisedGOAPValue(GOAPKey key) : base(key)
    {
        
    }

    public SerialisedGOAPValue(GOAPValue other) : base(other)
    {

    }

    public SerialisedGOAPValue(SerialisedGOAPValue<KeyType> other) : base(other)
    {
        m_key = other.m_key;
        m_valueType = other.m_valueType;

        m_floatValue    = other.m_floatValue;
        m_intValue      = other.m_intValue;
        m_boolValue     = other.m_boolValue;
        m_stringValue   = other.m_stringValue;
        m_objectValue   = other.m_objectValue;
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        var actualObjectValue = GetValue<object>();
        if (actualObjectValue == null)
        {
            switch (m_valueType)
            {
                case ValueType.FLOAT:
                    {
                        SetValue<float>(m_floatValue);
                        return;
                    }
                case ValueType.INT:
                    {
                        SetValue<int>(m_intValue);
                        return;
                    }
                case ValueType.BOOLEAN:
                    {
                        SetValue<bool>(m_boolValue);
                        return;
                    }
                case ValueType.STRING:
                    {
                        SetValue<string>(m_stringValue);
                        return;
                    }
                case ValueType.OBJECT:
                    {
                        return;
                    }
            }
        }

        var actualValueType = actualObjectValue.GetType();

        switch (m_valueType)
        {
            case ValueType.FLOAT:
                {
                    if (actualValueType != typeof(float))
                    {
                        return;
                    }
                    m_floatValue = GetValue<float>();
                    break;
                }
            case ValueType.INT:
                {
                    if (actualValueType != typeof(int))
                    {
                        return;
                    }
                    m_intValue = GetValue<int>();
                    break;
                }
            case ValueType.BOOLEAN:
                {
                    if(actualValueType != typeof(bool))
                    {
                        return;
                    }
                    m_boolValue = GetValue<bool>();
                    break;
                }
            case ValueType.STRING:
                {
                    if (actualValueType != typeof(string))
                    {
                        return;
                    }
                    m_stringValue = GetValue<string>();
                    break;
                }
            case ValueType.OBJECT:
                {
                    if (actualValueType != typeof(Object))
                    {
                        return;
                    }
                    m_objectValue = GetValue<Object>();
                    break;
                }
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        if (m_key != null)
        {
            var key = GetKey();
            key.id = m_key.GetHashCode();
            SetKey(key);
        }

        switch (m_valueType)
        {
            case ValueType.FLOAT:
                {
                    SetValue<float>(m_floatValue);
                    break;
                }
            case ValueType.INT:
                {
                    SetValue<int>(m_intValue);
                    break;
                }
            case ValueType.BOOLEAN:
                {
                    SetValue<bool>(m_boolValue);
                    break;
                }
            case ValueType.STRING:
                {
                    SetValue<string>(m_stringValue);
                    break;
                }
            case ValueType.OBJECT:
                {
                    SetValue<Object>(m_objectValue);
                    break;
                }
        }
    }
}

public static class SerialisedGOAPValue
{
    [System.Serializable]
    public enum ValueType
    {
        FLOAT,
        INT,
        BOOLEAN,
        STRING,
        OBJECT
    }
}