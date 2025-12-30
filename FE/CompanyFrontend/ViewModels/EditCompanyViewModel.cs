using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyApi.Facade.Sdk;
using CompanyFrontend.Services;
using System;
using System.Threading.Tasks;

namespace CompanyFrontend.ViewModels
{
    public partial class EditCompanyViewModel : ObservableObject
    {
        private readonly ICompanyService _companyService;
        private readonly Action<Company> _onSaved;
        private readonly Action _onCancelled;

        [ObservableProperty]
        private long companyId;

        [ObservableProperty]
        private string name = string.Empty;

        [ObservableProperty]
        private string vat = string.Empty;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private bool isSaving;

        public EditCompanyViewModel(ICompanyService companyService, Company company, Action<Company> onSaved, Action onCancelled)
        {
            _companyService = companyService;
            _onSaved = onSaved;
            _onCancelled = onCancelled;

            CompanyId = company.Id;
            Name = company.Name ?? string.Empty;
            Vat = company.Vat ?? string.Empty;

            System.Diagnostics.Debug.WriteLine($"📝 EditCompanyViewModel created for: {Name} (ID: {CompanyId})");
        }

        [RelayCommand]
        private async Task Save()
        {
            ErrorMessage = string.Empty;
            IsSaving = true;

            try
            {
                bool nameChanged = !string.IsNullOrWhiteSpace(Name);
                bool vatChanged = !string.IsNullOrWhiteSpace(Vat);
                
                if (nameChanged && vatChanged)
                {
                    var companyDto = new CompanyDto
                    {
                        Name = Name,
                        Vat = Vat
                    };

                    System.Diagnostics.Debug.WriteLine($"💾 Updating full company {CompanyId}: Name={Name}, Vat={Vat}");
                    await _companyService.UpdateCompanyAsync(CompanyId, companyDto);
                }
                else
                {
                    ErrorMessage = "At least one field (Name or VAT) must be filled";
                    System.Diagnostics.Debug.WriteLine("❌ Validation failed: No fields filled");
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"✅ Company {CompanyId} updated successfully");

                // Créer l'objet Company mis à jour
                var updatedCompany = new Company
                {
                    Id = CompanyId,
                    Name = Name,
                    Vat = Vat
                };
                
                // Passer la company mise à jour au callback
                _onSaved?.Invoke(updatedCompany);
            } 
            catch (Exception ex)
            {
                ErrorMessage = $"Error saving company: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"❌ Error updating company: {ex.Message}");
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
            System.Diagnostics.Debug.WriteLine("ℹ️ Edit cancelled by user");
            
            // Appeler le callback d'annulation
            _onCancelled?.Invoke();
        }
    }
}
