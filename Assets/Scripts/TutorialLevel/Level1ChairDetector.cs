using UnityEngine;

public class Level1ChairDetector : MonoBehaviour
{
    private bool chairOnTable = false;

    void OnTriggerEnter(Collider chair)
    {
        float chairY = chair.transform.position.y; //damit erst aktiviert wenn der Stuhl auch wirklich drauf liegt :)) 
        if(chair.CompareTag("Chair") && chairY >= 0.6f)
        {
            chairOnTable = true;
            Debug.Log("Ein Stuhl steht auf dem Tisch");
        }
    }

    public bool IsChairOnTable()
    {
        return chairOnTable;
    }
    
}
