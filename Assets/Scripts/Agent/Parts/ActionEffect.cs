using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ValueType = SerialisedGOAPValue.ValueType;

[System.Serializable]
public struct ActionEffect<TKey>
{
    public EffectEnum effect;
    public SerialisedGOAPValue<TKey> value;

    public void Apply(ref object lhs)
    {
        switch (effect)
        {
            case EffectEnum.Add:
                ApplyAddValue(ref lhs);
                break;
            case EffectEnum.Subtract:
                ApplySubtractValue(ref lhs);
                break;
            case EffectEnum.Set:
                lhs = value.GetObjectValue();
                break;
            default:
                break;
        }
    }

    void ApplyAddValue(ref object lhs)
    {
        switch (value.valueType)
        {
            case ValueType.FLOAT:
                {
                    lhs = (float)lhs + value.GetValue<float>();
                    break;
                }
            case ValueType.INT:
                {
                    lhs = (int)lhs + value.GetValue<int>();
                    break;
                }
            case ValueType.BOOLEAN:
                {
                    lhs = (bool)lhs | value.GetValue<bool>();
                    break;
                }
            case ValueType.STRING:
                {
                    lhs = (string)lhs + value.GetValue<string>();
                    break;
                }
            case ValueType.OBJECT:
                {
                    lhs = value.GetValue<Object>();
                    break;
                }
        }
    }

    void ApplySubtractValue(ref object lhs)
    {
        switch (value.valueType)
        {
            case ValueType.FLOAT:
                {
                    lhs = (float)lhs - value.GetValue<float>();
                    break;
                }
            case ValueType.INT:
                {
                    lhs = (int)lhs - value.GetValue<int>();
                    break;
                }
            case ValueType.BOOLEAN:
                {
                    lhs = (bool)lhs & value.GetValue<bool>();
                    break;
                }
            case ValueType.STRING:
                {
                    break;
                }
            case ValueType.OBJECT:
                {
                    lhs = null;
                    break;
                }
        }
    }
}

public enum EffectEnum
{
    Add,
    Subtract,
    Set
}