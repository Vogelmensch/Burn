using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDelayPlayer : MonoBehaviour
{
    [Tooltip("Die Audiodatei, die abgespielt werden soll")]
    public AudioClip audioClip;
    
    [Tooltip("Verzögerung in Sekunden vor dem Abspielen")]
    public float delay = 2.0f;
    
    [Tooltip("Lautstärke des Audios (0.0 bis 1.0)")]
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    
    private AudioSource audioSource;
    
    void Start()
    {
        // AudioSource-Komponente prüfen oder hinzufügen
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // AudioClip und Volume setzen
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        
        // Audio nach 2 Sekunden abspielen
        Invoke("PlayAudio", delay);
    }
    
    void PlayAudio()
    {
        if (audioSource != null && audioClip != null)
        {
            audioSource.Play();
            Debug.Log("Audio wird abgespielt: " + audioClip.name);
        }
        else
        {
            Debug.LogWarning("Audio konnte nicht abgespielt werden. AudioSource oder AudioClip fehlt.");
        }
    }
}