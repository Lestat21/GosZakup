using GosZakup.ParsClass;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GosZakup.ViewModel
{
    public class MainViewTable
    {

        // все поля большой таблицы
        public int Viewid { get; set; }
        public string num_purhchase { get; set; } // номер закупки
        public string name_of_purchase { get; set; } // название закупки
        public DateTime start_date { get; set; } // начало закупки
        public DateTime end_date { get; set; } // дата завершения закупки
        public double cost { get; set; } // стоимость общая закупки
        public string contact { get; set; } // контактные данные по закупке
        public string unp { get; set; } // унп организации
        public string name { get; set; } // название организации
        public string adress { get; set; } // адрес организации
        public string status { get; set; } // тип закупок
        public string type_of_purshase { get; set; } // тип закупок

        public List<MainViewTable> MainVTable() // создаем примежуточную таблицу для работы с вьюшкой программмы
        {
            UserContext db = new UserContext();

            var result = from P in db.Purchases
                         join Consumer in db.Consumers on P.ConsumerID equals Consumer.id
                         join Status in db.Statuses on P.StatusID equals Status.id
                         join Type_of_purshase in db.TypePurshases on P.TypeOfPurshaseID equals Type_of_purshase.id

                         select new MainViewTable
                         {
                             num_purhchase = P.num_purhchase,
                             name_of_purchase = P.name_of_purchase,
                             start_date = P.start_date,
                             end_date = P.end_date,
                             cost = P.cost,
                             contact = P.contact,
                             unp = Consumer.unp,
                             name = Consumer.name,
                             adress = Consumer.adress,
                             status = Status.status,
                             type_of_purshase = Type_of_purshase.type_of_purshase
                         };

            return result.ToList();
            // TODO добавить в карточку лоты в соотвествиии с номером закупки
        }
    }
}
