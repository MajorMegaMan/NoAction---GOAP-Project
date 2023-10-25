using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScaleRandomiser : MonoBehaviour
{
    [SerializeField] Transform m_targetTransform;
    [SerializeField] Vector3 m_randomSizeModRange = Vector3.one * 0.1f;
    [SerializeField] AnimationCurve m_curve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 1.0f);

    private void Awake()
    {
        Vector3 scale = m_targetTransform.localScale;

        //scale.x += Random.Range(-m_randomSizeModRange.x, m_randomSizeModRange.x);
        //scale.y += Random.Range(-m_randomSizeModRange.y, m_randomSizeModRange.y);
        //scale.z += Random.Range(-m_randomSizeModRange.z, m_randomSizeModRange.z);
        scale.x += Mathf.Lerp(-1.0f, 1.0f, m_curve.Evaluate(Random.Range(0.0f, 1.0f))) * m_randomSizeModRange.x;
        scale.y += Mathf.Lerp(-1.0f, 1.0f, m_curve.Evaluate(Random.Range(0.0f, 1.0f))) * m_randomSizeModRange.y;
        scale.z += Mathf.Lerp(-1.0f, 1.0f, m_curve.Evaluate(Random.Range(0.0f, 1.0f))) * m_randomSizeModRange.z;

        //agent.movementMachine.animator.transform.localScale = scale;
        m_targetTransform.localScale = scale;
    }
}
