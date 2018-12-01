using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Interfaces.ServerManagersInterfaces;
using Common.Logger;
using Common.Models;
using Common.RepositoryInterfaces;
using Db;

namespace Server.Managers
{
    public class LineManager : ILineManager
    {
        LoggerManager _logger;
        private IUnitOfWork _unitOfWork;
        public LineManager(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = new LoggerManager(new FileLogger(), "lineManager.txt");
        }

        public List<Line> GetCustomerLines(string idCard)
        {
            try
            {
                Customer customer = _unitOfWork.Customer.GetActiveCustomerWithLinesAndPackages(idCard);
                return customer.Lines.Where(l => l.Status == LineStatus.Used).ToList();
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Line AddNewLine(Line lineToAdd, int customerId)
        {
            Line lineNumberExsists;
            Customer customer;

            try
            {
                customer = _unitOfWork.Customer.Get(customerId);
                lineNumberExsists = _unitOfWork.Line.LineNumberIsAvailable(lineToAdd.LineNumber);
                if (lineNumberExsists == null)
                {
                    lineToAdd.Status = LineStatus.Used;
                    lineToAdd.CreatedDate = DateTime.Now;
                    customer.Lines.Add(lineToAdd);
                    _unitOfWork.Complete();

                    Line addedLine = _unitOfWork.Line.GetLineByLineNumber(lineToAdd.LineNumber);
                    return addedLine;
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

        public Line DeactivateLine(int lineId)
        {
            try
            {
                Line lineToEdit = _unitOfWork.Line.Get(lineId);

                if (lineToEdit != null)
                {
                    _unitOfWork.Line.DeactivateLine(lineToEdit);
                    _unitOfWork.Complete();
                }
                return lineToEdit;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }
    }
}