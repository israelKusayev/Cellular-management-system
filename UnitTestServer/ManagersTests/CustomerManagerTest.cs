using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;
using Common.RepositoryInterfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Server.Managers;
using UnitTestServer;

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class CustomerManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        CustomerManager _manager;

        [TestMethod]
        public void AddNewCustomer_ReturnCustomer()
        {
            _mock.Setup(s => s.Customer.GetActiveCustomerByIdCard("1"))
                .Returns(new Customer());
            _manager = new CustomerManager(_mock.Object);
            var res = _manager.AddNewCustomer(new Customer() { IdentityCard = "1" });
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void DeactivateCustomer_returnCustomer()
        {
            Customer customer = new Customer() { Address = "sdd" };

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines("1"))
                .Returns(customer);
            _manager = new CustomerManager(_mock.Object);
            var res = _manager.DeactivateCustomer("1");
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void GetActiveCustomer_returnCustomer()
        {
            _mock.Setup(s => s.Customer.GetActiveCustomerByIdCard(It.IsAny<string>()))
                .Returns(new Customer());
            _manager = new CustomerManager(_mock.Object);
            Assert.IsNotNull(_manager.GetActiveCustomer("2"));
        }

        [TestMethod]
        public void GetCustomerValue_ReturnZeroPointFive()
        {
            _mock.Setup(s => s.Customer.GetCustomerWithLinesAndPayments(It.IsAny<string>()))
                .Returns(_data.GetCustomer());
            _manager = new CustomerManager(_mock.Object);
            double value = _manager.GetCustomerValue("1");
            Assert.AreEqual(0.5, Math.Round(value, 1));
            Assert.AreNotEqual(0.2, Math.Round(value, 1));
        }


    }
}
