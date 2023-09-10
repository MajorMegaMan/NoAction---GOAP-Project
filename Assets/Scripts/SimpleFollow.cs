using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleFollow : MonoBehaviour
{
    [SerializeField] Transform m_targetFollow;

    public Transform targetFollow { get { return m_targetFollow; } set { m_targetFollow = value; } }

    private void Update()
    {
        if(m_targetFollow != null)
        {
            transform.position = m_targetFollow.position;
        }
    }
}
