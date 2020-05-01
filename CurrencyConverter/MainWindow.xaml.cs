﻿using CurrencyConverter.Model;
using System;
using System.Diagnostics;
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

        private Task checkValuesTask;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Close_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void LoadData_Click(object sender, RoutedEventArgs e)
        {
            #region Before loading rate data

            var watch = new Stopwatch();
            watch.Start();
            RateProgress.Visibility = Visibility.Visible;
            RateProgress.IsIndeterminate = true;

            #endregion

            try
            {
                rootObject = await GetFullRateAsync();
                RatesComboBox.ItemsSource = rootObject.Rates;
                FirstRatesComboBox.ItemsSource = rootObject.Rates;
                SecondRatesComboBox.ItemsSource = rootObject.Rates;

                this.checkValuesTask = Task.Factory.StartNew(this.CheckValues, TaskCreationOptions.LongRunning);

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

            #region After loading rate data

            RatesStatus.Text = $"Loaded rates in {watch.ElapsedMilliseconds}ms";
            RateProgress.Visibility = Visibility.Hidden;

            #endregion
        }

        private void CheckValues()
        {
            while(true)
            {
                if (FirstRatesComboBox.SelectedIndex > -1)
                {

                }
                else if(SecondRatesComboBox.SelectedIndex > -1)
                {

                }

                Thread.Yield();
            }
        }

        private async Task<RootObject> GetFullRateAsync()
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(@"http://api.nbp.pl/api/exchangerates/tables/c/?format=json", 
                HttpCompletionOption.ResponseHeadersRead, new CancellationTokenSource(TimeSpan.FromSeconds(Timeout)).Token);

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
            try
            {
                string tempString = ((Rate)RatesComboBox.SelectedItem).Currency.ToString();
                RatesGrid.ItemsSource = rootObject.Rates.FindAll(s => s.Currency == tempString);
            }
            catch(NullReferenceException ex)
            {
                Notes.Text += $"{ex.Message} Select currency!\n";
            }
        }

        private void ShowCurrency_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RatesGrid.ItemsSource = rootObject.Rates;
            }
            catch(NullReferenceException ex)
            {
                Notes.Text += $"{ex.Message} Load data!\n";
            }
        }
    }
}
