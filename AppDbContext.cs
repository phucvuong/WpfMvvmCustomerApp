using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMvvmCustomerApp
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Kết nối SQL Server
            optionsBuilder.UseSqlServer("Server=localhost;Database=CustomerDB;Trusted_Connection=True;TrustServerCertificate=True;");
        }
    }
}
