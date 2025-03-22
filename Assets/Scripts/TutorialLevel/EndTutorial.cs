using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndTutorial : MonoBehaviour
{
    private Level2Block fireDetector;
    public float delayBeforeLoading = 2.0f; // 2 seconds delay before loading the main menu

    void Start()
    {
        fireDetector = FindObjectOfType<Level2Block>();
        if (fireDetector != null)
        {
            fireDetector.FireOnEvent2 += FinishTut;
        }
    }

    void FinishTut()
    {   
        Cursor.lockState = CursorLockMode.Confined;
        StartCoroutine(LoadMainMenuWithDelay());
    }

    private IEnumerator LoadMainMenuWithDelay()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeLoading);
        
        // Then load the main menu scene (assuming scene index 0 is the main menu)
        SceneManager.LoadScene(0);
    }
}