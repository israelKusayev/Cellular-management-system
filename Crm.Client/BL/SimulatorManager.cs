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

        /// <summary>
        /// Get customer lines from api
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Collection of founded lines, or null</returns>
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

        /// <summary>
        /// Sending to api SimulateDTO model to simulate calls or sms
        /// </summary>
        /// <param name="simulateDTO">simulateDTO model</param>
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

        /// <summary>
        /// Get package of specified line id
        /// </summary>
        /// <param name="lineId">Line id</param>
        /// <returns>Founded package, or null</returns>
        internal Package GetPackage(int lineId)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"{_baseUrl}/crm/package/{lineId}").Result;
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
