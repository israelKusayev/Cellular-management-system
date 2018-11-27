using Common.ClientsModels;
using Common.Models;
using Crm.Client.Converters;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Crm.Client.BL
{
    class LineManager
    {
        private readonly string _url = Config.Default.BaseUrl;

        private List<Line> _customerLines;
        private readonly Customer _currentCustomer;


        // ctor
        public LineManager(Customer customer)
        {
            _customerLines = new List<Line>();
            _currentCustomer = customer;
            GetCustomerLinesFromDb(_currentCustomer.IdentityCard);
        }

        // get all package template for display them in the combo box
        internal List<Package> GetPackageTemplates()
        {
            List<Package> packages = new List<Package>();
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/package").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        packages = result.Content.ReadAsAsync<List<Package>>().Result;
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
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return packages;
        }

        // get all user lines
        private void GetCustomerLinesFromDb(string identityCard)//? id
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/line/{identityCard}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        if (result.StatusCode == System.Net.HttpStatusCode.NoContent)
                        {
                            // this customer has no lines
                        }
                        else
                        {
                            _customerLines = result.Content.ReadAsAsync<List<Line>>().Result;
                        }
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
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
        }

        internal ICollection<Line> GetCustomerLines()
        {
            return _customerLines;
        }

        /// <summary>
        /// get line package by line id
        /// </summary>
        /// <param name="lineId"> line id</param>
        /// <returns>package if exists or null if not</returns>
        internal Package GetLinePackage(int lineId)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/package/{lineId}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsAsync<Package>().Result;
                    }
                    else
                    {
                        //string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        //MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return null;
        }

        // add new line to customer
        internal bool AddLine(string lineNumber, int customerId)
        {
            Line line = new Line() { LineNumber = lineNumber };
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PostAsJsonAsync($"{_url}/crm/line/{customerId}", line).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        _customerLines.Add(result.Content.ReadAsAsync<Line>().Result);
                        MessageBox.Show(Application.Current.MainWindow, "Line added successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        return true;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.Found)
                    {
                        // line exists in customer lines
                        // return true for continue to add package to this line
                        return true;
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
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return false;

        }

        // add new package or edit exists package
        internal Package SavePackage(string number, Package newPackage)
        {
            int lineId = GetLineId(number);
            if (lineId == 0) MessageBox.Show("something went wrong");
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/crm/package/{lineId}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Package oldPackage = result.Content.ReadAsAsync<Package>().Result;
                        return EditPackage(newPackage, oldPackage, lineId, http);
                    }
                    else
                    {
                        return AddPackage(newPackage, lineId, http);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return null;
        }

        // edit exists package
        private Package EditPackage(Package package, Package oldPackage, int lineId, HttpClient http)
        {
            var result = http.PutAsJsonAsync($"{_url}/crm/package/{oldPackage.PackageId}/{lineId}", package).Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show(Application.Current.MainWindow, "Package edited successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return result.Content.ReadAsAsync<Package>().Result;
            }
            else
            {
                string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // add new package
        private Package AddPackage(Package package, int lineId, HttpClient http)
        {
            var result = http.PostAsJsonAsync($"{_url}/crm/package/{lineId}", package).Result;
            if (result.IsSuccessStatusCode)
            {
                MessageBox.Show(Application.Current.MainWindow, "Package added successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return result.Content.ReadAsAsync<Package>().Result;
            }
            else
            {
                string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        // add friends model to exists package
        internal void AddFriends(int packageId, Friends friends)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PostAsJsonAsync($"{_url}/crm/package/friends/{packageId}", friends).Result;
                    if (result.IsSuccessStatusCode) { }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
        }

        // edit friends model to exists package
        internal void EditFriends(int packageId, Friends friends)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PutAsJsonAsync($"{_url}/crm/package/friends/{packageId}", friends).Result;
                    if (result.IsSuccessStatusCode) { }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
        }

        // get line number by line id
        internal string GetLineNumber(int lineId)
        {
            return _customerLines.SingleOrDefault(l => l.LineId == lineId).LineNumber ?? null;
        }

        /// <summary>
        /// Get line id by line number
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <returns>return line id or -1 if not found</returns>
        internal int GetLineId(string lineNumber)
        {
            var line = _customerLines.FirstOrDefault(l => l.LineNumber == lineNumber);
            if (line != null)
            {
                return line.LineId;
            }
            return -1;
        }

        /// <summary>
        /// Delete exists line
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        internal void DeleteLine(string lineNumber)
        {
            int lineId = GetLineId(lineNumber);
            if (lineId != -1)
            {
                try
                {
                    using (var http = new HttpClient())
                    {
                        var result = http.DeleteAsync($"{_url}/crm/line/{lineId}").Result;
                        if (result.IsSuccessStatusCode)
                        {
                            _customerLines.Remove(_customerLines.SingleOrDefault(l => l.LineId == lineId));
                            MessageBox.Show(Application.Current.MainWindow, "Line deleted successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        else
                        {
                            string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                            MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Server error");
                }
            }
        }

        /// <summary>
        /// Delete package in line
        /// </summary>
        /// <param name="lineId">Line id</param>
        internal void DeletePackage(int lineId)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.DeleteAsync($"{_url}/crm/package/{lineId}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        _customerLines.Remove(_customerLines.SingleOrDefault(l => l.LineId == lineId));
                        MessageBox.Show(Application.Current.MainWindow, "Package deleted successfully", "", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        MessageBox.Show(Application.Current.MainWindow, message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
        }
    }
}
