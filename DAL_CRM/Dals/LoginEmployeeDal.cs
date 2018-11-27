using Common.DataConfig;
using Common.Enums;
using Common.Exeptions;
using Common.Logger;
using Common.Models;
using Common.ModelsDTO;
using Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.Dal.Dals
{
    public class LoginEmployeeDal
    {
        LoggerManager _logger;

        public LoginEmployeeDal()
        {
            _logger = new LoggerManager(new FileLogger(), "loginDal.txt");
        }

        /// <summary>
        /// Checking system login
        /// </summary>
        /// <returns>A employee model if login successful </returns>
        public Employee Login(LoginDTO loginEmployee)
        {
            Employee requstedEmployee = null;

            try
            {
                using (var context = new CellularContext())
                {
                    requstedEmployee = context.EmplyeesTable.SingleOrDefault((e) => e.UserName == loginEmployee.UserName);
                }
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

            return requstedEmployee;
        }
    }
}
