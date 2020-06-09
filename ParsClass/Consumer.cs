using GosZakup.ParsClass;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GosZakup
{
    class Consumer
    {
        public int id { get; set; }
       
        public string unp { get; set; }
        public string name { get; set; }
        public string adress { get; set; }

       

        public ICollection<Purchase> Purchases { get; set; }
        public Consumer()
        {
            Purchases = new List<Purchase>();
        }

    }
}
