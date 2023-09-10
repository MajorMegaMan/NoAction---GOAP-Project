using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BBB.GOAP;

[ExecuteAlways]
public class TextBillboard : MonoBehaviour
{
    [SerializeField]
    [RequireInterface(typeof(IGOAPAgentElements))] Object m_agentObject;
    [SerializeField] Camera m_lookAtTarget;
    [SerializeField] TMP_Text m_text;

    // Update is called once per frame
    void LateUpdate()
    {
        if(m_lookAtTarget != null)
        {
            var position = transform.position;
            transform.LookAt(position - m_lookAtTarget.transform.forward);
        }

        //if(m_agentObject != null)
        //{
        //    var agent = m_agentObject as IGOAPAgentElements;
        //    if (agent.currentAction != null)
        //    {
        //        m_text.SetText(agent.currentAction.actionName);
        //    }
        //    else
        //    {
        //        m_text.SetText("No Action");
        //    }
        //}
    }

    public void SetCamera(Camera camera)
    {
        m_lookAtTarget = camera;
    }

    public void SetText(ChefAgent chef, IGOAPAgentActionMethods action)
    {
        if(action != null)
        {
            m_text.SetText(action.actionName);
        }
        else
        {
            m_text.SetText("No Action");
        }
    }
}
