using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool isPaused;

    void Start(){
        pauseMenu.SetActive(false);
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.X)){
            if(isPaused){
                ResumeGame();
            }else{
                PauseGame();
            }
        }
    }
    public void PauseGame(){
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;

        Cursor.lockState = CursorLockMode.Confined;
    }

    public void ResumeGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartGame(){
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
