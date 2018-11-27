using Common.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class CellularContext : DbContext
    {
        public CellularContext() : base("Cellular8")
        {
        }

        public void InitDataBase()
        {
            if (!Database.Exists())
            {
                new DbInitializer().Seed();
            }
        }

        public DbSet<Customer> CustomerTable { get; set; }
        public DbSet<CustomerType> CustomerTypesTable { get; set; }
        public DbSet<Friends> FavoriveNumbersTable { get; set; }
        public DbSet<Line> LinesTable { get; set; }
        public DbSet<Package> PackagesTable { get; set; }
        public DbSet<Payment> PaymentsTable { get; set; }
        public DbSet<Call> CallTable { get; set; }
        public DbSet<Sms> SmsTable { get; set; }
        public DbSet<Employee> EmplyeesTable { get; set; }
    }
}

