using UnityEngine;
using System.Collections;

public class Musicplayer : MonoBehaviour
{
  //public AudioSource audioSource;
  public double musicDuration;
  public double goalTime;
  public AudioSource[] _audioSources;
  public int audioTogle;
  public AudioClip currentClip;

  private void Update()
  {
    if(AudioSettings.dspTime > goalTime)
    {
        Debug.Log(AudioSettings.dspTime);
        Debug.Log(goalTime);
        PlayScheduledClip();
    }
  }
  private void PlayScheduledClip()
  {
    musicDuration = (double)currentClip.samples / currentClip.frequency;
    goalTime = goalTime + musicDuration;

    _audioSources[audioTogle].clip = currentClip;
    _audioSources[audioTogle].PlayScheduled(goalTime);

    audioTogle = 1 - audioTogle;

  }
  
}