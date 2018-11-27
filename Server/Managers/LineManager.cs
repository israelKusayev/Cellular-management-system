using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Db;

namespace Server.Managers
{
    public class LineManager
    {
        LoggerManager _logger;

        public LineManager()
        {
            _logger = new LoggerManager(new FileLogger(), "lineManager.txt");
        }

        internal List<Line> GetCustomerLines(string idCard)
        {
            try
            {
                Customer customer;
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    customer =  context.Customer.GetActiveCustomerWithLines(idCard);
                }
                return customer.Lines.Where(l => l.Status == LineStatus.Used).ToList();

            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        internal Line AddNewLine(Line lineToAdd, int customerId)
        {
            Line existLine;
            try
            {
                using (var context = new CellularContext())
                {
                    Customer customer = context.CustomerTable.SingleOrDefault((c) => c.CustomerId == customerId);
                    existLine = LineNumberIsAvailable(newLine, context);
                    if (existLine == null)
                    {
                        newLine.Status = LineStatus.Used;
                        newLine.CreatedDate = DateTime.Now;
                        customer.Lines.Add(newLine);
                        context.SaveChanges();
                        Line addedLine = context.LinesTable.SingleOrDefault((l) => l.LineNumber == newLine.LineNumber && l.Status == LineStatus.Used);
                        return addedLine;
                    }
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (existLine.CustomerId == customerId)
            {
                throw new AlreadyExistExeption("this customer", "Line");
            }
            else
            {
                throw new FoundLineExeption("other customer", "Line");
            }
        }
    }
}