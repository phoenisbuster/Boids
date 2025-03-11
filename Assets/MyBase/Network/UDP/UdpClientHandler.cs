using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace MyBase.Network.UDP
{
    public class UdpClientHandler<T>
    {
        private UdpClient client;
        private IPEndPoint endPoint;
        private Thread receiveThread;

        private readonly string host;
        private readonly int port;

        public delegate void HandleMsgObjectType(T obj);
        public delegate bool HandleMsgBinaryType(byte[] token);

        private bool isQueue = false;
        private readonly List<T> listMessage = new();

        public HandleMsgObjectType OnHandleMessage;
        public HandleMsgBinaryType OnHandleBinaryMessage;
        private volatile bool isRunning = false;

        public UdpClientHandler(string ip, int port)
        {
            host = ip;
            this.port = port;
        }

        public void Connect()
        {
            try
            {   
                client = new UdpClient(host, port);
                endPoint = new IPEndPoint(IPAddress.Parse(host), port);
                receiveThread = new Thread(ReceiveData) { IsBackground = true };
                receiveThread.Start();
            }
            catch (Exception e)
            {
                Debug.LogError("❌ UDP Connect Error: " + e.Message);
            }
        }

        public void Disconnect()
        {
            receiveThread?.Abort();
            client?.Close();
        }

        void ReceiveData()
        {
            try
            {
                while(true)
                {
                    byte[] data = client?.Receive(ref endPoint);
                    OnHandleBinaryMessage?.Invoke(data);
                    
                    T obj = ByteConverter.ParseByteArrayToJsonObject<T>(data);
                    HandleMessage(obj);
                }
            }
            catch(Exception e)
            {
                Debug.LogError("❌ UDP Receive Error: " + e.Message);
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

}