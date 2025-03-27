using UnityEngine;
using UnityEngine.UI;

public class FOVManipulator : MonoBehaviour
{
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Camera cam;

    private const float DEFAULT_FOV = 60f; // Standard-FOV, falls kein Wert gespeichert ist

    private void Start()
    {
        if (!PlayerPrefs.HasKey("FOV"))
        {
            PlayerPrefs.SetFloat("FOV", DEFAULT_FOV); // Standardwert setzen
            PlayerPrefs.Save(); // Speichern nicht vergessen!
        }

        float savedFOV = PlayerPrefs.GetFloat("FOV", DEFAULT_FOV); // Falls kein Wert existiert, nutze Standardwert
        cam.fieldOfView = savedFOV;

        if (FOVSlider != null)
        {
            FOVSlider.value = savedFOV;
            FOVSlider.onValueChanged.AddListener(SetFOV);
        }
    }

    public void SetFOV(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
        PlayerPrefs.Save(); // Speichern, damit der Wert beim Szenenwechsel bleibt
        cam.fieldOfView = value;
    }
}
