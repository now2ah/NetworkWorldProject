using System;
using System.Text;
using UnityEngine;

public class Packet
{
    byte[] _headerBuffer;
    byte[] _msgTypeBuffer;
    byte[] _messageBuffer;
    byte[] _packetBuffer;
    int _packetSize;

    public byte[] PacketBuffer => _packetBuffer;

    public Packet(int size)
    {
        _packetSize = size;
        _headerBuffer = new byte[Defines.PACKET_HEADER_SIZE];
        _msgTypeBuffer = new byte[Defines.PACKET_MSGTYPE_SIZE];
        _messageBuffer = new byte[_packetSize - Defines.PACKET_HEADER_SIZE - Defines.PACKET_MSGTYPE_SIZE];
        _packetBuffer = new byte[_packetSize];
    }

    public void CreatePacket(string message, Defines.EMessageType type)
    {
        if (Encoding.UTF8.GetBytes(message).Length > _packetSize - Defines.PACKET_HEADER_SIZE - Defines.PACKET_MSGTYPE_SIZE)
        {
            Debug.LogError("message size is bigger than packet size");
            return;
        }
        else
        {
            _messageBuffer = Encoding.UTF8.GetBytes(message);
            _headerBuffer = BitConverter.GetBytes(message.Length);
            _msgTypeBuffer = BitConverter.GetBytes((int)type);
            Buffer.BlockCopy(_headerBuffer, 0, _packetBuffer, 0, _headerBuffer.Length);
            Buffer.BlockCopy(_msgTypeBuffer, 0, _packetBuffer, Defines.PACKET_HEADER_SIZE, _msgTypeBuffer.Length);
            Buffer.BlockCopy(_messageBuffer, 0, _packetBuffer, Defines.PACKET_HEADER_SIZE + Defines.PACKET_MSGTYPE_SIZE, _messageBuffer.Length);
        }
    }

    public string ReadPacket()
    {
        if (null != _packetBuffer)
        {
            int length = BitConverter.ToInt32(_packetBuffer, 0);
            int type = BitConverter.ToInt32(_packetBuffer, Defines.PACKET_HEADER_SIZE);
            string message = null;

            if ((Defines.EMessageType)type == Defines.EMessageType.CONNECT_USER)
            {
                message = Encoding.UTF8.GetString(_packetBuffer, Defines.PACKET_HEADER_SIZE + Defines.PACKET_MSGTYPE_SIZE, length);
            }
            else if ((Defines.EMessageType)type == Defines.EMessageType.SEND_CHAT)
            {
                message = Encoding.UTF8.GetString(_packetBuffer, Defines.PACKET_HEADER_SIZE + Defines.PACKET_MSGTYPE_SIZE, length);
            }
            return message;
        }
        return null;
    }
}
