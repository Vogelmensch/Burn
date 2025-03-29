using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class SensDisplay : MonoBehaviour
{   
    [SerializeField] private TMP_Text textfield;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textfield.text = PlayerPrefs.GetFloat("MouseSensitivity").ToString("0.0");
    }

    void Update(){

        if(!(textfield.text == PlayerPrefs.GetFloat("MouseSensitivity").ToString("0.0"))){
            if(PlayerPrefs.GetFloat("MouseSensitivity") < 1.1f && PlayerPrefs.GetFloat("MouseSensitivity") > 0.9f|| PlayerPrefs.GetFloat("MouseSensitivity") > 9.9f){
                textfield.text = PlayerPrefs.GetFloat("MouseSensitivity").ToString("0");
            }else{
                textfield.text = PlayerPrefs.GetFloat("MouseSensitivity").ToString("0.0");
            }
        }

    }

    // Update is called once per frame
}
