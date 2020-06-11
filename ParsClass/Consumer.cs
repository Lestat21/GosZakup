using GosZakup.ParsClass;
using System.Collections.Generic;

namespace GosZakup
{
    class Consumer
    {
        public int id { get; set; }

        public string unp { get; set; }
        public string name { get; set; }
        public string adress { get; set; }



        public virtual ICollection<Purchase> Purchases { get; set; }
        public Consumer()
        {
            Purchases = new List<Purchase>();
        }

    }
}
