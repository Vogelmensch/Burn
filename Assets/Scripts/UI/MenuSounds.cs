using UnityEngine;
using UnityEngine.EventSystems;

public class HoverSound : MonoBehaviour
{
    public AudioClip hoverSoundClip;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // Diese öffentliche Methode kann vom Event Trigger aufgerufen werden
    public void PlayHoverSound()
    {
        if (hoverSoundClip != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSoundClip);
        }
    }
}