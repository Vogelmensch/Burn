using UnityEngine;
using System.Collections;

public class Musicplayer : MonoBehaviour
{
  public AudioSource[] _audioSources;
  public float musicduration;
  public AudioClip currentClip;
  public float delay;

  private void Start()
  {
  }
  private void Update()
  {
    playmusic();
    //Volumechange((float)1.0);
    //Debug.Log("1" + _audioSources[0].isPlaying);
    //Debug.Log("2" + _audioSources[1].isPlaying);
    

  }

  private void playmusic()
  {
    //1.spielt grade Musik?
    //1.1 JA? do nothing
    //1.2 Nein? aktivier eine der Musicdisks
    //
    if(!_audioSources[0].isPlaying && !_audioSources[1].isPlaying)
    {
      Debug.Log("damn daniel you went into the audios");
      _audioSources[0].clip = currentClip;
      _audioSources[0].PlayDelayed(delay);
      Debug.Log(AudioSettings.dspTime);
      _audioSources[1].clip = currentClip;
      _audioSources[1].PlayDelayed(136 + delay);
    }else
    {
      return;
    }
  }

  public void Volumechange(float f)
  {
    _audioSources[0].volume = f;
    _audioSources[1].volume = f;
  }
  //public AudioSource audioSource;
  /**
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
  
  **/
}