using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Logger;
using Common.Models;
using Common.ModelsDTO;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Managers
{
    public class CustomerWebsiteManager : ICustomerWebsiteManager
    {
        private LoggerManager _logger;
        private IUnitOfWork _unitOfWork;

        //ctor
        public CustomerWebsiteManager(IUnitOfWork unitOfWork)
        {
            _logger = new LoggerManager(new FileLogger(), "customerWebsiteManager.txt");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get the requested customer with his lines
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Customer  if succeeded otherwise null</returns>
        public Customer GetCustomerWithLines(string idCard)
        {
            try
            {
                return _unitOfWork.Customer.GetActiveCustomerWithLines(idCard);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Get line usage details and packages recommendation for the last month
        /// </summary>
        /// <param name="lineNumber"></param>
        /// <returns>LineWebsiteDTO with last month line details and Recommended Packages if succeeded otherwise null</returns>
        public LineWebsiteDTO GetLineDetails(string lineNumber)
        {
            Line foundedLine;
            Line lineWithCalls;
            LineWebsiteDTO lineWebsiteDTO = new LineWebsiteDTO();

            try
            {
                foundedLine = _unitOfWork.Line.GetLineByLineNumberWithPaymentsAndPackageAndFriends(lineNumber);

                if (foundedLine != null && foundedLine.Payments != null)
                {

                    List<Payment> payments = new List<Payment>(foundedLine.Payments);
                    Payment payment = payments[payments.Count - 1];
                    List<int> totalMinutesTopNumbers = new List<int>();

                    lineWithCalls = _unitOfWork.Line.GetLineWithCalls(lineNumber, payment.Date); //Get the calls of the line per month requested

                    var Top3Numbers = lineWithCalls.Calls //Retrieve the top numbers
                                                   .GroupBy(c => c.DestinationNumber)
                                                   .Select(d => new { count = d.Count(), des = d.Key })
                                                   .OrderByDescending(x => x.count)
                                                   .Take(3);

                    foreach (var item in Top3Numbers)//Calculates total call minutes for top numbers
                    {
                        int total = lineWithCalls.Calls.Where(c => c.DestinationNumber == item.des).Sum(d => d.Duration) / 60;
                        totalMinutesTopNumbers.Add(total);
                    }

                    Customer customer = _unitOfWork.Customer.GetCustomerWithTypeAndLines(lineWithCalls.CustomerId); //Calculates total call minutes within the family
                    int totalFamilyMinutes = 0;
                    foreach (var item in customer.Lines)
                    {
                        totalFamilyMinutes += lineWithCalls.Calls.Where(c => c.DestinationNumber == item.LineNumber).Sum(d => d.Duration) / 60;
                    }

                    if (payment != null)
                    {
                        List<Package> templates = _unitOfWork.Package.GetPackageTemplate();
                        lineWebsiteDTO.RecommendPackages =  templates.OrderBy(p => Math.Abs(p.MaxMinute - payment.UsageCall)).Take(3).ToList();
                        lineWebsiteDTO.TotalLinePrice = payment.LineTotalPrice;
                        lineWebsiteDTO.TotalMinutes = payment.UsageCall / 60;
                        lineWebsiteDTO.TotalSms = payment.UsageSms;
                        lineWebsiteDTO.TotalMinutesTopNumber = totalMinutesTopNumbers[0];
                        lineWebsiteDTO.TotalMinutesTop3Numbers = totalMinutesTopNumbers.Sum();
                        lineWebsiteDTO.TotalMinutesWithFamily = totalFamilyMinutes;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            return lineWebsiteDTO;
        }
    }
}