using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CompanyFrontend.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

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
            var services = new ServiceCollection();
            ConfigureServices(services);
            Services = services.BuildServiceProvider();

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register your services here
            //services.AddSingleton<MainWindow>();
            //services.AddSingleton<MainWindowViewModel>();

            // Add other services, repositories, HTTP clients, etc.
            // services.AddHttpClient<ICompanyService, CompanyService>();
        }
    }
}