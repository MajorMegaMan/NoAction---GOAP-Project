using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBB.GOAP
{
    public struct GOAPKey
    {
        public int id;
        public int valueType;

        static GOAPKey m_zero;
        public static GOAPKey zero { get { return m_zero; } }

        public GOAPKey(int id, int valueType)
        {
            this.id = id;
            this.valueType = valueType;
        }

        static GOAPKey()
        {
            m_zero = new GOAPKey(0, 0);
        }
    }
}
