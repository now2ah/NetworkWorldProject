using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;
using System;

public class Server : MonoBehaviour
{
    string _localHostName;
    IPAddress _localIPAddress;
    IPEndPoint _localEndPoint;
    Socket _listeningSocket;
    bool _isBind = false;
    bool _isListening = false;

    List<UserToken> _userList;

    List<Socket> _clientsSocketList;
    List<Socket> _checkReadList;        //select Àü¿ë

    public int backlog;

    public event EventHandler OnServerCreated;
    public event EventHandler<string> OnReceiveMessage;
    public event EventHandler<string> OnSendMessage;

    public IPEndPoint IPEndPoint => _localEndPoint;
    public bool IsBind => _isBind;
    public bool IsListening => _isListening;

    private void Update()
    {
        if (_isListening)
        {
            _PollingSocketList(_clientsSocketList);
        }
    }

    private void OnDisable()
    {
        if (OnServerCreated != null)
        {
            foreach (EventHandler invocation in OnServerCreated.GetInvocationList())
            {
                OnServerCreated -= invocation;
            }
        }

        if (_clientsSocketList.Count > 0)
        {
            foreach (var client in _clientsSocketList)
            {
                client.Close();
            }
        }

        if (_listeningSocket != null)
        {
            _listeningSocket.Shutdown(SocketShutdown.Both);
            _listeningSocket.Close();
        }
    }

    public void Initialize(string id)
    {
        _localHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(_localHostName);
        _localIPAddress = ipHostEntry.AddressList[0];       //need to add multi address
        _localEndPoint = new IPEndPoint(IPAddress.Any, Defines.PORT);

        _clientsSocketList = new List<Socket>();
        _checkReadList = new List<Socket>();

        _listeningSocket = _CreateServerSocket();

        _userList = new List<UserToken>();
        UserToken host = new UserToken(id, _listeningSocket);

        _Bind(_localEndPoint);
        _Listen(backlog);
    }

    public void SubscribeChatEvent(ChatManager chatManager)
    {
        if (chatManager != null)
        {
            chatManager.OnSendButtonClicked += _OnSendButtonClicked;
        }
    }

    void _OnSendButtonClicked(object sender, string message)
    {
        ServerSendChat sendMessage = new ServerSendChat();
        sendMessage.message = message;
        string sendMessageString = JsonUtility.ToJson(sendMessage);
        _BroadcastMessage(sendMessageString, Defines.EMessageType.SEND_CHAT);
    }

    Socket _CreateServerSocket(/*need to add TCP, UDP option*/)
    {
        return new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }

    bool _Bind(IPEndPoint endPoint)
    {
        try
        {
            if (null == _listeningSocket)
            {
                throw new System.Exception("server socket is null");
            }

            _listeningSocket.Bind(endPoint);
        }
        catch (SocketException e)
        {
            Debug.LogError(e.Message);
        }

        return _listeningSocket.IsBound;
    }

    void _Listen(int backlog)
    {
        try
        {
            if (null == _listeningSocket)
            {
                throw new System.Exception("server socket is null");
            }

            _listeningSocket.Listen(backlog);
            _isListening = true;

            OnServerCreated?.Invoke(this, EventArgs.Empty);

            _clientsSocketList.Add(_listeningSocket);
        }
        catch (SocketException e)
        {
            Debug.LogError(e.Message);
        }
    }

    bool _PollingSocketList(List<Socket> clientSocketList)
    {
        if (clientSocketList.Count > 0)
        {
            _checkReadList = new List<Socket>(clientSocketList);

            if (null == _listeningSocket)
                return false;

            Socket.Select(_checkReadList, null, null, 10);

            foreach (var checkedSocket in _checkReadList)
            {
                if (checkedSocket == _listeningSocket)
                {
                    Socket clientSocket = checkedSocket.Accept();
                    _clientsSocketList.Add(clientSocket);
                }
                else
                {
                    Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
                    int receiveSize = checkedSocket.Receive(packet.PacketBuffer);

                    if (receiveSize > 0)
                    {
                        _HandleMessage(checkedSocket, packet);
                    }
                }
            }
        }
        return true;
    }

    void _HandleMessage(Socket socket, Packet packet)
    {
        Defines.EMessageType type = packet.GetPacketType();

        if (type == Defines.EMessageType.CONNECT_USER)
        {
            string message = packet.ReadPacket();
            ClientConnectUser receiveMessage = JsonUtility.FromJson<ClientConnectUser>(message);
            UserToken userToken = new UserToken(receiveMessage.id, socket);

            ServerConnectUser sendMessage = new ServerConnectUser();
            sendMessage.id = userToken.UserID;
            string sendMessageString = JsonUtility.ToJson(sendMessage);
            _BroadcastMessage(sendMessageString, type);
            OnReceiveMessage?.Invoke(this, userToken.UserID);
        }
        else if (type == Defines.EMessageType.SEND_CHAT)
        {
            string message = packet.ReadPacket();
            ClientSendChat receiveMessage = JsonUtility.FromJson<ClientSendChat>(message);
            
            ServerSendChat sendMessage = new ServerSendChat();
            sendMessage.message = receiveMessage.message;
            string sendMessageString = JsonUtility.ToJson(sendMessage);
            _BroadcastMessage(sendMessageString, type);
        }
    }

    void _BroadcastMessage(string message, Defines.EMessageType type)
    {
        if (_clientsSocketList.Count > 0)
        {
            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            packet.CreatePacket(message, type);

            foreach (var socket in _clientsSocketList)
            {
                socket.Send(packet.PacketBuffer);
            }
        }
    }
}
