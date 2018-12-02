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
using Common.Enums;

namespace UnitTestServer
{
    public class MockData
    {
        public Employee GetEmployee()
        {
            return new Employee() { UserName = "israel", Password = "1", EmployeeId = 1, Customers = GetCustomers() };
        }
        public List<Employee> GetEmployees()
        {
            return new List<Employee>()
                {
                new Employee() { UserName = "israel", Password = "1" },
                new Employee() { UserName = "shay", Password = "1" },
                new Employee() { UserName = "levi", Password = "1" }
                };
        }

        internal List<Line> GetAllLinesWithPackageAndFriend()
        {
            List<Line> lines = GetLines(1);
            foreach (var line in lines)
            {
                line.Package = GetCustomPackage();
            }

            return lines;
        }

        public Customer GetCustomer()
        {
            return new Customer() { CallsToCenter = 1, Address = "Modi'in", CustomerTypeId = 2, CustomerId = 2, FirstName = "shay", LastName = "oo", IdentityCard = "124", IsActive = true, ContactNumber = "12341", EmplyeeId = 1, Lines = GetLines(2) };
        }
        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>() {
             new Customer() { CallsToCenter = 1, Address = "telAviv", CustomerTypeId = 1, CustomerId = 1, FirstName = "david",LastName ="oo", IdentityCard = "123", IsActive = true, ContactNumber="12340",EmplyeeId=1,Lines = GetLines(1)},
            new Customer() { CallsToCenter = 0, Address = "Modi'in", CustomerTypeId = 2, CustomerId = 2, FirstName = "shay", LastName = "oo", IdentityCard = "124", IsActive = true, ContactNumber = "12341", EmplyeeId = 1 ,Lines = GetLines(2)},
            new Customer() { CallsToCenter = 3, Address = "USA", CustomerTypeId = 3, CustomerId = 3, FirstName = "moshe", LastName = "oo", IdentityCard = "125", IsActive = true, ContactNumber = "12342", EmplyeeId = 2 ,Lines = GetLines(3)},
            new Customer() { CallsToCenter = 2, Address = "telAviv", CustomerTypeId = 1, CustomerId = 4, FirstName = "sara", LastName = "oo", IdentityCard = "126", IsActive = true, ContactNumber = "12343", EmplyeeId = 2 ,Lines = GetLines(4)}
            };
            return customers;
        }
        public Customer GetCustomerWithLinesAndPackage()
        {
            Customer customer = GetCustomer();
            foreach (var item in customer.Lines)
            {
                item.Package = GetCustomPackage();
            }
            return customer;
        }

        public Line GetLine()
        {
            return new Line() { LineId = 1, LineNumber = "1234567", CreatedDate = DateTime.Now, Status = Common.Enums.LineStatus.Used };
        }
        public Line GetLineWithPackageAndFriend()
        {
            Line line = GetLine();
            line.Package = GetCustomPackage();
            return line;
        }
        public List<Line> GetLines(int uniqueParamater)
        {
            return new List<Line>
            {
                new Line(){LineId=1+uniqueParamater,LineNumber ="1234567"+uniqueParamater.ToString(),CreatedDate = DateTime.Now,Status = Common.Enums.LineStatus.Used, Messages = GetSms("3456",1 + uniqueParamater), Calls = GetCalls("3556",1 + uniqueParamater)},
                new Line(){LineId=2+uniqueParamater,LineNumber ="1234568"+uniqueParamater.ToString(),CreatedDate = DateTime.Now.AddMonths(-1),Status = Common.Enums.LineStatus.Used, Messages = GetSms("234",2 + uniqueParamater), Calls = GetCalls("3453456",2 + uniqueParamater)},
                new Line(){LineId=3+uniqueParamater,LineNumber ="1234569"+uniqueParamater.ToString(),CreatedDate = DateTime.Now.AddMonths(-2),Status = Common.Enums.LineStatus.Removed,  Messages = GetSms("2345334",3 + uniqueParamater), Calls = GetCalls("2345",3 + uniqueParamater)},
            };
        }

        public List<Package> GetPackageTemplates()
        {
            return new List<Package>
            {
            new Package() {PackageId=1,PackageName="Soliders", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=100, MaxSms=1000, PriorityContact=false, TotalPrice=50},
            new Package() {PackageId=2, PackageName="Moms", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=200, MaxSms=5000, PriorityContact=false, TotalPrice=150},
            new Package() {PackageId=3, PackageName="SelaYahalom", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=300, MaxSms=10000, PriorityContact=false, TotalPrice=500},
            };
        }
        public Package GetPackageTemplate()
        {
            return GetPackageTemplates()[0];
        }
        public Package GetCustomPackage()
        {
            return new Package() { PackageName = "Custom", IsPackageTemplate = false, InsideFamilyCalles = true, DiscountPrecentage = 0, MaxMinute = 100, MaxSms = 1000, PriorityContact = true, TotalPrice = 50, PackageId = 1, Friends = GetFriends() };
        }

        public Friends GetFriends()
        {
            return new Friends()
            {
                FirstNumber = "1234",
                SecondNumber = "12345",
                ThirdNumber = "123456",
                FriendsId = 1
            };
        }

        public List<CustomerType> GetCustomerTypes()
        {
            return new List<CustomerType>()
                {
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Private, MinutePrice=3, SmsPrice=3},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Business, MinutePrice=2, SmsPrice=2},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Vip, MinutePrice=1, SmsPrice=1}
                };
        }
        public CustomerType GetCustomerType(CustomerTypeEnum type)
        {
            return GetCustomerTypes().Find(t => t.CustomerTypeEnum == type);
        }

        public List<Sms> GetSms(string destinationNumber, int unique)
        {
            return new List<Sms>() {
             new Sms(){ DataOfMessage = DateTime.Now, DestinationNumber = destinationNumber, SmsId = 1 + unique },
             new Sms() { DataOfMessage = DateTime.Now, DestinationNumber = destinationNumber, SmsId = 2 + unique },
             new Sms() { DataOfMessage = DateTime.Now, DestinationNumber = destinationNumber, SmsId = 3 + unique },
             new Sms() { DataOfMessage = DateTime.Now, DestinationNumber = destinationNumber, SmsId = 4 + unique },
             new Sms() { DataOfMessage = DateTime.Now, DestinationNumber = destinationNumber, SmsId = 5 + unique }
                    };
        }

        public List<Call> GetCalls(string destinationNumber, int unique)
        {
            return new List<Call>() {
             new Call(){ DateOfCall = DateTime.Now, DestinationNumber = destinationNumber, CallId = 1 + unique ,Duration =434},
             new Call() { DateOfCall= DateTime.Now, DestinationNumber = destinationNumber, CallId = 2 + unique ,Duration =9032},
             new Call() { DateOfCall= DateTime.Now, DestinationNumber = destinationNumber, CallId = 3 + unique ,Duration =323},
             new Call() { DateOfCall= DateTime.Now, DestinationNumber = destinationNumber, CallId = 4 + unique ,Duration =3403},
             new Call() { DateOfCall= DateTime.Now, DestinationNumber = destinationNumber, CallId = 5 + unique ,Duration =2322}
                    };
        }
    }
}
