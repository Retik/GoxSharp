using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GoxSharp.Models
{
    public class Info
    {
        public string id { get; set; }
        public string index { get; set; }
        public string language { get; set; }
        public DateTime last_login { get; set; }
        public DateTime created { get; set; }
        public string login { get; set; }
        public Double trade_fee { get; set; }
        public Right[] rights { get; set; }
        public List<Wallet> wallets { get; set; }


        public Info(dynamic jsonObj)
        {
            JToken token = null;
            if (jsonObj.TryGetValue("data", out token))
            {
                this.login = token["Login"].Value<String>();
                this.id = token["Id"].Value<String>();
                this.index = token["Index"].Value<String>();
                this.rights = getRights((JArray)token["Rights"]);
                this.language = token["Language"].Value<String>();
                this.created = token["Created"].Value<DateTime>();
                this.last_login = token["Last_Login"].Value<DateTime>();
                this.wallets = getWallets(token["Wallets"]);
                this.trade_fee = token["Trade_Fee"].Value<Double>();

            }
        }

        private List<Wallet> getWallets(JToken token)
        {
            List<Wallet> result = new List<Wallet>();
            foreach (JProperty t in token)
            {
                Wallet wallet = new Wallet();

                wallet.currency = t.Name;
                wallet.operations = t.Value["Operations"].Value<Int32>();
                wallet.details = getWalletDetails(t);
                result.Add(wallet);
            }
            return result;

        }

        private List<WalletDetails> getWalletDetails(JProperty token)
        {
            string[] detailTypes = { "Balance", "Daily_Withdraw_Limit", "Monthly_Withdraw_Limit", "Max_Withdraw", "Open_Orders" };
            List<WalletDetails> results = new List<WalletDetails>();

            foreach (String type in detailTypes)
            {
                WalletDetails details = new WalletDetails();
                JToken obj = token.Value[type];
                try
                {
                    details.type = getType(type);
                    details.display = obj["display"].Value<String>();
                    details.display_short = obj["display_short"].Value<String>();
                    details.value = obj["value"].Value<Double>();
                    details.value_int = obj["value_int"].Value<Int64>();

                    results.Add(details);
                }
                catch (Exception exc)
                {
                    details.type = getType(type);
                    results.Add(details);
                }
            }
            return results;
        }

        private DetailType getType(String type)
        {
            switch (type)
            {
                case "Balance":
                    return DetailType.Balance;
                case "Daily_Withdraw_Limit":
                    return DetailType.Daily_Withdraw_Limit;
                case "Monthly_Withdraw_Limit":
                    return DetailType.Monthly_Withdraw_Limit;
                case "Max_Withdraw":
                    return DetailType.Max_Withdraw;
                case "Open_Orders":
                    return DetailType.Open_Orders;
                default:
                    return DetailType.None;

            }
        }
        private Right[] getRights(JArray token)
        {
            //TODO add the rest of the rights.
            Right[] rights = new Right[token.Count()];
            int count = 0;
            foreach (JToken t in token)
            {
                switch (t.Value<String>())
                {
                        
                    case "get_info":
                        rights[count] = Right.Get_Info;
                        count++;
                        break;
                    case "deposit":
                        rights[count] = Right.Deposit;
                        count++;
                        break;
                    case "merchant":
                        rights[count] = Right.Merchant;
                        count++;
                        break;
                    case "trade":
                        rights[count] = Right.Trade;
                        count++;
                        break;
                    case "withdraw":
                        rights[count] = Right.Withdraw;
                        count++;
                        break;
                    default:
                        rights[count] = Right.None;
                        count++;
                        break;


                }
                
            }
            return rights;
        }
    }

    public class Wallet
    {
        public String currency { get; set; }
        public List<WalletDetails> details { get; set; }
        public int operations { get; set; }


    }

    public class WalletDetails
    {
        public DetailType type { get; set; }
        public string display { get; set; }
        public string display_short { get; set; }
        public Double value { get; set; }
        public Int64 value_int { get; set; }
    }

    public enum DetailType
    {
        Balance, Daily_Withdraw_Limit, Max_Withdraw, Monthly_Withdraw_Limit, Open_Orders, None
    }

    public enum Right
    {
        Get_Info, Trade, Deposit, Withdraw, Merchant, None
    }


}
