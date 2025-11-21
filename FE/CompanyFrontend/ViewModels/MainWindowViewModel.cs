using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Company = CompanyApi.Facade.Sdk.Company;

namespace CompanyFrontend.ViewModels
{
    public partial class MainWindowViewModel : ObservableObject
    {
        [ObservableProperty]
        private string welcomeMessage = "Welcome";

        [ObservableProperty]
        private ObservableCollection<Company> companies = [];

        [ObservableProperty]
        internal bool isLoading;

        [RelayCommand]
        public async Task LoadCompanies()
        {
            IsLoading = true;
            try
            {
                var httpClient = new HttpClient();
                var client = new CompanyClient(httpClient)
                {
                    BaseUrl = "https://localhost:7223"
                };
                var companiesList = await client.GetAllCompaniesAsync();

                companiesList.ForEach(c => Companies.Add(c));
          
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading companies: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}