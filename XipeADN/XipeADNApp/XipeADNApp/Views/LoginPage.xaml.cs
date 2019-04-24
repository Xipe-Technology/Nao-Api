using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using XipeADNApp.ViewModels;

namespace XipeADNApp.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            MessagingCenter.Subscribe<LoginViewModel>(this, "SignInRequested", OnSignInRequested);
        }

        void OnSignInRequested(LoginViewModel loginViewModel)
        {
            if (!loginViewModel.IsValid)
            {
                if (!string.IsNullOrEmpty(loginViewModel.Email.Value))
                    EmailEntry.TextColor = loginViewModel.Email.IsValid ? Color.White : Color.Red;
                if (!string.IsNullOrEmpty(loginViewModel.Password.Value))
                    PasswordEntry.TextColor = loginViewModel.Password.IsValid ? Color.White : Color.Red;
            }
        }
    }
}