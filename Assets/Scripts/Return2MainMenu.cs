using UnityEngine;
using UnityEngine.SceneManagement;

public class Return2MainMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
     public void OpenLevel(int levelID){

        SceneManager.LoadScene(levelID);

    }
    
}
