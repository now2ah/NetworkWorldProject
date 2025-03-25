using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ChatManager : MonoBehaviour
{
    public NetworkManager networkManager;
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
        if (networkManager != null && networkManager.Server != null)
        {
            networkManager.Server.OnServerCreated += _OnServerCreated;
            networkManager.Server.OnReceiveMessage += _OnReceiveMessage;
            networkManager.Server.OnSendMessage += _OnSendMessage;
        }
    }

    public void SubscribeClientEvent(Client client)
    {
        if (networkManager != null && client != null)
        {
            client.OnClientConnected += _OnClientConnected;
            client.OnReceiveMessage += _OnReceiveMessage;
        }
    }

    void _OnSendButtonClicked()
    {
        if (inputText != null)
        {
            //WriteMessage(inputText.text);
            OnSendButtonClicked.Invoke(this, inputText.text);
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
