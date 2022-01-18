using LazyReader.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace LazyReader
{
    public class LazyReaderContext : DbContext
    {
        private static string baseConnectionString = $"Data Source={AppDomain.CurrentDomain.BaseDirectory}LazyReader.db";
        private static string password = "LazyReader";
        public static string connectionString = new SqliteConnectionStringBuilder(baseConnectionString)
        {
            Mode = SqliteOpenMode.ReadWriteCreate,
            Password = password
        }.ToString();

        static LazyReaderContext instance = null;
        private static readonly object padlock = new object();
        public static LazyReaderContext Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new LazyReaderContext();
                        }
                    }
                }
                return instance;
            }
        }

        private static bool _created = false;
        public LazyReaderContext()
        {
            if (!_created)
            {
                _created = true;
                //Database.EnsureDeleted();
                Database.EnsureCreated();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Book>().HasKey(t => new { t.Name, t.BaseDomain });
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite(connectionString);
            base.OnConfiguring(options);
        }

        public DbSet<BaseDomain> BaseDomain { set; get; }
        public DbSet<Book> Book { set; get; }
    }
}
