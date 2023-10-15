using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSoundPackage : MonoBehaviour
{
    [SerializeField] AudioSource m_audioSource;
    [SerializeField] List<AudioClip> m_sounds = new List<AudioClip>();
    [SerializeField] Vector2 m_pitchRange = new Vector2(0.8f, 1.2f);

    private void Update()
    {
        if(!m_audioSource.isPlaying)
        {
            PlayRandom();
        }
    }

    public void PlayRandom()
    {
        m_audioSource.clip = m_sounds[Random.Range(0, m_sounds.Count)];
        m_audioSource.pitch = Mathf.Lerp(m_pitchRange.x, m_pitchRange.y, Random.Range(0.0f, 1.0f));
        m_audioSource.Play();
    }
}
