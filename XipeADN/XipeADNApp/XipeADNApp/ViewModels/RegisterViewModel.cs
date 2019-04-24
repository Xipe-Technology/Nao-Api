using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XipeADNApp.Validations;
using XipeServices.Validations;

namespace XipeADNApp.ViewModels
{
	public class RegisterViewModel : ViewModelBase
	{
        string checkBoxImage = "CheckBoxOff.png";
        ValidatableObject<string> email;
        ValidatableObject<string> fullName;
        ValidatableObject<string> password;
        ValidatableObject<string> passwordConfirm;
        ValidatableObject<bool> acceptTerms;

        public string CheckBoxImage
        { get => checkBoxImage; set => SetProperty(ref checkBoxImage, value); }
        public ValidatableObject<string> Email
        { get => email; set => SetProperty(ref email, value); }
        public ValidatableObject<string> FullName
        { get => fullName; set => SetProperty(ref fullName, value); }
        public ValidatableObject<string> Password
        { get => password; set => SetProperty(ref password, value); }
        public ValidatableObject<string> PasswordConfirm
        { get => passwordConfirm; set => SetProperty(ref passwordConfirm, value); }
        public ValidatableObject<bool> AcceptTerms
        { get => acceptTerms; set => SetProperty(ref acceptTerms, value); }

        public DelegateCommand RegisterCommand => new DelegateCommand(Register);
        public DelegateCommand ToggleCheckBoxCommand => new DelegateCommand(ToggleCheckBox);
        public bool IsValid { get; set; }

        public RegisterViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
			Title = "Register";
            email = new ValidatableObject<string>();
            fullName = new ValidatableObject<string>();
            password = new ValidatableObject<string>();
            passwordConfirm = new ValidatableObject<string>();
            acceptTerms = new ValidatableObject<bool>();
            AddValidations();
        }

        async void Register()
        {
            IsValid = Validate();
            if (IsValid && AcceptTerms.Value)
            {
                var res = await ApiManager.TryRegister(Email.Value, FullName.Value, Password.Value);
                if (res) NavigateBack();
            }
            // MessagingCenter.Send(this, "SignInRequested");
        }

        void ToggleCheckBox()
        {
            AcceptTerms.Value = !AcceptTerms.Value;
            CheckBoxImage = AcceptTerms.Value ? "CheckBoxOn.png" : "CheckBoxOff.png";
        }

        bool Validate()
        {
            Email.IsValid = email.Validate();
            FullName.IsValid = fullName.Validate();
            Password.IsValid = password.Validate();
            PasswordConfirm.IsValid = passwordConfirm.Validate();
            // AcceptTerms.IsValid = acceptTerms.Validate();

            return Email.IsValid && FullName.IsValid && Password.IsValid &&
                PasswordConfirm.IsValid;// && AcceptTerms.IsValid;
        }

        void AddValidations()
        {
            email.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "User field can not be empty." });
            email.Validations.Add(new EmailRule());
            fullName.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Fullname field can not be empty." });
            password.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Password field can not be empty." });
            password.Validations.Add(new PasswordRule { ValidationMessage = "Password must have at least 6 alphanumeric characters." });
            passwordConfirm.Validations.Add(new IsNotNullOrEmptyRule<string> { ValidationMessage = "Confirm Password field cannot be empty." });
            passwordConfirm.Comparison.Add(new ComparePasswordsRule { ValidationMessage = "Passwords do not match.", ValueToCompare = Password });
            // acceptTerms.Validations.Add(new IsNotNullOrEmptyRule<bool> { ValidationMessage = "AcceptTerms field can not be empty." });
        }
    }
}