using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GosZakup.ParsClass
{
    class Purchase // закупка
    {
        public int id { get; set; }
        public string num_purhchase { get; set; } // номер закупки
        public int id_type_of_purchase { get; set; } // тип закупки
        public string name_of_putchase { get; set; } // название закупки
        public string start_date { get; set; } // начало закупки
        public string end_date { get; set; } // дата завершения закупки
        public string cost { get; set; } // стоимость общая закупки
        public int id_company { get; set; } // id организации закупки - из таблицы consumers
        public string contact { get; set; } // контактные данные по закупке
        public string submission_of_docs { get; set; } // порядок подачи документов
        public string list_of_docs { get; set; } // перечень подаваемых документов
        public int id_docs_of_consumer { get; set; } // список ссылок на документы покупателя к закупке
        public string lonk_to_web { get; set; } //ссылка на карточку для обновления всего документа или некоторых полей

    }
}
