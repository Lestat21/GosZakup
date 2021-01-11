using System.Collections.Generic;

namespace GosZakup.ParsClass
{
    class Status
    {

        public int id { get; set; }
        public string status { get; set; } // тип закупок

        public ICollection<Purchase> Purchases { get; set; }
        public Status()
        {
            Purchases = new List<Purchase>();
        }
    }
}
