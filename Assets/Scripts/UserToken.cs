using System.Net.Sockets;
using UnityEngine;

public class UserToken
{
    string _userID;
    Socket _userSocket;

    public string UserID { get { return _userID; } set { _userID = value; } }
    public Socket Socket { get { return _userSocket; } set { _userSocket = value; } }
}
