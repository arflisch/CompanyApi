using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CompanyFrontend.Services;
using System;
using System.Threading.Tasks;

namespace CompanyFrontend.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;
        private readonly INavigationService _navigationService;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string errMessage = string.Empty;

        public LoginViewModel(IAuthService authService, INavigationService navigationService)
        {
            _authService = authService;
            _navigationService = navigationService;
        }
        [RelayCommand]
        public async Task LoginAsync()
        {
            try
            {
                IsBusy = true;
                ErrMessage = string.Empty;
                var token = await _authService.LoginAsync();
                if (!string.IsNullOrEmpty(token))
                {
                    // Navigate to the main page after successful login
                    _navigationService.NavigateToList();
                }
                else
                {
                    ErrMessage = "Login failed. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrMessage = $"An error occurred during login: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
