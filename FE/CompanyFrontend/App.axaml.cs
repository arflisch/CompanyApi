using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CompanyFrontend.ViewModels;
using CompanyFrontend.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;

namespace CompanyFrontend
{
    public partial class App : Application
    {
        public IServiceProvider? Services { get; private set; }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            Services = ConfigureServices();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var mainWindow = Services.GetRequiredService<MainWindow>();
                desktop.MainWindow = mainWindow;
            }

            base.OnFrameworkInitializationCompleted();
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Configure HttpClient with SSL validation bypass for development
            services.AddHttpClient<ICompanyService, CompanyService>(client =>
            {
                client.BaseAddress = new Uri("https://localhost:7223");
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
            });

            // Register Navigation Service (Singleton car il garde l'état de navigation)
            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<IAuthService, AuthService>();
            // Register ViewModels
            services.AddTransient<MainWindowViewModel>();
            
            // Register Lazy<MainWindowViewModel> to break circular dependency
            services.AddSingleton<Lazy<MainWindowViewModel>>(provider => 
                new Lazy<MainWindowViewModel>(() => provider.GetRequiredService<MainWindowViewModel>()));

            services.AddTransient<LoginViewModel>();
            services.AddSingleton<Lazy<LoginViewModel>>(provider =>
                new Lazy<LoginViewModel>(() => provider.GetRequiredService<LoginViewModel>()));

            // Register Views
            services.AddSingleton<MainWindow>();

            return services.BuildServiceProvider();
        }
    }
}