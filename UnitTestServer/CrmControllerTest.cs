using System;
using System.Net.Http;
using Common.Models;
using Common.RepositoryInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Server.Controllers.CrmControllers;
using Server.Managers;
using Db.Repositories;
using Server;
using Common.Interfaces.ServerManagersInterfaces;
using System.Collections.Generic;

namespace UnitTestServer
{
    [TestClass]
    public class CrmControllerTest
    {
        Mock<IUnitOfWork> mock = new Mock<IUnitOfWork>();
        CustomerManager manager;

        [TestMethod]
        public void AddNewCustomer_ReturnCustomer()
        {
            mock.Setup(s => s.Customer.GetActiveCustomerByIdCard("1"))
                .Returns(new Customer());
            manager = new CustomerManager(mock.Object);
            var res = manager.AddNewCustomer(new Customer() { IdentityCard = "1" });
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void DeactivateCustomer_returnCustomer()
        {
            Customer customer = new Customer() { Address = "sdd" };

            mock.Setup(s => s.Customer.GetActiveCustomerWithLines("1"))
                .Returns(customer);
            manager = new CustomerManager(mock.Object);
            var res = manager.DeactivateCustomer("1");
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void GetActiveCustomer_returnCustomer()
        {
            mock.Setup(s => s.Customer.GetActiveCustomerByIdCard(It.IsAny<string>()))
                .Returns(new Customer());
            manager = new CustomerManager(mock.Object);
            Assert.IsNotNull(manager.GetActiveCustomer("2"));
        }

        [TestMethod]
        public void GetCustomerValue_ReturnZeroPointThree()
        {
            mock.Setup(s => s.Customer.GetCustomerWithLinesAndPayments(It.IsAny<string>()))
                .Returns(GetCustomer());
            manager = new CustomerManager(mock.Object);
            double value = manager.GetCustomerValue("1");
            Assert.AreEqual(0.3, Math.Round(value, 1));
            Assert.AreNotEqual(0.2, Math.Round(value, 1));
        }

        private Customer GetCustomer()
        {
            List<Line> lines = new List<Line>()
            { new Line{ LineNumber = "111", CreatedDate = DateTime.Now },
            new Line() { LineNumber = "111", CreatedDate = DateTime.Now } };
            return new Customer() { Lines = lines, CallsToCenter = 1 };
        }
    }
}
