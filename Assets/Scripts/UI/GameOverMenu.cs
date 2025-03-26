using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject gameOverScreen;

    [Header("Settings")]
    public string looseBlockTag = "LooseBlock";
    public float checkInterval = 0.5f; // How often to check for burning blocks
    
    private bool gameOver = false;
    private float timeSinceLastCheck = 0f;

    void Start()
    {
        // Ensure Game Over screen is hidden at start
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(false);
        }
        else
        {
            Debug.LogError("GameOverMenu: Game Over Screen canvas reference is missing!");
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
            CheckForBurningLooseBlocks();
            timeSinceLastCheck = 0f;
        }
    }

    void CheckForBurningLooseBlocks()
    {
        // Find all loose blocks
        GameObject[] looseBlocks = GameObject.FindGameObjectsWithTag(looseBlockTag);
        
        foreach (GameObject block in looseBlocks)
        {
            Burnable burnable = block.GetComponent<Burnable>();
            
            // If any loose block is burning, trigger game over
            if (burnable != null && burnable.isOnFire)
            {
                ShowGameOver();
                return;
            }
        }
    }

    void ShowGameOver()
    {
        if (gameOver)
            return;
            
        gameOver = true;
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Show Game Over screen
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
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