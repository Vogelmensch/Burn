using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class CheckOptionsOnStartUp : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    void Awake()
    {
        LoadSettings();
    }

    void LoadSettings()
    {
        // Sound-Lautstärke einstellen (Standardwert: 1.0)
        float soundVolume = PlayerPrefs.GetFloat("soundVolume");
        float musicVolume = PlayerPrefs.GetFloat("musicVolume");
        SetMusicVolume(musicVolume);
        SetSoundVolume(soundVolume);
        Debug.Log("soundVolume geladen: " + soundVolume);
        Debug.Log("musicVolume geladen: " + musicVolume);

        // Field of View (FOV) einstellen (Standardwert: 60)
        float fov = PlayerPrefs.GetFloat("FOV", 60.0f);
        if (Camera.main != null)
        {
            Camera.main.fieldOfView = fov;
            Debug.Log("FOV geladen: " + fov);
        }
        else
        {
            Debug.LogWarning("Keine Kamera gefunden, um FOV zu setzen.");
        }

        // Maus-Sensitivität einstellen (Standardwert: 1.0)
    }

    public void SetMusicVolume(float volume){
        myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
         AudioListener.volume = volume;
    }
    public void SetSoundVolume(float volume){
        myMixer.SetFloat("sound", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("soundVolume", volume);
         AudioListener.volume = volume;
    }

}
