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

        public BiManager(IUnitOfWork unitOfWork)
        {
            _logger = new LoggerManager(new FileLogger(), "biManager.txt");
            _unitOfWork = unitOfWork;
        }

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
    }
}