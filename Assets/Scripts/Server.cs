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

    public bool Initialize()
    {
        _localHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(_localHostName);
        _localIPAddress = ipHostEntry.AddressList[0];       //need to add multi address
        _localEndPoint = new IPEndPoint(IPAddress.Any, Defines.PORT);

        _userList = new List<UserToken>();

        _clientsSocketList = new List<Socket>();
        _checkReadList = new List<Socket>();

        _listeningSocket = _CreateServerSocket();

        return _listeningSocket != null ? true : false;
    }

    public void Bind()
    {
        if (null != _localEndPoint) { _isBind = _Bind(_localEndPoint); }
    }

    public void Listen(int backlog)
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

    public void SubscribeChatEvent(ChatManager chatManager)
    {
        if (chatManager != null)
        {
            chatManager.OnSendButtonClicked += _OnSendButtonClicked;
        }
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
                        //OnReceiveMessage.Invoke(this, message);
                        _HandleMessage(checkedSocket, packet);

                        //old
                        //string message = packet.ReadPacket();
                        //OnReceiveMessage.Invoke(this, message);
                        //Send(message);
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
            UserToken userToken = new UserToken();
            string message = packet.ReadPacket();
            ClientConnectUser receiveMessage = JsonUtility.FromJson<ClientConnectUser>(message);
            userToken.UserID = receiveMessage.id;
            userToken.Socket = socket;

            ServerConnectUser sendMessage = new ServerConnectUser();
            sendMessage.id = userToken.UserID;
            string sendMessageString = JsonUtility.ToJson(sendMessage);
            _BroadcastMessage(sendMessageString, type);
        }
        else if (type == Defines.EMessageType.SEND_CHAT)
        {

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

    public void Send(string message)
    {
        if (_clientsSocketList.Count > 0)
        {
            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            packet.CreatePacket(message, Defines.EMessageType.SEND_CHAT);

            foreach (var socket in _clientsSocketList)
            {
                if (socket == _listeningSocket)
                    continue;

                socket.Send(packet.PacketBuffer);
            }
        }
    }

    void _OnSendButtonClicked(object sender, string message)
    {
        Send(message);
        OnSendMessage.Invoke(this, message);
    }
}
