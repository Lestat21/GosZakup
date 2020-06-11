namespace GosZakup.ParsClass
{
    class Lot //Лоты
    {

        public int lotid { get; set; } // ID лота
        public int numlot { get; set; } // номер лота пп
        public string delivery_date { get; set; } // дата поставки
        public string place_of_delivery { get; set; } //место поставки
        public string source_of_financ { get; set; } // источник финансирования
        public string payment_method { get; set; } // метод оплаты
        public string description { get; set; } // описание лота
        public string kodOKRB { get; set; } // код ОКРБ
        public string price_quantity { get; set; } //цена-количество в одном
        public string lot_status { get; set; } // статус лота - можно отдельной таблицей


        public int pursaseID { get; set; }
        public Purchase Purchase { get; set; }

    }
}
