using Bi.Client.Halpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Client.ViewModels
{
    public class LoginViewModel : ViewModelPropertyChanged
    {
        private IFrameNavigationService _frameNavigationService;

        public LoginViewModel(IFrameNavigationService frameNavigationService)
        {
            _frameNavigationService = frameNavigationService;
        }
        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }


        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

    }
}
