using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Api2
{
    public class DBDataContext : DbContext
    {
        //#region Field(s)

        //public DbSet<LendOrders> lendOrders;

        //public DbSet<Exchange> currencies;

        //#endregion

        #region public method

        public static bool Insert(object o)
        {
            try
            {
                using (var context = new DBDataContext())
                {
                    context.Add(o);
                    context.SaveChanges();
                }

                return true;
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return false;
        }

        public static bool Update<TKEY, TVALUE>(TKEY o, Expression<Func<TKEY, TVALUE>> expresion) where TKEY : class
        {
            try
            {
                using (var context = new DBDataContext())
                {
                    context.Set<TKEY>().Attach(o);
                    context.Entry(o).Property(expresion).IsModified = true;

                    context.SaveChanges();
                }

                return true;
            }
            catch (DbUpdateConcurrencyException ex)
            {
                var entries = ex.Entries;
                foreach (var entry in entries)
                {
                    // change state to remove it from context 
                    entry.State = EntityState.Detached;
                }
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return false;
        }

        public static TKEY Select<TKEY>(Expression<Func<TKEY, bool>> expresion) where TKEY : class
        {
            try
            {
                using (var context = new DBDataContext())
                {
                    return context.Set<TKEY>().FirstOrDefault(expresion);
                }
            }
            catch (Exception exc)
            {
                Logger.Instance.Save(exc);
            }

            return null;
        }

        #endregion

        #region Override method

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserInfo>().ToTable("userinfo");
            foreach (var i in DBManager.managers)
            {
                //modelBuilder.Entity(i.Value.ItemType()).ToTable(i.Key);
                
            }
            //modelBuilder.Entity("").ToTable();
            //modelBuilder.Entity<UserInfo>().ToTable("userinfo");
            //modelBuilder.Entity<PartnerInfo>().ToTable("partnerinfo");
            //modelBuilder.Entity<ContactInfo>().ToTable("contactinfo");
            //modelBuilder.Entity<PartnerFileInfo>().ToTable("partnerfileinfo");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string mySQLConnection = AppSettings.Instance.GetSection("ConnectionStrings").GetValue<string>("MySQLConnection");

            optionsBuilder.UseMySQL(mySQLConnection);
        }

        #endregion
    }
}
