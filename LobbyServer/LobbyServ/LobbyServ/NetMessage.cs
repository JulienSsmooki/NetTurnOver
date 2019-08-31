using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public enum LobbyProto
{
    Unknown,
    Connection,
    Disconnection,
    StartMM,
    IsInRoom,
    Count,
}

[System.Serializable]
public struct HeadMsg
{
    public LobbyProto lobbyProto;
}

[System.Serializable]
public class NetMessage
{
    public HeadMsg head;
    public byte[] body;
}

public class SerializeUtils
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