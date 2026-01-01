using Avalonia.Controls;
using CompanyFrontend.Services;

namespace CompanyFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow(INavigationService navigationService)
        {
            InitializeComponent();
            DataContext = navigationService;
        }
    }
}