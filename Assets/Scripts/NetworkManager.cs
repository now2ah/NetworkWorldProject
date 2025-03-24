using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;



public class NetworkManager : MonoBehaviour
{
    Server _server;
    Client _client;
    int backlog = 5;

    #region OLD Version Variables
    public enum NetworkType
    {
        TCP,
        UDP
    }

    Socket _serverSocket;
    Socket _serverListeningSocket;

    Socket _clientSocket;

    string _localHostName;
    IPAddress _ipAddress;
    IPEndPoint _localEndPoint;

    bool _isBound;
    #endregion

    private void Update()
    {

    }

    public void CreateRoom()
    {
        _StartServer();
    }

    public void JoinRoom()
    {
        _StartClient();
    }

    public void SendTest()
    {
        if (_client != null)
        {
            _client.Send("Fuck YES This is Test Messsage!!");
        }
    }

    void _StartServer()
    {
        _server = gameObject.AddComponent<Server>();
        _server.Initialize();
        _server.Bind();
        _server.Listen(backlog);
    }

    void _StartClient()
    {
        _client = gameObject.AddComponent<Client>();
        _client.StartClient();
    }

    

    #region OLD Version
    public void StartServer()
    {
        if (InitializeServer(NetworkType.TCP) && _localEndPoint != null)
        {
            if (_Bind(_localEndPoint))
            {
                _isBound = true;
            }
            else
            {
                Debug.Log("Server socket isn't bound to local end point");
            }

            try
            {
                _serverSocket.Listen(5);
                Debug.Log("Server Listening...");
            }
            catch (SocketException e)
            {
                Debug.Log($"Socket Exception while in listen : {e.Message}");
            }

            if (_serverSocket != null && _isBound)
            {
                //while(true)
                {
                    _serverListeningSocket = _serverSocket.Accept();
                    IPEndPoint clientEndpoint = _serverListeningSocket.RemoteEndPoint as IPEndPoint;
                    Debug.Log($"Connected to Client({clientEndpoint.Address}, port Num ({clientEndpoint.Port})");

                    byte[] recvBuffer = new byte[Defines.MAX_MESSAGE_BUFFER_SIZE]; //1024
                    int recvByteSize = _serverListeningSocket.Receive(recvBuffer);
                    string recvString = Encoding.UTF8.GetString(recvBuffer, 0, recvByteSize);

                    Debug.Log($"{clientEndpoint.Address} : {recvString}");

                    byte[] sendBuffer = Encoding.UTF8.GetBytes("Server Received your message");
                    _serverListeningSocket.Send(sendBuffer);

                    _serverListeningSocket.Shutdown(SocketShutdown.Both);
                    _serverListeningSocket.Dispose();
                }
            }
        }
    }

    public void StartClient()
    {
        if (InitializeClient(NetworkType.TCP) && _localEndPoint != null)
        {
            _clientSocket.Connect(_localEndPoint);
            Debug.Log($"Connected to Server : {_localEndPoint.Address}, port number : {_localEndPoint.Port}");

            byte[] sendBuffer = Encoding.UTF8.GetBytes("Hi! I am client.");
            int sendByteSize = _clientSocket.Send(sendBuffer);

            byte[] recvBuffer = new byte[Defines.MAX_MESSAGE_BUFFER_SIZE];
            int recvByteSize = _clientSocket.Receive(recvBuffer);
            string recvString = Encoding.UTF8.GetString(recvBuffer, 0, recvByteSize);

            Debug.Log($"From Server : {recvString}");

            _clientSocket.Shutdown(SocketShutdown.Both);
            _clientSocket.Close();
        }
    }

    public bool InitializeServer(NetworkType type)
    {
        _localHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(_localHostName);
        _ipAddress = ipHostEntry.AddressList[0];
        _localEndPoint = new IPEndPoint(_ipAddress, 7890);

        Debug.Log($"Loading local Host name and Address : {_localHostName} , {_ipAddress}");

        if (type == NetworkType.TCP)
        {
            _serverSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        else if (type == NetworkType.UDP)
        {
            _serverSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }

        return _serverSocket != null ? true : false;
    }

    public bool InitializeClient(NetworkType type)
    {
        _localHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(_localHostName);
        _ipAddress = ipHostEntry.AddressList[0];
        _localEndPoint = new IPEndPoint(_ipAddress, 7890);

        Debug.Log($"Loading local Host name and Address : {_localHostName} , {_ipAddress}");

        if (type == NetworkType.TCP)
        {
            _clientSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        }
        else if (type == NetworkType.UDP)
        {
            _clientSocket = new Socket(_localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
        }

        return _clientSocket != null ? true : false;
    }

    bool _Bind(IPEndPoint endPoint)
    {
        if (_serverSocket != null)
        {
            _serverSocket.Bind(endPoint);
        }

        return _serverSocket.IsBound == true;
    }
}

#endregion
