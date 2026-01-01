using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Services;
using System;
using System.Threading.Tasks;

namespace CompanyFrontend.ViewModels
{
    public partial class CreateCompanyViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;
        private readonly Action<CompanyDto> _onCreated;
        private readonly Action _onCancelled;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string vat = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isSaving;

        public CreateCompanyViewModel(ICompanyService companyService, Action<CompanyDto> onCreated, Action onCancelled)
        {
            _companyService = companyService;
            _onCreated = onCreated;
            _onCancelled = onCancelled;
            
            System.Diagnostics.Debug.WriteLine("➕ CreateCompanyViewModel created");
        }

        [RelayCommand]
        private async Task Create()
        {
            ErrorMessage = string.Empty;
            IsSaving = true;

            try
            {
                bool nameChanged = !string.IsNullOrWhiteSpace(Name);
                bool vatChanged = !string.IsNullOrWhiteSpace(Vat);

                if (!nameChanged || !vatChanged)
                {
                    ErrorMessage = "Both Name and VAT fields are required";
                    System.Diagnostics.Debug.WriteLine("❌ Validation failed: Name or VAT missing");
                    return;
                }

                var companyDto = new CreateCompanyDto
                {
                    Name = Name,
                    Vat = Vat,
                };

                System.Diagnostics.Debug.WriteLine($"💾 Creating company: Name={Name}, Vat={Vat}");
                
                var createdCompany = await _companyService.CreateCompanyAsync(companyDto);

                System.Diagnostics.Debug.WriteLine("✅ Company created successfully");

                // Appeler le callback sans paramètre
                _onCreated?.Invoke(createdCompany);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error creating company: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Error creating company: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                IsSaving = false;
            }
        }

        [RelayCommand]
        private void Cancel()
        {
            System.Diagnostics.Debug.WriteLine("ℹ️ Create cancelled by user");
            _onCancelled?.Invoke();
        }
    }
}
