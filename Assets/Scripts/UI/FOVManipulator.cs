using UnityEngine;
using UnityEngine.UI;

public class FOVManipulator : MonoBehaviour
{
    [SerializeField] private Slider FOVSlider;
    [SerializeField] private Camera cam;
    [SerializeField] private Slider mouseSlider; // Referenz zum Mausempfindlichkeits-Slider

    private const float DEFAULT_FOV = 60f;
    private RectTransform mouseSliderHandleRect; // Für Zugriff auf den Handle-Transform

    private void Start()
    {
        // FOV initialisieren
        if (!PlayerPrefs.HasKey("FOV"))
        {
            PlayerPrefs.SetFloat("FOV", DEFAULT_FOV);
            PlayerPrefs.Save();
        }

        float savedFOV = PlayerPrefs.GetFloat("FOV", DEFAULT_FOV);
        cam.fieldOfView = savedFOV;

        if (FOVSlider != null)
        {
            FOVSlider.value = savedFOV;
            FOVSlider.onValueChanged.AddListener(SetFOV);
        }

        // Mouse Slider Handle referenzieren
        if (mouseSlider != null)
        {
            // Handle-Referenz bekommen (meist ein Kind des Sliders)
            mouseSliderHandleRect = mouseSlider.transform.Find("Handle Slide Area/Handle") as RectTransform;
            
            // Slider sofort beim Start korrigieren
            AdjustMouseSlider(savedFOV);
        }
    }

    public void SetFOV(float value)
    {
        PlayerPrefs.SetFloat("FOV", value);
        PlayerPrefs.Save();
        cam.fieldOfView = value;
        
        // Mouse Slider bei FOV-Änderung aktualisieren
        AdjustMouseSlider(value);
    }

    private void AdjustMouseSlider(float fovValue)
    {
        if (mouseSlider == null || mouseSliderHandleRect == null) return;

        // UI-Layout aktualisieren
        Canvas.ForceUpdateCanvases();
        
        // Option 1: Handle-Größe basierend auf FOV anpassen
        float sizeFactor = 1.0f + (90f - fovValue) / 90f; // Anpassungsfaktor berechnen
        mouseSliderHandleRect.sizeDelta = new Vector2(mouseSliderHandleRect.sizeDelta.x, 
                                                    Mathf.Max(20f, 30f * sizeFactor)); // Minimalgröße setzen
        
        // Option 2: Interaktionsbereich vergrößern (falls nötig)
        Image handleImage = mouseSliderHandleRect.GetComponent<Image>();
        if (handleImage != null)
        {
            // Raycast-Fläche vergrößern
            handleImage.raycastPadding = new Vector4(10, 10, 10, 10);
        }
        
        // Layout neu berechnen nach Änderungen
        LayoutRebuilder.ForceRebuildLayoutImmediate(mouseSlider.GetComponent<RectTransform>());
    }
}