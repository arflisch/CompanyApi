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

        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<CompanyDto> companies = [];

        [ObservableProperty]
        private bool isLoading;

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
            
            try
            {
                Companies.Clear();

                IsAdminUser = _authService.IsAdmin;

                var companiesList = await _companyService.GetAllCompaniesAsync();

                System.Diagnostics.Debug.WriteLine($"✅ Received {companiesList.Count} companies");
                
                foreach (var company in companiesList)
                {
                    Companies.Add(company);
                    System.Diagnostics.Debug.WriteLine($"  Added: Id={company.Id}, Name={company.Name}, Vat={company.Vat}");
                }
                
                System.Diagnostics.Debug.WriteLine($"✅ Companies collection count: {Companies.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Error loading companies: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
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
            // 1. Nettoyer le cache MSAL (Token)
            await _authService.LogoutAsync();

            IsAdminUser = false;

            // 2. Retourner à la page de login
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
                
                System.Diagnostics.Debug.WriteLine($"✅ Company {updatedCompany.Id} updated in collection at index {index}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Company {updatedCompany.Id} not found in collection, adding it");
                Companies.Add(updatedCompany);
            }

            _navigationService.NavigateToList();
        }

        public void OnCompanyCreated(CompanyDto createdCompany)
        {
            Companies.Add(createdCompany);
            _navigationService.NavigateToList();
        }
    }
}