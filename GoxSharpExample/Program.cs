using System;
using System.Text.RegularExpressions;
using GoxSharp.Models;

namespace GoxSharp
{
    class Program
    {
        static void Main()
        {
            try
            {
                TradeFunctions goxClient = new TradeFunctions();
                Console.Write
                (
                    Environment.NewLine +
                    "     1. GetTicker" + Environment.NewLine +
                    "     2. GetDepth" + Environment.NewLine +
                    "     3. GetCurrency" + Environment.NewLine +
                    "     4. GetInfo" + Environment.NewLine +
                    "     5. GetIdKey" + Environment.NewLine +
                    "     6. GetOrders" + Environment.NewLine +
                    Environment.NewLine +
                    "Choose a Trade Function: "
                );

                string userEntry = Console.ReadLine();
                if(string.IsNullOrEmpty(userEntry))
                    throw new Exception("You must make an entry!");

                Match tradeFunction = new Regex(@"[0-9]").Match(userEntry);
                if (!tradeFunction.Success)
                    throw new Exception("You must enter a number!");

                switch (tradeFunction.ToString())
                {
                    case "1":
                        Console.WriteLine(Environment.NewLine + goxClient.GetTicker(Currency.USD).ToString());
                        break;
                    case "2":
                        Console.WriteLine(Environment.NewLine + goxClient.GetOrders().ToString());
                        break;
                    case "3":
                        break;
                    case "4":
                        break;
                    case "5":
                        break;
                    case "6":
                        break;
                }
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
