using System;
using System.Collections.Specialized;
using RestSharp;
using GoxSharp.Models;

namespace GoxSharp
{
    public class TradeFunctions
    {
        protected MtGoxRestClient mgrc = new MtGoxRestClient();

        public TradeFunctions()
        {
        }

        public Info GetInfo()
        {
            return (Info)mgrc.GetResponse("Info", "money/info", Method.POST, null);
        }

        public IdKey GetIdKey()
        {
            return (IdKey)mgrc.GetResponse("IdKey", "money/idkey", Method.POST, null);
        }

        public Orders GetOrders()
        {
            return (Orders)mgrc.GetResponse("Orders", "money/orders", Method.POST, null);
        }

        public IdKey GetCurrency(Currency currency)
        {
            //TDO create currency model, rename the ENUM.
            return (IdKey)mgrc.GetResponse("Currency", String.Format("BTC{0}/money/currency", currency.ToString()), Method.POST, null);
        }

        public Tickers GetTicker(Currency currency)
        {
            return (Tickers)mgrc.GetResponse("Tickers", String.Format("BTC{0}/money/ticker", currency.ToString()), Method.GET, null);
        }

        public Depth GetDepth(Currency currency)
        {
            return (Depth)mgrc.GetResponse("Depth", String.Format("BTC{0}/money/depth/fetch", currency.ToString()), Method.GET, null);
        }

        public OrderResponse CreateOrder(Currency currency, Order order)
        {
            NameValueCollection nvc = new NameValueCollection
            {
                {"type", order.type.ToString().ToLower()},
                {"amount_int", order.amount.value_int.ToString()},
                {"price_int", order.price.value_int.ToString()}
            };
            return (OrderResponse)mgrc.GetResponse("OrderResponse", String.Format("BTC{0}/money/order/add", currency.ToString()), Method.POST, nvc);
        }

        public OrderCancelResponse CancelOrder(Order order)
        {
            NameValueCollection nvc = new NameValueCollection {{"oid", order.oid.ToString()}};
            return (OrderCancelResponse)mgrc.GetResponse("OrderCancelResponse", String.Format("money/order/cancel"), Method.POST, nvc);
        }

       

    }
}
