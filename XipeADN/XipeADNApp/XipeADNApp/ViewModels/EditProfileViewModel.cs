using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using XipeADNApp.Validations;

namespace XipeADNApp.ViewModels
{
	public class EditProfileViewModel : ViewModelBase
	{
        ValidatableObject<string> name;
        ValidatableObject<string> phone;
        ValidatableObject<string> email;
        ValidatableObject<string> company;
        ValidatableObject<string> companyRole;

        public ValidatableObject<string> Name
        { get => name; set => SetProperty(ref name, value); }
        public ValidatableObject<string> Phone
        { get => phone; set => SetProperty(ref phone, value); }
        public ValidatableObject<string> Email
        { get => email; set => SetProperty(ref email, value); }
        public ValidatableObject<string> Company
        { get => company; set => SetProperty(ref company, value); }
        public ValidatableObject<string> CompanyRole
        { get => companyRole; set => SetProperty(ref companyRole, value); }
        public bool IsValid { get; set; }

        public DelegateCommand EditProfileCommand => new DelegateCommand(EditProfile);

        public EditProfileViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
			Title = "EditProfile";
            name = new ValidatableObject<string>();
            phone = new ValidatableObject<string>();
            email = new ValidatableObject<string>();
            company = new ValidatableObject<string>();
            companyRole = new ValidatableObject<string>();
            AddValidations();
        }


        async void EditProfile()
        {
            IsValid = Validate();
            if (IsValid)
            {
                var res = await ApiManager.TryEditProfile(Name.Value, Phone.Value, Email.Value,
                    Company.Value, CompanyRole.Value);
                if (res) NavigateBack();
            }
            MessagingCenter.Send(this, "SignInRequested");
        }

        bool Validate()
        {
            Name.IsValid = Name.Validate();
            Phone.IsValid = Phone.Validate();
            Email.IsValid = Email.Validate();
            Company.IsValid = Company.Validate();
            CompanyRole.IsValid = CompanyRole.Validate();
            return Name.IsValid && Phone.IsValid && Email.IsValid && Company.IsValid && CompanyRole.IsValid;
        }

        void AddValidations()
        {
            name.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Name field can not be empty" });
            phone.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Phone field can not be empty" });
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Email field can not be empty" });
            email.Validations.Add(new EmailRule());
            company.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Company field can not be empty" });
            companyRole.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Company role field can not be empty" });
        }
    }
}