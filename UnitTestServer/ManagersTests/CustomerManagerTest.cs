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
        public void AddNewCustomer_AddCustomer_ReturnCustomer()
        {
            //arrange
            _mock.Setup(s => s.Customer.GetActiveCustomerByIdCard("1"))
                .Returns(new Customer());
            _manager = new CustomerManager(_mock.Object);

            //act
            var res = _manager.AddNewCustomer(new Customer() { IdentityCard = "1" });

            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void DeactivateCustomer_ChangeCustomerIsActiveValue_ReturnCustomer()
        {
            //arrange
            Customer customer = _data.GetCustomer();
            customer.IsActive = true;

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                .Returns(customer);
            _mock.Setup(s => s.Line.DeactivateLine(It.IsAny<Line>()))
               .Returns(new Line());
            _manager = new CustomerManager(_mock.Object);

            //act
            var res = _manager.DeactivateCustomer("349834");

            //assert
            Assert.IsNotNull(res);
            Assert.AreEqual(false, res.IsActive);

        }

        [TestMethod]
        public void GetActiveCustomer_GetCustomer_ReturnCustomer()
        {
            //arrange
            _mock.Setup(s => s.Customer.GetActiveCustomerByIdCard(It.IsAny<string>()))
                .Returns(new Customer());
            _manager = new CustomerManager(_mock.Object);

            //act
            var res = _manager.GetActiveCustomer("2");

            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void GetCustomerValue_CustomerWith3LinesAnd1CallToCenter_ReturnZeroPointFive()
        {
            //arrange
            _mock.Setup(s => s.Customer.GetCustomerWithLinesAndPayments(It.IsAny<string>()))
                .Returns(_data.GetCustomer());
            _manager = new CustomerManager(_mock.Object);

            //act
            double value = _manager.GetCustomerValue("1");

            //assert
            Assert.AreEqual(0.5, Math.Round(value, 1));
            Assert.AreNotEqual(0.2, Math.Round(value, 1));
        }

        [TestMethod]
        public void EditCustomer_ChangeFirstName_ReturnEditedCustomer()
        {
            //arrange
            Customer customerToEdit = _data.GetCustomer();

            _mock.Setup(s => s.Customer.GetActiveCustomerByIdCard(It.IsAny<string>()))
                .Returns(_data.GetCustomer());

            _mock.Setup(s => s.Customer.Edit(It.IsAny<Customer>(), It.IsAny<Customer>()))
                .Callback<Customer, Customer>((c1, c2) =>
                 {
                     customerToEdit.FirstName = c2.FirstName;
                 });

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLinesAndPackages(It.IsAny<string>()))
                .Returns(customerToEdit);

            _manager = new CustomerManager(_mock.Object);
            customerToEdit.FirstName = "newFirstName";

            //act
            var res = _manager.EditCustomer(customerToEdit);

            //assert
            Assert.AreEqual("newFirstName", res.FirstName);
        }
    }
}
