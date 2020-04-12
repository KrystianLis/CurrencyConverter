using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyConverter.Model
{
    public class Rate
    {
        public string currency { get; set; }
        public string code { get; set; }
        public double mid { get; set; }
    }
}
