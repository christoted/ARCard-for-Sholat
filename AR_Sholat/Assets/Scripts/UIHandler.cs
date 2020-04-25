using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIHandler : MonoBehaviour
{

    

    [SerializeField] private Canvas canvasGame, canvasRegister;
    [SerializeField] private Button buttonSubmit, buttonLogOut;

    private string key_token = "TOKEN";



    void Start()
    {
        if (!PlayerPrefs.HasKey(key_token)){            
            ShowRegisterUI();
            
        } else {
            ShowGameUI();
        }

        buttonSubmit.GetComponent<Button>().onClick.AddListener(Submit);
        buttonLogOut.GetComponent<Button>().onClick.AddListener(Logout);

        if ( Input.GetKeyDown(KeyCode.Escape)){
            Application.Quit();
        }

    }

    


    private void Submit(){
        buttonSubmit.GetComponent<Button>().interactable = false;
        Invoke("EnableButton",2);


        Text serial_number_text = canvasRegister.transform.Find("Canvas/InputField/Text").GetComponent<Text>();
        string serial_number = serial_number_text.text;
        if (serial_number.Length > 11){
            
        } else {
            transform.GetComponent<APIHandler>().RegisterSerialNumber(serial_number);            
        }
    }

    private void Logout(){
        transform.GetComponent<APIHandler>().UnregisterDevice();
        PlayerPrefs.DeleteAll();
        Application.Quit();
    }

    public void ShowRegisterUI(){
        canvasGame.enabled = false;
        canvasRegister.enabled = true;        
    }

    public void ShowGameUI(){
        canvasGame.enabled = true;
        canvasRegister.enabled = false;        
    }

    private void EnableButton(){
        buttonSubmit.GetComponent<Button>().interactable = true;
    }
}
