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

        /// <summary>
        ///  Get all package template for display them in the combo box
        /// </summary>
        /// <returns><list type="Package" </returns>
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

        /// <summary>
        /// get all user lines
        /// </summary>
        /// <param name="identityCard">Customer identity card</param>
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
        /// Get line package by line id
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

        /// <summary>
        ///  Add new line to customer
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="customerId">Customer id</param>
        /// <returns>true if succeeded otherwise fasle</returns>
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

        /// <summary>
        /// Edit exists package
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="newPackage">new package to replace with old package</param>
        /// <returns></returns>
        internal Package EditPackage(string lineNumber, Package newPackage)
        {
            int lineId = GetLineId(lineNumber);
            if (lineId == 0) MessageBox.Show("something went wrong");
            try
            {
                using (var http = new HttpClient())
                {

                    Package oldPackage = http.GetAsync($"{_url}/crm/package/{lineId}").Result.Content.ReadAsAsync<Package>().Result;
                    var result = http.PutAsJsonAsync($"{_url}/crm/package/{oldPackage.PackageId}/{lineId}", newPackage).Result;
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
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return null;
        }

        /// <summary>
        ///  Add new package
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="newPackage">New package to add</param>
        /// <returns>package if secceeded otherwise null</returns>
        internal Package AddPackage(string lineNumber, Package newPackage)
        {
            int lineId = GetLineId(lineNumber);
            if (lineId == 0) MessageBox.Show("something went wrong");
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PostAsJsonAsync($"{_url}/crm/package/{lineId}", newPackage).Result;
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
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Server error");
            }
            return null;
        }

        /// <summary>
        ///  Add friends model to exists package
        /// </summary>
        /// <param name="packageId">Package id</param>
        /// <param name="friends">Friend model to add </param>
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

        /// <summary>
        /// Edit friends model to exists package
        /// </summary>
        /// <param name="packageId">Package id</param>
        /// <param name="friends">Friend model to edit </param>
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

        /// <summary>
        ///  Get line number by line id
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns></returns>
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
        internal void DeleteLine(int lineId)
        {
            if (lineId > 0)
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
                        //_customerLines.Remove(_customerLines.SingleOrDefault(l => l.LineId == lineId));
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
