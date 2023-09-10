using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSkinRandomiser : MonoBehaviour
{
    [SerializeField] List<SkinData> m_skinDatas = new List<SkinData>();

    SkinData m_activeSkin = null;

    [System.Serializable]
    class SkinData
    {
        [SerializeField] SkinnedMeshRenderer m_meshRenderer;
        [SerializeField] List<Material> m_materials = new List<Material>();

        public void SetActive()
        {
            m_meshRenderer.gameObject.SetActive(true);
            m_meshRenderer.material = m_materials[Random.Range(0, m_materials.Count)];
        }

        public void Disable()
        {
            m_meshRenderer.gameObject.SetActive(false);
        }
    }

    private void Awake()
    {
        if(m_skinDatas.Count > 0)
        {
            foreach(SkinData skinData in m_skinDatas)
            {
                skinData.Disable();
            }

            if (m_activeSkin != null)
            {
                m_activeSkin.SetActive();
            }
            else
            {
                m_activeSkin = m_skinDatas[0];
                m_activeSkin.SetActive();
            }

        }
    }

    private void OnEnable()
    {
        RandomiseSkin();
    }

    public void RandomiseSkin()
    {
        if(m_activeSkin != null)
        {
            m_activeSkin.Disable();
        }
        m_activeSkin = m_skinDatas[Random.Range(0, m_skinDatas.Count)];
        m_activeSkin.SetActive();
    }
}
