using TMPro;
using UnityEngine;
public class WizardDialogue : MonoBehaviour
{
    public GameObject dialogueBox; //Der Magier und die Sprechblase :))
    public TextMeshProUGUI dialogueText; // Das TMP-Textfeld für den Text
    public GameObject triggerObject;
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
        "Level 2 is waiting!"
    }
};

    private int currentIndex = 0;
    private int currentArray = 0;
    private void Start()
    {
        Collider collider = triggerObject.GetComponent<Collider>();
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

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger Enter erkannt: " + other.gameObject.name);
        if(other.CompareTag("MainCamera"))
        {
            ShowDialogue(dialogueSets[currentArray][currentIndex]);
            Debug.Log("Kamera hat den Trigger betreten!");
        }
    }



}
