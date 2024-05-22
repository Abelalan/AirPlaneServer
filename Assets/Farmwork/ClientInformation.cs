using System.Net.Sockets;


/// <summary>
/// 客户端信息
/// </summary>
public class ClientInformation
{
    public Socket ClientSocket;
    public byte[] Data = new byte[1024];
    public string UserName;
}


