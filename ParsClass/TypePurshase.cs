using System.Collections.Generic;

namespace GosZakup.ParsClass
{
    class TypePurshase
    {
        public int id { get; set; }
        public string type_of_purshase { get; set; } // тип закупок

        public virtual ICollection<Purchase> Purchases { get; set; }

        public TypePurshase()
        {
            Purchases = new List<Purchase>();
        }


    }
}
