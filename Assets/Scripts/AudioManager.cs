using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("_______________Audio Source_______________")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource soundSource;

    [Header("_______________Audio Clips_______________")]
    public AudioClip backgroundMusic;
    public AudioClip death;
    public AudioClip doorOpen;
    public AudioClip pickUp;
    public AudioClip fireBall;

    private void Start(){
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }

/*    public void PlaySound(AudioClip clip){
        soundSource.playOneShot(clip);
    }
    */

}
