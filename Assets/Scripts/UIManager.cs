using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameObject loginPanel;
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public GameObject chatPanel;
    public NetworkManager networkManager;

    GameObject currentPanel;

    private void Start()
    {
        if (loginPanel != null)
        {
            currentPanel = loginPanel;
        }
    }

    public void SubscribeEvent()
    {
        if (networkManager != null && networkManager.Server != null)
        {
            networkManager.Server.OnServerCreated += _OnServerCreated;
        }
    }

    public void OnStartServer()
    {
        if (idInput.text == null || pwInput.text == null)
        {
            Debug.LogError("need to input id and pw");
        }
        else
        {
            if (networkManager != null)
            {
                networkManager.StartServer();
            }
        }
    }

    public void OnStartClient()
    {
        if (idInput.text == null || pwInput.text == null)
        {
            Debug.LogError("need to input id and pw");
        }
        else
        {
            if (networkManager != null)
            {
                networkManager.StartClient();
            }
        }
    }

    void _OnServerCreated(object sender, EventArgs e)
    {
        if (chatPanel != null)
        {
            _LoadPanel(chatPanel);
        }
    }

    void _LoadPanel(GameObject panelObj)
    {
        currentPanel.SetActive(false);
        currentPanel = panelObj;
        currentPanel.SetActive(true);
    }
}
