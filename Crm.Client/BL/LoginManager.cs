using Common.ClientsModels;
using Common.Models;
using Common.ModelsDTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Crm.Client.BL
{
    class LoginManager
    {
        private readonly string _url = Config.Default.BaseUrl;
        internal Employee _currentEmployee { get; set; }

        /// <summary>
        /// Sending login request to api
        /// </summary>
        /// <param name="username">Employee username</param>
        /// <param name="password">Employee password</param>
        /// <returns>True if succeeded. otherwise, false</returns>
        public Task<bool> Login(string username, string password)
        {
            Task<bool> task = Task.Run(() =>
            {
                LoginDTO loginDTO = new LoginDTO() { UserName = username, Password = password };
                Employee emplyee;
                try
                {
                    using (var client = new HttpClient())
                    {
                        var result = client.PostAsJsonAsync($"{_url}/crm/login", loginDTO).Result;
                        if (result.IsSuccessStatusCode)
                        {
                            emplyee = result.Content.ReadAsAsync<Employee>().Result;
                            if (emplyee == null)
                            {
                                MessageBox.Show("Something went wrong please try again.");
                                return false;
                            }
                            _currentEmployee = emplyee;
                            return true;
                        }
                        else
                        {
                            string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                            MessageBox.Show(message);
                            return false;
                        }
                    }
                }
                catch (Exception)
                {
                    MessageBox.Show(Application.Current.MainWindow, "server error");
                }
                return false;
            });
            return task;
        }
    }
}
