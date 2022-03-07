using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class musicController : MonoBehaviour
{
    private AudioSource selfAudio;
    private bool volumeReached;
    private float currentVolume;
    private float targetVolume;
    private float rate;
    private bool selfTerminate = false;

    private void Update()
    {
        if (!volumeReached)
        {
            if (Mathf.Abs(targetVolume - currentVolume) > 0.01f)
            {
                currentVolume += (targetVolume - currentVolume) * rate * Time.deltaTime * 4f;
            }
            else
            {
                currentVolume = targetVolume;
                volumeReached = true;
            }

            selfAudio.volume = currentVolume;
        }
        else if (selfTerminate)
        {
            if (currentVolume == 0)
            {
                Destroy(gameObject);
            }
        }
    }

    public void playClip(AudioClip clipToPlay)
    {
        selfAudio = GetComponent<AudioSource>();

        currentVolume = 0;
        targetVolume = 0;
        rate = 0.1f;

        selfAudio.clip = clipToPlay;
        selfAudio.Play();
    }

    public void setVolume(float volumeToSet, float rate)
    {
        if (volumeToSet != currentVolume)
        {
            targetVolume = volumeToSet;
            volumeReached = false;
        }
    }

    public void terminate()
    {
        targetVolume = 0;
        volumeReached = false;
        selfTerminate = true;
    }
}