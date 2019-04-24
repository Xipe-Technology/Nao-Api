using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace XipeADNApp.ViewModels
{
	public class BalanceHistoryViewModel : ViewModelBase
	{
        public ObservableCollection<object> HistoryList { get; set; }

		public BalanceHistoryViewModel(INavigationService navigationService, IApiManager apiManager)
			: base(navigationService, apiManager)
		{
			Title = "BalanceHistory";
		}
	}
}