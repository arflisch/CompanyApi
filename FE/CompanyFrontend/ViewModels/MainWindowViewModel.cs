using Avalonia.Controls.ApplicationLifetimes;
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

        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<Company> companies = [];

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private object? currentView;

        public MainWindowViewModel(ICompanyService companyService)
        {
            _companyService = companyService;
            
            // Afficher la liste au démarrage
            CurrentView = this;
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
            if (company == null)
            {
                System.Diagnostics.Debug.WriteLine("❌ Company is null");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"✏️ Navigating to edit view for: {company.Name} (ID: {company.Id})");

            // Créer le ViewModel pour l'édition avec callbacks
            var editViewModel = new EditCompanyViewModel(
                _companyService,
                company,
                OnCompanySaved,      // Callback quand sauvegardé
                NavigateBackToList   // Callback quand annulé
            );

            // Naviguer vers la vue d'édition
            CurrentView = editViewModel;
        }

        private void OnCompanySaved(Company updatedCompany)
        {
            System.Diagnostics.Debug.WriteLine($"🔄 Updating company {updatedCompany.Id} in the list");

            // Trouver l'index de la company dans la collection
            var existingCompany = Companies.FirstOrDefault(c => c.Id == updatedCompany.Id);
            
            if (existingCompany != null)
            {
                var index = Companies.IndexOf(existingCompany);
                
                // Remplacer l'élément à cet index
                Companies[index] = updatedCompany;
                
                System.Diagnostics.Debug.WriteLine($"✅ Company {updatedCompany.Id} updated in collection at index {index}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"⚠️ Company {updatedCompany.Id} not found in collection, adding it");
                Companies.Add(updatedCompany);
            }

            // Revenir à la liste
            NavigateBackToList();
        }

        private void NavigateBackToList()
        {
            System.Diagnostics.Debug.WriteLine("🔙 Navigating back to company list");
            
            // Revenir à la vue de la liste
            CurrentView = this;
        }
    }
}