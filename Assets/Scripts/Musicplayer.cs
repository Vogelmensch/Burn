using UnityEngine;
using System.Collections;

public class Musicplayer : MonoBehaviour
{
  public AudioSource[] _audioSources;
  public float musicduration;
  public AudioClip currentClip;
  //pause zwischen den Songs
  public float delay;

  private void Start()
  {
  }
  private void Update()
  {
    playmusic();
  }

  private void playmusic()
  {
    //1.spielt grade Musik?
    //1.1 JA? do nothing
    //1.2 Nein? aktivier eine der Musicdisks
    //
    if(!_audioSources[0].isPlaying && !_audioSources[1].isPlaying)
    {
      _audioSources[0].clip = currentClip;
      _audioSources[0].PlayDelayed(delay);
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
}