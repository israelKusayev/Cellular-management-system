using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common.RepositoryInterfaces;
using Moq;
using Server.Managers;
using Common.Models;

namespace UnitTestServer.ManagersTests
{
    [TestClass]
    public class PackageManagerTest
    {
        MockData _data = new MockData();
        Mock<IUnitOfWork> _mock = new Mock<IUnitOfWork>();
        PackageManager _manager;

        [TestMethod]
        public void GetPackageTemplates_GetAllPackagesTemplates_ReturnFoundedPackageTemplates()
        {
            //arrange
            _mock.Setup(s => s.Package.GetPackageTemplate())
                .Returns(_data.GetPackageTemplates());
            _manager = new PackageManager(_mock.Object);

            //act
            var packages = _manager.GetPackageTemplates();
            //assert
            Assert.IsNotNull(packages);
        }

        [TestMethod]
        public void GetPackage_GetLinePackageByLineId_ReturnFoundedPackage()
        {
            //arrange
            Line line = _data.GetLine();
            line.Package = _data.GetCustomPackage();
            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(1))
                .Returns(line);
            _manager = new PackageManager(_mock.Object);

            //act
            var package = _manager.GetPackage(1);

            //assert
            Assert.IsNotNull(package);
        }

        [TestMethod]
        public void AddPackageToLine_AddCustomPackage_ReturnAddedPackage()
        {
            //arrange
            Line line = _data.GetLine();
            _mock.Setup(s => s.Line.Get(It.IsAny<int>()))
                .Returns(line);

            _manager = new PackageManager(_mock.Object);

            //act
            var package = _manager.AddPackageToLine(1, _data.GetCustomPackage());

            //assert
            Assert.IsNotNull(package);
        }

        [TestMethod]
        public void AddPackageToLine_AddTemplatePackage_ReturnAddedPackage()
        {
            //arrange
            Line line = _data.GetLine();
            _mock.Setup(s => s.Line.Get(It.IsAny<int>()))
                .Returns(line);
            _mock.Setup(s => s.Package.Get(1))
                .Returns(_data.GetPackageTemplate());
            _mock.Setup(s => s.Line.Add(It.IsAny<Line>()));

            _manager = new PackageManager(_mock.Object);

            //act
            var package = _manager.AddPackageToLine(1, _data.GetPackageTemplate());

            //assert
            Assert.IsNotNull(package);
        }

        [TestMethod]
        public void RemovePackageFromLine_RemovePackage_ReturnRemovedPackage()
        {
            //arrange
            Line line = _data.GetLine();
            line.Package = _data.GetCustomPackage();
            _mock.Setup(s => s.Line.GetLineWithPackageAndFriends(It.IsAny<int>()))
                .Returns(line);

            _manager = new PackageManager(_mock.Object);

            //act
            var package = _manager.RemovePackageFromLine(1);

            //assert
            Assert.IsNotNull(package);
        }

        [TestMethod]
        public void EditPackage_EditPackageTemplate_ReturnEditedPackage()
        {
            //arrange
            Line line = _data.GetLine();
            line.LineNumber = "090";

            _mock.Setup(s => s.Line.Get(It.IsAny<int>()))
                .Returns(line);

            _mock.Setup(s => s.Package.Get(It.IsAny<int>()))
                .Returns(_data.GetPackageTemplate());

            _manager = new PackageManager(_mock.Object);

            //act
            var package = _manager.EditPackage(1, 1, _data.GetPackageTemplate());

            //assert
            Assert.IsNotNull(package.Lines.Contains(line));
        }

        [TestMethod]
        public void EditPackage_EditCustomTemplate_ReturnEditedPackage()
        {
            //arrange
            Package package = _data.GetCustomPackage();
            package.PriorityContact = false;

            Package packageToEdit = _data.GetCustomPackage();
            packageToEdit.PriorityContact = true;

            _mock.Setup(s => s.Package.GetPackageWithFriends(It.IsAny<int>()))
                .Returns(package);

            _mock.Setup(s => s.Package.Edit(It.IsAny<Package>(), It.IsAny<Package>()))
                .Callback<Package, Package>((p1, p2) =>
                {
                    package.PriorityContact = p2.PriorityContact;
                });
            _manager = new PackageManager(_mock.Object);

            //act
            var EditedPackage = _manager.EditPackage(1, 1, packageToEdit);

            //assert
            Assert.IsNotNull(EditedPackage.PriorityContact = true);
        }

        [TestMethod]
        public void RemoveLineFromTemplatePackage_RemoveLine_ReturnTrue()
        {
            //arrange
            Line line = _data.GetLine();
            line.Package = _data.GetPackageTemplate();

            _mock.Setup(s => s.Line.GetLineWithPackage(It.IsAny<int>()))
                .Returns(line);

            _manager = new PackageManager(_mock.Object);

            //act
            bool succeeded = _manager.RemoveLineFromTemplatePackage(1);

            //assert
            Assert.IsTrue(succeeded);
        }

        [TestMethod]
        public void AddFriends_AddFriendsModelToPackage_ReturnFriends()
        {
            //arrange
            Package package = _data.GetCustomPackage();
            package.Friends = null;

            _mock.Setup(s => s.Package.Get(It.IsAny<int>()))
                .Returns(package);

            _mock.Setup(s => s.Package.GetPackageWithFriends(It.IsAny<int>()))
               .Returns(_data.GetCustomPackage());

            _manager = new PackageManager(_mock.Object);

            //act
            var res = _manager.AddFriends(1, _data.GetFriends());

            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void EditFriends_EditFriendsToPackage_ReturnEditedFriends()
        {
            //arrange
            Package package = _data.GetCustomPackage();

            _mock.Setup(s => s.Package.GetPackageWithFriends(It.IsAny<int>()))
               .Returns(package);

            _mock.Setup(s => s.Friends.Edit(It.IsAny<Friends>(), It.IsAny<Friends>()));


            _manager = new PackageManager(_mock.Object);

            //act
            var res = _manager.EditFriends(1, _data.GetFriends());

            //assert
            Assert.IsNotNull(res);
        }

        [TestMethod]
        public void EditFriends_ChangeFriendsToNull_ReturnNull()
        {
            //arrange
            Package package = _data.GetCustomPackage();

            _mock.Setup(s => s.Package.GetPackageWithFriends(It.IsAny<int>()))
               .Returns(package);

            _mock.Setup(s => s.Friends.Edit(It.IsAny<Friends>(), It.IsAny<Friends>()));


            _manager = new PackageManager(_mock.Object);

            //act
            var res = _manager.EditFriends(1, _data.GetFriends());

            //assert
            Assert.IsNotNull(res);
        }
    }
}
