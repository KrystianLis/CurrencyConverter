using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;

namespace CurrencyConverter.Model
{
    public class Rate : INotifyPropertyChanged
    {
        private string _currency;


        [JsonPropertyName("currency")]
        public string Currency 
        {
            get 
            {
                return _currency;
            }
            set
            {
                _currency = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("bid")]
        public double Buy { get; set; }

        [JsonPropertyName("ask")]
        public double Sell { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
