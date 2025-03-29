using UnityEngine;
using UnityEngine.UI;

public class MouseSensitivitySlider : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;
    private PlayerLook playerLook;

    private const float DEFAULT_SENSITIVITY = 4f;

    void Start()
    {
        playerLook = FindObjectOfType<PlayerLook>(); // Sucht das PlayerLook-Skript in der Szene

        if (!PlayerPrefs.HasKey("MouseSensitivity"))
        {
            PlayerPrefs.SetFloat("MouseSensitivity", DEFAULT_SENSITIVITY);
            PlayerPrefs.Save();
        }

        float savedSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", DEFAULT_SENSITIVITY);
        sensitivitySlider.value = savedSensitivity;

        if (playerLook != null)
        {
            playerLook.UpdateSensitivity(savedSensitivity);
        }

        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
    }

    public void SetSensitivity(float value)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", value);
        PlayerPrefs.Save();

        if (playerLook != null)
        {
            playerLook.UpdateSensitivity(value);
        }
    }
}
