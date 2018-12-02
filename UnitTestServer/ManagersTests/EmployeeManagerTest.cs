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
using Common.ModelsDTO;

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class EmployeeManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        EmployeeManager _manager;

        [TestMethod]
        public void Login_FindMatchBetweenUsernameNndPassword_ReturnFoundedEmployee()
        {
            //arrange
            LoginDTO loginDTO = new LoginDTO() { UserName = "israel", Password = "1" };
            Employee employee = _data.GetEmployee();
            _mock.Setup(s => s.Employee.GetEmployeeByUserName("israel"))
                .Returns(employee);
            _manager = new EmployeeManager(_mock.Object);

            //act
            var res = _manager.Login(loginDTO);

            //assert
            Assert.AreEqual("israel", res.UserName);
            Assert.AreEqual("1", res.Password);
        }



    }
}
