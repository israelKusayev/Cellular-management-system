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

                    lineWithCalls = _unitOfWork.Line.GetLineWithCalls(lineNumber, payment.Date);

                    //var totalMinutesTopNumber = lineWithCalls.Calls.GroupBy(c => c.DestinationNumber).Select(d=>d.OrderByDescending(c => c.DestinationNumber)).Take(3).ToList();

                    var totalMinutesTopNumber = lineWithCalls.Calls.GroupBy(c => c.DestinationNumber).Select(d => new { count = d.Count(), des = d.Key }).OrderByDescending(x => x.count);


                    if (payment != null)
                    {
                        lineWebsiteDTO.TotalLinePrice = payment.LineTotalPrice;
                        lineWebsiteDTO.TotalMinutes = payment.UsageCall / 60;
                        lineWebsiteDTO.TotalSms = payment.UsageSms;
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