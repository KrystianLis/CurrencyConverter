using CurrencyConverter.Model;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CurrencyConverter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Regex _regex = new Regex("[^0-9]+");

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
                Dispatcher.Invoke(() =>
                {
                    if (TypeComboBox.SelectedIndex > -1 && SecondRatesComboBox.SelectedIndex > -1 && FirstValueTextBox.Text != string.Empty)
                    {
                        var selectedAction = TypeComboBox.SelectedIndex;
                        var toCurrency = ((Rate)SecondRatesComboBox.SelectedItem).Currency;

                        var rate = this.rootObject.Rates.First(x => x.Currency.Equals(toCurrency));
                        var times = ChooseAction(selectedAction, rate);

                        SecondValueTextBox.Text = Convert.ToString(Convert.ToInt32(FirstValueTextBox.Text) * times);
                    }
                });

                Thread.Yield();
            }
        }

        private static double ChooseAction(int selectedAction, Rate rate) =>
            (SelectedType)selectedAction switch
            {
                SelectedType.Sale => rate.Sale,
                SelectedType.Purchase =>  rate.Purchase,
                _ => throw new NotImplementedException()
            };

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

        private void FirstValueTextBox_NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            return _regex.IsMatch(text);
        }

        private void FirstValueTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string text = (string)e.DataObject.GetData(typeof(string));
                if (!IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }
}
