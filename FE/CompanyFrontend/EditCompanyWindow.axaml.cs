using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CompanyFrontend.ViewModels;

namespace CompanyFrontend;

public partial class EditCompanyWindow : Window
{
    public EditCompanyWindow(EditCompanyViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}