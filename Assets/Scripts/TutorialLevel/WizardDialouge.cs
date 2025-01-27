using TMPro;
using UnityEngine;
public class WizardDialogue : MonoBehaviour
{
    public GameObject dialogueBox; // Die Sprechblase (Parent-Objekt)
    public TextMeshProUGUI dialogueText; // Das TMP-Textfeld für den Text
    public Level1ChairDetector chairDetector;

    private void Start()
    {
        ShowDialogue("Hello there young traveler!", 5f);
    }

    public void ShowDialogue(string text)
    {
        dialogueBox.SetActive(true); // Sprechblase anzeigen
        dialogueText.text = text; // Text setzen
    }

    public void HideDialogue()
    {
        dialogueBox.SetActive(false); // Sprechblase verstecken
    }
}
