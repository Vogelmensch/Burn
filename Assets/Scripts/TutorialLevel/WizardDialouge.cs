using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
public class WizardDialogue : MonoBehaviour
{
    public GameObject dialogueBox; //Der Magier und die Sprechblase :))
    public TextMeshProUGUI dialogueText; // Das TMP-Textfeld für den Text
    [SerializeField] private RevealLevel2 doorDetector;
    private RevealLevel3 doorDetector2;
    private bool hasTriggered = false;
    private bool hasTriggered2 = false;
    private InputAction nextDialogueAction;
    private string[][] dialogueSets = 
{
    new string[] 
{
    "Ah, welcome, young traveler!",
    "In this trial, you must take one of the chairs before you.",
    "Once you have done so, place it upon the table to your left – as if guided by unseen forces.",
    "If you're using a mouse, press the right mouse button to do so",
    "However, if you're playing with a controller, press X or the left shoulder button",
    "May fate be on your side!"
},
new string[] 
{
    "A most excellent deed!",
    "Step forth into the next chamber – if you have not done so already!",
    "Now, take a chair and let the sacred flame from the cupboard's candle consume it.",
    "Once this is done, wield its burning remains to ignite the great fire in the hearth!"
},
new string[]
{
    "Splendid work, apprentice of the arcane arts!",
    "Now, you shall learn a most potent craft – the art of hurling fire-laden objects!",
    "With this newfound power, you shall set ablaze that which lies beyond your reach!",
    "With a keyboard, you shall press F to unleash a blazing orb of flame",
    "However, with a controller, you may press Y to do so",
    "To regulate the force of your throw, use Q and E on the keyboard.",
    "On the controller, use the back-buttons.",
    "Burn the Barrel behind the fence!"
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
        
        nextDialogueAction = InputSystem.actions.FindAction("Jump");
    }

    void Update(){
        if(nextDialogueAction.WasPressedThisFrame())
        {
            NextDialogue();
            Debug.Log("button pressed!");
        }
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
