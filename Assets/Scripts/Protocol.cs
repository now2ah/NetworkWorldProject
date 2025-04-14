using System;
using UnityEngine;

#region CLIENT

[Serializable]
public class ClientConnectUser
{
    public string id;
}

[Serializable]
public class ClientSendChat
{
    public string message;
}

#endregion


#region SERVER

[Serializable]
public class ServerConnectUser
{
    public string id;
}

[Serializable]
public class ServerSendChat
{
    public string id;
    public string message;
}

#endregion

