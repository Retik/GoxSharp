using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace GoxSharp.Models
{

    public class Depth
    {
        public List<Order> orders { get; set; }
        public Decimal totalVolume { get; set; }
        public Decimal averagePrice { get; set; }
        public Decimal weightedAverageAskPrice
        {
            get
            {
                Decimal weightedOrderPrice = orders.Where(od => od.type == OrderType.Ask).Sum(od => od.amount.value * od.price.value);
                Decimal totalVolume = orders.Where(od => od.type == OrderType.Ask).Sum(od => od.amount.value);
                return weightedOrderPrice / totalVolume;
                
            }
        }
        public Decimal weightedAverageBidPrice
        {
            get
            {
                Decimal weightedOrderPrice = orders.Where(od => od.type == OrderType.Bid).Sum(od => od.amount.value * od.price.value);
                Decimal totalVolume = orders.Where(od => od.type == OrderType.Bid).Sum(od => od.amount.value);
                return weightedOrderPrice / totalVolume;

            }
        }
        public Decimal lowestAsk
        {
            get
            {
                return orders.Where(od => od.type == OrderType.Ask).Select(od => od.price.value).Min();
            }
        }
        public Decimal highestBid
        {
            get
            {
                return orders.Where(od => od.type == OrderType.Bid).Select(od => od.price.value).Max();
            }
        }

        public Depth(dynamic jsonObj)
        {
          
            string[] orderTypes = { "asks", "bids" };
            List<Order> orders = new List<Order>();

            foreach (string orderType in orderTypes)
            {
                JToken tokens = null;
                if (jsonObj.data.TryGetValue(orderType, out tokens))
                {

                    foreach (JToken token in (JArray)tokens)
                    {
                        Order order = new Order(token, orderType);
                  
                        orders.Add(order);
                    }

                }
            }
            this.orders = orders;          
        }
    } 
}
