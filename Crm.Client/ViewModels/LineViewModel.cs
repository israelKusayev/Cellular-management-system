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
    /// <summary>
    /// View model for AddLinesView and for ManageLinesView
    /// </summary>
    class LineViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly LineManager _lineManager;
        private readonly Customer _currentCustomer;

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

        #region commands
        public ICommand AddCommand { get; set; }
        public ICommand EditPackageCommand { get; set; }
        public ICommand RemovePackageCommand { get; set; }
        public ICommand DeleteLineCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand LineSelectedCommand { get; set; }
        public ICommand PackageSelectedCommand { get; set; }
        public ICommand UpdateTotalPriceCommand { get; set; }
        #endregion

        #region props
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
        }

        private int? _selectedPackage;
        public int? SelectedPackage
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
                if (value == true)
                {
                    LineWithoutPackage = false;
                }
                OnPropertyChanged();
            }
        }

        private bool _lineWithoutPackage;
        public bool LineWithoutPackage
        {
            get { return _lineWithoutPackage; }
            set
            {
                if (value == true)
                {
                    IsPackage = false;
                }
                _lineWithoutPackage = value;
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

        #endregion

        // ctor
        public LineViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _currentCustomer = (Customer)_navigationService.Parameter;//?
            _lineManager = new LineManager((Customer)_navigationService.Parameter);

            InitCommands();

            CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            PackageTemplates = _lineManager.GetPackageTemplates();
            PackageTemplates.Add(new Package() { PackageName = "Custom package", PackageId = 0 });
            LineWithoutPackage = true;
        }

        /// <summary>
        /// Initialize commands
        /// </summary>
        private void InitCommands()
        {
            ClearCommand = new RelayCommand(Clear);
            AddCommand = new RelayCommand(() =>
            {
                if (SelectedPackage == null && IsPackage)
                {
                    MessageBox.Show("Please select package");
                }
                else
                {
                    AddLine();
                }
            });
            EditPackageCommand = new RelayCommand(() =>
            {
                if (SelectedPackage == null)
                {
                    MessageBox.Show("Please select package");
                }
                else
                {
                    EditLine();
                }
            });
            RemovePackageCommand = new RelayCommand(RemovePackage);
            DeleteLineCommand = new RelayCommand(DeleteLine);
            GoBackCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("CustomerDetails");
                ViewModelLocator.UnRegisterLineViewModel();
            });
            LineSelectedCommand = new RelayCommand(HandleLineSelect);
            PackageSelectedCommand = new RelayCommand(HandlePackageSelect);
            UpdateTotalPriceCommand = new RelayCommand(UpdateTotalPrice);
        }

        /// <summary>
        /// remove package from exists line
        /// </summary>
        private void RemovePackage()
        {
            _lineManager.DeletePackage(SelectedLine ?? -1);
            SelectedPackage = null;
            Clear();
        }

        /// <summary>
        /// Add Line to customer
        /// </summary>
        private void AddLine()
        {
            if (ValidateLineNumber())// if line number is valid
            {
                if (LineWithoutPackage)
                {
                    // add line without package
                    _lineManager.AddLine(Number, _currentCustomer.CustomerId);
                }
                else
                {
                    // add line with package
                    if (_lineManager.AddLine(Number, _currentCustomer.CustomerId))
                    {
                        AddPackageToLine();
                    }
                }
                CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            }
        }

        /// <summary>
        /// Edit package to exists line
        /// </summary>
        private void EditLine()
        {
            if (SelectedLine == null)
            {
                MessageBox.Show("Please select line");
            }
            else
            {

                Number = _lineManager.GetLineNumber(SelectedLine.Value);
                Package package = _lineManager.GetLinePackage(SelectedLine.Value);

                if (package == null)
                {
                    //add package to exists line
                    AddPackageToLine();
                }
                else
                {
                    //edit package to exists line

                    EditPackageToExistingLine();
                }
                CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            }
        }

        private void EditPackageToExistingLine()
        {
            if (SelectedPackage != 0) // template package
            {
                _lineManager.EditPackage(Number, PackageTemplates.SingleOrDefault(p => p.PackageId == SelectedPackage));
            }
            else // custom package
            {
                string error = ValidateFields();
                if (error != null)
                {
                    MessageBox.Show(error);
                    return;
                }
                Package package = CreateCustomPackage();

                Package newPackage = _lineManager.EditPackage(Number, package);
                if (newPackage != null)
                {
                    Friends friends = CreatePackageFriends();

                    if (newPackage.Friends != null)
                    {
                        _lineManager.EditFriends(newPackage.PackageId, Friends ? friends : null);
                    }
                    else if (Friends) // if the friends check box is true
                    {
                        _lineManager.AddFriends(newPackage.PackageId, friends);
                    }
                }
            }
        }

        /// <summary>
        /// Delete line
        /// </summary>
        private void DeleteLine()
        {
            if (SelectedLine == null)
            {
                MessageBox.Show("Please select line");
            }
            else
            {
                _lineManager.DeleteLine(SelectedLine ?? -1);
                SelectedLine = null;
                SelectedPackage = null;
                Clear();
                CustomerLines = new ObservableCollection<Line>(_lineManager.GetCustomerLines());
            }
        }

        /// <summary>
        /// Add custom or template package to exists line
        /// </summary>
        private void AddPackageToLine()
        {
            if (SelectedPackage != 0) // template package
            {
                _lineManager.AddPackage(Number, PackageTemplates.SingleOrDefault(p => p.PackageId == SelectedPackage));
            }
            else // custom package
            {
                string error = ValidateFields();
                if (error != null)
                {
                    MessageBox.Show(error);
                    return;
                }
                Package package = CreateCustomPackage();

                Package newPackage = _lineManager.AddPackage(Number, package);
                if (newPackage != null)
                {
                    if (Friends) // if the friends check box is true
                    {
                        Friends friends = CreatePackageFriends();
                        _lineManager.AddFriends(newPackage.PackageId, friends);
                    }
                }
            }
        }

        /// <summary>
        /// Create new friends model from props binding
        /// </summary>
        /// <returns>Friends model</returns>
        private Friends CreatePackageFriends()
        {
            return new Friends()
            {
                FirstNumber = string.IsNullOrWhiteSpace(Friend1) ? null : Friend1,
                SecondNumber = string.IsNullOrWhiteSpace(Friend2) ? null : Friend2,
                ThirdNumber = string.IsNullOrWhiteSpace(Friend3) ? null : Friend3,
            };
        }

        /// <summary>
        /// Create new package model from props binding
        /// </summary>
        /// <returns>Package model</returns>
        private Package CreateCustomPackage()
        {
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
            return package;
        }

        /// <summary>
        ///  Get selected line number and package
        /// </summary>
        private void HandleLineSelect()
        {
            if (SelectedLine != null)
            {
                Clear();
                Number = _lineManager.GetLineNumber((int)SelectedLine);
                Package package = _lineManager.GetLinePackage((int)SelectedLine);
                if (package != null)
                {
                    SelectedPackage = package.IsPackageTemplate ? package.PackageId : 0;
                    DisplayPackage(package);
                }
                else
                {
                    SelectedPackage = null;
                }

            }
        }

        /// <summary>
        /// Update the view with selected package details
        /// </summary>
        private void HandlePackageSelect()
        {
            if (SelectedPackage != null)
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
        }

        /// <summary>
        /// Display the given package on view
        /// </summary>
        /// <param name="package"></param>
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

        /// <summary>
        /// update the total price, if the customer choose special deals or not 
        /// </summary>
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

        /// <summary>
        ///  validate the line number
        /// </summary>
        /// <returns>true if valid otherwise false</returns>
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

        /// <summary>
        ///  validate all package details
        /// </summary>
        /// <returns>true if valid otherwise false</returns>
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

        /// <summary>
        ///  clear the view
        /// </summary>
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
