using GosZakup.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GosZakup.View

{
    
    public partial class MainWindow : Window
    {
        UserContext db;

        public MainWindow()
        {
            InitializeComponent();
            db = new UserContext();

            var unic = db.Consumers.FirstOrDefault(p => p.id == 1); // проверяем бд на наличие данных  

            if (unic == null)
            {
                MessageBox.Show($"Ваша база данных пуста.\nДля начала работы с программой необходимо загрузить данные с сайта Госзакупок. Это можно сделать через меню: Парсинг.", "Внимание!.", MessageBoxButton.OK, MessageBoxImage.Information);
                DataLoad();
            }
            
            DataLoad();
            
            this.Closing += MainWindow_Closing; // чистим память
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void dGrid_LoadingRow(object sender, DataGridRowEventArgs e) // нумерация строк
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Button_Click(object sender, RoutedEventArgs e) // выпоняем парсинг в отдельном потоке
        {
            Thread thread = new Thread(pars);
            thread.Start();
        }

        public void pars()
        {
            string firstPage = "https://goszakupki.by/tenders/posted?page="; // входная точка для парсинга
            string startPage = firstPage + "1"; // стартовая страница для парсинга
            string page = GosZak.GetPage(startPage);// получаем код страницы
            int num_page = int.Parse(GosZak.Pars_Num_Page(page));  // общее количество страниц
            pars_page(firstPage, num_page); // метод парсинга страниц с адресами карточек
        }

        public void pars_page(string page, int num_of_page)    // в цикле парсим всю базу
        {
            int f = 0;
            int tmp = 100; //временная переменная - указываю сколько парсить страниц(*20 = карточек) потом передам переменную num_page
            for (int i = 1; i <= tmp; i++) // i < = num_of_page
            {
                Thread.Sleep(TimeSpan.FromSeconds(5)); // устанавливаем таймаут между парсингом блока страниц иначе может сойти за DoS атаку

                string evrytPage = page + i; // страница для парсинга адресов карточек
                string code_page = GosZak.GetPage(evrytPage); // получаем исходный кон
                int mun = GosZak.ParsPage(code_page); // парсинг

                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    f += mun;
                    pbStatus.Maximum = tmp;
                    pbStatus.Value = i;
                    online_status2.Text = $"Обработано карточек - {f} ";
                    online_status1.Text = $"Обработано страниц {i - 1} из {num_of_page}";

                    DataLoad();

                });

            }

            MessageBox.Show($"Парсинг завершен.", "Внимание.", MessageBoxButton.OK, MessageBoxImage.Information);

        }

        private void MenuItem_Click(object sender, RoutedEventArgs e) //Открываем окошко о программе
        {
            About about = new About();
            about.ShowDialog();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)  // Заркыть программу
        {
            Close();
        }

        private void Del_Data(object sender, RoutedEventArgs e) // метод очистки базы данных + сбрасываем все индексы в нуль
        {
            MessageBoxResult result = MessageBox.Show($"Парсит целый день что бы потом вот просто так все удалить? Одумайтеь!", "Внимание.", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                db.Database.ExecuteSqlCommand(
                    "DELETE FROM Consumers " +
                    "DELETE FROM Purchases " +
                    "DELETE FROM Lots  " +
                    "DELETE FROM Status  " +
                    "DELETE FROM TypePurshases " +
                    "DBCC CHECKIDENT ('consumers', RESEED, 0)" +
                    "DBCC CHECKIDENT ('Purchases', RESEED, 0)" +
                    "DBCC CHECKIDENT ('Lots', RESEED, 0)" +
                    "DBCC CHECKIDENT ('Status', RESEED, 0)" +
                    "DBCC CHECKIDENT ('TypePurshases', RESEED, 0)"
                    );
                DataLoad();
                MessageBox.Show($"База данных пуста!", "Внимание.", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }

        }

        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)  // просмотр карточки закупки и печать
        {
            DataGridRow row = sender as DataGridRow;

            if (row != null)
            {
                TextBlock tbl = MainTabl.Columns[0].GetCellContent(row) as TextBlock;
                Card card = new Card(tbl.Text);
                card.ShowDialog();
            }
        }

        private void DataLoad()  // загрузка из БД данных согласно полей таблицы в основную форму из виртуальной таблицы
        {
            MainViewTable MainViewTable = new MainViewTable();
            var res1 = from test in MainViewTable.MainVTable() select test;

            MainTabl.ItemsSource = res1.ToList();

            var type = db.TypePurshases.Select(p => p.type_of_purshase);  // добавляем в комбобок список типов закупок
            List<string> list_of_type = new List<string>(type);
            CB.ItemsSource = list_of_type;

            var type2 = db.Statuses.Select(p => p.status); // добавляем в комбобокс статусы закупок
            List<string> list_of_stulis = new List<string>(type2);
            Status.ItemsSource = list_of_stulis;

        }

        private void BC_Serch(object sender, RoutedEventArgs e) // реализация поиска по БД  
        {
            MainViewTable MainViewTable = new MainViewTable();
            var result = from test in MainViewTable.MainVTable() select test;

            // приведение цены в формат для организации поиска 
            double start_price, end_price;
            double.TryParse(TB_PriceStart.Text, out start_price);
            double.TryParse(TB_PriceEnd.Text, out end_price);

            if (end_price == 0) // устанолвение макисмального значения конечной цены если она не указана в параметрах поиска
            {
                end_price = Int32.MaxValue;
            }

            // работа с датой.. реализовано автозаполнение минимальным и максимальным значением если не выбраны
            if (DP_StartDate.SelectedDate != null && DP_EndDate.SelectedDate == null)
            {
                DP_EndDate.SelectedDate = DateTime.MaxValue;
            }
            else if (DP_StartDate.SelectedDate == null && DP_EndDate.SelectedDate != null)
            {
                DP_StartDate.SelectedDate = DateTime.MinValue;
            }
            else if (DP_StartDate.SelectedDate == null && DP_EndDate.SelectedDate == null)
            {
                DP_EndDate.SelectedDate = DateTime.MaxValue;
                DP_StartDate.SelectedDate = DateTime.MinValue;
            }

            // реализация поиска в виде запроса к промежуточной таблице с выбранными параметрами
            var serch = result.Where(p => p.unp.Contains(TB_UNP.Text))    //потом сделать поля из формы через проверку 
                                .Where(p => p.name.ToLower().Contains(TB_Name.Text))
                                .Where(p => p.name_of_purchase.ToLower().Contains(TB_NameOfPurshase.Text))
                                .Where(p => p.type_of_purshase.Contains(CB.Text))
                                .Where(p => p.status.Contains(Status.Text))
                                .Where(p => p.cost >= start_price && p.cost <= end_price)
                                .Where(p => p.start_date >= DP_StartDate.SelectedDate && p.end_date <= DP_EndDate.SelectedDate)
                                .Select(c => c);

            MainTabl.ItemsSource = serch.ToList();

            // обнуление параметров даты
            DP_EndDate.SelectedDate = DateTime.Now;
            DP_EndDate.SelectedDate = null;
            DP_StartDate.SelectedDate = DateTime.Now;
            DP_StartDate.SelectedDate = null;

        }

        private void Bc_Clear(object sender, RoutedEventArgs e)   // очистка формы
        {
            TB_Name.Text = "";
            TB_NameOfPurshase.Text = "";
            TB_Num.Text = "";
            TB_UNP.Text = "";
            TB_PriceStart.Text = "";
            TB_PriceEnd.Text = "";
            CB.SelectedItem = null;
            Status.SelectedItem = null;
            DP_EndDate.SelectedDate = null;
            DP_StartDate.SelectedDate = null;
            DataLoad();

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            ViewConsumerDic viewConsumerDic = new ViewConsumerDic();
            ConsumerDic consumerDic = new ConsumerDic(viewConsumerDic);
            consumerDic.ShowDialog();
        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e) // удаление завершенных/отмененных закупок
        {
            MessageBoxResult result = MessageBox.Show($"Парсит целый день что бы потом вот просто так все удалить? Одумайтеь!", "Внимание.", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                db.Database.ExecuteSqlCommand(
                    "DELETE FROM Lots FROM Lots join Purchases on lots.purshaseID = Purchases.id " +
                    "where Purchases.num_purhchase in (select Purchases.num_purhchase " +
                    "FROM Purchases join Status on Purchases.StatusID = Status.id " +
                    "where status.status = 'Завершен' OR status.status = 'Отменен') " +
                    "DELETE FROM Purchases FROM Purchases join Status on Purchases.StatusID = Status.id " +
                    "where status.status = 'Завершен' OR status.status = 'Отменен'" +
                    "DBCC CHECKIDENT ('purchases', RESEED, 0)" +
                    "DBCC CHECKIDENT ('status', RESEED, 0)"
                    );
                DataLoad();
                MessageBox.Show($"База данных пуста!", "Внимание.", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            }
        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e) // парсинг новых закупок - дополнение базы данных
        {
            //int last; // номер последней закупки

            MessageBox.Show("Тут будет вновь созданных закупок");

            //while (true)
            //{

            //}


        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Тут обновим статусы");
        }

        //TODO переделать меню под WPF Command
    }
}
