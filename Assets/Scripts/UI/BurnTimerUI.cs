using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BurnTimerManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject burnTimerPanel;      // The panel containing the burn timer UI
    public Slider burnTimerSlider;         // Slider for the timer bar
    public Image sliderFillImage;          // Reference to the fill image of the slider
    [Tooltip("Optional: Leave empty if you don't want text")]
    public TextMeshProUGUI burnTimerText;  // Optional text displaying the timer

    [Header("Settings")]
    public Color startColor = Color.green;  // Color when burn just started
    public Color endColor = Color.red;      // Color when about to burn out
    
    private UpPicker upPicker;             // Reference to the UpPicker script
    private Burnable currentBurnable;      // Reference to the currently carried Burnable object
    private float maxBurnTime;             // Maximum burn time for the current object
    
    void Start()
    {
        // Find the UpPicker component in the scene
        upPicker = FindObjectOfType<UpPicker>();
        
        if (!upPicker)
        {
            Debug.LogError("BurnTimerManager: UpPicker component not found in scene!");
        }
        
        // Hide the UI at start
        if (burnTimerPanel)
        {
            burnTimerPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("BurnTimerManager: burnTimerPanel reference is missing!");
        }
    }
    
    void Update()
    {
        // Ensure required references exist
        if (!burnTimerPanel || !upPicker || !burnTimerSlider || !sliderFillImage)
        {
            return;
        }
        
        // Check if player is carrying an object
        try {
            if (upPicker.IsCurrentlyCarrying())
            {
                GameObject carriedObject = upPicker.GetCurrentlyCarriedObject();
                
                if (carriedObject != null)
                {
                    // Try to get a Burnable component
                    Burnable burnable = carriedObject.GetComponent<Burnable>();
                    if (!burnable)
                    {
                        burnable = carriedObject.GetComponentInParent<Burnable>();
                    }
                    
                    if (burnable != null && burnable.isOnFire)
                    {
                        // Force activate the panel
                        burnTimerPanel.SetActive(true);
                        
                        // We have a burning object, update or initialize the UI
                        if (currentBurnable != burnable)
                        {
                            // New object being carried, initialize
                            currentBurnable = burnable;
                            
                            try {
                                maxBurnTime = burnable.hitPoints / burnable.GetDamageRate();
                            }
                            catch (System.Exception) {
                                maxBurnTime = 10f; // Fallback
                            }
                        }
                        
                        UpdateBurnTimeDisplay();
                        return; // Early return to avoid hiding the panel
                    }
                }
            }
            
            // If we got here, hide the panel
            if (burnTimerPanel.activeSelf)
            {
                burnTimerPanel.SetActive(false);
                currentBurnable = null;
            }
        }
        catch (System.Exception e) {
            Debug.LogError("Error in BurnTimerManager Update: " + e.Message);
        }
    }
    
    void UpdateBurnTimeDisplay()
    {
        if (currentBurnable == null)
            return;
            
        try {
            // Calculate remaining time
            float remainingHitPoints = currentBurnable.hitPoints;
            float damageRate = currentBurnable.GetDamageRate();
            float remainingTime = remainingHitPoints / Mathf.Max(0.1f, damageRate);
            
            // Update slider value (1.0 = full, 0.0 = empty)
            float fillAmount = remainingHitPoints / (maxBurnTime * Mathf.Max(0.1f, damageRate));
            burnTimerSlider.value = Mathf.Clamp01(fillAmount);
            
            // Update color based on remaining time
            sliderFillImage.color = Color.Lerp(endColor, startColor, fillAmount);
            
            // Update text if it exists
            if (burnTimerText != null)
            {
                string timeText = FormatTime(remainingTime);
                burnTimerText.text = timeText;
            }
        }
        catch (System.Exception e) {
            Debug.LogError("Error in UpdateBurnTimeDisplay: " + e.Message);
        }
    }
    
    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }
}