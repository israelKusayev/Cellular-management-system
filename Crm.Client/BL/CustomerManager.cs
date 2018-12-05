using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.ClientsModels;
using Common.Models;
using Crm.Client.Converters;
using Newtonsoft.Json;

namespace Crm.Client.BL
{
    class CustomerManager
    {
        private readonly string _url = Config.Default.BaseUrl;
        private readonly Employee _currentEmployee;

        // ctor
        public CustomerManager(Employee employee)
        {
            _currentEmployee = employee;
            string url = Config.Default.BaseUrl;
        }

        /// <summary>
        /// Get Customer from api
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Founded customer or null</returns>
        internal Customer GetCustomerByIdCard(string idCard)
        {
            Customer customer;
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/customer/{idCard}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        customer = result.Content.ReadAsAsync<Customer>().Result;
                    }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                        return null;//?
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server error");
                return null;
            }
            return customer;
        }

        /// <summary>
        /// Get customer value from api
        /// </summary>
        /// <param name="idCard">customer identity card</param>
        /// <returns>customer value if customer exists otherwise -1</returns>
        internal double GetCustomerValue(string idCard)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/customer/customerValue/{idCard}").Result;

                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsAsync<double>().Result;
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server error");
            }
            return -1;
        }

        /// <summary>
        /// Add or edit customer dependent if exists or not
        /// </summary>
        /// <param name="customer">Customer to add or edit</param>
        internal void SaveCustomer(Customer customer)
        {
            customer.EmplyeeId = _currentEmployee.EmployeeId;
            try
            {
                using (var http = new HttpClient())
                {
                    var httpResponse = http.GetAsync($"{_url}/crm/customer/{customer.IdentityCard}").Result;

                    if (httpResponse.IsSuccessStatusCode)// this customer already exists
                    {
                        customer.CustomerId = httpResponse.Content.ReadAsAsync<Customer>().Result.CustomerId;
                        EditCustomer(http, customer);
                    }
                    else
                    {
                        AddCustomer(http, customer);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server error");
            }
        }

        /// <summary>
        /// Sending delete cutomer request to api
        /// </summary>
        /// <param name="idCard">customer identity card</param>
        internal void DeleteCustomer(string idCard)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.DeleteAsync($"{_url}/crm/Customer/{idCard}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Customer deleted successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Server error");
            }
        }

        /// <summary>
        /// Sending add customer request to api
        /// </summary>
        /// <param name="http">Http conecction</param>
        /// <param name="customer">Customer to add</param>
        private void AddCustomer(HttpClient http, Customer customer)
        {
            var result = http.PostAsJsonAsync($"{_url}/crm/Customer", customer).Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show(Application.Current.MainWindow, "customer added successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Sending edit customer request to api
        /// </summary>
        /// <param name="http">Http conecction</param>
        /// <param name="customer">Customer to edit</param>
        private void EditCustomer(HttpClient http, Customer customer)
        {
            var result = http.PutAsJsonAsync($"{_url}/crm/customer", customer).Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show(Application.Current.MainWindow, "customer edit successfully.", "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
