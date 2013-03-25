using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoxSharp.Models
{
    public class Price
    {
        public Decimal value { get; set; }
        public Int64 value_int { get; set; }
        public string display { get; set; }
        public string display_short { get; set; }
        public Currency currency { get; set; }

        public Price()
        {
        }

        public Price(double value)
        {
            this.value = (Decimal)value;
            this.value_int = (Int64)((Decimal)value / (Decimal)0.00001);
        }

    }
    public class Amount
    {
        public Decimal value { get; set; }
        public Int64 value_int { get; set; }
        public string display { get; set; }
        public string display_short { get; set; }
        public Currency currency { get; set; }

        public Amount()
        {
        }
        public Amount(double value)
        {
            this.value = (Decimal)value;
            this.value_int = (Int64)((Decimal)value / (Decimal)0.00000001);
        }
    }
}
