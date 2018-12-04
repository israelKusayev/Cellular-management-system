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

            if(!requstedEmployee.IsManager)
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
                    //employeeDTO.LastMonthSells ???
                    employeesDTO.Add(employeeDTO);
                }
            }
            return employeesDTO;
        }
    }
}