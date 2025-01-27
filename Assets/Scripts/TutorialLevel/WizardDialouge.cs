using TMPro;
using UnityEngine;
public class WizardDialogue : MonoBehaviour
{
    public GameObject dialogueBox; //Der Magier und die Sprechblase :))
    public TextMeshProUGUI dialogueText; // Das TMP-Textfeld für den Text
    private string[] dialogueLines1 = 
    {
        "Hello there, young travaler!",
        "In this level you will need to take one of the chairs in front of you",
        "Once you have done this, drag the chair onto the table to your left",
        "Good luck!"
    };
    private string[] dialogueLines2 = 
    {
        "Great Job!",
        "Level 2 is waiting!"
    }
    private
    private int currentIndex = 0;
    private void Start()
    {
        ShowDialogue(dialogueLines1[currentIndex]);
    }

    private void Update()
    {
        
    }


    public void NextDialogue()
    {
         if (currentIndex >= dialogueLines1.Length - 1)
         {
            HideDialogue();
         }
         else
         {
            currentIndex ++;
            ShowDialogue(dialogueLines1[currentIndex]);
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
