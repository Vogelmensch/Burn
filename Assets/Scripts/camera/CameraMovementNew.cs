using UnityEngine;
using UnityEngine.UI;

public class CameraMovementNew : MonoBehaviour
{
    public float sensX;
    public float sensY;
    public Transform orientation;
    float xRotation;
    float yRotation;
    public Vector2 initRotation;
    private void Start(){
        setSense();
        Cursor.lockState = CursorLockMode.Locked;

        xRotation = initRotation.x;
        yRotation = initRotation.y;

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0f);
    }

    void setSense(){
        sensX = PlayerPrefs.GetFloat("MouseSensitivity")*100.0f;
        sensY = PlayerPrefs.GetFloat("MouseSensitivity")*100.0f;
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
