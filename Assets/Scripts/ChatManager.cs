using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChatManager : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TMP_InputField inputText;
    public Button sendButton;

    string _messageText = "--Start Chat--";

    public event EventHandler<string> OnSendButtonClicked;

    private void Start()
    {
        sendButton.onClick.AddListener(_OnSendButtonClicked);
    }

    private void OnDisable()
    {
        if (OnSendButtonClicked != null)
        {
            foreach(EventHandler<string> invocation in OnSendButtonClicked.GetInvocationList())
            {
                OnSendButtonClicked -= invocation;
            }
        }
    }

    public void WriteMessage(string msg)
    {
        _messageText = _messageText + "\n" + msg;
        
        if (messageText != null)
        {
            messageText.text = _messageText;
        }
    }

    public void SubscribeServerEvent()
    {
        if (NetworkManager.Singleton.Server != null)
        {
            NetworkManager.Singleton.Server.OnServerCreated += _OnServerCreated;
            NetworkManager.Singleton.Server.OnReceiveMessage += _OnReceiveMessage;
            NetworkManager.Singleton.Server.OnSendMessage += _OnSendMessage;
        }
    }

    public void SubscribeClientEvent(Client client)
    {
        if (client != null)
        {
            client.OnReceiveMessage += _OnReceiveMessage;
        }
    }

    void _OnSendButtonClicked()
    {
        if (inputText != null)
        {
            OnSendButtonClicked?.Invoke(this, inputText.text);
            inputText.text = "";
        }
    }

    void _OnServerCreated(object sender, EventArgs e)
    {
        WriteMessage("--Server is created--");
    }

    void _OnReceiveMessage(object sender, string message)
    {
        WriteMessage(message);
    }

    void _OnSendMessage(object sender, string message)
    {
        WriteMessage(message);
    }

    void _OnClientConnected(object sender, EventArgs e)
    {
        WriteMessage("--Client is connected--");
    }
}
