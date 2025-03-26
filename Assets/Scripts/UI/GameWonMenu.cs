using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameWonMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject winScreen;

    [Header("Settings")]
    public string winBlockTag = "WinBlock";
     public float checkInterval = 0.5f;
     float delayBeforeLoading = 5.0f;
     private float timeSinceLastCheck = 0f;
     private bool gameOver = false;

    void Start()
    {
        // Ensure Game Over screen is hidden at start
        if (winScreen != null)
        {
            winScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("winScreen canvas reference is missing!");
        }
    }

    void Update()
    {
        // Don't check if game is already over
        if (gameOver)
            return;

        // Check at intervals to improve performance
        timeSinceLastCheck += Time.deltaTime;
        if (timeSinceLastCheck >= checkInterval)
        {
            CheckForBurningWinBlocks();
            timeSinceLastCheck = 0f;
        }
    }

    void CheckForBurningWinBlocks()
    {
        // Find all win blocks
        GameObject[] winBlocks = GameObject.FindGameObjectsWithTag(winBlockTag);
        
        foreach (GameObject block in winBlocks)
        {
            Burnable burnable = block.GetComponent<Burnable>();
            
            // If any win block is burning, trigger game over
            if (burnable != null && burnable.isOnFire && burnable.hitPoints <= 10)
            {
                StartCoroutine(endGameWithDelay());
                return;
            }
        }
    }

    void endGame(){
        ShowGameOver();
    }

    private IEnumerator endGameWithDelay()
    {
        yield return new WaitForSeconds(delayBeforeLoading);
        ShowGameOver();
    }
    void ShowGameOver()
    {
        if (gameOver)
            return;
            
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

    public void RestartGame()
    {
        // Reset time scale
        Time.timeScale = 1f;
        gameOver = false;
        
        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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