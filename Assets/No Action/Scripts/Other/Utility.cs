using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    [System.Serializable]
    public class AnimationID
    {
        [SerializeField] string m_stringID = "";
        protected int m_id;
        public int id { get { return m_id; } }

        public void InitHashID()
        {
            m_id = Animator.StringToHash(m_stringID);
        }
    }

    [System.Serializable]
    public class AnimationStateID : AnimationID
    {
        [SerializeField] float m_transitionDuration = 0.1f;
        [SerializeField] int m_layer = 0;

        public void CrossFade(Animator anim)
        {
            anim.CrossFade(m_id, m_transitionDuration, m_layer);
        }
    }

    [System.Serializable]
    public class SmoothFloat
    {
        [SerializeField] float m_value;
        [SerializeField] float m_smoothTime = 0.1f;
        [SerializeField] float m_smoothVel = 0.0f;

        public float value { get { return m_value; } set { m_value = value; } }
        public float velocity { get { return m_smoothVel; } set { m_smoothVel = value; } }

        public float Smooth(float target)
        {
            m_value = Mathf.SmoothDamp(m_value, target, ref m_smoothVel, m_smoothTime);
            return m_value;
        }
    }

    [System.Serializable]
    public class SmoothVector3
    {
        [SerializeField] Vector3 m_value;
        [SerializeField] float m_smoothTime = 0.1f;
        [SerializeField] Vector3 m_smoothVel = Vector3.zero;

        public Vector3 value { get { return m_value; } set { m_value = value; } }
        public Vector3 velocity { get { return m_smoothVel; } set { m_smoothVel = value; } }

        public Vector3 Smooth(Vector3 target)
        {
            m_value = Vector3.SmoothDamp(m_value, target, ref m_smoothVel, m_smoothTime);
            return m_value;
        }
    }

    #region Gizmos
    public static void DrawColourCube(Vector3 pos, Vector3 scale)
    {
        Color orig = Gizmos.color;
        Color colour = orig;
        Gizmos.color = colour;
        Gizmos.DrawWireCube(pos, scale);

        colour.a *= 0.4f;
        Gizmos.color = colour;
        Gizmos.DrawCube(pos, scale);

        Gizmos.color = orig;
    }
    #endregion // Gizmos
}
