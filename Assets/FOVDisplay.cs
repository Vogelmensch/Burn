using UnityEngine;
using TMPro;
using System;

public class FOVDisplay : MonoBehaviour
{   
    [SerializeField] private Camera cam;
    [SerializeField] private TMP_Text textfield;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textfield.text = cam.fieldOfView.ToString();
    }

    void Update(){

        if(!(textfield.text == cam.fieldOfView.ToString())){
            textfield.text = cam.fieldOfView.ToString();
        }

    }

    // Update is called once per frame
}
