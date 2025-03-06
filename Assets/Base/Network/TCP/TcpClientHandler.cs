using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class TcpClientHandler<T>
{
    private const int MIN_DISPATCH_INTERVAL = 16; // 16ms = 60fps

    private TcpClient tcpClient;
    private NetworkStream stream;
    private Thread receiveThread;

    private readonly string host;
    private readonly int port;
    private readonly int bufferSize = 1024;
    public int BufferSize => bufferSize;
    public bool IsConnected => tcpClient?.Connected ?? false;
    private volatile bool isRunning = false;

    public delegate void HandleMsgObjectType(T obj);
    public delegate bool HandleMsgBinaryType(byte[] token);

    private readonly List<byte[]> messageQueue = new();
    private readonly object IncomingMessageLock = new object();

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
            if(IsConnected || isRunning) return;
            
            tcpClient = new TcpClient(host, port);
            stream = tcpClient.GetStream();
            isRunning = true;
            receiveThread = new Thread(ReceiveData) { IsBackground = true };
            receiveThread.Start();

            _ = DispatchQueueAsync();
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Connect Error: " + e.Message);
        }
    }

    public void Disconnect()
    {
        if (!isRunning) return;

        isRunning = false;
        try
        {
            stream?.Close();
            tcpClient?.Close();
            receiveThread?.Join(1000); // Wait up to 1 second for thread to exit
            Debug.Log("✅ TCP Disconnected");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ TCP Disconnect Error: {e.Message}");
        }
    }

    public void Send(byte[] data)
    {
        if (!IsConnected || stream == null)
        {
            Debug.LogWarning("Cannot send data: Not connected.");
            return;
        }

        try
        {
            lock (stream) // Thread-safe write
            {
                stream.Write(data, 0, data.Length);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Send Error: " + e.Message);
        }
    }

    void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[bufferSize];
            while (isRunning)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if(bytesRead == 0) break;
                
                byte[] data = new byte[bytesRead];
                Array.Copy(buffer, data, bytesRead);

                try
                {
                    lock (IncomingMessageLock)
                    {
                        messageQueue.Add(data);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"❌ Parse Error: {e.Message}");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Receive Error: " + e.Message);
        }
        finally
        {
            Disconnect(); // Auto-disconnect on thread exit
        }
    }

    public void DispatchMessageQueue()
    {
        if (messageQueue.Count > 0)
        {
            List<byte[]> list;
            lock (IncomingMessageLock)
            {
                list = new List<byte[]>(messageQueue);
                messageQueue.Clear();
            }

            int count = list.Count;
            for (int i = 0; i < count; i++)
            {
                OnHandleBinaryMessage?.Invoke(list[i]);
                T obj = ByteConverter.ParseByteArrayToJsonObject<T>(list[i]);
                if (obj != null)
                {
                    OnHandleMessage?.Invoke(obj);
                }
            }
        }
    }

    private async Task DispatchQueueAsync()
    {
        while(isRunning)
        {
            DispatchMessageQueue();

            int delay = (int)(Time.unscaledDeltaTime * 1000f);
            if (delay < MIN_DISPATCH_INTERVAL) delay = MIN_DISPATCH_INTERVAL;
            await Task.Delay(delay);
        }
    }
}
