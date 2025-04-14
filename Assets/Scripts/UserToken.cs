using System.Net.Sockets;
using UnityEngine;

public class UserToken
{
    string _userID;
    Socket _userSocket;

    public string ID { get { return _userID; } }
    public Socket Socket { get { return _userSocket; } }
}
