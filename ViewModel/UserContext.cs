using GosZakup.ParsClass;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GosZakup
{
    class UserContext : DbContext
    {
        public UserContext()
            : base("DbConnection")
        { }

        public DbSet<Consumer> Consumers { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<TypePurshase> TypePurshases { get; set; }
        public DbSet<Lot> Lots { get; set; }


    }
}
