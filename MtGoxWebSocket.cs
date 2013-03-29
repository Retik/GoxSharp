using System;
using SocketIOClient;

namespace GoxSharp
{
    public class MtGoxWebSocketClient
    {
        private const string MTGOXSOCKETURL = @"https://socketio.mtgox.com";
        private readonly Client _client;

        public MtGoxWebSocketClient()
        {
            _client = new Client(MTGOXSOCKETURL);
            _client.Error += _client_Error;
            _client.Message += _client_Message;
            _client.SocketConnectionClosed += _client_SocketConnectionClosed;

            _client.On("subscribe", data => SetStatus("Subscribed to channel: " + data.Json.ToJsonString() + Environment.NewLine));
            _client.On("channel", data => SetStatus("Ticker: " + data.Json.ToJsonString() + Environment.NewLine));
            _client.On("connect", data => SetStatus("Connected to the socket.." + Environment.NewLine));

            _client.Connect("/mtgox");
        }

        private void SetStatus(String status)
        {
            Console.Out.WriteLine(status);
        }

        void _client_SocketConnectionClosed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void _client_Message(object sender, MessageEventArgs e)
        {
            throw new NotImplementedException();
        }

        void _client_Error(object sender, ErrorEventArgs e)
        {
            throw new NotImplementedException();
        }

    }
}
