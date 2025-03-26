using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LevelMenu : MonoBehaviour
{
    public Button[] buttons;

    private void Awake(){
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 4); // workaround um bonus freizuschalten lg. leon
        for(int i = 0; i < buttons.Length; i++){
            buttons[i].interactable = false;
        }
        for(int i = 0; i < unlockedLevel; i++){
            buttons[i].interactable = true;
        }
    }

    public void OpenLevel(int levelID){

        SceneManager.LoadScene(levelID);

    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
            EditorApplication.isPlaying = false;  // Stoppt das Spiel im Editor
        #else
            Application.Quit();  // Beendet das Spiel im Build
        #endif
    }
    
}
