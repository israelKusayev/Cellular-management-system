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
                    customer = context.Customer.GetActiveCustomerWithLines(idCard);
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
            Line lineNumberExsists;
            Customer customer;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    customer = context.Customer.Get(customerId);
                    lineNumberExsists = context.Line.LineNumberIsAvailable(lineToAdd.LineNumber);
                    if (lineNumberExsists == null)
                    {
                        lineToAdd.Status = LineStatus.Used;
                        lineToAdd.CreatedDate = DateTime.Now;
                        customer.Lines.Add(lineToAdd);
                        context.Complete();

                        Line addedLine = context.Line.GetLineByLineNumber(lineToAdd.LineNumber);
                        return addedLine;
                    }
                }
                
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (lineNumberExsists.CustomerId == customerId)
            {
                throw new AlreadyExistExeption("this customer", "Line");
            }
            else
            {
                throw new FoundLineExeption("other customer", "Line");
            }
        }

        internal Line DeactivateLine(int lineId)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Line lineToEdit = context.Line.Get(lineId);

                    if (lineToEdit != null)
                    {
                        context.Line.DeactivateLine(lineToEdit);
                        context.Complete();
                    }
                    return lineToEdit;
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