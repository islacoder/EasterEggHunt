using UnityEngine;
using System.Collections;
using HoloToolkit.Unity;

public class MessageBox : Singleton<MessageBox> {
    public string Message;
    private string DebugMessage;
    private string Score;

    TextMesh messageTextMesh;
	// Use this for initialization
	void Start () {
        messageTextMesh = gameObject.GetComponentInChildren<TextMesh>();
        Message = "Welcome";
        DebugMessage = "";
        Score = "";
	}

    // Update is called once per frame
    void Update() {
        string separator = "\r\n";
        string message;

        if (Score.Trim() != "")
            message = Score;
        else
            message = "";
        if (Message.Trim() != "")
        {
            if (message.Trim() != "")
                message += separator;

            message += Message;
        }
        if (DebugMessage.Trim() != "") { 
        if (message.Trim() != "")
            message += separator;
        message += DebugMessage;
        }
        messageTextMesh.text = message;
    }

    public void UpdateScore(int collectedEggs, int totalEggs)
    {
        Score = collectedEggs + "/" + totalEggs;
        if (collectedEggs == totalEggs)
        {
            Message = "Congratulations! You have collected all the eggs.";
        }
        else
        {
            Message = "";
        }


    }

    public void Log(string s)
    {
        DebugMessage = s;
    }

}
