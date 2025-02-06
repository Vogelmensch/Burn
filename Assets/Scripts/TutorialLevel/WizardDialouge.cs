using TMPro;
using UnityEngine;
using System.Collections;
public class WizardDialogue : MonoBehaviour
{
    public GameObject dialogueBox; //Der Magier und die Sprechblase :))
    public TextMeshProUGUI dialogueText; // Das TMP-Textfeld für den Text
    [SerializeField] private RevealLevel2 doorDetector;
    private RevealLevel3 doorDetector2;
    private bool hasTriggered = false;
    private bool hasTriggered2 = false;
    private string[][] dialogueSets = 
{
    new string[] 
    {
        "Hello there, young traveler!",
        "In this level you will need to take one of the chairs in front of you",
        "Once you have done this, drag the chair onto the table to your left",
        "Good luck!"
    },
    new string[] 
    {
        "Great Job!",
        "Go to the next room - if you are not allready there!",
        "Now take one of the Chairs and burn it using the canlde in the cupboard",
        "Once thats done, use this chair and enlight the fire at the fireplace!"
    },
    new string[]
    {
        "Great Job!",
        "Now you will learn to handle the FIREBALL!"
    }
};

    private int currentIndex = 0;
    private int currentArray = 0;
    private void Start()
    {
        if (doorDetector == null)
        {
            doorDetector = FindObjectOfType<RevealLevel2>();
        }
        if(doorDetector2 == null)
        {
             doorDetector2 = FindObjectOfType<RevealLevel3>();
        }
        doorDetector.DoorOpenedEvent += OnDoorOpened;
        doorDetector2.DoorOpenedEvent2 += OnDoorOpened2;
        ShowDialogue(dialogueSets[currentArray][currentIndex]);
        
    }
    private void OnDoorOpened()
    {
        Debug.Log("Tür wurde geöffnet - Event empfangen!");
        hasTriggered = true;
        Debug.LogError("Invoke Next Dialoge");
        Invoke("ShowNextDialoge", 3f);
    }
    private void OnDoorOpened2()
    {
        Debug.Log("Tür wurde geöffnet - Event empfangen!");
        hasTriggered2 = true;
        Invoke("ShowNextDialoge", 3f);
    }

    public void ShowNextDialoge()
    {
        Debug.LogError($"CurrentArry = {currentArray}");
        Debug.LogError($"CurrentIndex = {currentIndex}");
        ShowDialogue(dialogueSets[currentArray][currentIndex]);
    }

    public void NextDialogue()
    {
         if (currentIndex >= dialogueSets[currentArray].Length - 1)
         {
            HideDialogue();
            currentArray ++;
            currentIndex = 0;
         }
         else
         {
            currentIndex ++;
            ShowDialogue(dialogueSets[currentArray][currentIndex]);
         }
    }


    public void ShowDialogue(string text)
    {
        dialogueBox.SetActive(true); // Sprechblase anzeigen
        dialogueText.text = text; // Text setzen
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false); 
    }
}
