using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.ClientsModels;
using Common.ModelsDTO;

namespace Bi.Client.BL
{
    class ReportsManager
    {
        private readonly string _url = Config.Default.BaseUrl;

        /// <summary>
        /// Get report from db
        /// </summary>
        /// <typeparam name="T">reportDTO type </typeparam>
        /// <param name="urlActionName">name of api action </param>
        /// <returns><list type="T"></list> </returns>
        internal List<T> GetReports<T>(string urlActionName)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/bi/{urlActionName}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsAsync<List<T>>().Result;
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
            return new List<T>();
        }

        /// <summary>
        /// create payments for this month
        /// </summary>
        internal void GeneratePayments()
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_url}/receipt/generatePayments").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Payments was created successfully");
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
    }
}
