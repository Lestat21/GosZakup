using System.Collections.Generic;
using System.Linq;

namespace GosZakup.ViewModel
{
    public class ViewConsumerDic
    {
        public string unp { get; set; }
        public string name { get; set; }
        public string contact { get; set; }
        public string adress { get; set; }


        public List<ViewConsumerDic> consumerDics() // создаем примежуточную таблицу для работы с вьюшкой программмы
        {
            UserContext db = new UserContext();

            var result = from C in db.Consumers
                         join P in db.Purchases on C.id equals P.ConsumerID

                         select new ViewConsumerDic
                         {
                             contact = P.contact,
                             unp = C.unp,
                             name = C.name,
                             adress = C.adress
                         };

            return result.ToList();
            // TODO Убрать класс.. вообще. сделать по аналогии с карточкой
        }
    }
}
