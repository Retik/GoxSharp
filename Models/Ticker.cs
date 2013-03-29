using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace GoxSharp.Models
{
    public class Tickers
    {
        public Dictionary<TickerType, Ticker> tickers { get; set; }
        public Tickers(JToken jsonObj)
        {
            tickers = new Dictionary<TickerType, Ticker>();
            string[] tickerNames = { "avg", "high", "low", "vwap", "last_all", "last_local", "last_orig", "last", "buy", "sell", "vol" };
            foreach (string tickName in tickerNames)
            {
                if (jsonObj.Count() == 2)  //we have the REST API schema
                {
                    Ticker tick = new Ticker(jsonObj["data"][tickName], tickName);
                    tickers[tick.tickerType] = tick;

                }
                else // we have the socketIO schema
                {
                    JToken token = jsonObj[tickName];
                    Ticker tick = new Ticker(token, tickName);
                    tickers[tick.tickerType] = tick;
                }
            }
        }

        public override string ToString()
        {
            return string.Format(
                    "Average:{0}" + Environment.NewLine +
                    "High:{1}" + Environment.NewLine +
                    "Low:{2}" + Environment.NewLine +
                    "Vwap:{3}" + Environment.NewLine +
                    "Last_All:{4}" + Environment.NewLine +
                    "Last_Orig:{5}" + Environment.NewLine +
                    "Last:{6}" + Environment.NewLine +
                    "Buy:{7}" + Environment.NewLine +
                    "Sell:{8}" + Environment.NewLine + 
                    "Volume:{9}",
                    tickers[TickerType.Avg].display, tickers[TickerType.High].display, tickers[TickerType.Low].display, tickers[TickerType.Vwap].display,
                    tickers[TickerType.Last_all].display,
                    tickers[TickerType.Last_orig].display, tickers[TickerType.Last].display, tickers[TickerType.Buy].display,
                    tickers[TickerType.Sell].display, tickers[TickerType.Vol].display);
        }
    }

    public class Ticker
    {
        public Currency currency { get; set; }
        public TickerType tickerType { get; set; }
        public Decimal value { get; set; }
        public Int64 value_int { get; set; }
        public string display { get; set; }
        public string display_short { get; set; }
        public DateTime update_ts { get; set; }

        public Ticker(JToken token, string type)
        {
            tickerType = getTickerType(type);
            value = Decimal.Parse(token["value"].Value<String>());
            value_int = Int64.Parse(token["value_int"].Value<String>());
            currency = getCurrency(token["currency"].Value<String>());
            display = token["display"].Value<String>();
            display_short = token["display_short"].Value<String>();
            update_ts = DateTime.Now;
        }


        public TickerType getTickerType(String name)
        {
            TickerType result = new TickerType();
            switch (name)
            {
                case "avg":
                    result = TickerType.Avg;
                    break;
                case "high":
                    result = TickerType.High;
                    break;
                case "low":
                    result = TickerType.Low;
                    break;
                case "vwap":
                    result = TickerType.Vwap;
                    break;
                case "last_all":
                    result = TickerType.Last_all;
                    break;
                case "last_local":
                    result = TickerType.Last_local;
                    break;
                case "last_orig":
                    result = TickerType.Last_orig;
                    break;
                case "last":
                    result = TickerType.Last;
                    break;
                case "buy":
                    result = TickerType.Buy;
                    break;
                case "sell":
                    result = TickerType.Sell;
                    break;
                case "vol":
                    result = TickerType.Vol;
                    break;


            }
            return result;
        }

        public Currency getCurrency(String currencyString)
        {
            Currency result = new Currency();
            switch (currencyString)
            {
                case "USD":
                    result = Currency.USD;
                    break;
                case "BTC":
                    result = Currency.BTC;
                    break;
            }

            return result;
        }
    }

    public enum TickerType
    {
        Avg, High, Low, Vwap, Last_all, Last_local, Last_orig, Last, Buy, Sell, Vol
    }
    public enum Currency
    {
        BTC, USD, AUD, CAD, CHF, CNY, DKK, EUR, GBP, HKD, JPY, NZD, PLN, RUB, SEK, SGD, THB, None
    }
}
