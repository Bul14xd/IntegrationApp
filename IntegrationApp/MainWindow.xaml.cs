using IntegralCalculator.Services;
using IntegralCalculator.ViewModels;
using System.Windows;

namespace IntegralCalculator
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            DatabaseService.InitializeDatabase();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;
        }
    }
}