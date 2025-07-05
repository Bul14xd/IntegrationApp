using IntegralCalculator.Models;
using IntegralCalculator.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.ComponentModel;
using IntegrationApp.Services;

namespace IntegralCalculator.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private IntegralData _integralData = new();
        private CalculationResult _result = new();
        private string _statusMessage = "Готов к работе";

        public IntegralData IntegralData
        {
            get => _integralData;
            set { _integralData = value; OnPropertyChanged(); }
        }

        public CalculationResult Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }
             
        public ICommand CalculateCommand { get; }

        public MainViewModel()
        {
            CalculateCommand = new RelayCommand(async () => await CalculateIntegralAsync());
        }

        private async Task<double> TrapezoidalRule(Func<double, double> f, double a, double b, int n)
        {
            if (n <= 0) throw new ArgumentException("Число интервалов должно быть больше нуля.");

            double h = (b - a) / n;
            double sum = 0.5 * (f(a) + f(b));

            var tasks = new Task<double>[n - 1];

            for (int i = 1; i < n; i++)
            {
                double x = a + i * h;
                tasks[i - 1] = Task.Run(() => f(x));
            }

            double[] results = await Task.WhenAll(tasks);

            foreach (var val in results)
            {
                sum += val;
            }

            return sum * h;
        }

        private async Task CalculateIntegralAsync()
        {
            try
            {
                StatusMessage = "Вычисление...";

                Func<double, double> function = FunctionParser.Parse(IntegralData.FunctionString);

                double resultValue = await TrapezoidalRule(
                    function,
                    IntegralData.LowerBound,
                    IntegralData.UpperBound,
                    IntegralData.NumberOfIntervals);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    Result = new CalculationResult
                    {
                        Result = resultValue,
                        CalculationTime = DateTime.Now
                    };

                    StatusMessage = "Вычисление завершено.";
                });

                DatabaseService.SaveResult(IntegralData, Result);
            }
            catch (Exception ex)
            {
                StatusMessage = $"Ошибка: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}