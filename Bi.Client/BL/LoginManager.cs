using Common.ClientsModels;
using Common.Models;
using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Bi.Client.BL
{
    class LoginManager
    {
        private readonly string _url = Config.Default.BaseUrl;

        /// <summary>
        /// Login to BI system
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>true if succeeded to connect. otherwise, false</returns>
        public Task<bool> Login(string username, string password)
        {
            Task<bool> task = Task.Run(() =>
            {
                LoginDTO loginDTO = new LoginDTO() { UserName = username, Password = password };
                try
                {
                    using (var client = new HttpClient())
                    {
                        var result = client.PostAsJsonAsync($"{_url}/bi/login", loginDTO).Result;
                        if (result.IsSuccessStatusCode)
                        {
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
