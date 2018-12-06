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
    public class CustomerWebsiteManager : ICustomerWebsiteManager
    {
        private LoggerManager _logger;
        private IUnitOfWork _unitOfWork;

        //ctor
        public CustomerWebsiteManager(IUnitOfWork unitOfWork)
        {
            _logger = new LoggerManager(new FileLogger(), "customerWebsiteManager.txt");
            _unitOfWork = unitOfWork;
        }

        public Customer GetCustomerWithLines(string idCard)
        {
            try
            {
                return _unitOfWork.Customer.GetActiveCustomerWithLines(idCard);
            }
            catch (Exception e)
            {
                _logger.Log($"{Messages.messageFor[MessageType.GeneralDbFaild]} Execption details: {e.Message}");
                throw new FaildToConnectDbExeption(Messages.messageFor[MessageType.GeneralDbFaild]);
            }
        }

        public LineWebsiteDTO GetLineDetails(int lineId)
        {
            throw new NotImplementedException();
        }
    }
}