using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XipeADNApp.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            if (Device.RuntimePlatform == Device.iOS)
            {
                var paddingTop = DeviceDisplay.MainDisplayInfo.Height > 1334 ? 55 : 35;
                var paddingLeft = DeviceDisplay.MainDisplayInfo.Height > 1334 ? 35 : 20;
                //BackContentView.Padding = new Thickness(paddingLeft, paddingTop, 0, 0);
            }
        }
    }
}