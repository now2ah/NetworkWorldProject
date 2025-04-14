using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    string _remoteAddress;
    Socket _socket;

    public event EventHandler OnClientConnected;
    public event EventHandler<string> OnReceiveMessage;

    private void Update()
    {
        if (_socket != null & _socket.Connected)
        {
            _PollingSocket();
        }
    }

    private void OnDisable()
    {
        if (OnClientConnected != null)
        {
            foreach (EventHandler invocation in OnClientConnected.GetInvocationList())
            {
                OnClientConnected -= invocation;
            }
        }

        if ( _socket != null )
        {
            _socket.Close();
        }
    }

    public bool StartClient(string id, string address = null)
    {
        IPEndPoint endPoint = null;
        if (null == address)
        {
            endPoint = new IPEndPoint(IPAddress.Loopback, Defines.PORT);
        }
        else
        {
            endPoint = new IPEndPoint(IPAddress.Parse(address), Defines.PORT);
        }
        
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        _socket.Connect(endPoint);

        if (_socket.Connected)
        {
            //send connect
            ClientConnectUser msg = new ClientConnectUser();
            msg.id = id;
            string serializedMsg = JsonUtility.ToJson(msg);

            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            packet.CreatePacket(serializedMsg, Defines.EMessageType.CONNECT_USER);

            OnClientConnected.Invoke(this, EventArgs.Empty);
        }

        return _socket.Connected;
    }

    public void SubscribeChatEvent(ChatManager chatManager)
    {
        if (chatManager != null)
        {
            chatManager.OnSendButtonClicked += _OnSendButtonClicked;
        }
    }

    public void Send(string message)
    {
        if (_socket.Connected)
        {
            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            packet.CreatePacket(message, Defines.EMessageType.SEND_CHAT);
            _socket.Send(packet.PacketBuffer);
        }
    }

    void _OnSendButtonClicked(object sender, string message)
    {
        Send(message);
    }

    bool _PollingSocket()
    {
        if (null == _socket)
            return false;

        List<Socket> socket = new List<Socket>();
        socket.Add(_socket);

        Socket.Select(socket, null, null, 10);

        foreach (var checkedSocket in socket)
        {
            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            int receiveSize = checkedSocket.Receive(packet.PacketBuffer);

            if (receiveSize > 0)
            {
                string message = packet.ReadPacket();
                OnReceiveMessage.Invoke(this, message);
            }
        }
        return true;
    }
}
