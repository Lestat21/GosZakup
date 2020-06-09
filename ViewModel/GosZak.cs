using System.Collections.Generic;
using System.Linq;
using Leaf.xNet;
using AngleSharp.Html.Parser;
using GosZakup.ParsClass;
using System;
using AngleSharp.Dom;

namespace GosZakup
{
    class GosZak
    {

        public static string GetPage(string link)  // метод получения кода страницы 
        {
            HttpRequest request = new HttpRequest();
            request.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            request.AddHeader("Accept-Encoding", "gzip, deflate");
            request.AddHeader("Accept-Language", "ru,en;q=0.9,gl;q=0.8");
            request.AddHeader("Host", "goszakupki.by");
            request.AddHeader("Referer", link);
            request.KeepAlive = true;
            request.UserAgent = Http.IEUserAgent();
            string resp = request.Get(link).ToString();
            return resp;
        }

        public static string Pars_Num_Page(string page)  // подсчет количества страниц для парсинга (количество обходов для цикла парсинга карточек закупок)
        {
            HtmlParser Hp = new HtmlParser();
            var doc = Hp.ParseDocument(page);
            string last_page = doc.QuerySelector("li.last").TextContent;
            return last_page;
        }

        public static List<string> ParsPage(string page) // парсинг номера закупки
        {

            using (UserContext db = new UserContext())
            {

                string main_Site = "http://goszakupki.by";

                HtmlParser Hpars = new HtmlParser();
                var doc = Hpars.ParseDocument(page); // получаем исходник страницы

                List<string> links = new List<string>();
                string full_addr = "";

                int num_tags = doc.QuerySelector("table.table.table-hover > tbody").ChildElementCount; // узнаем количество записей на странице 

                for (int i = 1; i <= num_tags; i++)// поменять цифру на num_tags
                {

                    string last_page = doc.QuerySelector($"#w0 > table > tbody > tr:nth-child({i}) > td:nth-child(2) > a").OuterHtml; // в первой скобке номер элемента на странице .. в цикле меняем именно его... вывод на странице 20 элементов.
                    string numb_of_pursase = doc.QuerySelector($"#w0 > table > tbody > tr:nth-child({i}) > td:nth-child(1)").InnerHtml; // вырезаем номер закупки
                    string status = doc.QuerySelector("#w0 > table > tbody > tr:nth-child(1) > td:nth-child(4) > span").InnerHtml;// вырезаем статус

                    // вырезаем искомый текст =========================== - создание ссылки на карточку
                    int indexOfChar = last_page.IndexOf("\"") + 1;//номер первого вхождения двойных ковычек
                    int lastindexOfChar = last_page.LastIndexOf("\""); //номер последнего вхождения двойных ковычек
                    last_page = last_page.Substring(indexOfChar);
                    int NextindexOfChar = last_page.IndexOf("\"");
                    string temp = last_page.Substring(0, NextindexOfChar);
                    // ==================================================

                    full_addr = main_Site + temp;// конкатенация и вывод полного адреса страницы-карточки закупки.

                    links.Add(full_addr); // добавляем адрес в список (потом не понадобится, когда будет написан парсинг карточки)

                    // обработка карточки

                    string blank_kod = GetPage(full_addr);
                    HtmlParser blankpars = new HtmlParser();
                    var blank = blankpars.ParseDocument(blank_kod);

                    var a = blank.QuerySelector("body > div.wrap > div").InnerHtml;

                    // парсим и пишем данные в базу

                    //=====================================================================================================================================================

                    // таблица покупателей - готово
                    {
                        Consumer consumer = new Consumer(); // заполняем класс покупателя
                                                
                        var flag = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > div > b").InnerHtml;       //если без организатора то этот код

                        if (flag == "Сведения о заказчике")
                        {
                            consumer.unp = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            consumer.name = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                            consumer.adress = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                        }
                        else
                        {
                            consumer.unp = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            consumer.name = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                            consumer.adress = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                        }
                        //если с организатором то этот код

                        var new2 = db.Consumers.FirstOrDefault(p => p.unp == consumer.unp); // если организации не существует то добавляем иначе пропускаем

                        if (new2 == null)
                        {
                            db.Consumers.Add(consumer);
                            db.SaveChanges();
                        }
                    }


                    // таблица типов закупок - готово
                    {
                        TypePurshase typePurshase = new TypePurshase();
                        var flag = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > div > b").InnerHtml;       //если без организатора то этот код


                        if (flag == "Сведения о заказчике")
                        {
                            typePurshase.type_of_purshase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                        }
                        else
                        {
                            typePurshase.type_of_purshase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                        }

                        var new3 = db.TypePurshases.FirstOrDefault(p => p.type_of_purshase == typePurshase.type_of_purshase);
                        if (new3 == null)
                        {
                            db.TypePurshases.Add(typePurshase);
                            db.SaveChanges();
                        }

                    }

                    // таблица закупок - готово 
                    {
                        Purchase purchase = new Purchase(); //заполняем класс закупки
                        var flag = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > div > b").InnerHtml;       //если без организатора то этот код

                      
                        if (flag == "Сведения о заказчике")
                        {
                            purchase.num_purhchase = numb_of_pursase;
                            purchase.unp = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            purchase.name_of_purchase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                            purchase.start_date = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                            purchase.end_date = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                            purchase.cost = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            purchase.contact = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(4) > td").InnerHtml;
                            purchase.type_of_purshase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(1) > td").InnerHtml;

                            var new2 = db.Consumers.FirstOrDefault(p => p.unp == purchase.unp);
                            purchase.ConsumerID = new2.id;
                            var new3 = db.TypePurshases.FirstOrDefault(p => p.type_of_purshase == purchase.type_of_purshase);
                            purchase.TypeOfPurshaseID = new3.id;

                        }
                        else
                        {
                            purchase.num_purhchase = numb_of_pursase;
                            purchase.unp = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            purchase.name_of_purchase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                            purchase.start_date = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                            purchase.end_date = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                            purchase.cost = blank.QuerySelector("body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                            purchase.contact = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(3) > td").InnerHtml;//body > div.wrap > div > div:nth-child(5) > table > tbody > tr:nth-child(4) > td
                            purchase.type_of_purshase = blank.QuerySelector("body > div.wrap > div > div:nth-child(3) > table > tbody > tr:nth-child(1) > td").InnerHtml;

                            var new2 = db.Consumers.FirstOrDefault(p => p.unp == purchase.unp);
                            purchase.ConsumerID = new2.id;
                            var new3 = db.TypePurshases.FirstOrDefault(p => p.type_of_purshase == purchase.type_of_purshase);
                            purchase.TypeOfPurshaseID = new3.id;
                        }

                        db.Purchases.Add(purchase);
                        db.SaveChanges();

                    }

                    // таблица лотов
                    {
                        Lot lot = new Lot();



                        // Счетчик лотов в карточке - 

                        int num_lot = blank.QuerySelector("#lotsList > tbody").ChildElementCount;
                        //num_lot = num_lot / 2;

                        for (int j = 1; j <= num_lot; j=j+2)
                        {
                            
                            lot.num_purhchase = numb_of_pursase;
                            lot.NumLot = Convert.ToInt32(blank.QuerySelector($"#lotsList > tbody > tr:nth-child({j}) > th").InnerHtml); //#lotsList > tbody > tr:nth-child(1) > th   #lotsList > tbody > tr:nth-child(3) > th
                            lot.description = blank.QuerySelector($"#lotsList > tbody > tr:nth-child({j}) > td.lot-description").InnerHtml;
                            lot.delivery_date = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(1) > span").InnerHtml;//#lot-inf-4 > td:nth-child(3) > ul:nth-child(1) > li:nth-child(1) > span
                            lot.place_of_delivery = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(2) > span").InnerHtml;
                            lot.source_of_financ = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(3) > span").InnerHtml;
                            lot.payment_method = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(4) > span").InnerHtml;
                            lot.price_quantity = blank.QuerySelector($"#lotsList > tbody > tr:nth-child({j}) > td.lot-count-price").InnerHtml;
                            lot.status = blank.QuerySelector($"#lotsList > tbody > tr:nth-child({j}) > td.lot-status > span").InnerHtml;


                            var tempOKRB = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(5) > b").InnerHtml;
                            if (tempOKRB == "Код предмета закупки по ОКРБ:")
                            {
                                lot.kodOKRB = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(5) > span").InnerHtml;
                            }
                            else
                            {
                                lot.kodOKRB = blank.QuerySelector($"#lot-inf-{lot.NumLot} > td:nth-child(3) > ul:nth-child(1) > li:nth-child(6) > span").InnerHtml;
                            }
                            
                            
                            
                            var new2 = db.Purchases.FirstOrDefault(p => p.num_purhchase == lot.num_purhchase);

                            lot.pursaseID = new2.id;

                            db.Lots.Add(lot);
                            db.SaveChanges();

                        }


                        

                        
                    }





                }

                return links;

            }

            
        }

        // public static 
    }
}
