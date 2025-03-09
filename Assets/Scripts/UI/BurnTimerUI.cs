using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BurnTimerUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject burnTimerPanel;      // The panel containing the burn timer UI
    public Slider burnTimerSlider;         // Slider for the timer bar
    public Image sliderFillImage;          // Reference to the fill image of the slider
    public TextMeshProUGUI burnTimerText;  // Text displaying the timer

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
            Debug.LogError("BurnTimerUI: UpPicker component not found in scene!");
        }
        
        // Hide the UI at start
        burnTimerPanel.SetActive(false);
    }
    
    void Update()
    {
        // Check if player is carrying an object
        if (upPicker && upPicker.IsCurrentlyCarrying())
        {
            GameObject carriedObject = upPicker.GetCurrentlyCarriedObject();
            
            if (carriedObject != null)
            {
                // Try to get a Burnable component from the carried object
                Burnable burnable = carriedObject.GetComponent<Burnable>();
                if (!burnable)
                {
                    // Also check parent in case the CarryAndShoot component is on a parent object
                    burnable = carriedObject.GetComponentInParent<Burnable>();
                }
                
                if (burnable && burnable.isOnFire)
                {
                    // We have a burning object, update or initialize the UI
                    if (currentBurnable != burnable)
                    {
                        // New object being carried, initialize
                        currentBurnable = burnable;
                        maxBurnTime = burnable.hitPoints / burnable.GetDamageRate();
                        burnTimerPanel.SetActive(true);
                    }
                    
                    UpdateBurnTimeDisplay();
                }
                else
                {
                    // Object not burning or no burnable component
                    currentBurnable = null;
                    burnTimerPanel.SetActive(false);
                }
            }
        }
        else
        {
            // Not carrying anything
            currentBurnable = null;
            burnTimerPanel.SetActive(false);
        }
    }
    
    void UpdateBurnTimeDisplay()
    {
        if (currentBurnable == null)
            return;
            
        // Calculate remaining time
        float remainingHitPoints = currentBurnable.hitPoints;
        float damageRate = currentBurnable.GetDamageRate();
        float remainingTime = remainingHitPoints / damageRate;
        
        // Update slider value (1.0 = full, 0.0 = empty)
        float fillAmount = remainingHitPoints / (maxBurnTime * damageRate);
        burnTimerSlider.value = Mathf.Clamp01(fillAmount);
        
        // Update color based on remaining time
        if (sliderFillImage != null)
        {
            sliderFillImage.color = Color.Lerp(endColor, startColor, fillAmount);
        }
        
        // Update text
        string timeText = FormatTime(remainingTime);
        burnTimerText.text = timeText;
    }
    
    string FormatTime(float seconds)
    {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int remainingSeconds = Mathf.FloorToInt(seconds % 60f);
        
        return string.Format("{0:00}:{1:00}", minutes, remainingSeconds);
    }
}