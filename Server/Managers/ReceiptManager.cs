using Common.DataConfig;
using Common.Enums;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Logger;
using Common.Models;
using Common.ModelsDTO;
using Common.RepositoryInterfaces;
using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Server.Managers
{
    public class ReceiptManager : IReceiptManager
    {
        private readonly IUnitOfWork _unitOfWork;
        readonly LoggerManager _logger;
        public ReceiptManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new LoggerManager(new FileLogger(), "invoiceDal.txt");
        }


        public List<Payment> GeneratePaymentsToAllLines(DateTime requstedTime)
        {
            List<Line> allLines = _unitOfWork.Line.GetAllLinesWithAllEntities().ToList();
            List<Payment> payments = new List<Payment>();
            foreach (var line in allLines)
            {
                if (line.RemovedDate == null ||
                    (line.RemovedDate.Value.Year == requstedTime.Year
                    && line.RemovedDate.Value.Month == requstedTime.Month)) //add this if stetment to linq quary at line 29
                {
                    if (!line.Payments.Any(p => p.Date.Year == requstedTime.Year && p.Date.Month == requstedTime.Month))
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
                        payments.Add(newPayment);
                        line.Payments.Add(newPayment);
                    }
                }
            }
            _unitOfWork.Complete();
            return payments;
        }

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

                    if (linePayment != null)
                    {
                        LineReceiptDTO newReceipt = new LineReceiptDTO(linePayment);

                        newReceipt.CustomerName = $"{customer.FirstName} {customer.LastName}";
                        newReceipt.LineNumber = line.LineNumber;
                        int usageCallMinute = newReceipt.UsageCall / 60;
                        newReceipt.MinutesBeyondPackageLimit = TimeSpan.FromMinutes(linePayment.MinutesBeyondPackageLimit);
                        newReceipt.LeftMinutes = newReceipt.PackageMinute - usageCallMinute < 0 ? TimeSpan.Zero : TimeSpan.FromMinutes(newReceipt.PackageMinute - usageCallMinute);
                        newReceipt.LeftSms = newReceipt.PackageMinute - usageCallMinute < 0 ? 0 : newReceipt.PackageSms - newReceipt.UsageSms;
                        newReceipt.MinutesUsagePrecent = newReceipt.PackageMinute == 0 ? 0 : (int)(newReceipt.LeftMinutes.TotalMinutes / newReceipt.PackageMinute) * 100;
                        newReceipt.SmsUsagePrecent = newReceipt.PackageSms == 0 ? 0 : (newReceipt.LeftSms / newReceipt.PackageSms) * 100;
                        newReceipt.PricePerMinute = customer.CustomerType.MinutePrice;
                        newReceipt.PricePerSms = customer.CustomerType.SmsPrice;
                        newReceipt.ExceptionalMinutesPrice = newReceipt.MinutesBeyondPackageLimit.TotalMinutes * newReceipt.PricePerMinute;
                        newReceipt.ExceptionalSmsPrice = newReceipt.SmsBeyondPackageLimit * newReceipt.PricePerSms;

                        receipts.Add(newReceipt);
                    }

                }
            }
            return receipts;
        }

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

        private double CalculateLineTotalMinutesPrice(Customer customer, Line line, Payment newPayment, DateTime requstedTime)
        {
            int friendsDuration = 0;
            int PriorityContactDuration = 0;
            int insideFamilyDuration = 0;

            if (line.Package != null)
            {
                if (line.Package.MaxMinute != 0 && line.Package.MaxMinute < (newPayment.UsageCall / 60))
                {
                    newPayment.MinutesBeyondPackageLimit = (newPayment.UsageCall / 60) - line.Package.MaxMinute;
                }

                if (newPayment.MinutesBeyondPackageLimit > 0)
                {
                    if (line.Package.Friends != null)
                    {
                        friendsDuration = line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year
                                        && c.DateOfCall.Month == requstedTime.Month
                                        && (c.DestinationNumber == line.Package.Friends.FirstNumber
                                        || c.DestinationNumber == line.Package.Friends.SecondNumber
                                        || c.DestinationNumber == line.Package.Friends.ThirdNumber))
                                        .Sum((d) => d.Duration);

                        friendsDuration = (int)(friendsDuration * (PackagePrices.FriendsNumbersPrecent / 100));
                    }

                    if (line.Package.PriorityContact)
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

                    if (line.Package.InsideFamilyCalles)
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

        private int GetTotalLineCallsDurationPerMonth(Line line, DateTime requstedTime)
        {
            return line.Calls.Where((c) => c.DateOfCall.Year == requstedTime.Year && c.DateOfCall.Month == requstedTime.Month)
                .Sum(s => s.Duration);
        }

        private int GetTotalLineSmsPerMonth(Line line, DateTime requstedTime)
        {
            return line.Messages.Count((m) => m.DataOfMessage.Year == requstedTime.Year && m.DataOfMessage.Month == requstedTime.Month);
        }
    }
}