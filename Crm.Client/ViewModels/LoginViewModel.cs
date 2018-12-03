using Common.Models;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;


namespace Crm.Client.ViewModels
{
    class LoginViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly LoginManager _loginManager;

        public string Username { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; set; }
        public ICommand SimulateCommand { get; set; }
        public ICommand ReceiptsCommand { get; set; }

        private Visibility _loadingVisibility;
        public Visibility LoadingVisibility
        {
            get { return _loadingVisibility; }
            set
            {
                _loadingVisibility = value;
                OnPropertyChanged();
            }
        }

        public LoginViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _loginManager = new LoginManager();
            LoginCommand = new RelayCommand(Login);
            SimulateCommand = new RelayCommand(GoToSimulator);
            ReceiptsCommand = new RelayCommand(GoToReceipts);
            LoadingVisibility = Visibility.Collapsed;
        }

        private void GoToReceipts()
        {
            _navigationService.NavigateTo("ChooseReceipts");
        }

        private void GoToSimulator()
        {
            _navigationService.NavigateTo("Simulator");
            ViewModelLocator.UnRegisterLoginViewModel();
        }

        private bool CanLogin()
        {
            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }
            return true;
        }

        private async void Login()
        {
            if (!CanLogin())
            {
                MessageBox.Show("All fields are required.");
                return;
            }
            LoadingVisibility = Visibility.Visible;

            if (await _loginManager.Login(Username, Password))
            {
                ViewModelLocator.UnRegisterLoginViewModel();
                _navigationService.NavigateTo("CustomerDetails",_loginManager._currentEmployee);
            }
            LoadingVisibility = Visibility.Collapsed;

        }
    }
}
