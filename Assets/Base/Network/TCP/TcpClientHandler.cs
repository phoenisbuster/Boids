using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class TcpClientHandler<T>
{
    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread receiveThread;

    private readonly string host;
    private readonly int port;
    private readonly int bufferSize = 1024;
    public int BufferSize => bufferSize;

    public delegate void HandleMsgObjectType(T obj);
    public delegate bool HandleMsgBinaryType(byte[] token);

    private bool isQueue = false;
	private readonly List<T> listMessage = new();

    public HandleMsgObjectType OnHandleMessage;
    public HandleMsgBinaryType OnHandleBinaryMessage;

    public TcpClientHandler(string ip, int port)
    {
        host = ip;
        this.port = port;
    }
    
    public TcpClientHandler(string ip, int port, int bufferSize)
    {
        host = ip;
        this.port = port;
        this.bufferSize = bufferSize;
    }

    public void Connect()
    {
        try
        {
            tcpClient = new TcpClient(host, port);
            stream = tcpClient.GetStream();
            receiveThread = new Thread(ReceiveData) { IsBackground = true };
            receiveThread.Start();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Connect Error: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[bufferSize];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if(bytesRead == 0) break;
                
                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);

                OnHandleBinaryMessage?.Invoke(data);
                
                T obj = ByteConverter.ParseByteArrayToJsonObject<T>(data);
                HandleMessage(obj);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Receive Error: " + e.Message);
        }
    }

    private void HandleMessage(T message)
    {
        if(isQueue)
        {
            listMessage.Add(message);
            return;
        }

        isQueue = true;
        OnHandleMessage?.Invoke(message);
        OnHandleQueueList();
    }

    private void OnHandleQueueList() 
	{
		if(!isQueue) return;

		isQueue = false;
		List<T> listMessageTemp = new(listMessage);
		listMessage.Clear();
        foreach(T message in listMessageTemp)
        {
            HandleMessage(message);
        }
	}
}
