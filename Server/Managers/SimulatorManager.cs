using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Common.ModelsDTO;
using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Managers
{
    public class SimulatorManager
    {
        private LoggerManager _logger;
        private Random _durationRand;
        private Random _destinationRand;

        public SimulatorManager()
        {
            _logger = new LoggerManager(new FileLogger(), "simulatorDal.txt");
            _durationRand = new Random();
            _destinationRand = new Random();
        }

        internal void SimulateCallsOrSms(SimulateDTO simulateDTO)
        {
            Customer selectedCustomer;
            Line selectedLine;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    selectedCustomer = context.Customer.GetActiveCustomerWithLines(simulateDTO.IdentityCard);
                    selectedLine = context.Line.GetLineWithPackageAndFriends(simulateDTO.LineId);

                    selectedCustomer.CallsToCenter += simulateDTO.CallToCenter;
                    context.Complete();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            List<string> destinationNumbers = GetDestinationNumbers(simulateDTO, selectedCustomer, selectedLine);

            if (destinationNumbers == null)
            {
                throw new EmptyException("System can not find any destination number");
            }

            if (simulateDTO.IsSms)
            {
                for (int i = 0; i < simulateDTO.NumberOfCallsOrSms; i++)
                {
                    string destination = destinationNumbers[_destinationRand.Next(destinationNumbers.Count)];
                    AddSms(simulateDTO.LineId, destination);
                }
            }
            else
            {
                for (int i = 0; i < simulateDTO.NumberOfCallsOrSms; i++)
                {
                    string destination = destinationNumbers[_destinationRand.Next(destinationNumbers.Count)];

                    int douration = _durationRand.Next(simulateDTO.MinDuration, simulateDTO.MaxDuration + 1);
                    AddCall(simulateDTO.LineId, destination, douration);
                }
            }
        }

        private List<string> GetDestinationNumbers(SimulateDTO simulateDTO, Customer customer, Line line)
        {
            List<string> numbers = new List<string>();

            switch (simulateDTO.SendTo)
            {
                case SimulateSendTo.Friends:
                    {
                        if (line.Package.Friends.FirstNumber != null)
                            numbers.Add(line.Package.Friends.FirstNumber);
                        if (line.Package.Friends.SecondNumber != null)
                            numbers.Add(line.Package.Friends.SecondNumber);
                        if (line.Package.Friends.ThirdNumber != null)
                            numbers.Add(line.Package.Friends.ThirdNumber);
                        return numbers;
                    }

                case SimulateSendTo.Family:
                    {
                        return customer.Lines.Where((l) => l.LineNumber != line.LineNumber).Select((x) => x.LineNumber).ToList();
                    }

                case SimulateSendTo.General:
                    {
                        simulateDTO.SendTo = SimulateSendTo.Family;
                        List<string> family = GetDestinationNumbers(simulateDTO, customer, line);
                        simulateDTO.SendTo = SimulateSendTo.Friends;
                        List<string> friends = GetDestinationNumbers(simulateDTO, customer, line);
                        simulateDTO.SendTo = SimulateSendTo.All;
                        List<string> all = GetDestinationNumbers(simulateDTO, customer, line);

                        return all.Except(family).Except(friends).ToList();
                    }

                case SimulateSendTo.All:
                    {
                        try
                        {
                            using (var context = new UnitOfWork(new CellularContext()))
                            {
                                return context.Line.GetAll().Where(l => l.LineNumber != line.LineNumber).Select(l => l.LineNumber).ToList();
                            }
                        }
                        catch (Exception e)
                        {
                            _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                            throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
                        }
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Add sms
        /// </summary>
        /// <param name="from">sender</param>
        /// <param name="to">reciver</param>
        private void AddSms(int from, string to)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Sms sms = new Sms
                    {
                        LineId = from,
                        DestinationNumber = to,
                        DataOfMessage = DateTime.Now
                    };
                    context.Sms.Add(sms);
                    context.Complete();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        private void AddCall(int from, string to, int duration)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Call call = new Call
                    {
                        LineId = from,
                        DestinationNumber = to,
                        Duration = duration,
                        DateOfCall = DateTime.Now
                    };
                    context.Call.Add(call);
                    context.Complete();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }
    }
}