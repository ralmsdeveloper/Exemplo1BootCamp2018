using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Expressions;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace EFCoreBootcamp2018
{
    class Program
    {
        static void Main(string[] args)
        { 
            using(var db = new EventoContext())
            {
                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();

                var evento = db.Set<Evento>()
                    .Where(p => Funcoes.DateDIFF("DAY", p.Data, DateTime.Now) > 0)
                    .ToList();

            }

            Console.ReadKey();
        }
    }  

    class EventoContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            base.OnConfiguring(optionsBuilder);
            var cnxStr = new System.Data.SqlClient.SqlConnectionStringBuilder
            {
                InitialCatalog = "EVENTOTOPUFS",
                DataSource = "(localdb)\\mssqllocaldb",
                IntegratedSecurity = true
            }.ConnectionString;

            optionsBuilder.UseSqlServer(cnxStr);
            optionsBuilder.UseLoggerFactory(new LoggerFactory()
                .AddConsole()
                .AddDebug());
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Evento>();

            modelBuilder
                .HasDbFunction(typeof(Funcoes).GetMethod(nameof(Funcoes.DateDIFF)))
                .HasTranslation(p =>
                {
                    var args = p.ToList();
                    args[0] = new SqlFragmentExpression((string)((ConstantExpression)args.First()).Value);
                    return new SqlFunctionExpression(
                        "DATEDIFF",
                        typeof(int),
                        args);
                });
            
        }
    }

    class Evento
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public DateTime Data { get; set; }
    }

    class Funcoes
    {
        public static int DateDIFF(
            string datePart,
            DateTime start,
            DateTime end)
            => 0;
    }
}
