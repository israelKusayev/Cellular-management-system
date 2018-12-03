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
                new Employee() { UserName = "admin", Password = "admin" , IsManager = true}
                };

                List<Package> packages = new List<Package>()
                {
                new Package() {PackageName="Soliders", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=100, MaxSms=1000, PriorityContact=false, TotalPrice=50},
                new Package() {PackageName="Moms", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=200, MaxSms=5000, PriorityContact=false, TotalPrice=150},
                new Package() {PackageName="SelaYahalom", IsPackageTemplate=true, InsideFamilyCalles=false, DiscountPrecentage=0, MaxMinute=300, MaxSms=10000, PriorityContact=false, TotalPrice=500},
                };

                List<CustomerType> clientTypes = new List<CustomerType>()
                {
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Private, MinutePrice=3, SmsPrice=3},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Business, MinutePrice=2, SmsPrice=2},
                new CustomerType() {CustomerTypeEnum = CustomerTypeEnum.Vip, MinutePrice=1, SmsPrice=1}
                };

                List<Customer> customers = new List<Customer>
                {
                    new Customer() { IdentityCard = "1", Address = "1", FirstName = "1", LastName = "1", ContactNumber = "1", CustomerTypeId = 1, IsActive = true },
                    new Customer() { IdentityCard = "2", Address = "2", FirstName = "2", LastName = "2", ContactNumber = "2", CustomerTypeId = 2, IsActive = true },
                    new Customer() { IdentityCard = "3", Address = "3", FirstName = "3", LastName = "3", ContactNumber = "3", CustomerTypeId = 1, IsActive = true }
                };

                context.EmplyeesTable.AddRange(emplyees);
                context.PackagesTable.AddRange(packages);
                context.CustomerTypesTable.AddRange(clientTypes);
                context.SaveChanges();

                context.CustomerTable.AddRange(customers);

                context.SaveChanges();
            }

        }
    }
}
