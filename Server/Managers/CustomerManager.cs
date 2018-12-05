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
using Db.Repositories;

namespace Server.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private LoggerManager _logger;
        private IUnitOfWork _unitOfWork;

        //ctor
        public CustomerManager(IUnitOfWork unitOfWork)
        {
            _logger = new LoggerManager(new FileLogger(), "customerManager.txt");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get customer who fits identity card and active status
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Customer if succeeded otherwise null</returns>
        public Customer GetActiveCustomer(string idCard)
        {
            try
            {
                return _unitOfWork.Customer.GetActiveCustomerByIdCard(idCard);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Get the customer value referring to the his data calculation
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Calculated value if succeeded otherwise 0</returns>
        public double GetCustomerValue(string idCard)
        {
            Customer customer;
            try
            {
                customer = _unitOfWork.Customer.GetCustomerWithLinesAndPayments(idCard);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }
            List<Line> lines = customer.Lines.ToList();

            var num = lines.Count * 0.2 < 4 ? lines.Count * 0.2 : 4; //Calculation of the relative share of the number of lines

            if (lines.Count != 0)
            {
                double totalPayment = 0;//total Payment Of last Three Month
                DateTime dateBeforeThreeMonth = DateTime.Now.AddMonths(-4);
                foreach (var line in lines)
                {
                    totalPayment += line.Payments.Where(p => p.Date > dateBeforeThreeMonth).Sum(s => s.LineTotalPrice);
                }
                num += totalPayment / 1000 < 6 ? totalPayment / 1000 : 6;
            }
            num += customer.CallsToCenter * -0.1 > -3 ? customer.CallsToCenter * -0.1 : -3;
            return num < 0 ? 0 : num;
        }

        /// <summary>
        /// Add a new customer only if it does not already exist in the system
        /// </summary>
        /// <param name="newCustomer">Customer details</param>
        /// <returns>Added customer if succeeded otherwise null</returns>
        public Customer AddNewCustomer(Customer newCustomer)
        {
            newCustomer.IsActive = true;
            newCustomer.JoinDate = DateTime.Now;
            Customer addedCustomer = null;

            try
            {
                var customer = _unitOfWork.Customer.GetActiveCustomerByIdCard(newCustomer.IdentityCard); //Check whether the customer already exists
                if (customer == null)
                {
                    _unitOfWork.Employee.Get(newCustomer.EmplyeeId).Customers.Add(newCustomer);
               
                    _unitOfWork.Complete();
                    addedCustomer = _unitOfWork.Customer.GetActiveCustomerByIdCard(newCustomer.IdentityCard);
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (addedCustomer == null)
            {
                throw new AlreadyExistExeption("Customer");
            }

            return addedCustomer;
        }

        /// <summary>
        /// Edit existing customer information
        /// </summary>
        /// <param name="customerToEdit">Customer details</param>
        /// <returns>Edited customer if succeeded otherwise null</returns>
        public Customer EditCustomer(Customer customerToEdit)
        {
            customerToEdit.IsActive = true;
            try
            {
                Customer foundCustomer = _unitOfWork.Customer.GetActiveCustomerByIdCard(customerToEdit.IdentityCard);

                if (foundCustomer != null)
                {
                    _unitOfWork.Customer.Edit(foundCustomer, customerToEdit);
                    _unitOfWork.Complete();
                    return _unitOfWork.Customer.GetActiveCustomerWithLinesAndPackages(foundCustomer.IdentityCard);
                }
                return null;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        /// <summary>
        /// Transfer the customer and all his lines to the deactive status
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>Customer if succeeded otherwise null</returns>
        public Customer DeactivateCustomer(string idCard)
        {
            try
            {
                Customer customerToDeactivate = _unitOfWork.Customer.GetActiveCustomerWithLines(idCard);
                if (customerToDeactivate != null)
                {
                    customerToDeactivate.IsActive = false;

                    foreach (var line in customerToDeactivate.Lines)
                    {
                        _unitOfWork.Line.DeactivateLine(line);
                    }

                    _unitOfWork.Complete();
                }
                return customerToDeactivate;
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }
    }
}