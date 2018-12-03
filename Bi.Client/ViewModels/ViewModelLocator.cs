using Bi.Client.Halpers;
using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bi.Client.ViewModels
{
    public class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<ReportsViewModel>();
            SetupNavigation();
        }


        public static void UnRegisterReportsViewModel()
        {
            SimpleIoc.Default.Unregister<ReportsViewModel>();
            SimpleIoc.Default.Register<ReportsViewModel>();
        }

        public static void UnRegisterLoginViewModel()
        {
            SimpleIoc.Default.Unregister<LoginViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }


        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Login", new Uri("../Views/LoginView.xaml", UriKind.Relative));
            navigationService.Configure("Reports", new Uri("../Views/ReportsView.xaml", UriKind.Relative));
            SimpleIoc.Default.Register<IFrameNavigationService>(() => navigationService);
        }

        public MainViewModel Main
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainViewModel>();
            }
        }

        public LoginViewModel LoginViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LoginViewModel>();
            }
        }

        public ReportsViewModel ReportsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReportsViewModel>();
            }
        }



        /// <summary>
        /// Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}
