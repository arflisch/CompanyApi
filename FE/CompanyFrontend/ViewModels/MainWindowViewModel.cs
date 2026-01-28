using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace CompanyFrontend.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;
        private readonly INavigationService _navigationService;
        private readonly IAuthService _authService;
        
        private int _currentPage = 1;
        private const int _pageSize = 20; 
        private bool _hasMoreData = true;

        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<CompanyDto> companies = [];

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool isLoadingMore;

        [ObservableProperty]
        private bool isAdminUser;

        public MainWindowViewModel(ICompanyService companyService, INavigationService navigationService, IAuthService authService)
        {
            _companyService = companyService;
            _navigationService = navigationService;
            _authService = authService;
        }

        public void RefreshUserRoles()
        {
            IsAdminUser = _authService.IsAdmin;
        }

        [RelayCommand]
        public async Task LoadCompanies()
        {
            IsLoading = true;
            _currentPage = 1;
            _hasMoreData = true;
            
            try 
            {
                Companies.Clear();
                IsAdminUser = _authService.IsAdmin;
                
                await LoadNextPage();
            }
            finally
            {
                IsLoading = false;
            }
        }
        
        public async Task LoadNextPage()
        {

            if (IsLoadingMore || !_hasMoreData) return;

            try
            {
                IsLoadingMore = true;
                
                System.Diagnostics.Debug.WriteLine($"🔄 Loading page {_currentPage}...");
                
                var newBatch = await _companyService.GetAllCompaniesAsync(_currentPage, _pageSize);

                if (newBatch.Count < _pageSize)
                {
                    _hasMoreData = false; // Plus de données après ça
                    System.Diagnostics.Debug.WriteLine("🏁 End of data reached.");
                }

                foreach (var company in newBatch)
                {
                    Companies.Add(company);
                }

                System.Diagnostics.Debug.WriteLine($"✅ Added {newBatch.Count} companies. Total: {Companies.Count}");
                
                _currentPage++;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading page {_currentPage}: {ex.Message}");
            }
            finally
            {
                IsLoadingMore = false;
            }
        }

        [RelayCommand]
        private void EditCompany(CompanyDto company)
        {
            _navigationService.NavigateToEdit(company);
        }

        [RelayCommand]
        private void CreateCompany()
        {
            _navigationService.NavigateToCreate();
        }

        [RelayCommand]
        public async Task Logout()
        {
            await _authService.LogoutAsync();
            IsAdminUser = false;
            _navigationService.NavigateToLogin();
        }

        public void OnCompanySaved(CompanyDto updatedCompany)
        {
            System.Diagnostics.Debug.WriteLine($"🔄 Updating company {updatedCompany.Id} in the list");

            var existingCompany = Companies.FirstOrDefault(c => c.Id == updatedCompany.Id);
            
            if (existingCompany != null)
            {
                var index = Companies.IndexOf(existingCompany);
                Companies[index] = updatedCompany;
            }
            else
            {
                Companies.Insert(0, updatedCompany); 
            }

            _navigationService.NavigateToList();
        }

        public void OnCompanyCreated(CompanyDto createdCompany)
        {
            Companies.Insert(0, createdCompany);
            _navigationService.NavigateToList();
        }
    }
}