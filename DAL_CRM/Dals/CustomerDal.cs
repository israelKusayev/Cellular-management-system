using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Db;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Dal.Dals
{
    public class CustomerDal
    {
        LoggerManager _logger;
        LineDal _lineManager;

        public CustomerDal()
        {
            _lineManager = new LineDal();
            _logger = new LoggerManager(new FileLogger(), "customerDal.txt");
        }

        public Customer AddNewCustomer(Customer newCustomer)
        {
            newCustomer.IsActive = true;
            Customer addedCustomer = null;

            try
            {
                using (var context = new CellularContext())
                {
                    if (context.CustomerTable.SingleOrDefault((c) => c.IdentityCard == newCustomer.IdentityCard && c.IsActive == true) == null)
                    {
                        addedCustomer = context.CustomerTable.Add(newCustomer);
                        context.SaveChanges();
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

        public List<Customer> GetAllCustomers()
        {
            try
            {
                using (var context = new CellularContext())
                {
                    return context.CustomerTable.Where((c) => c.IsActive == true).ToList();
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        //Get customer by his identity card number
        public Customer GetCustomer(string id)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    return context.CustomerTable.SingleOrDefault((c) => c.IdentityCard == id && c.IsActive == true);
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        /// <summary>
        /// calculate customer value 
        /// </summary>
        /// <param name="idCard">customer identity card</param>
        /// <returns>return customer value between 0 to 10</returns>
        public double GetCustomerValue(string idCard)
        {
            using (var context = new CellularContext())
            {
                Customer customer = context.CustomerTable.Where((c) => c.IdentityCard == idCard && c.IsActive == true).Include(l => l.Lines.Select(x => x.Payments)).SingleOrDefault();
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
        }

        public Customer EditCustomer(Customer CustomerToEdit)
        {
            CustomerToEdit.IsActive = true;
            try
            {
                using (var context = new CellularContext())
                {

                    Customer foundCustomer = context.CustomerTable.SingleOrDefault((c) => c.CustomerId == CustomerToEdit.CustomerId && c.IsActive == true);

                    if (foundCustomer != null)
                    {
                        context.Entry(foundCustomer).CurrentValues.SetValues(CustomerToEdit);
                        context.SaveChanges();
                    }
                    return foundCustomer;
                }
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

        }

        public Customer DeactivateCustomer(string id)
        {
            try
            {
                using (var context = new CellularContext())
                {
                    Customer customerToDeactivate = context.CustomerTable.Where((c) => c.IdentityCard == id && c.IsActive == true).Include(l => l.Lines).SingleOrDefault();
                    if (customerToDeactivate != null)
                    {
                        customerToDeactivate.IsActive = false;

                        foreach (var line in customerToDeactivate.Lines)//deactive all customer lines.
                        {
                            _lineManager.DeactivateLine(line.LineId);
                        }

                        context.SaveChanges();
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

