using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Assets.Base.Network.Websocket;
using System;
using System.Text;
using System.Threading.Tasks;

public class BaseWebsocketLite: MonoBehaviour
{
    private const int PING_TIME_OUT = 10000;
	private const int MAX_PING_FAIL = 10;
    private const float PING_INTERVAL = 1f;
    
    private WebSocket _ws  = null;
    private IWsConnectOptions _option = null;

    private event Action<NetData> OnMessageHandler = null;
    public event Action OnReconnected;
    public event Action OnReconnectFail;
    public event Action<float> OnPingUpdated;
    public event Action OnPingFail;

    private bool _start = false;
    private bool _reconnecting = false;

    private float _timePing = 0f;
	private float _timeSend = 0f;
	private int _pingFailCount = 0;
	private float _pingDuration = 0f;
    public float Ping () => _pingDuration;

    public async Task ConnectWs(
        IWsConnectOptions options,
        Action<NetData> onMessageHandler,
        Action onComplete = null,
        Action onError = null
    )
    {
        if(_ws != null && (_ws.State == WebSocketState.Connecting || _ws.State == WebSocketState.Open))
        {
            return;
        }
        
        _option = options;
        OnMessageHandler = onMessageHandler;
        string url = !string.IsNullOrEmpty(options.Url)? options.Url : $"{options.Protocol}://{options.Host}:{options.Port}";

        if((bool)options.UrlToken)
        {
            url += $"?token={options.Token}";
            if(options.Params != null)
            {
                url = FormatUrl(url, options.Params);
            }
        }

        _ws = new WebSocket(url, options.Subprotocols, options.Headers);
        _ws.OnOpen += async ()=>
        {
            _start = true;
            _timePing = 0f;
            _pingDuration = 0f;
            await HandlePing();
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
                onMessageHandler?.Invoke(netData);
            }
        };

        _ws.OnError += (error) =>
        {
            Debug.LogError("WebSocket Error: " + error);
            onError?.Invoke();
        };

        _ws.OnClose += (code) =>
        {
            Debug.Log("WebSocket Closed: " + code);
        };

        await _ws.Connect();
    }

    public async void SendMessage(byte[] buffer)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            await _ws.Send(buffer);
        }
    }

    public async void SendMessageString(string message)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            await _ws.SendText(message);
        }
    }

    public async void CloseWs()
    {
        if (_ws != null)
        {
            await _ws.Close();
            _start = false;
        }
    }

    private async Task HandlePing()
    {
        _pingFailCount = 0;
        while(_start)
        {
            await Task.Delay((int)(PING_INTERVAL * 1000));
            await SendPing();
        }
    }

    private async Task SendPing()
    {
        if (!_start) return;
        _timePing += PING_INTERVAL;
        byte[] data = { 0x9 };
        SendMessage(data);
        _timeSend = Time.time;

        if (_timePing > PING_TIME_OUT)
        {
            _pingDuration = _timePing;
            await OnPingTimeout();
        }
    }

    private void OnReceivePong()
    {
        _timePing = 0;
        _pingDuration = Time.time - _timeSend;
        if (_pingDuration > PING_TIME_OUT)
        {
            Debug.LogWarning("Ping Timeout: " + _pingDuration);
        }
        else
        {
            _pingFailCount = 0;
            OnPingUpdated?.Invoke(_pingDuration);
            Debug.Log("Ping OK: " + _pingDuration);
        }
    }

    private async Task OnPingTimeout()
    {
        Debug.LogWarning("Ping Timeout Count: " + _pingFailCount);
        _pingFailCount++;
        if (_pingFailCount >= MAX_PING_FAIL)
        {
            Debug.LogError("Ping Error - Closing WebSocket");
            _pingFailCount = 0;
            OnPingFail?.Invoke();
            CloseWs();
            if (_option?.AutoReconnect > 0)
            {
                await DoReconnect((int)_option.AutoReconnect);
            }
        }
    }

    private async Task DoReconnect(int maxRetries = 3)
    {
        _reconnecting = true;
        int retries = 0;
        while (retries < maxRetries)
        {
            await Task.Delay(10000);
            await ConnectWs(_option, OnMessageHandler);
            retries++;
            if (_ws.State == WebSocketState.Open)
            {
                Debug.Log("Reconnected successfully");
                return;
            }
        }
        Debug.LogError("Reconnect failed");
        OnReconnectFail?.Invoke();
    }

    private void Update()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            _ws.DispatchMessageQueue();
        #endif
    }

    public async Task StartUpdateLoop()
    {
        #if !UNITY_WEBGL || UNITY_EDITOR
            while (_start)
            {
                int miliseconds = (int)(Time.deltaTime * 1000);
                await Task.Delay(miliseconds); // ~30 FPS (32ms per frame)
                _ws?.DispatchMessageQueue(); // Needed for NativeWebSocket to process messages
            }
        #endif
    }

    private string FormatUrl(string address, Dictionary<string, string> parameters)
    {
        var uriBuilder = new UriBuilder(address);
        var query = new StringBuilder();
        foreach (var param in parameters)
        {
            if (query.Length > 0) query.Append('&');
            query.Append(Uri.EscapeDataString(param.Key)).Append('=').Append(Uri.EscapeDataString(param.Value));
        }
        uriBuilder.Query = query.ToString();
        return uriBuilder.ToString();
    }
}
