using UnityEngine;
using UnityEngine.SceneManagement;
public class SettingsPanel : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject settings;
   
    void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0){
            if(Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape)){
                settings.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex != 0){
            if(Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.Escape)){
                settings.SetActive(false);
            }
        }
    }
}
