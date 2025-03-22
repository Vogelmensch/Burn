using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTutorial : MonoBehaviour
{
    private Level2Block fireDetector;
    public float delayBeforeLoading = 5.0f;
    
    public GameObject character;
    private Animator characterAnimator;

    public AudioClip deathSound;
    private AudioSource audioSource;
    
    // Flag, um zu verhindern, dass die Methode mehrmals aufgerufen wird
    private bool hasTriggeredDeath = false;

    void Start()
    {
        fireDetector = FindObjectOfType<Level2Block>();
        if (fireDetector != null)
        {
            fireDetector.FireOnEvent2 += FinishTut;
        }
        
        if (character != null)
        {
            characterAnimator = character.GetComponent<Animator>();
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void FinishTut()
    {   
        // Überprüfe, ob diese Methode bereits ausgelöst wurde
        if (hasTriggeredDeath)
            return;
            
        // Markiere als ausgelöst
        hasTriggeredDeath = true;
        
        Cursor.lockState = CursorLockMode.Confined;
        
        // Animation abspielen
        if (characterAnimator != null)
        {
            characterAnimator.SetTrigger("Death");
        }
        
        // Sound nur einmal abspielen
        if (deathSound != null && audioSource != null)
        {
            audioSource.Stop(); // Stoppe alle laufenden Sounds
            audioSource.PlayOneShot(deathSound);
            Debug.Log("Death-Sound abgespielt");
        }
        
        StartCoroutine(LoadMainMenuWithDelay());
    }

    private IEnumerator LoadMainMenuWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoading);
        SceneManager.LoadScene(0);
    }
    
    // Wenn das Skript zerstört wird (z.B. bei Szenenwechsel), Event abmelden
    private void OnDestroy()
    {
        if (fireDetector != null)
        {
            fireDetector.FireOnEvent2 -= FinishTut;
        }
    }
}