using UnityEngine;
using System.Net.Sockets;
using System.Net;

public class NetworkManager : MonoBehaviour
{
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
        }
    }

    public void StartClient()
    {
        if (InitializeClient(NetworkType.TCP) && _localEndPoint != null)
        {
            _clientSocket.Connect(_localEndPoint);
            Debug.Log($"Connected to Server : {_localEndPoint.Address}, port number : {_localEndPoint.Port}");
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

    // Update is called once per frame
    void Update()
    {
        if (_serverSocket != null && _isBound)
        {
            _serverListeningSocket = _serverSocket.Accept();
            IPEndPoint clientEndpoint = _serverListeningSocket.RemoteEndPoint as IPEndPoint;
            Debug.Log($"Connected to Client({clientEndpoint.Address}, port Num ({clientEndpoint.Port})");

            _serverListeningSocket.Shutdown(SocketShutdown.Both);
            _serverListeningSocket.Dispose();
        }
    }
}
