using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AudioManager : MonoBehaviour
{

    [Header("Audio Clips")]
    public AudioClip[] playerFireAR;
    public AudioClip[] playerFirePistol;
    public AudioClip[] playerDeath;
    public AudioClip[] playerJump;
    public AudioClip[] playerLand;
    public AudioClip[] playerTakeDamage;
    public AudioClip[] playerWalkStep;
    public AudioClip[] enemyDeath;
    public AudioClip[] enemyTakeDamage;

    [Header("Audio Sources")]
    public AudioSource[] audioSources;

    [SerializeField] PhotonView PV;

    private int GetNonPlayingSource()
    {
        int x = 0;

        for (int i = 0; i < audioSources.Length; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                x = i;
            }

            if(x != 0)
            {
                return x;
            }
        }

        return x;  
    }

    private void Start()
    {
        if (!PV.IsMine)
        {
            foreach(AudioSource a in audioSources)
            {
                a.spatialBlend = 1;
            }
        }
    }

    public void PlaySound(AudioClip _clip)
    {
        int i = 0;

        int j = GetNonPlayingSource();

        audioSources[j].pitch = 1;

        audioSources[j].PlayOneShot(_clip);
    }

    public void PlaySound(AudioClip _clip, float minPitch, float maxPitch)
    {
        int i = 0;

        int j = GetNonPlayingSource();

        float pitch = Random.Range(minPitch, maxPitch);

        pitch = Mathf.Clamp(pitch, -3, 3);

        audioSources[j].pitch = pitch;

        audioSources[j].PlayOneShot(_clip);
    }
}
