using UnityEngine;

public class elevator : MonoBehaviour
{
    public float weight = 0;
    public ElevatorManager manager;
    public Vector3 startPos;

    public void Start()
    {
        startPos = transform.position;
    }
    private void OnTriggerEnter(Collider collision)
    {
        if(collision.GetComponent<Rigidbody>() != null){
            weight += collision.GetComponent<Rigidbody>().mass;
            Debug.Log(weight);
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if(collision.GetComponent<Rigidbody>() != null){
            weight -= collision.GetComponent<Rigidbody>().mass;
        }
    }
    public void invokeForce(float force){
        GetComponent<Rigidbody>().AddForce(Vector3.down * (force * 2));
    }
}
