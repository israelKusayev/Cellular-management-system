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

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class ReceiptManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        ReceiptManager _manager;

        [TestMethod]
        public void GeneratePaymentsToAllLines_ChangeFriendsToNull_ReturnReceiptDTO()
        {
            //arrange
            Package package = _data.GetCustomPackage();

            var lines = _data.GetLines(1);
            foreach (var item in lines)
            {
                item.Package = _data.GetCustomPackage();
            }

            _mock.Setup(s => s.Line.GetAllLinesWithAllEntities())
                .Returns(_data.GetLines(1));

            var customer = _data.GetCustomer();
            customer.CustomerType
            _mock.Setup(s => s.Customer.GetCustomerWithTypeAndLines(It.IsAny<int>()))
               .Returns();

            _manager = new ReceiptManager(_mock.Object);

            //act
            var res = _manager.GeneratePaymentsToAllLines(DateTime.Now);

            //assert
            Assert.IsNotNull(res);
        }
    }
}
