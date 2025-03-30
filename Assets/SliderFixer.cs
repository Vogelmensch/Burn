using UnityEngine;
using UnityEngine.UI;

public class SliderFixer : MonoBehaviour
{
    [SerializeField] private Slider mouseSlider; // Drag dein problematischer Slider hier rein

    void Start()
    {
        // Kurze Verzögerung, um sicherzustellen, dass alles geladen ist
        Invoke("FixSlider", 0.1f);
    }

    void FixSlider()
    {
        if (mouseSlider != null)
        {
            // Methode 1: Deaktivieren und wieder aktivieren
            mouseSlider.gameObject.SetActive(false);
            mouseSlider.gameObject.SetActive(true);
            
            // Methode 2: Layout neu berechnen
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(mouseSlider.GetComponent<RectTransform>());
            
            Debug.Log("Slider wurde zurückgesetzt");
        }
    }
}