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

    [SerializeField] private ChatManager _chatManager;
    public ChatManager ChatManager { get { return _chatManager; } private set { } }

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
        if (_startOptionPanel != null)
        {
            currentPanel = _startOptionPanel;
        }
    }

    public void SubscribeServerEvent()
    {
        if (NetworkManager.Singleton.Server != null)
        {
            NetworkManager.Singleton.Server.OnServerCreated += _OnServerCreated;
        }
    }

    public void SubscribeClientEvent(Client client)
    {
        if (client != null)
        {
            client.OnClientConnected += _OnClientConnected;
        }
    }

    void _OnServerCreated(object sender, EventArgs e)
    {
        if (_lobbyPanel != null)
        {
            _LoadPanel(_lobbyPanel);
        }
    }

    void _OnClientConnected(object sender, EventArgs e)
    {
        if (_lobbyPanel != null)
        {
            _LoadPanel(_lobbyPanel);
        }
    }

    void _LoadPanel(GameObject panelObj)
    {
        currentPanel.SetActive(false);
        currentPanel = panelObj;
        currentPanel.SetActive(true);
    }
}
