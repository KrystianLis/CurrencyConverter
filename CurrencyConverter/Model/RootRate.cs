using System;
using System.Collections.Generic;
using System.Text;

namespace CurrencyConverter.Model
{
    public class RootRate
    {
        public string table { get; set; }
        public string no { get; set; }
        public string effectiveDate { get; set; }
        public List<Rate> rates { get; set; }
    }
}
