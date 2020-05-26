using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GosZakup
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
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string startPage = "http://goszakupki.by/tenders/posted?page=" + "1";
            string page = GosZak.GetPage(startPage);
            int num_page = int.Parse(GosZak.Pars_Num_Page(page));  // общее количество страниц
            
            List<string> in_Page = new List<string>();// удалить после отработки всего модуля
            
            for (int i = 1; i <= 2; i++)
            {
                string evrytPage = "http://goszakupki.by/tenders/posted?page=" + i;
                string code_page = GosZak.GetPage(evrytPage);
                in_Page.AddRange(GosZak.ParsPage(code_page));// убрать за ненадобностью потом - это сбор всех ссылок на карточки в одном листе
                online_status2.Text = $"Обработано карточек {in_Page.Count.ToString()} ";
                online_status1.Text = $"Обработано страниц {i} из {num_page}";
            }

            links.DataContext = in_Page; // просто вывод для того что бы знать какие страницы в работе

        }
    }
}
