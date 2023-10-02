using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ValueType = SerialisedGOAPValue.ValueType;

[System.Serializable]
public struct ActionCondition<TKey>
{
    public CheckEnum check;
    public SerialisedGOAPValue<TKey> value;

    public bool Compare(object lhs)
    {
        switch (check)
        {
            case CheckEnum.Greater:
                return CompareGreaterValue(lhs);
            case CheckEnum.Less:
                return CompareLesserValue(lhs);
            case CheckEnum.Equal:
                return lhs.Equals(value.GetObjectValue());
            case CheckEnum.NotEqual:
                return !lhs.Equals(value.GetObjectValue());
            default:
                return false;
        }
    }

    bool CompareGreaterValue(object lhs)
    {
        switch (value.valueType)
        {
            case ValueType.FLOAT:
                {
                    return (float)lhs > value.GetValue<float>();
                }
            case ValueType.INT:
                {
                    return (int)lhs > value.GetValue<int>();
                }
            case ValueType.BOOLEAN:
                {
                    return (bool)lhs & value.GetValue<bool>();
                }
            case ValueType.STRING:
                {
                    return ((string)lhs).Length > value.GetValue<string>().Length;
                }
            case ValueType.OBJECT:
                {
                    return value.GetValue<Object>(); // ???
                }
            default:
                return false;
        }
    }

    bool CompareLesserValue(object lhs)
    {
        switch (value.valueType)
        {
            case ValueType.FLOAT:
                {
                    return (float)lhs < value.GetValue<float>();
                }
            case ValueType.INT:
                {
                    return (int)lhs < value.GetValue<int>();
                }
            case ValueType.BOOLEAN:
                {
                    return (bool)lhs | value.GetValue<bool>();
                }
            case ValueType.STRING:
                {
                    return ((string)lhs).Length < value.GetValue<string>().Length;
                }
            case ValueType.OBJECT:
                {
                    return value.GetValue<Object>(); // ???
                }
            default:
                return false;
        }
    }
}

public enum CheckEnum
{
    Greater,
    Less,
    Equal,
    NotEqual
}
