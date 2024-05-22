using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
/// <summary>
/// 网络管理
/// </summary>
public class NetManager : Singleton<NetManager>
{
    Socket _server;
    public void InitServer()
    {
        _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _server.Bind(new IPEndPoint(IPAddress.Any, 9999));
        _server.Listen(50);

        Debug.Log("服务器开启");
        //开启接受客户端的连接
        _server.BeginAccept(AsynAcceptCall, null);
    }

    private void AsynAcceptCall(IAsyncResult ar)
    {
        //连接上的客户端
        Socket cli = _server.EndAccept(ar);

        Debug.Log($"{cli.RemoteEndPoint} 已上线");
        ClientInformation client = new ClientInformation()
        {
            ClientSocket = cli,
        };
        //接收发送的数据
        client.ClientSocket.BeginReceive(client.Data, 0, client.Data.Length, SocketFlags.None, AsynReceiveCall, client);
        //重启接收连接
        _server.BeginAccept(AsynAcceptCall, null);
    }
    /// <summary>
    /// 接收消西回调
    /// </summary>
    /// <param name="ar"></param>
    private void AsynReceiveCall(IAsyncResult ar)
    {
        ClientInformation client = ar.AsyncState as ClientInformation;
        //接收长度
        int len = client.ClientSocket.EndReceive(ar);
        if (len > 0)
        {
            byte[] buffer = new byte[len];
            Buffer.BlockCopy(client.Data, 0, buffer, 0, len);
            //解决粘包
            while (buffer.Length > 4)
            {
                int bodyLen = BitConverter.ToInt32(buffer, 0);
                //包体
                byte[] body = new byte[bodyLen];
                Buffer.BlockCopy(buffer, 4, body, 0, bodyLen);

                int massageID = BitConverter.ToInt32(body, 0);
                //信息
                byte[] info = new byte[bodyLen - 4];
                Buffer.BlockCopy(body, 4, info, 0, info.Length);

                MassageData data = new MassageData()
                {
                    Client = client,
                    Information = info,
                };

                MassageCenter.GetInstance().Dispatch<MassageData>(massageID, data);
                //计算剩余长度
                int syLen = buffer.Length - bodyLen - 4;

                byte[] syData = new byte[syLen];
                Buffer.BlockCopy(buffer, bodyLen + 4, syData, 0, syLen);

                buffer = syData;
            }

            //接收发送的数据
            client.ClientSocket.BeginReceive(client.Data, 0, client.Data.Length, SocketFlags.None, AsynReceiveCall, client);

        }
        else
        {
            Debug.Log($"{client.ClientSocket.RemoteEndPoint} 已下线");
            client.ClientSocket.Shutdown(SocketShutdown.Both);
            client.ClientSocket.Close();
        }

    }
    /// <summary>
    /// 发送信息到客户端
    /// </summary>
    /// <param name="id"></param>
    /// <param name="data"></param>
    /// <param name="client"></param>
    public void SendDataToClient(int id, byte[] data, ClientInformation client)
    {
        byte[] buffer = new byte[0];
        buffer = buffer.Concat(BitConverter.GetBytes(data.Length + 4)).Concat(BitConverter.GetBytes(id)).Concat(data).ToArray();
        client.ClientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, AsynSendCall, client);
    }

    private void AsynSendCall(IAsyncResult ar)
    {
        ClientInformation client = ar.AsyncState as ClientInformation;
        int len = client.ClientSocket.EndSend(ar);
       // Debug.Log("同步位置信息");
    }
}


