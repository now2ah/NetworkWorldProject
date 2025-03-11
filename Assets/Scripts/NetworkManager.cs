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
    Socket _clientSocket;

    string _localHostName;
    IPAddress _ipAddress;
    IPEndPoint _localEndPoint;

    bool _isBound;

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

    bool _Bind(IPEndPoint endPoint)
    {
        if (_serverSocket != null)
        {
            _serverSocket.Bind(endPoint);
        }

        return _serverSocket.IsBound == true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
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
            }
            catch (SocketException e)
            {
                Debug.Log($"Socket Exception while in listen : {e.Message}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_isBound)
        {
            _clientSocket = _serverSocket.Accept();
        }
    }
}
