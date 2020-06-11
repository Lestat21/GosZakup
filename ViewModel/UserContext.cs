using GosZakup.ParsClass;
using System.Data.Entity;

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
        public DbSet<Status> Statuses { get; set; }

    }
}
