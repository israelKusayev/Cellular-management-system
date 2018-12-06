using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Common.ClientsModels;
using Common.Models;
using Common.ModelsDTO;

namespace Crm.Client.BL
{
    class ReceiptsManager
    {
        private readonly string _baseUrl = Config.Default.BaseUrl;
        internal List<LineReceiptDTO> _receipts;

        /// <summary>
        /// Get customer receipts from api
        /// </summary>
        /// <param name="customerId">Customer identity card</param>
        /// <param name="year">Receipt year</param>
        /// <param name="month">Receipt month</param>
        /// <returns>True if succeeded. otherwise, false</returns>
        internal bool GetReceipt(string customerId, int year, int month)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_baseUrl}/receipt/{ customerId}/{year}/{month}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        _receipts = result.Content.ReadAsAsync<List<LineReceiptDTO>>().Result;
                        if (_receipts.Count == 0) return false;
                        return true;
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
                MessageBox.Show("server error");
            }
            return false;
        }
    }
}
