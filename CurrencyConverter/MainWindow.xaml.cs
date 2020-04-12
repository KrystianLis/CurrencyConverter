using CurrencyConverter.Model;
using System;
using System.Collections.Generic;
using System.IO;
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
                var response = await GetFullRateAsync();
            }
            catch (ResponeException ex)
            {

            }
            catch(JsonException ex)
            {

            }
            catch (Exception)
            {

            }
        }

        private async Task<RootRate> GetFullRateAsync()
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, $"http://api.nbp.pl/api/exchangerates/tables/a/?format=json");
            using var response = await client.SendAsync(request, new CancellationTokenSource(TimeSpan.FromSeconds(Timeout)).Token);

            var contentStream = await response.Content.ReadAsStreamAsync();

            if (response.IsSuccessStatusCode)
            {
                return await JsonSerializer.DeserializeAsync<RootRate>(contentStream, new JsonSerializerOptions 
                { 
                    IgnoreNullValues = true, 
                    PropertyNameCaseInsensitive = true 
                });
            }

            var content = await StreamToStringAsync(contentStream);

            throw new ResponeException
            {
                StatusCpde = (int)response.StatusCode,
                Content = content
            };
        }

        private static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = null;

            if (stream != null)
            {
                using (var sr = new StreamReader(stream))
                {
                    content = await sr.ReadToEndAsync();
                }
            }

            return content;
        }
    }
}
