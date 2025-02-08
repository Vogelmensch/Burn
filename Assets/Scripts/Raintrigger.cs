using System.Collections.Generic;
using DigitalRuby.RainMaker;
using UnityEngine;

/*
First, there is a phase of light rain (Nieselregen) where the intensity doesn't increase.
Then, the intensity starts increasing until it hits the maximum.
*/

public class RainTrigger : MonoBehaviour
{
    private bool isRaining = false;
    public GameObject rainPrefab;
    private GameObject rainEffectInstance;
    private RainScript rainScript;
    private float startIntensity = 0.02f; // Nieselregen
    private float intensityIncreasePerSecond = 0.05f;
    public float timeOfLightRain = 30;
    public float timeToFullIntensity = 3 * 60;
    private float passedTime = 0;

    void OnTriggerEnter()
    {
        if (!isRaining) StartRain();
    }

    void Start()
    {
        intensityIncreasePerSecond = 1 / timeToFullIntensity;
    }

    void Update()
    {
        if (isRaining) passedTime += Time.deltaTime;

        // DEBUG ONLY
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Got R");
            if (!isRaining) StartRain();
            else StopRain();
        }

        if (passedTime > timeOfLightRain && rainScript != null){
            //rainScript.IncreaseIntensity(intensityIncreasePerSecond * Time.deltaTime);
        }
    }

    public void StartRain()
    {
        isRaining = true;
        if (rainPrefab != null)
        {
            rainEffectInstance = Instantiate(rainPrefab);
            rainScript = rainEffectInstance.GetComponent<RainScript>();
            if (rainScript != null){
                //rainScript.SetIntensity(startIntensity);
            }
        }
    }

    void StopRain()
    {
        isRaining = false;
        if (rainEffectInstance != null)
        {
            Destroy(rainEffectInstance);
        }
    }

    public bool IsRaining()
    {
        return isRaining;
    }
}
