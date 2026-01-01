using CompanyApi.Facade.Sdk;
using CompanyFrontend.ViewModels;
using System;
using System.ComponentModel;

namespace CompanyFrontend.Services
{
    public class NavigationService : INavigationService, INotifyPropertyChanged
    {
        private readonly ICompanyService _companyService;
        private readonly Lazy<MainWindowViewModel> _mainWindowViewModel;
        private readonly Lazy<LoginViewModel> _loginViewModel;
        private object? _currentView;

        public event PropertyChangedEventHandler? PropertyChanged;

        public object CurrentView
        {
            get => _currentView ?? _loginViewModel.Value;
            private set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentView)));
                }
            }
        }

        private MainWindowViewModel MainWindowViewModel => _mainWindowViewModel.Value;

        public NavigationService(ICompanyService companyService, Lazy<MainWindowViewModel> mainWindowViewModel, Lazy<LoginViewModel> loginViewModel)
        {
            _companyService = companyService;
            _mainWindowViewModel = mainWindowViewModel;
            _loginViewModel = loginViewModel;
        }

        public void NavigateToLogin()
        {
            CurrentView = _loginViewModel.Value;
        }

        public void NavigateToList()
        {
            System.Diagnostics.Debug.WriteLine("🔙 Navigating to company list");
            var vm = MainWindowViewModel;

            vm.RefreshUserRoles();

            CurrentView = vm;
        }

        public void NavigateToEdit(CompanyDto company)
        {
            if (company == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Cannot navigate to edit: company is null");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"✏️ Navigating to edit view for: {company.Name} (ID: {company.Id})");

            var editViewModel = new EditCompanyViewModel(
                _companyService,
                company,
                MainWindowViewModel.OnCompanySaved,
                NavigateToList
            );

            CurrentView = editViewModel;
        }

        public void NavigateToCreate()
        {
            System.Diagnostics.Debug.WriteLine("➕ Navigating to create company view");
            
            var createViewModel = new CreateCompanyViewModel(
                _companyService,
                MainWindowViewModel.OnCompanyCreated,  // Callback sans paramètre
                NavigateToList  // Callback pour Cancel
            );
            
            CurrentView = createViewModel;
        }
    }
}
