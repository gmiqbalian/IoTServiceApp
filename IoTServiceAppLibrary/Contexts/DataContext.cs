using IoTServiceAppLibrary.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTServiceAppLibrary.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext()
        {
            Database.EnsureCreated();
            try
            {
                Database.Migrate();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); };
        }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.EnsureCreated();
            try
            {
                Database.Migrate();
            }
            catch (Exception ex) { Debug.WriteLine(ex.Message); };
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database.sqlite.db");
        }
        public DbSet<AppSettings> AppSettings { get; set; }
    }
}
