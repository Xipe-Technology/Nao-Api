using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.Generic;
using System.Text;

namespace XipeADNApp.ViewModels
{
    public class ViewModelBase : BindableBase, INavigationAware, IDestructible
    {
        protected INavigationService NavigationService { get; private set; }
        protected IApiManager ApiManager { get; private set; }
        public DelegateCommand<String> NavigateCommand { get; private set; }
        public DelegateCommand<String> ModalNavigateCommand { get; private set; }
        public DelegateCommand<String> AbsoluteNavigationCommand { get; private set; }
        public DelegateCommand BackCommand { get; private set; }
        public DelegateCommand PopToRootCommand { get; private set; }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public ViewModelBase(INavigationService navigationService, IApiManager apiManager)
        {
            NavigationService = navigationService;
            ApiManager = apiManager;
            NavigateCommand = new DelegateCommand<string>(Navigate);
            AbsoluteNavigationCommand = new DelegateCommand<string>(AbsoluteNavigation);
            BackCommand = new DelegateCommand(NavigateBack);
            PopToRootCommand = new DelegateCommand(PopToRoot);
            ModalNavigateCommand = new DelegateCommand<string>(ModalNavigate);
        }

        public async void Navigate(String navigateTo) =>
            await NavigationService.NavigateAsync(navigateTo);

        public async void ModalNavigate(String navigateTo) =>
            await NavigationService.NavigateAsync(navigateTo, null, true);

        public async void NavigateBack() =>
            await NavigationService.GoBackAsync();

        public async void PopToRoot() =>
           await NavigationService.GoBackToRootAsync();


        public async void AbsoluteNavigation(String navigateTo) =>
            await NavigationService.NavigateAsync(new System.Uri(navigateTo, System.UriKind.Absolute));

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatingTo(INavigationParameters parameters)
        {

        }

        public virtual void Destroy()
        {

        }
    }
}
