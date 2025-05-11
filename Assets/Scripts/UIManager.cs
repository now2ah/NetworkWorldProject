using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Singleton { get; private set; }

    private Canvas _canvas;
    private EventSystem _eventSystem;

    [SerializeField] private GameObject _startOptionPanel;
    [SerializeField] private GameObject _lobbyPanel;

    public GameObject loginPanel;
    public TMP_InputField idInput;
    public TMP_InputField pwInput;
    public GameObject chatPanel;
    public NetworkManager networkManager;

    GameObject currentPanel;

    private void Awake()
    {
        if (Singleton == null)
        {
            Singleton = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(Singleton);
        }

        _canvas = GetComponentInChildren<Canvas>();
        _eventSystem = GetComponentInChildren<EventSystem>();
    }

    private void Start()
    {
        if (loginPanel != null)
        {
            currentPanel = loginPanel;
        }
    }

    public void SubscribeServerEvent()
    {
        if (networkManager != null && networkManager.Server != null)
        {
            networkManager.Server.OnServerCreated += _OnServerCreated;
        }
    }

    public void SubscribeClientEvent(Client client)
    {
        if (networkManager != null && client != null)
        {
            client.OnClientConnected += _OnClientConnected;
        }
    }

    void _OnServerCreated(object sender, EventArgs e)
    {
        if (chatPanel != null)
        {
            _LoadPanel(chatPanel);
        }
    }

    void _OnClientConnected(object sender, EventArgs e)
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
