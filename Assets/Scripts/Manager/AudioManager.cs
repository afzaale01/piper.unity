using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource m_AudioSource;


    public void PlayAudioClip(AudioClip audioClip)
    {
        if (m_AudioSource.isPlaying)
            m_AudioSource.Stop();

        m_AudioSource.clip = audioClip;
        m_AudioSource.Play();
    }

}
