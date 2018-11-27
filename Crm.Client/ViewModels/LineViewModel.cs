using Common.DataConfig;
using Common.Models;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Crm.Client.ViewModels
{
    class LineViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly LineManager _lineManager;
        private readonly Customer _currentCustomer;
        public ICommand SaveCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand LineSelectedCommand { get; set; }
        public ICommand PackageSelectedCommand { get; set; }
        public ICommand UpdateTotalPriceCommand { get; set; }

        public List<Package> PackageTemplates { get; set; }

        private ObservableCollection<Line> _customerLines;
        public ObservableCollection<Line> CustomerLines
        {
            get
            {
                return _customerLines;
            }
            set
            {
                _customerLines = value;
                OnPropertyChanged();
            }
        }

        private string _number;
        public string Number
        {
            get { return _number; }
            set
            {
                _number = value;
                OnPropertyChanged();
            }
        }

        private string _minute;
        public string Minute
        {
            get { return _minute; }
            set
            {
                _minute = value;
                OnPropertyChanged();
            }
        }

        private string _sms;
        public string Sms
        {
            get { return _sms; }
            set
            {
                _sms = value;
                OnPropertyChanged();
            }
        }

        private string _price;
        public string Price
        {
            get { return _price; }
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        private string _friend1;
        public string Friend1
        {
            get { return _friend1; }
            set
            {
                _friend1 = value;
                OnPropertyChanged();
            }
        }

        private string _friend2;
        public string Friend2
        {
            get { return _friend2; }
            set
            {
                _friend2 = value;
                OnPropertyChanged();
            }
        }

        private string _friend3;
        public string Friend3
        {
            get { return _friend3; }
            set
            {
                _friend3 = value;
                OnPropertyChanged();
            }
        }

        private bool _friends;
        public bool Friends
        {
            get { return _friends; }
            set
            {
                _friends = value;
                OnPropertyChanged();
            }
        }

        private bool _insideFamilyCalles;
        public bool InsideFamilyCalles
        {
            get { return _insideFamilyCalles; }
            set
            {
                _insideFamilyCalles = value;
                OnPropertyChanged();
            }
        }

        private bool _priorityContact;
        public bool PriorityContact
        {
            get { return _priorityContact; }
            set
            {
                _priorityContact = value;
                OnPropertyChanged();
            }
        }

        private int? _selectedLine;
        public int? SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                OnPropertyChanged();
            }
        }// need propf??

        private int _selectedPackage;
        public int SelectedPackage
        {
            get { return _selectedPackage; }
            set
            {
                _selectedPackage = value;
                OnPropertyChanged();
            }
        }

        private bool _isPackage;
        public bool IsPackage
        {
            get { return _isPackage; }
            set
            {
                _isPackage = value;
                OnPropertyChanged();
            }
        }

        private double _totalPrice;
        public double TotalPrice
        {
            get { return _totalPrice; }
            set
            {
                _totalPrice = value;
                OnPropertyChanged();
            }
        }

        // ctor
        public LineViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _currentCustomer = (Customer)_navigationService.Parameter;//?
            _lineManager = new LineManager((Customer)_navigationService.Parameter);
            ClearCommand = new RelayCommand(Clear);
            SaveCommand = new RelayCommand(Save);
            DeleteCommand = new RelayCommand(DeleteLine);
            GoBackCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("CustomerDetails");
                ViewModelLocator.UnRegisterLineViewModel();
            });
            LineSelectedCommand = new RelayCommand(HandleLineSelect);
            PackageSelectedCommand = new RelayCommand(HandlePackageSelect);
            UpdateTotalPriceCommand = new RelayCommand(UpdateTotalPrice);
            CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            PackageTemplates = _lineManager.GetPackageTemplates();
            PackageTemplates.Add(new Package() { PackageName = "Custom package", PackageId = 0 });
        }

        // get selected line number and package
        private void HandleLineSelect()
        {
            if (SelectedLine != null)
            {
                Clear();
                Number = _lineManager.GetLineNumber((int)SelectedLine);
                Package package = _lineManager.GetLinePackage((int)SelectedLine);
                if (package != null)
                {
                    IsPackage = true;
                    if (package.IsPackageTemplate)
                    {
                        SelectedPackage = package.PackageId;
                    }
                    else
                    {
                        SelectedPackage = 0;
                    }
                    DisplayPackage(package);
                }
                else
                {
                    IsPackage = false;
                }
            }
        }

        // update the view with selected package details
        private void HandlePackageSelect()
        {
            string number = Number;
            Clear();
            Number = number;
            if (SelectedPackage != 0) // template package
            {
                Package package = PackageTemplates.FirstOrDefault(p => p.PackageId == SelectedPackage);
                DisplayPackage(package);
            }
        }

        // display the given package on view
        private void DisplayPackage(Package package)
        {
            Minute = package.MaxMinute.ToString();
            Sms = package.MaxSms.ToString();
            TotalPrice = package.TotalPrice;
            PriorityContact = package.PriorityContact;
            InsideFamilyCalles = package.InsideFamilyCalles;
            double price = package.TotalPrice;
            price -= package.InsideFamilyCalles ? PackagePrices.FamalyDiscountPrice : 0;
            price -= package.PriorityContact ? PackagePrices.PriorityContactPrice : 0;
            if (package.Friends != null)
            {
                Friends = true;
                Friend1 = package.Friends.FirstNumber;
                Friend2 = package.Friends.SecondNumber;
                Friend3 = package.Friends.ThirdNumber;
                price -= PackagePrices.FriendsNumbersPrice;
            }
            Price = price.ToString();
        }

        // update the total price if the customer choose special deals or not 
        private void UpdateTotalPrice()
        {
            int.TryParse(Price, out int price);
            TotalPrice = price;
            if (InsideFamilyCalles)
            {
                TotalPrice += PackagePrices.FamalyDiscountPrice;
            }
            if (Friends)
            {
                TotalPrice += PackagePrices.FriendsNumbersPrice;
            }
            if (PriorityContact)
            {
                TotalPrice += PackagePrices.PriorityContactPrice;
            }
        }

        // validate the line number
        private bool ValidateLineNumber()
        {
            if (string.IsNullOrWhiteSpace(Number))
            {
                MessageBox.Show("Number is required");
                return false;
            }
            if (!int.TryParse(Number, out int n))
            {
                MessageBox.Show("Line number must be a number");
                return false;
            }
            return true;
        }

        // validate all package details
        private string ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(Price))
                return "Price is required !";
            if (string.IsNullOrWhiteSpace(Minute) && string.IsNullOrWhiteSpace(Sms))
                return "Minute count or sms count are required !";
            if (Friends && string.IsNullOrWhiteSpace(Friend1) && string.IsNullOrWhiteSpace(Friend2) && string.IsNullOrWhiteSpace(Friend3))
                return "You must fill at least one friend field !";
            if (!int.TryParse(Price, out int price))
                return "Package price must be a number !";
            if (!string.IsNullOrWhiteSpace(Minute) && !int.TryParse(Minute, out int minute))
                return "Minute count must be a number !";
            if (!string.IsNullOrWhiteSpace(Sms) && !int.TryParse(Sms, out int sms))
                return "Sms count must be a number !";
            return null;
        }

        // add or edit line and/or package and/or friends
        private void Save()
        {
            if (ValidateLineNumber())// if line number is valid
            {
                if (!IsPackage)
                {
                    int lineId = _lineManager.GetLineId(Number);
                    Package package = _lineManager.GetLinePackage(lineId);
                    if (package == null)
                    {
                        // add line without package
                        _lineManager.AddLine(Number, _currentCustomer.CustomerId);
                    }
                    else
                    {
                        _lineManager.DeletePackage(lineId);
                    }
                }
                else
                {
                    // add line with package
                    if (SelectedPackage != 0) // template package
                    {
                        if (_lineManager.AddLine(Number, _currentCustomer.CustomerId))
                        {
                            _lineManager.SavePackage(Number, PackageTemplates.SingleOrDefault(p => p.PackageId == SelectedPackage));
                        }
                    }
                    else // custom package
                    {
                        string error = ValidateFields();
                        if (error != null)
                        {
                            MessageBox.Show(error);
                            return;
                        }
                        int.TryParse(Minute, out int minute);
                        int.TryParse(Sms, out int sms);

                        Package package = new Package()
                        {
                            PackageName = "Custom package",
                            TotalPrice = TotalPrice,
                            MaxMinute = minute,
                            MaxSms = sms,
                            InsideFamilyCalles = InsideFamilyCalles,
                            PriorityContact = PriorityContact,
                        };

                        if (_lineManager.AddLine(Number, _currentCustomer.CustomerId))
                        {
                            Package newPackage = _lineManager.SavePackage(Number, package);
                            if (newPackage != null)
                            {
                                Friends friends = new Friends()
                                {
                                    FirstNumber = string.IsNullOrWhiteSpace(Friend1) ? null : Friend1,

                                    SecondNumber = string.IsNullOrWhiteSpace(Friend2) ? null : Friend2,
                                    ThirdNumber = string.IsNullOrWhiteSpace(Friend3) ? null : Friend3,
                                };

                                if (newPackage.Friends != null)
                                {
                                    if (Friends)
                                    {
                                        _lineManager.EditFriends(newPackage.PackageId, friends);
                                    }
                                    else
                                    {
                                        _lineManager.EditFriends(newPackage.PackageId, null);
                                    }
                                }
                                else if (Friends) // if the friends check box is true
                                {
                                    _lineManager.AddFriends(newPackage.PackageId, friends);
                                }
                            }
                        }
                    }
                }
                CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());

            }
        }

        // --- //
        private void DeleteLine()
        {
            if (ValidateLineNumber())
            {
                _lineManager.DeleteLine(Number);
                SelectedLine = null;
                CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            }
        }

        // clear the view
        private void Clear()
        {
            Number = "";
            Minute = "";
            Sms = "";
            Price = "";
            Friend1 = "";
            Friend2 = "";
            Friend3 = "";
            Friends = false;
            InsideFamilyCalles = false;
            PriorityContact = false;
            TotalPrice = 0;
        }
    }
}
