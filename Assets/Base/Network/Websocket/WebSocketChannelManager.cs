using System;
using System.Collections;
using System.Collections.Generic;
using MyWebsocket;
using UnityEngine;

public class WebSocketChannelManager
{
    private static WebSocketChannelManager _instance;
    public static WebSocketChannelManager Instance() => _instance ??= new WebSocketChannelManager();

    private Dictionary<string, BaseWebsocketLite> _channels = new Dictionary<string, BaseWebsocketLite>();

    public void AddChannel(string channelId, BaseWebsocketLite channel) => _channels.Add(channelId, channel);
    public void RemoveChannel(string channelId) => _channels.Remove(channelId);

    public Action<string, string, object> ChannelEvent;

    public void Connect(
        IWsConnectOptions options,
        Action<string, NetData> onMessageHandler,
        Action onComplete = null,
        Action<string> onError = null
    )
    {
        string channelId = options.ChannelId;
        if (_channels.TryGetValue(channelId, out BaseWebsocketLite channel))
        {
            channel.OnDisconnected += ()=>
            {
                ChannelEvent?.Invoke(channelId, WebsocketEventKey.ON_DISCONNECT, null);
            };

            channel.OnReconnected += ()=>
            {
                ChannelEvent?.Invoke(channelId, WebsocketEventKey.ON_RECONNECTED, null);
            };

            channel.OnReconnectFail += ()=>
            {
                ChannelEvent?.Invoke(channelId, WebsocketEventKey.ON_RECONNECT_FAILED, null);
            };

            channel.OnPinged += (ping)=>
            {
                ChannelEvent?.Invoke(channelId, WebsocketEventKey.ON_PING_OK, ping);
            };
            
            _ = channel.ConnectWs(options, onMessageHandler, onComplete, onError);
        }
    }

    public void Send(NetData buffer, string channelId)
    {
        if(_channels.TryGetValue(channelId, out BaseWebsocketLite channel))
        {
            _ = channel.SendMessage(buffer);
        }
    }

    public void SendText(string message, string channelId)
    {
        if(_channels.TryGetValue(channelId, out BaseWebsocketLite channel))
        {
            _ = channel.SendMessageString(message);
        }
    }

    public void Close(string channelId)
    {
        if(_channels.TryGetValue(channelId, out BaseWebsocketLite channel))
        {
            _ = channel.CloseWs();
        }
    }

    public void CloseAll()
    {
        foreach (var channel in _channels.Values)
        {
            _ = channel.CloseWs();
        }
    }

    public void Clear() => _channels.Clear();
    public int TotalChannelCount => _channels.Count;
    public BaseWebsocketLite GetChannel(string channelId) => _channels[channelId];
}
