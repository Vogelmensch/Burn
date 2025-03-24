using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private Slider musicSlider;

    private void Start(){
        if(!PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("soundVolume")){
            SetMusicVolume();
            SetSoundVolume();
        }else if(!PlayerPrefs.HasKey("musicVolume") && PlayerPrefs.HasKey("soundVolume")){
            SetMusicVolume();
        }else if(PlayerPrefs.HasKey("musicVolume") && !PlayerPrefs.HasKey("soundVolume")){
            SetSoundVolume();
        }else{
            LoadVolume();
        }
       
    }

    public void SetMusicVolume(){
        float volume = musicSlider.value;
        myMixer.SetFloat("music", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("musicVolume", volume);
    }
    public void SetSoundVolume(){
       
        float volume = soundSlider.value;
        myMixer.SetFloat("sound", Mathf.Log10(volume)*20);
        PlayerPrefs.SetFloat("soundVolume", volume);
         AudioListener.volume = volume;
    }

    private void LoadVolume(){
       musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
       SetMusicVolume();
       soundSlider.value = PlayerPrefs.GetFloat("soundVolume");
       SetSoundVolume();
    }

}
