using UnityEngine;

public class RevealLevel2 : MonoBehaviour
{   
    public AudioClip doorSoundClip;  
    public Level1ChairDetector chairDetector;
    public GameObject door1;
    private AudioSource audioSource;
    private bool played = false;
    public event System.Action DoorOpenedEvent;

    void Start()
    {
        // AudioSource auf dem gleichen GameObject automatisch suchen
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>(); // Falls keine AudioSource vorhanden ist, hinzufügen
        }
    }

    void Update()
    {
        if (chairDetector != null && chairDetector.IsChairOnTable() && !played)
        {
            played = true;
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (door1 != null)
        {
            door1.transform.localRotation = Quaternion.Euler(0, 90, 0);
            door1.transform.localScale =  new Vector3(0.5f, 1f, 1f);
            
            // Sound abspielen, falls Clip zugewiesen ist
            if (doorSoundClip != null && audioSource != null)
            {
                DoorOpenedEvent?.Invoke();
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
