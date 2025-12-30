using CompanyApi.Facade.Sdk;

namespace CompanyFrontend.Services
{
    public interface INavigationService
    {
        object CurrentView { get; }
        
        void NavigateToList();
        void NavigateToEdit(CompanyDto company);
        void NavigateToCreate();
    }
}
