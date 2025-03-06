using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System;
using System.Text;
using System.Threading.Tasks;

namespace MyBase.Network.MyWebsocket
{

/// <summary>
/// A lightweight WebSocket handler for managing a single connection with ping/pong functionality,
/// automatic reconnection on timeout, and message handling. Uses NativeWebSocket for cross-platform support.
/// </summary>
public class BaseWebsocketLite
{
    // Constants
    private const int PING_TIME_OUT = 10000;
    private const int MAX_PING_FAIL = 10;
    private const int PING_INTERVAL = 1000;
    private const int MIN_DISPATCH_INTERVAL = 16; // 16ms = 60fps

    // Properties
    private bool _start = false;
    private bool _reconnecting = false;
    private float _timePing = 0f;
    private float _timeSend = 0f;
    private int _pingFailCount = 0;
    private float _pingDuration = 0f;

    // Fields
    private WebSocket _ws  = null;
    private IWsConnectOptions _option = null;
    private event Action<string, NetData> OnMessageHandler = null;

    // Events
    /// <summary>
    /// Occurs when the WebSocket successfully reconnects after a failure.
    /// </summary>
    public event Action OnReconnected;

    /// <summary>
    /// Occurs when the WebSocket connection is closed / disconnected.
    /// </summary>
    public event Action OnDisconnected;

    /// <summary>
    /// Occurs when all reconnection attempts fail.
    /// </summary>
    public event Action OnReconnectFail;

    /// <summary>
    /// Occurs when a pong is received successfully, providing the ping duration in milliseconds.
    /// </summary>
    public event Action<float> OnPinged;

    /// <summary>
    /// Gets the last measured ping duration in milliseconds.
    /// </summary>
    public float PingInMs => _pingDuration;

    /// <summary>
    /// Gets the last measured ping duration in seconds.
    /// </summary>
    public float PingInSecond => _pingDuration / 1000f;

    /// <summary>
    /// Gets a value indicating whether the WebSocket is connected.
    /// </summary>
    public bool IsConnected => _ws != null && _ws.State == WebSocketState.Open;

    /// <summary>
    /// Gets a value indicating whether the WebSocket is reconnecting.
    /// </summary>
    public bool IsReconnecting => _reconnecting;

    /// <summary>
    /// Gets the channel ID associated with the WebSocket connection.
    /// </summary>
    public string ChannelId => _option.ChannelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseWebsocketLite"/> class and starts the dispatch loop.
    /// </summary>
    public BaseWebsocketLite() {}

    public BaseWebsocketLite(
        Action _onDisconnected = null,
        Action _onReconnected = null,
        Action _onReconnectFail = null,
        Action<float> _onPinged = null
    )
    {
        OnDisconnected += _onDisconnected;
        OnReconnected += _onReconnected;
        OnReconnectFail += _onReconnectFail;
        OnPinged += _onPinged;
    }

    /// <summary>
    /// Asynchronously connects to a WebSocket server using the specified options.
    /// </summary>
    /// <param name="options">The connection options, such as URL, token, and reconnection settings.</param>
    /// <param name="onComplete">Callback invoked when the connection is established.</param>
    /// <param name="onMessage">Callback invoked when a message is received.</param>
    /// <param name="onError">Optional callback invoked if an error occurs during connection.</param>
    public async Task ConnectWs(
        IWsConnectOptions options,
        Action<string, NetData> onMessageHandler,
        Action onComplete = null,
        Action<string> onError = null
    )
    {
        if(_ws != null && (_ws.State == WebSocketState.Connecting || _ws.State == WebSocketState.Open))
        {
            return;
        }
        
        _option = options;
        OnMessageHandler = onMessageHandler;
        string url = SocketUtils.BuildUrl(options);
        _ws = new WebSocket(url, options.Subprotocols, options.Headers);
        
        _ws.OnOpen += ()=>
        {
            _start = true;
            _timePing = 0f;
            _pingDuration = 0f;
            _ = HandlePing();
            _ = DispatchQueueAsync();
            onComplete?.Invoke();

            if(_reconnecting)
            {
                _reconnecting = false;
                OnReconnected?.Invoke();
            }
        };

        _ws.OnMessage += (data) =>
        {
            if(data.Length == 1 && data[0] == 0xA)
            {
                OnReceivePong();
            }
            else if(data.Length == 1)
            {
                Debug.LogError("WebSocket On Receive St Else: " + data);
            }
            else
            {
                NetData netData = NetData.FromBytes(data);
                onMessageHandler?.Invoke(options.ChannelId,netData);
            }
        };

        _ws.OnError += (error) =>
        {
            Debug.LogError("WebSocket Error: " + error);
            onError?.Invoke(error);
        };

        _ws.OnClose += (code) =>
        {
            Debug.Log("WebSocket Closed: " + code);
            ResetWs();
        };

        await _ws.Connect();
    }

    /// <summary>
    /// Asynchronously sends data over the WebSocket connection.
    /// </summary>
    /// <param name="buffer">The data to send.</param>
    /// <returns>A task that resolves to <c>true</c> if the send succeeds, <c>false</c> otherwise.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="buffer"/> is null.</exception>
    public async Task<bool> SendMessage(NetData buffer)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            try
            {
                await _ws.Send(buffer.Data);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("WebSocket Send Error: " + e.Message);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Asynchronously sends a text message over the WebSocket connection.
    /// </summary>
    /// <param name="message">The text message to send.</param>
    /// <returns>A task that resolves to <c>true</c> if the send succeeds, <c>false</c> otherwise.</returns>
    public async Task<bool> SendMessageString(string message)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            try
            {
                await _ws.SendText(message);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("WebSocket Send Text Error: " + e.Message);
                return false;
            }
        }
        return false;
    }

    /// <summary>
    /// Disposes of the WebSocket handler, closing the connection and stopping background tasks.
    /// </summary>
    public void Dispose()
    {
        _ = CloseWs();
        OnMessageHandler = null;
        OnReconnected = null;
        OnReconnectFail = null;
        OnDisconnected = null;
        OnPinged = null;
    }

    /// <summary>
    /// Asynchronously closes the WebSocket connection.
    /// </summary>
    /// <returns>A task that completes when the connection is closed.</returns>
    public async Task<bool> CloseWs()
    {
        if (_ws == null || _ws.State == WebSocketState.Closed) return false;
        try
        {
            await _ws.Close();
            ResetWs();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError("WebSocket Close Error: " + e.Message);
            return false;
        }
    }

    /// <summary>
    /// Resets the WebSocket state, stopping ping and clearing the connection.
    /// </summary>
    private void ResetWs()
    {
        _start = false;
        _ws = null;
    }

    /// <summary>
    /// Asynchronously handles periodic ping requests to maintain the connection.
    /// </summary>
    /// <returns>A task that runs until the connection is stopped.</returns>
    private async Task HandlePing()
    {
        _pingFailCount = 0;
        while(_start)
        {
            await Task.Delay(PING_INTERVAL);
            SendPing();
        }
    }

    /// <summary>
    /// Asynchronously sends a ping message and updates timing.
    /// </summary>
    /// <param name="dt">The time delta in seconds since the last ping.</param>
    /// <returns>A task that completes when the ping is sent.</returns>
    private void SendPing()
    {
        if (!_start) return;
        _timePing += PING_INTERVAL;
        byte[] data = { 0x9 };
        _ = SendMessage(NetData.FromBytes(data));
        _timeSend = Time.realtimeSinceStartup * 1000f;
        if (_timePing > PING_TIME_OUT)
        {
            _pingDuration = _timePing;
            OnPingTimeout();
        }
    }

    /// <summary>
    /// Handles the receipt of a pong message, calculating ping duration and firing events.
    /// </summary>
    private void OnReceivePong()
    {
        _timePing = 0f;
        _pingDuration = (Time.realtimeSinceStartup * 1000f) - _timeSend;
        if (_pingDuration > PING_TIME_OUT)
        {
            Debug.LogWarning("Ping Timeout: " + _pingDuration);
        }
        else
        {
            _pingFailCount = 0;
            OnPinged?.Invoke(_pingDuration);
            Debug.Log("Ping OK: " + _pingDuration);
        }
    }

    /// <summary>
    /// Handles a ping timeout, closing the connection if the failure limit is reached.
    /// </summary>
    private void OnPingTimeout()
    {
        Debug.LogWarning("Ping Timeout Count: " + _pingFailCount);
        _pingFailCount++;
        if (_pingFailCount >= MAX_PING_FAIL)
        {
            Debug.LogError("Ping Error - Closing WebSocket");
            _pingFailCount = 0;
            OnDisconnected?.Invoke();
            _ = CloseWs();
            if (_option?.AutoReconnect > 0)
            {
                _ = DoReconnect(_option.AutoReconnect.Value);
            }
        }
    }

    /// <summary>
    /// Asynchronously attempts to reconnect to the WebSocket server after a failure.
    /// </summary>
    /// <param name="maxRetries">The maximum number of reconnection attempts.</param>
    /// <returns>A task that runs until reconnection succeeds or fails completely.</returns>
    private async Task DoReconnect(int maxRetries = 3)
    {
        _reconnecting = true;
        int retries = 0;
        while (retries < maxRetries && !_start)
        {
            await Task.Delay(10000);
            await ConnectWs(_option, OnMessageHandler);
            if (_ws.State == WebSocketState.Open)
            {
                Debug.Log("Reconnected successfully");
                return;
            }
            retries++;
        }
        Debug.LogError("Reconnect failed");
        OnReconnectFail?.Invoke();
    }

    /// <summary>
    /// Asynchronously dispatches the NativeWebSocket message queue to process events.
    /// </summary>
    /// <returns>A task that runs indefinitely until the object is disposed.</returns>
    private async Task DispatchQueueAsync()
    {
        while(_start)
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                _ws?.DispatchMessageQueue();
            #endif

            int delay = (int)(Time.unscaledDeltaTime * 1000f);
            if (delay < MIN_DISPATCH_INTERVAL) delay = MIN_DISPATCH_INTERVAL;
            await Task.Delay(delay);
        }
    }
}

}