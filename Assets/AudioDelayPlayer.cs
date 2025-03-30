using UnityEngine;

public class SimpleAudioPlayer : MonoBehaviour
{
    [Tooltip("Die Audiodatei, die abgespielt werden soll")]
    public AudioClip audioClip;
    
    [Tooltip("Verzögerung in Sekunden vor dem Abspielen")]
    public float delay = 2.0f;
    
    [Tooltip("Lautstärke des Audios (0.0 bis 1.0)")]
    [Range(0.0f, 1.0f)]
    public float volume = 1.0f;
    
    void Start()
    {
        // Stelle sicher, dass wir eine AudioSource haben
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // AudioSource einrichten
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.playOnAwake = false;
        
        // Audio nach der angegebenen Verzögerung abspielen
        if (audioClip != null)
        {
            audioSource.PlayDelayed(delay);
            Debug.Log("Audio wird abgespielt nach " + delay + " Sekunden");
        }
        else
        {
            Debug.LogWarning("Kein AudioClip zugewiesen!");
        }
    }
}