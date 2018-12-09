using Common.DataConfig;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Logger;
using Common.Models;
using Common.ModelsDTO;
using Common.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Managers
{
    public class ReceiptManager : IReceiptManager
    {
        private readonly IUnitOfWork _unitOfWork;
        readonly LoggerManager _logger;

        //ctor
        public ReceiptManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new LoggerManager(new FileLogger(), "receiptManager.txt");
        }

        /// <summary>
        /// Summarizes and generates an invoice to all the active lines according to the requested date
        /// </summary>
        /// <param name="requstedTime">Requested month and year</param>
        /// <returns>List of invoices if succeeded otherwise null</returns>
        public List<Payment> GeneratePaymentsToAllLines(DateTime requstedTime)
        {
            List<Line> allLines = _unitOfWork.Line.GetAllLinesWithAllEntities().ToList();
            List<Payment> payments = new List<Payment>();
            foreach (var line in allLines)
            {
                if (line.RemovedDate == null ||
                    (line.RemovedDate.Value.Year == requstedTime.Year
                    && line.RemovedDate.Value.Month == requstedTime.Month))
                {   // if line is active, or removed this month

                    if (!line.Payments.Any(p => p.Date.Year == requstedTime.Year && p.Date.Month == requstedTime.Month)) //Make sure that the current line does not have an existing payment
                    {   //Generates an payment according to line data
                        Payment newPayment = CreatePaymentModel(requstedTime, line);
                        payments.Add(newPayment);
                        line.Payments.Add(newPayment);
                    }
                }
            }
            _unitOfWork.Complete();
            return payments;
        }

        /// <summary>
        /// Produces an invoice for the customer according to all the lines in his possession
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <param name="date">Requested month and year</param>
        /// <returns>List of customer line's receipts  if succeeded otherwise null</returns>
        public List<LineReceiptDTO> GetCustomerReceipt(string idCard, DateTime date)
        {
            List<LineReceiptDTO> receipts = new List<LineReceiptDTO>();
            Customer customer;

            customer = _unitOfWork.Customer.GetCustomerWithTypeLinesAndPayment(idCard);

            if (customer != null)
            {
                foreach (var line in customer.Lines)
                {
                    Payment linePayment = line.Payments.Where((p) => p.Date.Year == date.Year && p.Date.Month == date.Month).SingleOrDefault();

                    if (linePayment != null) //Create new receipt according to payment details
                    {
                        LineReceiptDTO newReceipt = CreateReceiptDTOModel(customer, line, linePayment);

                        receipts.Add(newReceipt);
                    }
                }
            }
            return receipts;
        }


        /// <summary>
        /// Create new payment model
        /// </summary>
        /// <returns></returns>
        private Payment CreatePaymentModel(DateTime requstedTime, Line line)
        {
            Payment newPayment = new Payment() { Date = requstedTime };
            Customer customer = _unitOfWork.Customer.GetCustomerWithTypeAndLines(line.CustomerId);
            newPayment.CustomerType = customer.CustomerType;
            newPayment.LineId = line.LineId;

            newPayment.UsageSms = GetTotalLineSmsPerMonth(line, requstedTime);
            newPayment.UsageCall = GetTotalLineCallsDurationPerMonth(line, requstedTime);

            if (line.Package != null)
            {
                newPayment.PackageMinute = line.Package.MaxMinute;
                newPayment.PackageSms = line.Package.MaxSms;
                newPayment.PackagePrice = line.Package.TotalPrice;
            }
            double totalMinutesPrice = CalculateLineTotalMinutesPrice(customer, line, newPayment, requstedTime);
            double totalSmsPrics = CalculateLineTotalSmsPrice(customer, line, newPayment);
            newPayment.LineTotalPrice = totalMinutesPrice + totalSmsPrics + newPayment.PackagePrice;
            return newPayment;
        }

        /// <summary>
        /// Create new ReceiptDTO model
        /// </summary>
        private LineReceiptDTO CreateReceiptDTOModel(Customer customer, Line line, Payment linePayment)
        {
            LineReceiptDTO newReceipt = new LineReceiptDTO(linePayment);
            newReceipt.CustomerName = $"{customer.FirstName} {customer.LastName}";
            newReceipt.LineNumber = line.LineNumber;
            int usageCallMinute = newReceipt.UsageCall;
            newReceipt.MinutesBeyondPackageLimit = TimeSpan.FromMinutes(linePayment.MinutesBeyondPackageLimit);
            newReceipt.LeftMinutes = newReceipt.PackageMinute - usageCallMinute < 0 ? TimeSpan.Zero : TimeSpan.FromMinutes(newReceipt.PackageMinute - usageCallMinute);
            newReceipt.LeftSms = newReceipt.PackageMinute - usageCallMinute < 0 ? 0 : newReceipt.PackageSms - newReceipt.UsageSms;
            newReceipt.MinutesUsagePrecent = newReceipt.PackageMinute == 0 ? 0 : (100 - (int)((newReceipt.LeftMinutes.TotalMinutes / newReceipt.PackageMinute) * 100));
            newReceipt.SmsUsagePrecent = newReceipt.PackageSms == 0 ? 0 : (100 - (int)((newReceipt.LeftSms / newReceipt.PackageSms) * 100));
            newReceipt.PricePerMinute = customer.CustomerType.MinutePrice;
            newReceipt.PricePerSms = customer.CustomerType.SmsPrice;
            newReceipt.ExceptionalMinutesPrice = newReceipt.MinutesBeyondPackageLimit.TotalMinutes * newReceipt.PricePerMinute;
            newReceipt.ExceptionalSmsPrice = newReceipt.SmsBeyondPackageLimit * newReceipt.PricePerSms;
            return newReceipt;
        }

        /// <summary>
        /// Calculates the amount payable for messages sent from the line
        /// </summary>
        /// <param name="customer">Owner of the line</param>
        /// <param name="line">Line details</param>
        /// <param name="newPayment">The invoice generated for the line</param>
        /// <returns>The amount of payment for the messages for the requested month</returns>
        private double CalculateLineTotalSmsPrice(Customer customer, Line line, Payment newPayment)
        {
            if (line.Package != null)
            {
                if (line.Package.MaxSms != 0 && line.Package.MaxSms < newPayment.UsageSms)
                {
                    newPayment.SmsBeyondPackageLimit = newPayment.UsageSms - line.Package.MaxSms;
                    return newPayment.SmsBeyondPackageLimit * customer.CustomerType.SmsPrice;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return newPayment.UsageSms * customer.CustomerType.SmsPrice;
            }
        }

        /// <summary>
        /// Calculate the amount to pay for total call's minutes at a specific line
        /// </summary>
        /// <param name="customer">Owner of the line</param>
        /// <param name="line">Line details</param>
        /// <param name="newPayment">The invoice generated for the line</param>
        /// <param name="requstedTime">Requsted month and year</param>
        /// <returns>The amount to be paid for the minutes in the requested month</returns>
        private double CalculateLineTotalMinutesPrice(Customer customer, Line line, Payment newPayment, DateTime requstedTime)
        {
            int friendsDuration = 0;
            int PriorityContactDuration = 0;
            int insideFamilyDuration = 0;

            if (line.Package != null)
            {
                if (line.Package.MaxMinute != 0 && line.Package.MaxMinute < (newPayment.UsageCall / 60)) //Calculation of minutes outside the package
                {
                    newPayment.MinutesBeyondPackageLimit = (newPayment.UsageCall / 60) - line.Package.MaxMinute;
                }

                if (newPayment.MinutesBeyondPackageLimit > 0)
                {
                    if (line.Package.Friends != null) //Calculates the total call time in friend package
                    {
                        friendsDuration = line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year
                                        && c.DateOfCall.Month == requstedTime.Month
                                        && (c.DestinationNumber == line.Package.Friends.FirstNumber
                                        || c.DestinationNumber == line.Package.Friends.SecondNumber
                                        || c.DestinationNumber == line.Package.Friends.ThirdNumber))
                                        .Sum((d) => d.Duration);

                        friendsDuration = (int)(friendsDuration * (PackagePrices.FriendsNumbersPrecent / 100));
                    }

                    if (line.Package.PriorityContact) ////Calculates the total call time in a Priority contact package
                    {
                        string PriorityContact = line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year
                                                                  && c.DateOfCall.Month == requstedTime.Month)
                                                                   .GroupBy(n => n.DestinationNumber)
                                                                   .OrderByDescending(gp => gp.Count())
                                                                   .Take(1)
                                                                   .ToString();

                        PriorityContactDuration = line.Calls.Where((c) => c.DestinationNumber == PriorityContact).Sum((d) => d.Duration);
                        PriorityContactDuration = (int)(PriorityContactDuration * (PackagePrices.PriorityContactPrecent / 100));
                    }

                    if (line.Package.InsideFamilyCalles) //Calculates the total call time in family package
                    {
                        List<Line> familyLines = customer.Lines.Where((l) => l.LineNumber != line.LineNumber).ToList();

                        insideFamilyDuration = line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year
                                                                 && c.DateOfCall.Month == requstedTime.Month
                                                                 && familyLines.Any(y => y.LineNumber == c.DestinationNumber))
                                                                 .Sum(d => d.Duration);
                    }

                    int totalMinutesInPackages = friendsDuration + PriorityContactDuration + insideFamilyDuration;
                    int minutesLeftAfterPackagesDiscount = newPayment.MinutesBeyondPackageLimit - totalMinutesInPackages;

                    if (minutesLeftAfterPackagesDiscount > 0)
                    {
                        double minutesPriceBeyondPackage = minutesLeftAfterPackagesDiscount * customer.CustomerType.MinutePrice;
                        return minutesPriceBeyondPackage;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    return 0;
                }

            }
            return (newPayment.UsageCall / 60) * customer.CustomerType.MinutePrice;

        }

        /// <summary>
        /// Get the number of minutes the line spoke during the requested month
        /// </summary>
        /// <param name="line">Line details</param>
        /// <param name="requstedTime">Requested month and year</param>
        /// <returns>The amount of minutes that the line spoke during the requested month</returns>
        private int GetTotalLineCallsDurationPerMonth(Line line, DateTime requstedTime)
        {
            return line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year && c.DateOfCall.Month == requstedTime.Month)
                .Sum(s => s.Duration);
        }

        /// <summary>
        /// Get the number of messages thet sent during the requested month
        /// </summary>
        /// <param name="line">Line id</param>
        /// <param name="requstedTime">Requested month and year</param>
        /// <returns>Number of messages sent this month</returns>
        private int GetTotalLineSmsPerMonth(Line line, DateTime requstedTime)
        {
            return line.Messages.Count((m) => m.DataOfMessage.Year == requstedTime.Year && m.DataOfMessage.Month == requstedTime.Month);
        }
    }
}