using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayAudioClip(AudioClip audioClip)
    {
        if (m_AudioSource.isPlaying)
            m_AudioSource.Stop();

        m_AudioSource.clip = audioClip;
        m_AudioSource.Play();
    }

}
