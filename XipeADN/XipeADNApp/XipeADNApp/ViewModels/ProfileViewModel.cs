using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XipeADNApp.Validations;

namespace XipeADNApp.ViewModels
{
	public class ProfileViewModel : ViewModelBase
	{
        string name;
        string companyRole;
        string company;
        string email;
        string phone;
        string card;

        public string Name
        { get => name; set => SetProperty(ref name, value); }
        public string CompanyRole
        { get => companyRole; set => SetProperty(ref companyRole, value); }
        public string Company
        { get => company; set => SetProperty(ref company, value); }
        public string Email
        { get => email; set => SetProperty(ref email, value); }
        public string Phone
        { get => phone; set => SetProperty(ref phone, value); }
        public string Card
        { get => card; set => SetProperty(ref card, value); }

        public DelegateCommand LogoutCommand => new DelegateCommand(() => AbsoluteNavigation("/NavigationPage/LoginPage"));

        public ProfileViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
            var currentUser = ((ApiManager)apiManager).GetCurrentUser;
            Title = "Profile";
            Name = currentUser.FirstName ?? "User's Firstname";
            CompanyRole = currentUser.CompanyRole ?? "Puesto en empresa";
            Company = currentUser.Company ?? "Xipe Technology";
            Email = currentUser.Email;
            Phone = currentUser.Phone ?? "662 0000 000";
            Card = currentUser.Card ?? "**** 6200";
		}

	}
}