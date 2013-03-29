using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketIOClient;

namespace GoxSharp
{
    public class MtGoxWebSocket
    {
        Client _socketClient = null;
        public MtGoxWebSocket()
        {
            _socketClient = new Client("https://socketio.mtgox.com/mtgox");

        }

        public Client socketClient
        {
            get { return this._socketClient; }
        }

    }
}
