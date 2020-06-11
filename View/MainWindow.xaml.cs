using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GosZakup.View

{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        UserContext db;
        public MainWindow()
        {
            InitializeComponent();
            db = new UserContext();

            var unic = db.Consumers.FirstOrDefault(p => p.id == 1); // сохраняем только уникальные заказы, что бы исключить повторное внесение в базу  

            if (unic == null)
            {
                MessageBox.Show($"Ваша база данных пуста.\nДля начала работы с программой необходимо загрузить данные с сайта Госзакупок. Это можно сделать через меню: Парсинг.", "Внимание!.", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                db.Consumers.Load();// подключаем базу для вывода в DataGred
                MainTabl.ItemsSource = db.Consumers.Local.ToBindingList();

                var type = db.TypePurshases.Select(p => p.type_of_purshase);
                List<string> list_of_type = new List<string>(type);
                CB.ItemsSource = list_of_type;

                var type2 = db.Statuses.Select(p => p.status);
                List<string> list_of_stulis = new List<string>(type2);
                Status.ItemsSource = list_of_stulis;
            }

            this.Closing += MainWindow_Closing; // чистим память
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            db.Dispose();
        }

        private void Button_Click(object sender, RoutedEventArgs e) // выпоняем парсинк в отдельном потоке
        {
            Thread thread = new Thread(pars);
            thread.Start();
        }

        public void pars()
        {

            string firstPage = "http://goszakupki.by/tenders/posted?page=";
            string startPage = firstPage + "1";
            string page = GosZak.GetPage(startPage);// получаем
            int num_page = int.Parse(GosZak.Pars_Num_Page(page));  // общее количество страниц
            pars_page(firstPage, num_page); // метод парсинга страниц с адресами карточек
            
        }

        public void pars_page( string page, int num_of_page)
        {   
            int f = 0;
            for (int i = 1; i <= 100; i++) // i < = num_of_page
            {
                Thread.Sleep(TimeSpan.FromSeconds(5));

                string evrytPage = page + i; // страница для парсинга адресов карточек
                string code_page = GosZak.GetPage(evrytPage); // получаем исходный кон
                int mun = GosZak.ParsPage(code_page); // парсинг
                
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (ThreadStart)delegate ()
                {
                    f += mun;
                    pbStatus.Maximum = 100;
                    online_status2.Text = $"Обработано карточек - {f} ";
                    online_status1.Text = $"Обработано страниц {i - 1} из {num_of_page}";
                    pbStatus.Value = i;

                    db = new UserContext();
                    db.Consumers.Load();// обновляем базу для вывода в DataGred - но уберем когда сделаем вьюшку
                    MainTabl.ItemsSource = db.Consumers.Local.ToBindingList();

                    var type = db.TypePurshases.Select(p => p.type_of_purshase);
                    List<string> list_of_type = new List<string>(type);
                    CB.ItemsSource = list_of_type;

                    var type2 = db.Statuses.Select(p => p.status);
                    List<string> list_of_stulis = new List<string>(type2);
                    Status.ItemsSource = list_of_stulis;

                });
               
            }

            MessageBox.Show($"Парсинг завершен.", "Внимание.", MessageBoxButton.OK, MessageBoxImage.Information);
           
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Парсит целый день что бы потом вот просто так все удалить? Одумайтеь!", "Внимание.", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
