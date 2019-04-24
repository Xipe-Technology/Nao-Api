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
	public class LoginViewModel : ViewModelBase
	{
        Color emailColor = Color.White;
        Color passwordColor = Color.White;
        ValidatableObject<string> email;
        ValidatableObject<string> password;

        public ValidatableObject<string> Email
        { get => email; set => SetProperty(ref email, value); }
        public ValidatableObject<string> Password
        { get => password; set => SetProperty(ref password, value); }
        public Color EmailColor
        { get => emailColor; set => SetProperty(ref emailColor, value); }
        public Color PasswordColor
        { get => passwordColor; set => SetProperty(ref passwordColor, value); }
        public bool IsValid { get; set; }

        public DelegateCommand SignInCommand => new DelegateCommand(SignIn);


        public LoginViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
			Title = "Login";
            email = new ValidatableObject<string>();
            password = new ValidatableObject<string>();
            AddValidations();
        }

        async void SignIn()
        {
            IsValid = Validate();
            if (IsValid)
            {
                var res = await ApiManager.TryLogin(Email.Value, Password.Value);
                if (res) Navigate("MainTabbedPage?selectedPage=MainPage");
            }
            MessagingCenter.Send(this, "SignInRequested");
        }

        bool Validate()
        {
            Email.IsValid = email.Validate();
            Password.IsValid = password.Validate();
            return Email.IsValid && Password.IsValid;
        }

        void AddValidations()
        {
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "User field can not be empty" });
            email.Validations.Add(new EmailRule());
            password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password field can not be empty" });
        }
    }
}