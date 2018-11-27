using Crm.Client.Helpers;
using Crm.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Common.Models;
using Common.Enums;
using System.Net.Http;
using Newtonsoft.Json;
using Crm.Client.BL;

namespace Crm.Client.ViewModels
{
    class CustomerDetailsViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly CustomerManager _customerManager;

        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand SearchCommand { get; set; }
        public ICommand LogoutCommand { get; set; }
        public ICommand LinesCommand { get; set; }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private string _firstName;
        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged();
            }
        }

        private string _lastName;
        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }

        private string _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
                OnPropertyChanged();
            }
        }

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                _address = value;
                OnPropertyChanged();
            }
        }

        private string _contactNumber;
        public string ContactNumber
        {
            get { return _contactNumber; }
            set
            {
                _contactNumber = value;
                OnPropertyChanged();
            }
        }

        private List<string> _types;
        public List<string> Types
        {
            get { return _types; }
            set
            {
                _types = value;
                OnPropertyChanged();
            }
        }

        private int _selectedClientType;
        public int SelectedClientType
        {
            get { return _selectedClientType; }
            set
            {
                _selectedClientType = value;
                OnPropertyChanged();
            }
        }

        private string _customerValue;
        public string CustomerValue
        {
            get
            {
                return _customerValue;
            }
            set
            {
                _customerValue = value;
                OnPropertyChanged();
            }
        }


        // ctor
        public CustomerDetailsViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _customerManager = new CustomerManager((Employee)_navigationService.Parameter);
            ClearCommand = new RelayCommand(Clear);
            SaveCommand = new RelayCommand(SaveCustomer);
            DeleteCommand = new RelayCommand(DeleteCustomer);
            SearchCommand = new RelayCommand(Search);
            LogoutCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("Login");
                ViewModelLocator.UnRegisterCustomerDetailsViewModel();
            });
            LinesCommand = new RelayCommand(GoToLines);
            Types = Enum.GetNames(typeof(CustomerTypeEnum)).ToList();
            CustomerValue = "-";
        }

        /// <summary>
        /// Navigate to line view(next page) if Id field matching to customer in DB
        /// </summary>
        private void GoToLines()
        {
            if (string.IsNullOrWhiteSpace(Id))
            {
                MessageBox.Show("id field is required.");
            }
            else
            {
                Customer customer = _customerManager.GetCustomerByIdCard(Id);
                if (customer != null)
                {
                    _navigationService.NavigateTo("Line", customer);
                }
            }

        }

        private void SaveCustomer()
        {
            if (CanSaveCustomer())
            {
                Customer customer = new Customer()
                {
                    FirstName = FirstName,
                    LastName = LastName,
                    Address = Address,
                    IdentityCard = Id,
                    ContactNumber = ContactNumber,
                    CustomerTypeId = SelectedClientType + 1
                };
                _customerManager.SaveCustomer(customer);
            }
        }

        private bool CanSaveCustomer()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) || string.IsNullOrWhiteSpace(Address) || string.IsNullOrWhiteSpace(Id) || string.IsNullOrWhiteSpace(ContactNumber))
            {
                MessageBox.Show(Application.Current.MainWindow, "All fields are requierd !");
                return false;
            }
            if (!int.TryParse(Id, out int id))
            {
                MessageBox.Show(Application.Current.MainWindow, "Id must be a number !");
                return false;
            }
            if (!int.TryParse(ContactNumber, out int contactNumber))
            {
                MessageBox.Show(Application.Current.MainWindow, "Contact number must be a number !");
                return false;
            }
            return true;
        }

        private void DeleteCustomer()
        {
            if (String.IsNullOrWhiteSpace(Id))
            {
                MessageBox.Show("id field is required.");
            }
            else
            {
                _customerManager.DeleteCustomer(Id);
            }
        }

        /// <summary>
        /// Search customer by customer identity card
        /// </summary>
        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                MessageBox.Show("You must write an Id card.");
            }
            else
            {
                Customer customer = _customerManager.GetCustomerByIdCard(SearchText);
                double customerValue = _customerManager.GetCustomerValue(SearchText);
                CustomerValue = customerValue == -1 ? "-" : customerValue.ToString();
                if (customer != null)
                {
                    UpdateFields(customer);
                }
            }
        }

        private void UpdateFields(Customer customer)
        {
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Address = customer.Address;
            Id = customer.IdentityCard;
            ContactNumber = customer.ContactNumber;
            SelectedClientType = customer.CustomerTypeId - 1;
        }

        // Clear all the text
        private void Clear()
        {
            SearchText = "";
            FirstName = "";
            LastName = "";
            Id = "";
            Address = "";
            ContactNumber = "";
            CustomerValue = "-";
            SelectedClientType = 0;
        }
    }
}
