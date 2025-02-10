using UnityEngine;
using UnityEngine.SceneManagement;

public class EndTutorial : MonoBehaviour
{
    private Level2Block fireDetector;

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
        SceneManager.LoadScene(0);
    }
}
