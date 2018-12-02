using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.RepositoryInterfaces;
using Moq;
using Server.Managers;
using Common.ModelsDTO;
using Common.Models;
using Common.Enums;

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class SimulatorManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        SimulatorManager _manager;

        [TestMethod]
        public void SimulateCallsOrSms_SendSmsWithStatusAll_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() {LineId = 1 ,SendTo = SimulateSendTo.All, IsSms = true };
          
            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List <Line> lines= _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Sms.Add(new Sms()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendSmsWithStatusGeneral_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.General, IsSms = true };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Sms.Add(new Sms()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendSmsWithStatusFriends_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.Friends, IsSms = true };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Sms.Add(new Sms()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendSmsWithStatusFamily_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.Family, IsSms = true };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Sms.Add(new Sms()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendCallWithStatusAll_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.All, IsSms = false, MinDuration=100, MaxDuration= 1000 };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Call.Add(new Call()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendCallWithStatusGeneral_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.General, IsSms = false, MinDuration = 100, MaxDuration = 1000 };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Call.Add(new Call()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendCallWithStatusFriends_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.Friends, IsSms = false, MinDuration = 100, MaxDuration = 1000 };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Call.Add(new Call()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }

        [TestMethod]
        public void SimulateCallsOrSms_SendCallWithStatusFamily_ReturnTrue()
        {
            //arrange
            SimulateDTO simulateDTO = new SimulateDTO() { LineId = 1, SendTo = SimulateSendTo.Family, IsSms = false, MinDuration = 100, MaxDuration = 1000 };

            Customer customer = _data.GetCustomerWithLinesAndPackage();
            Line line = _data.GetLineWithPackageAndFriend();
            List<Line> lines = _data.GetLines(1);

            _mock.Setup(s => s.Customer.GetActiveCustomerWithLines(It.IsAny<string>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                 .Returns(line);

            _mock.Setup(s => s.Line.GetAll())
                 .Returns(lines);

            _mock.Setup(s => s.Call.Add(new Call()));

            _manager = new SimulatorManager(_mock.Object);

            //act
            var res = _manager.SimulateCallsOrSms(simulateDTO);

            //assert
            Assert.AreEqual(true, res);
        }


    }
}
