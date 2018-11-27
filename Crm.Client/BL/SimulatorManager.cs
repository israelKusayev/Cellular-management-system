using Common.ClientsModels;
using Common.Models;
using Common.ModelsDTO;
using Crm.Client.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Crm.Client.BL
{
    class SimulatorManager
    {
        private readonly string _baseUrl = Config.Default.BaseUrl;

        internal ICollection<Line> GetLines(string idCard)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_baseUrl}/crm/line/{idCard}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsAsync<List<Line>>().Result;
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
            return null;
        }

        internal void Simulate(SimulateDTO simulateDTO)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PostAsJsonAsync($"{_baseUrl}/simulator", simulateDTO).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        MessageBox.Show("The simulation was seccessful");
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
        }

        internal Package GetPackage(int selectedLine)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_baseUrl}/crm/package/{selectedLine}").Result;
                    if (result.IsSuccessStatusCode)
                    {
                        return result.Content.ReadAsAsync<Package>().Result;
                    }
                    else if (result.StatusCode == System.Net.HttpStatusCode.BadRequest) { }
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
            return null;
        }
    }
}
