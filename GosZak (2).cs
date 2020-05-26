using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leaf.xNet;
using AngleSharp.Html.Parser;

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
            string main_Site = "http://goszakupki.by";

            HtmlParser Hpars = new HtmlParser();
            var doc = Hpars.ParseDocument(page); // получаем исходник страницы

            List<string> links = new List<string>();
            string full_addr = "";

            int num_tags = doc.QuerySelector("table.table.table-hover > tbody").ChildElementCount; // узнаем количество записей на странице

            for (int i = 1; i <= 1; i++)// поменять цифру на num_tags
            {

                string last_page = doc.QuerySelector($"#w0 > table > tbody > tr:nth-child({i}) > td:nth-child(2) > a").OuterHtml; // в первой скобке номер элемента на странице .. в цикле меняем именно его... вывод на странице 20 элементов.
                
                // вырезаем искомый текст ===========================
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

                // пишем данные в базу
                Consumer consumer = new Consumer(); // заполняем класс покупателя

                consumer.unp = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(3) > td").InnerHtml;
                consumer.name = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(1) > td").InnerHtml;
                consumer.adress = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(2) > td").InnerHtml;
                consumer.contact_person = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(4) > td").InnerHtml;
                consumer.contact_tel = blank.QuerySelector("body > div.wrap > div > div:nth-child(4) > table > tbody > tr:nth-child(4) > td").InnerHtml;

                // заполняем класс закупки (добавить вид процедуры закупки и название процедуры закупки)

                // заполняем класс лот

            }

            return links;
        }

        // public static 
    }
}
