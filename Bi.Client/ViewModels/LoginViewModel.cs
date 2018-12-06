using Bi.Client.BL;
using Bi.Client.Halpers;
using Bi.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Bi.Client.ViewModels
{
    public class LoginViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly LoginManager _loginManager;

        public string Username { get; set; }
        public string Password { get; set; }
        public ICommand LoginCommand { get; set; }

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
            LoadingVisibility = Visibility.Collapsed;
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
                _navigationService.NavigateTo("Reports");
            }
            LoadingVisibility = Visibility.Collapsed;

        }
    }
}
