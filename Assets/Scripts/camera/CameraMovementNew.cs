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
    private GameObject lastHitObject;
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

        if(PlayerPrefs.GetFloat("MouseSensitivity")*100.0f != sensX){
            setSense();
        }

        float MouseX = Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float MouseY = Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;

        yRotation += MouseX;

        xRotation -= MouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // rotate cam and orientation
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0f);

        // check if object is carryable and activate outline
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 2f))
        {
            GameObject hitObject = hit.transform.gameObject;
            if (hitObject!=lastHitObject) {
                if (lastHitObject != null) {
                    SetOutline(lastHitObject,false);
                }

                SetOutline(hitObject, true);
                lastHitObject = hitObject;
            }
        } else {
            // script deaktivieren, wenn wir nichts anschauen
            if (lastHitObject != null) {
                SetOutline(lastHitObject, false);
                lastHitObject = null;
            }
        }
    }

    public void SetOutline(GameObject obj, bool state) 
    {
        if (obj.GetComponent<Outline>() != null) {
            obj.GetComponent<Outline>().enabled = state;
        }

        // Notify the CarryAndShoot script about outline control
        CarryAndShoot carryAndShoot = obj.GetComponent<CarryAndShoot>();
        if (carryAndShoot != null) {
            carryAndShoot.SetOutlineControlByCamera(state);
        }
    }
}
