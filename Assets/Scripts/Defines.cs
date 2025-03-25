using UnityEngine;

public static class Defines
{
    public static int PORT = 7790;
    public static int PACKET_HEADER_SIZE = sizeof(int);
    public static int MAX_MESSAGE_BUFFER_SIZE = 1024;
    public static int MAX_CONNECTED_CLIENT_SIZE = 5;
    public static string DB_CONNECTION_STRING = "server=localhost;user=root;database=membership;password=32873287";
}
