using System.Collections.Generic;

namespace GosZakup.ParsClass
{
    class Purchase // закупка
    {
        public int id { get; set; }
        public string num_purhchase { get; set; } // номер закупки
        public string name_of_purchase { get; set; } // название закупки
        public string start_date { get; set; } // начало закупки
        public string end_date { get; set; } // дата завершения закупки
        public string cost { get; set; } // стоимость общая закупки
        public string contact { get; set; } // контактные данные по закупке
        public string link { get; set; } // ссылка на страницу
        //public string submission_of_docs { get; set; } // порядок подачи документов
        //public string list_of_docs { get; set; } // перечень подаваемых документов
        //public int id_docs_of_consumer { get; set; } // список ссылок на документы покупателя к закупке
        //public string lonk_to_web { get; set; } //ссылка на карточку для обновления всего документа или некоторых полей


        public int ConsumerID { get; set; }
        public virtual Consumer Consumer { get; set; }


        public int TypeOfPurshaseID { get; set; }
        public TypePurshase TypePurshase { get; set; }


        public int StatusID { get; set; }
        public Status Status { get; set; }


        public ICollection<Lot> Lots { get; set; }
        public Purchase()
        {
            Lots = new List<Lot>();
        }










    }
}
