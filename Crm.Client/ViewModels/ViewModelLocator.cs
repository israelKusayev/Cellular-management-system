using CommonServiceLocator;
using Crm.Client.Helpers;
using GalaSoft.MvvmLight.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Client.ViewModels
{
    class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
            SimpleIoc.Default.Register<CustomerDetailsViewModel>();
            SimpleIoc.Default.Register<LineViewModel>();
            SimpleIoc.Default.Register<SimulatorViewModel>();
            SimpleIoc.Default.Register<ReceiptsViewModel>();
            SetupNavigation();
        }


        public static void UnRegisterCustomerDetailsViewModel()
        {
            SimpleIoc.Default.Unregister<CustomerDetailsViewModel>();
            SimpleIoc.Default.Register<CustomerDetailsViewModel>();
        }
        public static void UnRegisterLineViewModel()
        {
            SimpleIoc.Default.Unregister<LineViewModel>();
            SimpleIoc.Default.Register<LineViewModel>();
        }
        public static void UnRegisterLoginViewModel()
        {
            SimpleIoc.Default.Unregister<LoginViewModel>();
            SimpleIoc.Default.Register<LoginViewModel>();
        }
        public static void UnRegisterSimulatorViewModel()
        {
            SimpleIoc.Default.Unregister<SimulatorViewModel>();
            SimpleIoc.Default.Register<SimulatorViewModel>();
        }
        public static void UnRegisterReceiptsViewModel()
        {
            SimpleIoc.Default.Unregister<ReceiptsViewModel>();
            SimpleIoc.Default.Register<ReceiptsViewModel>();
        }


        private static void SetupNavigation()
        {
            var navigationService = new FrameNavigationService();
            navigationService.Configure("Login", new Uri("../Views/LoginView.xaml", UriKind.Relative));
            navigationService.Configure("CustomerDetails", new Uri("../Views/CustomerDetailsView.xaml", UriKind.Relative));
            navigationService.Configure("AddLines", new Uri("../Views/AddLinesView.xaml", UriKind.Relative));
            navigationService.Configure("ManageLines", new Uri("../Views/ManageLinesView.xaml", UriKind.Relative));
            navigationService.Configure("Simulator", new Uri("../Views/SimulatorView.xaml", UriKind.Relative));
            navigationService.Configure("ChooseReceipts", new Uri("../Views/ChooseReceiptsView.xaml", UriKind.Relative));
            navigationService.Configure("Receipts", new Uri("../Views/ReceiptsView.xaml", UriKind.Relative));
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

        public CustomerDetailsViewModel CustomerDetailsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<CustomerDetailsViewModel>();
            }
        }

        public LineViewModel LineViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<LineViewModel>();
            }
        }

        public SimulatorViewModel SimulatorViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SimulatorViewModel>();
            }
        }

        public ReceiptsViewModel ReceiptsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ReceiptsViewModel>();
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
