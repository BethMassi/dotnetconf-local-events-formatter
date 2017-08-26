using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLib
{
   public class Location
    {
        public int ID { get; set; }
        public double Long { get; set; }
        public double Lat { get; set; }
        public string City { get; set; }

    }

    public class LocationsContext : DbContext
    {
        public DbSet<Location> Locations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=NETConfLocalEvents;Trusted_Connection=True;");
            
        }
    }
    public static class LocationsService
    {

        //private LocationsContext dbCtx;
        //public LocationsContext DbCtx
        //{
        //    get
        //    {
        //        if (dbCtx == null)
        //        {
        //            dbCtx = new LocationsContext();
        //        }
        //        return dbCtx;
        //    }
        //}

        public static void WriteLocation(Location loc)
        {
            using (var db = new LocationsContext())
            {
                db.Locations.Add(loc);
                db.SaveChanges();               
            }
        }

        public static Location ReadLocation(string city)
        {
            using (var db = new LocationsContext())
            {
                db.Database.EnsureCreated();

                var query = from l in db.Locations
                            where l.City.ToLower() == city.ToLower()
                            select l;

                if (query?.ToList().Count != 0)
                {
                    return query.ToList()[0];
                }
                else
                {
                    return null;
                }
                
            }

        }

    }
}


