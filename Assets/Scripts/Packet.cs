using System;
using System.Text;
using UnityEngine;

public class Packet
{
    byte[] _headerBuffer;
    byte[] _messageBuffer;
    byte[] _packetBuffer;
    int _size;

    public byte[] PacketBuffer => _packetBuffer;

    public Packet(int size)
    {
        _headerBuffer = new byte[Defines.PACKET_HEADER_SIZE];
        _size = size;
        _messageBuffer = new byte[_size - Defines.PACKET_HEADER_SIZE];
        _packetBuffer = new byte[_size];
    }

    public void CreatePacket(string message)
    {
        if (Encoding.UTF8.GetBytes(message).Length > _size - Defines.PACKET_HEADER_SIZE)
        {
            Debug.LogError("message size is bigger than packet size");
            return;
        }
        else
        {
            _messageBuffer = Encoding.UTF8.GetBytes(message);
            _headerBuffer = BitConverter.GetBytes(message.Length);
            Buffer.BlockCopy(_headerBuffer, 0, _packetBuffer, 0, _headerBuffer.Length);
            Buffer.BlockCopy(_messageBuffer, 0, _packetBuffer, Defines.PACKET_HEADER_SIZE, _messageBuffer.Length);
        }
    }

    public string ReadPacket()
    {
        if (null != _packetBuffer)
        {
            int length = BitConverter.ToInt32(_packetBuffer, 0);
            string message = Encoding.UTF8.GetString(_packetBuffer, Defines.PACKET_HEADER_SIZE, length);

            return message;
        }
        return null;
    }
}
