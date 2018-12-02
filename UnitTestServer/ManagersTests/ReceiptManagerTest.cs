using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.RepositoryInterfaces;
using Moq;
using Server.Managers;
using Common.Models;
using Common.Enums;

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class ReceiptManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        ReceiptManager _manager;

        [TestMethod]
        public void GeneratePaymentsToAllLines_CreatePaymentsforAllCustomerLines_ReturnListOfPayments()
        {
            //arrange
            var lines = _data.GetAllLinesWithPackageAndFriend();
            var customer = _data.GetCustomer();
            customer.CustomerType = _data.GetCustomerType(CustomerTypeEnum.Private);
            _mock.Setup(s => s.Line.GetAllLinesWithAllEntities())
                .Returns(lines);

            _mock.Setup(s => s.Customer.GetCustomerWithTypeAndLines(It.IsAny<int>()))
               .Returns(customer);

            _manager = new ReceiptManager(_mock.Object);

            //act
            var res = _manager.GeneratePaymentsToAllLines(DateTime.Now);

            //assert
            Assert.AreEqual(3, res.Count);
        }
    }
}
