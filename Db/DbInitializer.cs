using Common.Enums;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class DbInitializer
    {
        public void Seed()
        {
            using (var context = new CellularContext())
            {
                List<Employee> emplyees = new List<Employee>()
                {
                new Employee() { UserName = "israel", Password = "1", IsManager = false},
                new Employee() { UserName = "shay", Password = "1", IsManager = false },
                new Employee() { UserName = "levi", Password = "1" , IsManager = false},
                new Employee() { UserName = "oded", Password = "1234" , IsManager = false},
                new Employee() { UserName = "david", Password = "1" , IsManager = false},
                new Employee() { UserName = "admin", Password = "admin" , IsManager = true}
                };

                List<Package> packages = new List<Package>()
                {
                new Package() {PackageName="Soliders", IsPackageTemplate=true, InsideFamilyCalles=false,  MaxMinute=100, MaxSms=1000, PriorityContact=false, TotalPrice=50},
                new Package() {PackageName="Moms", IsPackageTemplate=true, InsideFamilyCalles=false,  MaxMinute=200, MaxSms=5000, PriorityContact=false, TotalPrice=150},
                new Package() {PackageName="SelaYahalom", IsPackageTemplate=true, InsideFamilyCalles=false,  MaxMinute=300, MaxSms=10000, PriorityContact=false, TotalPrice=500},
                new Package() {PackageName="Children", IsPackageTemplate=true, InsideFamilyCalles=false,  MaxMinute=20, MaxSms=10, PriorityContact=false, TotalPrice=15},
                new Package() {PackageName="students", IsPackageTemplate=true, InsideFamilyCalles=false,  MaxMinute=1000, MaxSms=4000, PriorityContact=false, TotalPrice=300},
                };

                List<CustomerType> clientTypes = new List<CustomerType>()
                {
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Private, MinutePrice=3, SmsPrice=3},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Business, MinutePrice=2, SmsPrice=2},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Vip, MinutePrice=1, SmsPrice=1}
                };

                List<Customer> customers = new List<Customer>
                {
                    new Customer() { IdentityCard = "1", Address = "1", FirstName = "1", LastName = "1", ContactNumber = "1", CustomerTypeId = 1, IsActive = true, Lines = GetLines(1) ,EmplyeeId=2},
                    new Customer() { IdentityCard = "2", Address = "2", FirstName = "2", LastName = "2", ContactNumber = "2", CustomerTypeId = 2, IsActive = true, Lines = GetLines(2) ,EmplyeeId=1},
                    new Customer() { IdentityCard = "3", Address = "3", FirstName = "3", LastName = "3", ContactNumber = "3", CustomerTypeId = 1, IsActive = true, Lines = GetLines(3) ,EmplyeeId=2},
                    new Customer() { IdentityCard = "4", Address = "4", FirstName = "4", LastName = "4", ContactNumber = "4", CustomerTypeId = 1, IsActive = true, Lines = GetLines(4) ,EmplyeeId=2},
                    new Customer() { IdentityCard = "5", Address = "5", FirstName = "5", LastName = "5", ContactNumber = "5", CustomerTypeId = 1, IsActive = true, Lines = GetLines(5) ,EmplyeeId=1},
                    new Customer() { IdentityCard = "6", Address = "6", FirstName = "6", LastName = "6", ContactNumber = "6", CustomerTypeId = 1, IsActive = true, Lines = GetLines(6) ,EmplyeeId=4},
                    new Customer() { IdentityCard = "7", Address = "7", FirstName = "7", LastName = "7", ContactNumber = "7", CustomerTypeId = 1, IsActive = true, Lines = GetLines(7) ,EmplyeeId=4},
                    new Customer() { IdentityCard = "8", Address = "8", FirstName = "8", LastName = "8", ContactNumber = "8", CustomerTypeId = 1, IsActive = true, Lines = GetLines(8) ,EmplyeeId=4},
                    new Customer() { IdentityCard = "9", Address = "9", FirstName = "9", LastName = "9", ContactNumber = "9", CustomerTypeId = 1, IsActive = true, Lines = GetLines(9) ,EmplyeeId=3}
                };




                context.EmplyeesTable.AddRange(emplyees);
                context.PackagesTable.AddRange(packages);
                context.CustomerTypesTable.AddRange(clientTypes);
                context.SaveChanges();

                context.CustomerTable.AddRange(customers);

                context.SaveChanges();
            }

        }

        public List<Line> GetLines(int uniqueParamater)
        {
            return new List<Line>
            {
                new Line(){LineNumber ="1234567"+uniqueParamater.ToString(),CreatedDate = DateTime.Now,Status = Common.Enums.LineStatus.Used ,Package = GetCustomPackage()},
                new Line(){LineNumber ="2234567"+uniqueParamater.ToString(),CreatedDate = DateTime.Now,Status = Common.Enums.LineStatus.Used,Package=GetCustomPackage() },
                new Line(){LineNumber ="2234567"+uniqueParamater.ToString(),CreatedDate = DateTime.Now,Status = Common.Enums.LineStatus.Used },
                new Line(){LineNumber ="1234568"+uniqueParamater.ToString(),CreatedDate = DateTime.Now.AddMonths(-1),Status = Common.Enums.LineStatus.Used },
                new Line(){LineNumber ="1234569"+uniqueParamater.ToString(),CreatedDate = DateTime.Now.AddMonths(-2),Status = Common.Enums.LineStatus.Removed }
            };
        }

        public Package GetCustomPackage()
        {
            return new Package() { PackageName = "Custom", IsPackageTemplate = false, InsideFamilyCalles = true, MaxMinute = 100, MaxSms = 1000, PriorityContact = true, TotalPrice = 50, PackageId = 1, Friends = GetFriends() };
        }
        public Friends GetFriends()
        {
            Random r = new Random();
            return new Friends()
            {

                FirstNumber = r.Next(1, 1000000).ToString(),
                SecondNumber = r.Next(1, 1000000).ToString(),
                ThirdNumber = r.Next(1, 1000000).ToString(),
            };
        }
    }
}
