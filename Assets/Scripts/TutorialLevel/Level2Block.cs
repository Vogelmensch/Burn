using UnityEngine;

public class Level2Block : MonoBehaviour
{
    private bool isOnFire = false;
    public event System.Action FireOnEvent;
    public event System.Action FireOnEvent2;

    void Start()
    {
        Debug.Log("🔥 Level2Block wurde initialisiert: " + gameObject.name);
    }

    void Update()
    {
        CheckFireStatus();
    }

    private void CheckFireStatus()
    {
        foreach (var burnable in GeneralizedCubeDivider.allBurnables)
        {
            if (burnable != null && burnable.gameObject.CompareTag("Level2Block") && burnable.isOnFire)
            {
                isOnFire = true;
                FireOnEvent?.Invoke();
                return;
            }
            if(burnable != null && burnable.gameObject.CompareTag("EndTutorialBlock") && burnable.isOnFire)
            {
                
                FireOnEvent2?.Invoke();
                return;
            }
        }

        isOnFire = false; // Falls keine Übereinstimmung gefunden wurde
    }


    public bool IsOnFire()
    {
        return isOnFire;
    }
}
