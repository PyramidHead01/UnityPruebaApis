using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using SimpleJSON;
using UnityEngine.UI;

public class Post : MonoBehaviour
{

    public Text messageText;
    public InputField scoreToSend;

    readonly string getURL = "", postURL = "";

    // Start is called before the first frame update
    void Start()
    {
        messageText.text = "Press buttons to interact with web server";
    }

    // Update is called once per frame
    [System.Obsolete]
    public void OnButtonGetScore()
    {

        messageText.text = "Downloading data...";
        StartCoroutine(SimpleGetRequest());

    }

    [System.Obsolete]
    IEnumerator SimpleGetRequest()
    {
        UnityWebRequest www = UnityWebRequest.Get(getURL);

        yield return www.SendWebRequest();

        if(www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            messageText.text = www.downloadHandler.text;
        }
    }

    [System.Obsolete]
    public void OnButtonSendScore()
    {
        if(scoreToSend.text == string.Empty)
        {
            messageText.text = "Error: No high score to send.\nEnter a value in the input field.";
        }
        else
        {
            messageText.text = "Sending data...";
            //Se non me equivoco, haberia que modificar o scoreToSend para que este sexa un JSONObject
            StartCoroutine(SimplePostRequest(scoreToSend.text));
        }
    }

    [System.Obsolete]
    IEnumerator SimplePostRequest(string curScore)
    {

        List<IMultipartFormSection> WWWForm = new List<IMultipartFormSection>();
        WWWForm.Add(new MultipartFormDataSection("curscoreKey", curScore));

        UnityWebRequest www = UnityWebRequest.Post(postURL, WWWForm);

        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            messageText.text = www.downloadHandler.text;
        }

    }

}
