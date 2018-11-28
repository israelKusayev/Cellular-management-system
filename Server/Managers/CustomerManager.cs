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
using Db;
using Db.Repositories;

namespace Server.Managers
{
    public class CustomerManager : ICustomerManager
    {
        private LoggerManager _logger;

        //Ctor
        public CustomerManager()
        {
            _logger = new LoggerManager(new FileLogger(), "customerDal.txt");
        }

        /// <summary>
        /// summary
        /// </summary>
        /// <param name="idCard">Customer identity card</param>
        /// <returns>return</returns>
        public Customer GetActiveCustomer(string idCard)
        {
            using (var context = new UnitOfWork(new CellularContext()))
            {
                return context.Customer.GetActiveCustomerByIdCard(idCard);
            }
        }

        public double GetCustomerValue(string idCard)
        {
            Customer customer;
            using (var context = new UnitOfWork(new CellularContext()))
            {
                customer = context.Customer.GetCustomerWithLinesAndPayments(idCard);
            }
            if (customer == null)
            {
                throw new KeyNotFoundException("Customer not found");
            }
            List<Line> lines = customer.Lines.ToList();

            var num = lines.Count * 0.2 < 4 ? lines.Count * 0.2 : 4;

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

        public Customer AddNewCustomer(Customer newCustomer)
        {
            newCustomer.IsActive = true;
            Customer addedCustomer = null;

            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    var customer = context.Customer.GetActiveCustomerByIdCard(newCustomer.IdentityCard);
                    if (customer == null)
                    {
                        context.Customer.Add(newCustomer);
                        context.Complete();
                        addedCustomer = context.Customer.GetActiveCustomerByIdCard(newCustomer.IdentityCard);
                    }
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

        public Customer EditCustomer(Customer customerToEdit)
        {
            customerToEdit.IsActive = true;
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Customer foundCustomer = context.Customer.GetActiveCustomerByIdCard(customerToEdit.IdentityCard);

                    if (foundCustomer != null)
                    {
                        context.Customer.Edit(foundCustomer, customerToEdit);
                        context.Complete();
                        return customerToEdit;
                    }
                    return null;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public Customer DeactivateCustomer(string idCard)
        {
            try
            {
                using (var context = new UnitOfWork(new CellularContext()))
                {
                    Customer customerToDeactivate = context.Customer.GetActiveCustomerWithLines(idCard);
                    if (customerToDeactivate != null)
                    {
                        customerToDeactivate.IsActive = false;

                        foreach (var line in customerToDeactivate.Lines)
                        {
                            context.Line.DeactivateLine(line);
                        }

                        context.Complete();
                    }
                    return customerToDeactivate;
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