using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public float gameDuration = 300f; // Spielzeit in Sekunden
    private float elapsedTime = 0f;
    private bool isGameOver = false;
    private bool hasFireStarted = false;
    private bool gameStarted = false;

    public GameObject gameOverCanvas;
    public GameObject youWonCanvas;
    public TextMeshProUGUI timerText;

    public Button startButton;
    public Button restartButton; 
    public GameObject startMenu;

    private string targetTag = "WinBlock"; // Tag für das Ziel-Objekt

    void Start()
    {
        // Initialisierung
        gameOverCanvas.SetActive(false);
        youWonCanvas.SetActive(false);
        startMenu.SetActive(true);
        UpdateTimerDisplay();

        // Button-Funktionalität
        startButton.onClick.AddListener(StartGame);

        // Spielersteuerung sperren
        LockPlayerControls(true);
    }

    void Update()
    {
        // Abbrechen, wenn das Spiel nicht gestartet ist oder vorbei ist
        if (!gameStarted || isGameOver) return;

        // Spielzeit aktualisieren
        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay();

        // Spielzeit abgelaufen
        if (elapsedTime >= gameDuration)
        {
            EndGame();
        }

        // Siegbedingung prüfen
        CheckWinCondition();

        // Feuer-Aus-Bedingung prüfen
        if (hasFireStarted && AreAllFiresOut())
        {
            EndGame();
        }
    }

    void StartGame()
    {
        // Spiel starten
        gameStarted = true;
        startMenu.SetActive(false);
        LockPlayerControls(false);
        Debug.Log("Game started! Player controls enabled.");
    }

    private void CheckWinCondition()
    {
        foreach (var burnable in GeneralizedCubeDivider.allBurnables)
        {
            if (burnable != null && burnable.gameObject.CompareTag(targetTag) && burnable.isOnFire)
            {
                ShowWinScreen();
                return;
            }
        }
    }

    private void LockPlayerControls(bool lockControls)
    {
        // Maus und Cursorsteuerung
        Cursor.lockState = lockControls ? CursorLockMode.None : CursorLockMode.None; // Maus bleibt sichtbar
        Cursor.visible = true;

        // Bewegungssteuerung deaktivieren
        var playerMovement = Object.FindFirstObjectByType<playerMovement>();
        if (playerMovement != null)
        {
            playerMovement.enabled = !lockControls;
        }

        // Kamerasteuerung deaktivieren
        var playerCamera = Object.FindFirstObjectByType<CameraController>();
        if (playerCamera != null)
        {
            playerCamera.enabled = !lockControls;
        }

        Debug.Log(lockControls ? "Player controls locked." : "Player controls unlocked.");
    }

    private void ShowWinScreen()
    {
        isGameOver = true;
        youWonCanvas.SetActive(true);
        Debug.Log("You Won!");
    }

    private bool AreAllFiresOut()
    {
        foreach (var burnable in GeneralizedCubeDivider.allBurnables)
        {
            if (burnable != null && burnable.isOnFire)
            {
                return false;
            }
        }

        Debug.Log("All fires are out!");
        return true;
    }

    void UpdateTimerDisplay()
    {
        float timeRemaining = Mathf.Max(0, gameDuration - elapsedTime);
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);

        if (timerText != null)
        {
            timerText.text = $"Time: {minutes:0}:{seconds:00}";
        }
    }

    public void NotifyFireStarted()
    {
        if (!hasFireStarted)
        {
            hasFireStarted = true;
            Debug.Log("Fire started!");
        }
    }

    void Restart(){
        SceneManager.LoadScene("ThreeRoomsAndGarden");
    }

    void EndGame()
    {
        if (isGameOver) return;
        isGameOver = true;
        LockPlayerControls(true);
        restartButton.onClick.AddListener(Restart);

        gameOverCanvas.SetActive(true);
        Debug.Log("Game Over!");
    }
}