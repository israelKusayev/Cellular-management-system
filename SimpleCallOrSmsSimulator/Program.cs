using Common.ClientsModels;
using Common.Enums;
using Common.Models;
using Common.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SimpleCallOrSmsSimulator
{
    class Program
    {
        static void Main(string[] args)
        {
            int simulateOption;

            Console.WriteLine("Simulate calls or sms");
            Console.WriteLine("type customer identity card");

            string identityCard = Console.ReadLine();
            Console.WriteLine();
            var linesId = GetLines(identityCard);



            int lineId;

            do
            {
                Console.WriteLine("select line id");
                linesId.ForEach(i => Console.Write("{0}\t", i));
                Console.WriteLine();
                int.TryParse(Console.ReadLine(), out lineId);
            } while (!linesId.Contains(lineId));

            Console.WriteLine();
            do
            {
                Console.WriteLine("Click 1 to simulate calls, Click 2 to simulate sms");
                int.TryParse(Console.ReadLine(), out simulateOption);
            } while (simulateOption != 1 && simulateOption != 2);

            Random r = new Random();
            SimulateDTO simulateDTO ;
            if (simulateOption == 1)
            {
                 simulateDTO = new SimulateDTO()
                {
                    IsSms = false,
                    CallToCenter = 2,
                    IdentityCard = identityCard,
                    MinDuration = r.Next(1, 100),
                    MaxDuration = r.Next(100, 1000),
                    LineId = lineId,
                    NumberOfCallsOrSms = r.Next(100, 500),
                    SendTo = SimulateSendTo.All
                };
            }
            else 
            {
                 simulateDTO = new SimulateDTO()
                {
                    IsSms = false,
                    CallToCenter = 2,
                    IdentityCard = identityCard,
                    LineId = lineId,
                    NumberOfCallsOrSms = r.Next(100, 500),
                    SendTo = SimulateSendTo.All
                };
            }

            Simulate(simulateDTO);
            Console.ReadKey();
        }



        internal static List<int> GetLines(string idCard)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.GetAsync($"http://localhost:54377/api/crm/line/" + idCard).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var lines = result.Content.ReadAsAsync<List<Line>>().Result;
                        List<int> linesId = new List<int>();
                        foreach (var line in lines)
                        {
                            linesId.Add(line.LineId);
                        }
                        return linesId;
                    }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        Console.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("server error");
            }
            return null;
        }


        internal static void Simulate(SimulateDTO simulateDTO)
        {
            try
            {
                using (var http = new HttpClient())
                {
                    var result = http.PostAsJsonAsync($"http://localhost:54377/api/simulator", simulateDTO).Result;
                    if (result.IsSuccessStatusCode)
                    {
                        Console.WriteLine("The simulation was seccessful");
                    }
                    else
                    {
                        string message = result.Content.ReadAsAsync<ResponseMessage>().Result.Message;
                        Console.WriteLine(message);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("server error");
            }
        }
    }
}
