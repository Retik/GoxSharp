using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GoxSharp.Models
{
    public class Order
    {
        public Guid oid { get; set; }
        public Currency currency { get; set; }
        public OrderType type { get; set; }
        public OrderItem item { get; set; }
        public Amount amount { get; set; }
        public Price price { get; set; }
        public DateTime stamp { get; set; }
        public Amount effective_amount { get; set; }
        public OrderStatus status { get; set; }
        public DateTime date { get; set; }
        public Int64 priority { get; set; }
        public decimal _volume { get; set; }
        public decimal _price { get; set; }
        public string actions { get; set; }

        public Order()
        {
        }
        public Order(Price price, Amount amount)
        {
            this.price = price;
            this.amount = amount;
        }
        public Order(JToken token, string orderType)
        {
            type = getOrderType(orderType);
            Amount _Amount = new Amount{ value = token["amount"].Value<Decimal>(), value_int = token["amount_int"].Value<Int64>() };
            Price _Price = new Price { value = token["price"].Value<Decimal>(), value_int = token["price_int"].Value<Int64>() };
            _price = _Price.value;
            _volume = _Amount.value;
            stamp = new DateTime((token["stamp"].Value<long>() * 10) + 621355968000000000);
        }

        public static OrderType getOrderType(String type)
        {
            switch (type)
            {
                case "asks":
                    return OrderType.Ask;
                case "bids":
                    return OrderType.Bid;
                case "market":
                    return OrderType.Market;
                case "ask":
                    return OrderType.Ask;
                case "bid":
                    return OrderType.Bid;
                default:
                    return OrderType.None;
            }
        }

        public override string ToString()
        {
            return string.Format("OrderID:{0}, Currency:{1}, OrderType:{2}, OrderItem:{3}, Amount:{4}, Price:{5}, TimeStamp:{6}, " +
                                 "Effective_Amount:{7}, OrderStatus:{8}, Date:{9}, Priority:{10}, Volume:{11}, Price:{12}, Actions:{13}",
                oid, currency, type, item, amount.display, price.display, stamp.ToString("HH:mm"), effective_amount.display, status, date.ToString("MM/dd/yyyy"), 
                priority, _volume, _price, actions);
        }
    }

    public class OrderResponse
    {
        public string status { get; set; }
        public string orderId { get; set; }

        public OrderResponse(dynamic jsonObj)
        {
            JToken result = null;
            JToken id = null;
            jsonObj.TryGetValue("result", out result);
            jsonObj.TryGetValue("data", out id);

            this.status = result.Value<String>();
            this.orderId = id.Value<String>();
        }
    }
    public class OrderCancelResponse
    {
        public string status { get; set; }
        public Guid oid { get; set; }
        public Guid qid { get; set; }

        public OrderCancelResponse(dynamic jsonObj)
        {
            JToken result = null;
            JToken data = null;
            jsonObj.TryGetValue("result", out result);
            jsonObj.TryGetValue("data", out data);

            this.status = result.Value<String>();
            this.oid = Guid.Parse(data["oid"].Value<String>());
            this.qid = Guid.Parse(data["qid"].Value<String>());
        }
    }

    public class Orders
    {
        public List<Order> orders { get; set; }

        public Orders(dynamic jsonObj)
        {
            this.orders = new List<Order>();
            JToken data = null;
            jsonObj.TryGetValue("data", out data);
            foreach (JToken token in data)
            {
                Order order = new Order();
                order.oid = Guid.Parse(token["oid"].Value<String>());
                order.currency = getCurrency(token["currency"].Value<String>());
                order.item = getItem(token["item"].Value<String>());
                order.amount = new Amount()
                {
                    value = token["amount"].Value<Decimal>("value"),
                    value_int = token["amount"].Value<Int64>("value_int"),
                    display = token["amount"].Value<String>("display"),
                    display_short = token["amount"].Value<String>("display_short"),
                    currency = getCurrency(token["amount"].Value<String>("currency"))
                };
                order.effective_amount = new Amount()
                {
                    value = token["effective_amount"].Value<Decimal>("value"),
                    value_int = token["effective_amount"].Value<Int64>("value_int"),
                    display = token["effective_amount"].Value<String>("display"),
                    display_short = token["effective_amount"].Value<String>("display_short"),
                    currency = getCurrency(token["effective_amount"].Value<String>("currency"))
                };
                order.price = new Price()
                {
                    value = token["price"].Value<Decimal>("value"),
                    value_int = token["price"].Value<Int64>("value_int"),
                    display = token["price"].Value<String>("display"),
                    display_short = token["price"].Value<String>("display_short"),
                    currency = getCurrency(token["price"].Value<String>("currency"))
                };
                order.date = UnixTimeStampToDate(token["date"].Value<Double>());
                order.status = getStatus(token["status"].Value<String>());
                order.priority = token["priority"].Value<Int64>();
                order.type = Order.getOrderType(token["type"].Value<String>());

                orders.Add(order);
            }
        }

        public override string ToString()
        {
            string ordersString = orders[0].ToString();
            for (int i = 1; i < orders.Count; i++)
                ordersString += Environment.NewLine +Environment.NewLine + orders[i].ToString();
            ordersString += Environment.NewLine;
            return ordersString;
        }

        public static DateTime UnixTimeStampToDate(double unixTimeStamp)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public Currency getCurrency(String currency)
        {
            switch (currency)
            {
                case "USD":
                    return Currency.USD;
                case "BTC":
                    return Currency.BTC;
                case "AUD":
                    return Currency.AUD;
                case "CAD":
                    return Currency.CAD;
                case "CHF":
                    return Currency.CHF;
                case "CNY":
                    return Currency.CNY;
                case "DKK":
                    return Currency.DKK;
                case "EUR":
                    return Currency.EUR;
                case "GBP":
                    return Currency.GBP;
                case "HKD":
                    return Currency.HKD;
                case "JPY":
                    return Currency.JPY;
                case "NZD":
                    return Currency.NZD;
                case "PLN":
                    return Currency.PLN;
                case "RUB":
                    return Currency.RUB;
                case "SEK":
                    return Currency.SEK;
                case "SGD":
                    return Currency.SGD;
                case "THB":
                    return Currency.THB;
                default:
                    return Currency.None;
            }
        }

        public OrderItem getItem(String item)
        {
            switch (item)
            {
                case "BTC":
                    return OrderItem.BTC;
                default:
                    return OrderItem.None;
            }
        }

        public OrderStatus getStatus(String status)
        {
            switch (status)
            {
                case "invalid":
                    return OrderStatus.Invalid;
                default:
                    return OrderStatus.None;
            }
        }
    }



    public enum OrderType
    {
        Ask, Bid, Market, None
    }

    public enum OrderStatus
    {
        Invalid, None
    }

    public enum OrderItem
    {
        BTC, None
    }


}
