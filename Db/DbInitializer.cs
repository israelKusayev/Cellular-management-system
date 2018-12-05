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
                new Employee() { UserName = "israel", Password = "1", IsManager = false  },
                new Employee() { UserName = "shay", Password = "1", IsManager = false    },
                new Employee() { UserName = "levi", Password = "1" , IsManager = false   },
                new Employee() { UserName = "oded", Password = "1234" , IsManager = false},
                new Employee() { UserName = "david", Password = "1" , IsManager = false  },
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

                context.EmplyeesTable.AddRange(emplyees);
                context.PackagesTable.AddRange(packages);
                context.CustomerTypesTable.AddRange(clientTypes);
                context.SaveChanges();
                for (int i = 1; i < 6; i++)
                {
                    var customers = GetCustomers(i);
                    for (int j = 0; j < customers.Count; j++)
                    {
                        var employee = context.EmplyeesTable.SingleOrDefault(x => x.EmployeeId == i);
                        customers[j].EmplyeeId = employee.EmployeeId;
                        employee.Customers.Add(customers[j]);
                    }
                }
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
        Random r = new Random();
        public Friends GetFriends()
        {
            return new Friends()
            {
                FirstNumber = r.Next(1, 1000000).ToString(),
                SecondNumber = r.Next(1, 1000000).ToString(),
                ThirdNumber = r.Next(1, 1000000).ToString(),
            };
        }

        public List<Customer> GetCustomers(int uniqueParamater)
        {
            return new List<Customer>
                {
                    new Customer() { IdentityCard = "1"+uniqueParamater.ToString(), Address =r.Next(1, 1000000).ToString(), FirstName =r.Next(1, 1000000).ToString(), LastName = "1", ContactNumber = r.Next(1, 1000000).ToString(), CustomerTypeId = 1, IsActive = true, Lines = GetLines(1+uniqueParamater)},
                    new Customer() { IdentityCard = "2"+uniqueParamater.ToString(), Address =r.Next(1, 1000000).ToString(), FirstName =r.Next(1, 1000000).ToString(), LastName = "2", ContactNumber = r.Next(1, 1000000).ToString(), CustomerTypeId = 2, IsActive = true, Lines = GetLines(2+uniqueParamater)},
                    new Customer() { IdentityCard = "3"+uniqueParamater.ToString(), Address =r.Next(1, 1000000).ToString(), FirstName =r.Next(1, 1000000).ToString(), LastName = "3", ContactNumber = r.Next(1, 1000000).ToString(), CustomerTypeId = 1, IsActive = true, Lines = GetLines(3+uniqueParamater)},
                    new Customer() { IdentityCard = "4"+uniqueParamater.ToString(), Address =r.Next(1, 1000000).ToString(), FirstName =r.Next(1, 1000000).ToString(), LastName = "4", ContactNumber = r.Next(1, 1000000).ToString(), CustomerTypeId = 1, IsActive = true, Lines = GetLines(4+uniqueParamater)},
                    new Customer() { IdentityCard = "5"+uniqueParamater.ToString(), Address =r.Next(1, 1000000).ToString(), FirstName =r.Next(1, 1000000).ToString(), LastName = "5", ContactNumber = r.Next(1, 1000000).ToString(), CustomerTypeId = 3, IsActive = true, Lines = GetLines(5+uniqueParamater)},
               };
        }
    }
}
