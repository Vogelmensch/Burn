using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTutorial : MonoBehaviour
{
    private Level2Block fireDetector;
    public float delayBeforeLoading = 5.0f;
    
    public GameObject character;
    public GameObject winScreen;
    private Animator characterAnimator;

    public AudioClip deathSound;
    private AudioSource audioSource;
    private bool gameOver = false;
    
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

        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("winScreen canvas reference is missing!");
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
        ShowGameOver();
    }
    
    // Wenn das Skript zerstört wird (z.B. bei Szenenwechsel), Event abmelden
    private void OnDestroy(){
        if (fireDetector != null)
        {
            fireDetector.FireOnEvent2 -= FinishTut;
        }
    }
    void ShowGameOver(){
        if (gameOver){
            return;
        }
            
        gameOver = true;
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Show Game Over screen
        if (winScreen != null)
        {
            winScreen.SetActive(true);
        }
        
        // Lock cursor to screen
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        // Disable player input
        DisablePlayerControls();
    }
    
    void DisablePlayerControls()
    {
        // Disable player movement
        PlayerMovement playerMovement = FindObjectOfType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
        
        // Disable camera controls
        CameraController cameraController = FindObjectOfType<CameraController>();
        if (cameraController != null)
        {
            cameraController.enabled = false;
        }
        
        // Alternative camera controls
        CameraMovementNew altCameraMovement = FindObjectOfType<CameraMovementNew>();
        if (altCameraMovement != null)
        {
            altCameraMovement.enabled = false;
        }
    }
    public void ReturnToMainMenu()
    {
        // Reset time scale
        Time.timeScale = 1f;
        gameOver = false;
        
        // Load the main menu scene (usually scene index 0)
        SceneManager.LoadScene(0);
    }
}