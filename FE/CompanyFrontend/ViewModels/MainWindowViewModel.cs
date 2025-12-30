using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Company = CompanyApi.Facade.Sdk.Company;

namespace CompanyFrontend.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<Company> companies = [];

        [ObservableProperty]
        private bool isLoading;

        public MainWindowViewModel(ICompanyService companyService, INavigationService navigationService)
        {
            _companyService = companyService;
            _navigationService = navigationService;
        }

        [RelayCommand]
        public async Task LoadCompanies()
        {
            IsLoading = true;
            
            try
            {
                Companies.Clear();
                
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
        private void EditCompany(Company company)
        {
            _navigationService.NavigateToEdit(company);
        }

        [RelayCommand]
        private void CreateCompany()
        {
            _navigationService.NavigateToCreate();
        }

        public void OnCompanySaved(Company updatedCompany)
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

        public void OnCompanyCreated()
        {
            System.Diagnostics.Debug.WriteLine("➕ Reloading company list after creation");
            
            // Recharger la liste complète pour obtenir la nouvelle company avec son ID
            _ = LoadCompanies();
            
            _navigationService.NavigateToList();
        }
    }
}