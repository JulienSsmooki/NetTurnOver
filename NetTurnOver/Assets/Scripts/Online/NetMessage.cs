///////////////////////////////////////////////////////////////////////////////
//                                                                           //
//@Autor : Julien Lopez                                                      //
//@Date : 01/09/2019                                                         //
//@Description : NetMessage.cs                                               //
//               This is an utils file :                                     //
//                   - There are some enum and struct sended througt network.//
//                   - There is a custom serializer class for our custom msg.//
//                                                                           //
///////////////////////////////////////////////////////////////////////////////
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public enum LobbyProto
{
    Unknown,
    Connection,
    Disconnection,
    StartMM,
    IsInRoom,
    Count,
}

[Serializable]
public struct HeadMsg
{
    public HeadMsg(LobbyProto _lobbyProto)
    {
        lobbyProto = _lobbyProto;
    }
    public LobbyProto lobbyProto;
}

[Serializable]
public class NetMessage
{
    public NetMessage(HeadMsg _head, byte[] _body)
    {
        head = _head;
        body = _body;
    }
    public HeadMsg head;
    public byte[] body;
}

public static class SerializeUtils
{
    public static byte[] SerializeMsg(NetMessage message)
    {
        if (message == null)
        {
            return null;
        }
        BinaryFormatter binForm = new BinaryFormatter();
        using (MemoryStream memStream = new MemoryStream())
        {
            binForm.Serialize(memStream, message);
            return memStream.ToArray();
        }
    }

    public static NetMessage DeserializeMsg(byte[] rawMessage)
    {
        if (rawMessage == null)
        {
            return null;
        }
        using (MemoryStream memStream = new MemoryStream())
        {
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(rawMessage, 0, rawMessage.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            NetMessage msg = (NetMessage)binForm.Deserialize(memStream);
            return msg;
        }
    }
}