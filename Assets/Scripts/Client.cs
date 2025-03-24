using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    string _remoteAddress;
    Socket _socket;

    private void OnDisable()
    {
        if ( _socket != null )
        {
            _socket.Close();
        }
    }

    public void StartClient(string address = null)
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
    }

    public void Send(string message)
    {
        if (_socket.Connected)
        {
            Packet packet = new Packet(Defines.MAX_MESSAGE_BUFFER_SIZE);
            packet.CreatePacket(message);
            _socket.Send(packet.PacketBuffer);
        }
    }
}
