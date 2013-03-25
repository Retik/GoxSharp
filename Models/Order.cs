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
            DateTime now = DateTime.Now;
            this.type = this.getOrderType(orderType);
            Amount amount = new Amount() { value = token["amount"].Value<Decimal>(), value_int = token["amount_int"].Value<Int64>() };
            Price price = new Price { value = token["price"].Value<Decimal>(), value_int = token["price_int"].Value<Int64>() };

            this.stamp = new DateTime((token["stamp"].Value<long>() * 10) + 621355968000000000);
        }

        public OrderType getOrderType(String type)
        {
            switch (type)
            {
                case "asks":
                    return OrderType.Ask;
                case "bids":
                    return OrderType.Bid;
                case "market":
                    return OrderType.Market;
                default:
                    return OrderType.None;
            }
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


                orders.Add(order);
            }
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
