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
    public class BiManager : IBiManager
    {
        private LoggerManager _logger;
        private IUnitOfWork _unitOfWork;

        //ctor
        public BiManager(IUnitOfWork unitOfWork)
        {
            _logger = new LoggerManager(new FileLogger(), "biManager.txt");
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Check if employee of manager type details are valid for login to the system
        /// </summary>
        /// <param name="loginEmployee">Contains username and password</param>
        /// <returns>Employee if succeeded otherwise null</returns>
        public Employee Login(LoginDTO loginEmployee)
        {
            Employee requstedEmployee = null;

            try
            {
                requstedEmployee = _unitOfWork.Employee.GetEmployeeByUserName(loginEmployee.UserName);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (requstedEmployee == null)
            {
                throw new IncorrectExeption("Username");
            }
            else if (requstedEmployee.Password != loginEmployee.Password)
            {
                throw new IncorrectExeption("Password");
            }

            if (!requstedEmployee.IsManager)
            {
                throw new IncorrectExeption("Employee level");
            }

            return requstedEmployee;
        }

        /// <summary>
        /// Get the customers who made the most calls to the service center
        /// </summary>
        /// <returns>List of most calling customer if succeeded otherwise null</returns>
        public List<MostCallCustomerDTO> GetMostCallingToCenterCustomers()
        {
            List<Customer> customers;
            List<MostCallCustomerDTO> customersDTO = null;

            try
            {
                customers = _unitOfWork.Customer.GetMostCallToCenterCustomers(DateTime.Now);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (customers != null)
            {
                customersDTO = new List<MostCallCustomerDTO>();

                foreach (var customer in customers)
                {
                    MostCallCustomerDTO customerDTO = new MostCallCustomerDTO();
                    customerDTO.FirstName = customer.FirstName;
                    customerDTO.LastName = customer.LastName;
                    customerDTO.IdentityCard = customer.IdentityCard;
                    customerDTO.CallsToCenter = customer.CallsToCenter;
                    customersDTO.Add(customerDTO);
                }
            }
            return customersDTO;
        }

        /// <summary>
        /// Get the employees who have added the most customers
        /// </summary>
        /// <returns>List of best seller employees if succeeded otherwise null</returns>
        public List<EmployeeBiDTO> GetBestSellerEmployees()
        {
            List<Employee> employees;
            List<EmployeeBiDTO> employeesDTO = null;

            try
            {
                employees = _unitOfWork.Employee.GetBestSellerEmployees(DateTime.Now);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (employees != null)
            {
                employeesDTO = new List<EmployeeBiDTO>();

                foreach (var employee in employees)
                {
                    EmployeeBiDTO employeeDTO = new EmployeeBiDTO();
                    employeeDTO.UserName = employee.UserName;
                    employeeDTO.LastMonthSells = employee.Customers.Count;
                    employeesDTO.Add(employeeDTO);
                }
            }
            return employeesDTO;
        }

        /// <summary>
        /// Get the customers who talk the most with different destinations 
        /// </summary>
        /// <returns>>List of customers if succeeded otherwise null</returns>
        public List<CustomerBiDTO> GetOpinionLeadersCustomers()
        {
            List<CustomerBiDTO> customerBiDTOs = new List<CustomerBiDTO>();
            List<Customer> customers;
            try
            {
                customers = _unitOfWork.Customer.GetOpinionLeadersCustomers();
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            if (customers != null)
            {
                customers.ForEach(c =>
                {
                    customerBiDTOs.Add(new CustomerBiDTO() { FirstName = c.FirstName, LastName = c.LastName, IdentityCard = c.IdentityCard });
                });
            }
            return customerBiDTOs;
        }

        /// <summary>
        /// Get the Customers who have the highest average invoices
        /// </summary>
        /// <returns>List of customers if succeeded otherwise null</returns>
        public List<ProfitableCustomerDTO> GetMostProfitableCustomers()
        {
            List<Customer> customers;
            List<ProfitableCustomerDTO> profitableCustomersDTO = null;

            try
            {
                customers = _unitOfWork.Customer.GetMostProfitableCustomers();
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
            if (customers != null)
            {
                profitableCustomersDTO = new List<ProfitableCustomerDTO>();

                foreach (var customer in customers)
                {
                    ProfitableCustomerDTO customerDTO = new ProfitableCustomerDTO();
                    customerDTO.FirstName = customer.FirstName;
                    customerDTO.LastName = customer.LastName;
                    customerDTO.IdentityCard = customer.IdentityCard;
                    customerDTO.LastMonthProfit = customer.Lines.Sum(p => p.Payments.Where(x => x.Date.Year == DateTime.Now.Year && x.Date.Month == DateTime.Now.Month).Sum(q => q.LineTotalPrice));

                    profitableCustomersDTO.Add(customerDTO);
                }
            }
            return profitableCustomersDTO;

        }

        /// <summary>
        /// Get the customers that two or more of the friends they call the most left the company
        /// </summary>
        /// <returns>List of customers if succeeded otherwise null</returns>
        public List<CustomerBiDTO> GetCustomersAtRiskOfAbandonment()
        {
            List<CustomerBiDTO> customersBiDTOs = new List<CustomerBiDTO>();
            List<Customer> customers;
            try
            {
                customers = _unitOfWork.Customer.GetCustomersAtRiskOfAbandonment();
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if(customers != null)
            {
                foreach (var customer in customers)
                {
                    CustomerBiDTO customerBi = new CustomerBiDTO();
                    customerBi.FirstName = customer.FirstName;
                    customerBi.LastName = customer.LastName;
                    customerBi.IdentityCard = customer.IdentityCard;

                    customersBiDTOs.Add(customerBi);
                }
            }
            return customersBiDTOs;
        }

        //Need to finish this function
        /// <summary>
        /// Get groups of customers who talks each other the most
        /// </summary>
        /// <returns>List of customers groups if succeeded otherwise null</returns>
        //public List<GroupDTO> GetGroupsOfFreindsWhoTalkEachOther()
        //{
        //    List<GroupDTO> groups = new List<GroupDTO>();
        //    List<Customer> customers;
        //    try
        //    {
        //        customers = _unitOfWork.Customer.GetActiveCustomersWithLinesAndCalls();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
        //        throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
        //    }

        //    if (customers != null)
        //    {
        //        List<MostCalledNumberCustomerDTO> mostCallCustomers = new List<MostCalledNumberCustomerDTO>();

        //        foreach (var customer in customers)
        //        {
        //            List<Call> calls = new List<Call>();
        //            foreach (var item in customer.Lines)
        //            {
        //                calls.AddRange(item.Calls);
        //            }
        //            List<Call> mostNumbers = calls.OrderByDescending((s) => calls.GroupBy(x => x.DestinationNumber).Count()).Take(3).ToList();

        //            MostCalledNumberCustomerDTO mostCallCustomer = new MostCalledNumberCustomerDTO()
        //            {
        //                Customer = customer,
        //                FirstNumber = mostNumbers[0].DestinationNumber,
        //                SecondNumber = mostNumbers[1].DestinationNumber,
        //                ThirdNumber = mostNumbers[2].DestinationNumber
        //            };

        //            mostCallCustomers.Add(mostCallCustomer);
        //        }

        //        //foreach (var customer in mostCallCustomers)
        //        //{
        //        //    foreach (var otherCustomer in mostCallCustomers)
        //        //    {
        //        //        if(customer.Customer)
        //        //    }
        //        //}
        //    }
        //    return groups;
        //}

        public List<GroupDTO> GetGroupsOfFreindsWhoTalkEachOther()
        {
            List<GroupDTO> groups = new List<GroupDTO>();
            List<Customer> customers;
            try
            {
                customers = _unitOfWork.Customer.GetActiveCustomersWithLinesAndCalls();
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }

            if (customers != null)
            {
                List<MostCalledNumberCustomerDTO> mostCallCustomers = new List<MostCalledNumberCustomerDTO>();

                foreach (var customer in customers)
                {
                    List<Call> calls = new List<Call>();
                    foreach (var item in customer.Lines)
                    {
                        calls.AddRange(item.Calls);
                    }
                    List<string> mostNumbers = calls.OrderByDescending((s) => calls.GroupBy(x => x.DestinationNumber).Count()).Select(d=>d.DestinationNumber).Take(3).ToList();

                    MostCalledNumberCustomerDTO mostCallCustomer = new MostCalledNumberCustomerDTO()
                    {
                        Customer = customer,
                        FirstNumber = mostNumbers[0],
                        SecondNumber = mostNumbers[1],
                        ThirdNumber = mostNumbers[2]
                    };

                    mostCallCustomers.Add(mostCallCustomer);
                }
            }
            return groups;
        }
    }
}