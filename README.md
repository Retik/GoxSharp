GoxSharp
========

A C# Client that connects to MtGox's REST API's


Add this project to an existing project to use the Trade Functions class to invoke the various web services that MtGox provides
This library is currently built for the latest MtGOX V2 API's.  Their documentation is still being built, so this
project will slowly adapt to accommodate their changes.

Here is an example on how to use this library:


using GoxSharp;
using GoxSharp.Models;
using ExtensionMethods;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            TradeFunctions tf = new TradeFunctions();

            Info info = tf.GetInfo();  // your account information.
            IdKey key = tf.GetIdKey();
            Tickers tickers = tf.GetTicker(Currency.USD); // current ticker
            Depth depth = tf.GetDepth(Currency.USD); // get the current depth
            Orders order = tf.GetOrders();  // get all open orders
            OrderResponse resp = tf.CreateOrder(Currency.USD,
                new Order()
                {
                    amount = new Amount(500),
                    price = new Price(80.75)
                });  // order 500 BTC at $80.75 USD
        }
    }
}

You must add this to your web/app config file (in the startup application, not the GoxSharp project) in order to use this library.
You can obtain your API key in the Security Center tab at MtGox and assign it some access rights.

 <appSettings>
    <add key = "MtGoxAPIKey" value="key" />
    <add key = "MtGoxAPISecret" value="secret"/>
  </appSettings>
  
I am currently building this library to be included in various other ideas I have (Windows 8 Application, arbitrage clients)

I am a freelance developer and won't argue against a donation!   19Z7esDYUf7Na4MSM1ps9pUDkNmvBNDLL3

Thanks!



