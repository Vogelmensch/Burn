using UnityEngine;

public class CameraMovementNew : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    float xRotation;
    float yRotation;
    private void Start(){
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update(){
        float MouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        yRotation += MouseX;

        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0f);
    }
}
