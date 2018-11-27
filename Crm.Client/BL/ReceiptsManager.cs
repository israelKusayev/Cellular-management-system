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
