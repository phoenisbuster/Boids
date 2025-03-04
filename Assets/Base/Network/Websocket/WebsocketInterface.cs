using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace MyWebsocket
{
    public class IWsConnectOptions
    {
        public string Token { get; set; }
        public Dictionary<string, string> Params { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public List<string> Subprotocols { get; set; } = new List<string>();

        public string Host { get; set; }
        public int? Port { get; set; }
        public string Protocol { get; set; }
        public string Url { get; set; }
        public int? AutoReconnect { get; set; } // Nullable int to support -1
        public string ChannelId { get; set; }
        public bool? UrlToken { get; set; }
        public string NativeUrl { get; set; } // For Android

        public IWsConnectOptions() 
        { 
            Params = new Dictionary<string, string>();
            Headers = new Dictionary<string, string>();
            Subprotocols = new List<string>();

            AutoReconnect = -1;
        }

        public static WsConnectOptionsBuilder Builder() => new WsConnectOptionsBuilder();
    }

    public class WsConnectOptionsBuilder
    {
        private readonly IWsConnectOptions _options = new();

        public WsConnectOptionsBuilder SetToken(string token)
        {
            _options.Token = token;
            return this;
        }

        public WsConnectOptionsBuilder SetParams(Dictionary<string, string> parameters)
        {
            _options.Params = parameters;
            return this;
        }

        public WsConnectOptionsBuilder AddParam(string key, string value)
        {
            _options.Params[key] = value;
            return this;
        }

        public WsConnectOptionsBuilder SetHost(string host)
        {
            _options.Host = host;
            return this;
        }

        public WsConnectOptionsBuilder SetPort(int port)
        {
            _options.Port = port;
            return this;
        }

        public WsConnectOptionsBuilder SetProtocol(string protocol)
        {
            _options.Protocol = protocol;
            return this;
        }

        public WsConnectOptionsBuilder SetUrl(string url)
        {
            _options.Url = url;
            return this;
        }

        public WsConnectOptionsBuilder SetAutoReconnect(int autoReconnect)
        {
            _options.AutoReconnect = autoReconnect;
            return this;
        }

        public WsConnectOptionsBuilder SetChannelId(string channelId)
        {
            _options.ChannelId = channelId;
            return this;
        }

        public WsConnectOptionsBuilder SetUrlToken(bool urlToken)
        {
            _options.UrlToken = urlToken;
            return this;
        }

        public WsConnectOptionsBuilder SetNativeUrl(string nativeUrl)
        {
            _options.NativeUrl = nativeUrl;
            return this;
        }

        public IWsConnectOptions Build()
        {
            return _options;
        }
    }

    public enum CodeSocket
    {
        FORCE_CLOSE = 4999,
        KOTIC = 1001,
        NORMAL_CLOSURE = 1000,
        RETRY = 3006,
    }

    public class WebsocketEventKey
    {
        public static string ON_RECONNECTED 		= "ON_RECONNECTED_T";
        public static string ON_DISCONNECT 		    = "ON_DISCONNECT_T";
        public static string ON_PING_FAIL 			= "ON_PING_FAIL_T";
        public static string ON_PING_OK 			= "ON_PING_OK_T";
        public static string ON_RECONNECT_FAILED 	= "ON_RECONNECT_FAILED_T";
    }

    public class NetData
    {
        private readonly byte[] _data; // Read-only field for immutability
        public byte[] Data => _data; // Public read-only access
        
        public NetData(byte[] data)
        {
            _data = data ?? throw new ArgumentNullException(nameof(data), "Data cannot be null.");
        }

        // Factory methods for different types of data
        public static NetData FromBytes(byte[] value) => new NetData(value);
        public static NetData FromMemoryStream(MemoryStream value) => new NetData(value.ToArray());
        public static NetData FromSpan(Span<byte> value) => new NetData(value.ToArray()); // Converts Span to byte[]
        public static NetData FromMemory(Memory<byte> value) => new NetData(value.ToArray()); // Converts Memory to byte[

        // Conversion methods
        public override string ToString()
        {
            return _data.Length > 0 ? System.Text.Encoding.UTF8.GetString(_data) : string.Empty;
        }
        public byte[] ToBytes() => _data;
        public MemoryStream ToMemoryStream() => new MemoryStream(_data);
        public Span<byte> ToSpan() => _data;
        public Memory<byte> ToMemory() => _data;
        public int Length => _data.Length;
    }

    public delegate void NetCallFunc(int cmd, object data);

    public class CallbackObject
    {
        public object Target { get; set; } // The callback target
        public NetCallFunc Callback { get; set; } // The callback function

        public CallbackObject(object target, NetCallFunc callback)
        {
            Target = target;
            Callback = callback;
        }

        public void Invoke(int cmd, object data)
        {
            Callback?.Invoke(cmd, data);
        }
    }

    public class RequestObject
    {
        public NetData Buffer { get; set; } // Request buffer
        public int RspCmd { get; set; } // Response command ID
        public CallbackObject RspObject { get; set; } // Response callback

        public RequestObject(NetData buffer, int rspCmd, CallbackObject rspObject)
        {
            Buffer = buffer;
            RspCmd = rspCmd;
            RspObject = rspObject;
        }

        public void ExecuteCallback()
        {
            RspObject?.Invoke(RspCmd, Buffer);
        }
    }

    public interface ISocket
    {
        // Connection events
        event Action<object> OnConnected;
        event Action<NetData> OnMessage;
        event Action<object> OnError;
        event Action<object> OnClosed;

        // Methods
        void Connect(object options);  // Connect to the server
        void Send(NetData buffer);     // Send data
        void Close(int code = 1000, string reason = "Normal Closure"); // Close connection
    }

    public class SocketUtils
    {
        public static string GetUniqueId()
        {
            long d = DateTime.UtcNow.Ticks; // Timestamp in ticks
            long d2 = System.Diagnostics.Stopwatch.IsHighResolution 
                    ? System.Diagnostics.Stopwatch.GetTimestamp() 
                    : 0; // High-precision timer if supported

            return Guid.NewGuid().ToString("N"); // Generates a 32-character unique ID
        }
    }
}