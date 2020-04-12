using System;
using System.Net.Http;
using System.Windows;

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LoadCurrency_Click(object sender, RoutedEventArgs e)
        {
            using(var client = new HttpClient())
            {
                try
                {
                    var respone = await client.GetAsync($"http://api.nbp.pl/api/exchangerates/tables/a/");
                    var content = await respone.Content.ReadAsStringAsync();
                    

                }
                catch(Exception)
                {

                }

            }
        }
    }
}
