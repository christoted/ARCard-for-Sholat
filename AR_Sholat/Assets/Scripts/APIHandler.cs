using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
public class APIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Text debugText;

    private string android_id = "asdf", key_token = "TOKEN";

    private float timer = -1;
    void Start()
    {   
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");

        
    }

    void Update(){

        if (timer > 0){
            timer -= Time.deltaTime;
        } else if (timer > -1){
            HideError();
            timer = -2;
        }
        

    }

    
    // API 1
    IEnumerator POST_RegisterSerialNumber(string android_id, string serial_number)
    {
        WWWForm form = new WWWForm();
        form.AddField("device_id", android_id);
        form.AddField("serial_number", serial_number);

        UnityWebRequest www = UnityWebRequest.Post("https://us-central1-license-server-2575b.cloudfunctions.net/register/shalatar", form);
        yield return www.SendWebRequest();

        // Debug.Log(www.responseCode+"");

        if (www.responseCode == 200){
            PlayerPrefs.DeleteAll();
            PlayerPrefs.SetString(key_token,www.downloadHandler.text);            
            transform.GetComponent<UIHandler>().ShowGameUI();
        } else if (www.responseCode == 410 || www.responseCode == 401){
            ShowError("serial number incorrect");
        } else if (www.responseCode == 409){
            ShowError("too many device registered (max 3)");
        } else {
            ShowError("network error");
        }
    }


    // API 2
    IEnumerator GET_CheckRegistration()
    {
        string uri = "https://us-central1-license-server-2575b.cloudfunctions.net/register/shalatar?token="+PlayerPrefs.GetString(key_token);

        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                debugText.text = ( webRequest.error);
            }
            else
            {
               debugText.text =  ( webRequest.downloadHandler.text);
            }
        }
    }


    // API 3
    IEnumerator POST_UnregisteredDevice()
    {
        WWWForm form = new WWWForm();
        form.AddField("token", PlayerPrefs.GetString(key_token));

        UnityWebRequest www = UnityWebRequest.Post("https://us-central1-license-server-2575b.cloudfunctions.net/unlink/shalatar", form);
        yield return www.SendWebRequest();

        // Debug.Log(www.responseCode+"");

        // if (www.responseCode == 200){
        //     PlayerPrefs.DeleteAll();
        //     PlayerPrefs.SetString(key_token,www.downloadHandler.text);            
        //     transform.GetComponent<UIHandler>().ShowGameUI();
        // } else if (www.responseCode == 410 || www.responseCode == 401){
        //     ShowError("serial number incorrect");
        // } else if (www.responseCode == 409){
        //     ShowError("too many device registered (max 3)");
        // } else {
        //     ShowError("network error");
        // }
    }

    public void RegisterSerialNumber(string serial_number){                
        StartCoroutine(POST_RegisterSerialNumber(android_id, serial_number));
    }

    public void CheckRegistration(){
        StartCoroutine(GET_CheckRegistration());
    }

    public void UnregisterDevice(){
        StartCoroutine(POST_UnregisteredDevice());
    }

    private void ShowError(string message){
        debugText.text = message;
        timer = 3F;
    }

    private void HideError(){
        debugText.text = "";
    }
}
