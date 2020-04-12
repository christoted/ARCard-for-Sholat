using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class APIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public Text debugText;


    void Start()
    {   
        AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject contentResolver = currentActivity.Call<AndroidJavaObject>("getContentResolver");
        AndroidJavaClass secure = new AndroidJavaClass("android.provider.Settings$Secure");
        string android_id = secure.CallStatic<string>("getString", contentResolver, "android_id");

        //   debugText.text = android_id;



       //  StartCoroutine(Upload(android_id));

        // A correct website page.
        StartCoroutine(GetRequest("https://us-central1-license-server-2575b.cloudfunctions.net/register/shalatar?token=12345"));
    }

    IEnumerator GetRequest(string uri)
    {
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

    IEnumerator Upload(string android_id)
    {
        WWWForm form = new WWWForm();
        form.AddField("device_id", android_id);
        form.AddField("serial_number", "TEST1234");

        UnityWebRequest www = UnityWebRequest.Post("https://us-central1-license-server-2575b.cloudfunctions.net/register/shalatar", form);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            debugText.text = (www.error);
        }
        else
        {
            debugText.text = ("Form upload complete!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
