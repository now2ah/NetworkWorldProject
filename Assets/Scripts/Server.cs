using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Collections.Generic;

public class Server : MonoBehaviour
{
    string _localHostName;
    IPAddress _localIPAddress;
    IPEndPoint _localEndPoint;
    Socket _listeningSocket;
    bool _isBind = false;
    bool _isListening = false;

    List<Socket> _clientsSocketList;
    List<Socket> _checkReadList;        //select Àü¿ë

    public int backlog;

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

    public bool Initialize()
    {
        _localHostName = Dns.GetHostName();
        IPHostEntry ipHostEntry = Dns.GetHostEntry(_localHostName);
        _localIPAddress = ipHostEntry.AddressList[0];       //need to add multi address
        _localEndPoint = new IPEndPoint(_localIPAddress, Defines.PORT);

        _clientsSocketList = new List<Socket>();

        _listeningSocket = _CreateServerSocket();

        return _listeningSocket != null ? true : false;
    }

    public void Bind()
    {
        if (null == _localEndPoint) { _isBind = _Bind(_localEndPoint); }
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

            _clientsSocketList.Add(_listeningSocket);
        }
        catch (SocketException e)
        {
            Debug.LogError(e.Message);
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
        if (_checkReadList.Count > 0)
        {
            _checkReadList = new List<Socket>(clientSocketList);

            if (null == _listeningSocket)
                return false;

            Socket.Select(_checkReadList, null, null, 10);

            foreach (var checkedSocket in _checkReadList)
            {

            }
        }
        return true;
    }

}
