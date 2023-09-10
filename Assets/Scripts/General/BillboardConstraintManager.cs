using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static Unity.VisualScripting.Member;

public class BillboardConstraintManager : MonoBehaviour
{
    [SerializeField] Transform m_target;
    [SerializeField] PositionConstraint m_constraint;
    ConstraintSource m_source;

    // Start is called before the first frame update
    void Start()
    {
        Thing(m_target);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Thing(Transform target)
    {
        m_source.sourceTransform = target;
        m_source.weight = 1.0f;
        m_constraint.AddSource(m_source);
        m_constraint.constraintActive = true;
    }
}
