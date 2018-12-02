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
    public class LineManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        LineManager _manager;

        [TestMethod]
        public void GetCustomerLines_GetAllLinesWithUsedStatus_ReturnsFoundedLines()
        {
            //arrange
            Customer customer = _data.GetCustomerWithLinesAndPackage();
            _mock.Setup(s => s.Customer.GetActiveCustomerWithLinesAndPackages(It.IsAny<string>()))
                .Returns(customer);
            _manager = new LineManager(_mock.Object);

            //act
            var res = _manager.GetCustomerLines("1");

            //assert
            Assert.AreEqual(2, res.Count);
        }

        [TestMethod]
        public void AddNewLine_LineNumberIsAvailable_ReturnsAddedLine()
        {
            //arrange
            Line lineToAdd = new Line() { LineNumber = "12345",Status = Common.Enums.LineStatus.Avaliable };
            Customer customer = _data.GetCustomer();


            _mock.Setup(s => s.Customer.Get(It.IsAny<int>()))
                 .Returns(customer);

            _mock.Setup(s => s.Line.LineNumberIsAvailable("1"))
            .Returns(() => null);

            _mock.Setup(s => s.Line.GetLineByLineNumber(It.IsAny<string>()))
                .Returns(lineToAdd);
            _manager = new LineManager(_mock.Object);

            //act
            var res = _manager.AddNewLine(lineToAdd, 1);

            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void DeactivateLine_EditLineToDeactiveStatus_ReturnsEditedLine()
        {
            //arrange
            Line line = _data.GetLine();
            line.Status = LineStatus.Used;

            _mock.Setup(s => s.Line.Get(It.IsAny<int>()))
                 .Returns(line);
            _mock.Setup(s => s.Line.DeactivateLine(line))
                .Callback<Line>((l)=> 
                {
                    l.Status = LineStatus.Removed;
                })
                 .Returns(line);

            _manager = new LineManager(_mock.Object);

            //act
            var res = _manager.DeactivateLine(1);

            //assert
            Assert.AreEqual(LineStatus.Removed,res.Status);
        }

    }
}
