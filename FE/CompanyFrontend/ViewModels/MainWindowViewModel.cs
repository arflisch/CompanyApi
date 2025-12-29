using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Company = CompanyApi.Facade.Sdk.Company;

namespace CompanyFrontend.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;

        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<Company> companies = [];

        [ObservableProperty]
        private bool isLoading;

        public MainWindowViewModel(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [RelayCommand]
        public async Task LoadCompanies()
        {
            IsLoading = true;
            
            try
            {
                Companies.Clear();
                
                var companiesList = await _companyService.GetAllCompaniesAsync();

                System.Diagnostics.Debug.WriteLine($"? Received {companiesList.Count} companies");
                
                foreach (var company in companiesList)
                {
                    Companies.Add(company);
                    System.Diagnostics.Debug.WriteLine($"  Added: Id={company.Id}, Name={company.Name}, Vat={company.Vat}");
                }
                
                System.Diagnostics.Debug.WriteLine($"? Companies collection count: {Companies.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"? Error loading companies: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}