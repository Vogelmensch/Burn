using UnityEngine;
public class RevealLevel3 : MonoBehaviour
{
    public GameObject door2;
    public AudioClip doorSoundClip; 
    private AudioSource audioSource;
    private Level2Block fireDetector;
    public event System.Action DoorOpenedEvent2;
    void Start()
    {
        // AudioSource auf dem gleichen GameObject automatisch suchen
        audioSource = gameObject.AddComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Falls keine AudioSource vorhanden ist, hinzufügen
        }
         if (fireDetector == null)
        {
            fireDetector = FindObjectOfType<Level2Block>();
        }
        fireDetector.FireOnEvent += OpenDoor;
    }


    public void OpenDoor()
    {
        if (door2 != null)
        {
            door2.transform.rotation = Quaternion.Euler(0, 10, 0);
            
            // Sound abspielen, falls Clip zugewiesen ist
            if (doorSoundClip != null && audioSource != null)
            {
                DoorOpenedEvent2?.Invoke();
                audioSource.PlayOneShot(doorSoundClip);
                
            }
            else
            {
                Debug.LogError("Fehlender AudioClip oder AudioSource für Türsound!");
            }
        }
        else
        {
            Debug.LogError("Kein Prefab für Tür zugewiesen!");
        }
    }

}