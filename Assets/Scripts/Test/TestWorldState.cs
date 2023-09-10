using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BBB.GOAP;
using UnityEditorInternal.Profiling.Memory.Experimental;

[System.Serializable]
public struct TestValues
{
    public TestEnum id;
    public int value;
}

public enum TestEnum
{
    HoldItem,
    ItemWorldCount,
    BlueCount,
    RedCount,
}
