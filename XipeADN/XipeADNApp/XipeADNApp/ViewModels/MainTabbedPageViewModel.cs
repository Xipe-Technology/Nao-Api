using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XipeADNApp.ViewModels
{
	public class MainTabbedPageViewModel : ViewModelBase
	{
		public MainTabbedPageViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
			Title = "MainTabbedPage";
		}
	}
}