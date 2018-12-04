using Bi.Client.Halpers;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bi.Client.ViewModels
{
    public class ReportsViewModel : ViewModelPropertyChanged
    {
        private IFrameNavigationService _frameNavigationService;


        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged();
            }
        }
        private ObservableCollection<Employee> _employees;

        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                OnPropertyChanged();
            }
        }


        public ReportsViewModel(IFrameNavigationService frameNavigationService)
        {
            _frameNavigationService = frameNavigationService;
            Customers = new ObservableCollection<Customer>()
            {
                new Customer() { IdentityCard = "1234", FirstName = "israel" },
                new Customer() { IdentityCard = "5678", FirstName = "shay" }
            };
            Employees = new ObservableCollection<Employee>()
            {
                new Employee(){UserName ="admin", Password ="333"},
                new Employee(){UserName ="------", Password ="333"}
            };

        }
    }
}
