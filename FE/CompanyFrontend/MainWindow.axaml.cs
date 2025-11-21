using Avalonia.Controls;
using Avalonia.Interactivity;
using CompanyFrontend.ViewModels;

namespace CompanyFrontend
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }
    }
}