using CurrencyConverter.Model;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int Timeout = 10;

        private RootObject rootObject;

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
            try
            {
                rootObject = await GetFullRateAsync();
                RatesComboBox.ItemsSource = rootObject.Rates;
                Rates.ItemsSource = rootObject.Rates;
            }
            catch (ResponeException ex)
            {
                Notes.Text += $"Response exception!\nStatus code: {ex.StatusCode}\nContent: {ex.Content}\n";
            }
            catch(JsonException ex)
            {
                Notes.Text += $"JSON exception: {ex.Message}\n";
            }
            catch (Exception ex)
            {
                Notes.Text += $"{ex.Message}\n";
            }
        }

        private async Task<RootObject> GetFullRateAsync()
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(@"http://api.nbp.pl/api/exchangerates/tables/c/?format=json", HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource(TimeSpan.FromSeconds(Timeout)).Token);

            var content = await response.Content.ReadAsStringAsync();

            content = content[1..^1];

            if (response.IsSuccessStatusCode)
            {
                return JsonSerializer.Deserialize<RootObject>(content, new JsonSerializerOptions 
                { 
                    IgnoreNullValues = true, 
                    PropertyNameCaseInsensitive = true 
                });
            }

            throw new ResponeException
            {
                StatusCode = (int)response.StatusCode,
                Content = content
            };
       
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            string ss = ((Rate)RatesComboBox.SelectedItem).Currency.ToString();

        }
    }
}
