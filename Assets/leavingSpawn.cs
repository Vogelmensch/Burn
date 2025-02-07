using Unity.VisualScripting;
using UnityEngine;

public class leavingSpawn : MonoBehaviour
{
    public RainTrigger rainTrigger;
    public GameObject player;
    Collider playerCollider;
    Collider spawnCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnCollider = GetComponent<Collider>();
        playerCollider = player.GetComponent<Collider>();

        if (spawnCollider == null)
            Debug.LogError("Spawn needs a collider to trigger rain");
        else if (!spawnCollider.isTrigger)
            Debug.LogError("Spawn-Collider needs to be a trigger to trigger rain");
        if (playerCollider == null)
            Debug.LogError("Player needs a collider to trigger rain");
    }

    void OnTriggerExit(Collider col)
    {
        if (!rainTrigger.IsRaining() && col == playerCollider)
        {
            rainTrigger.StartRain();
        }
    }
}
