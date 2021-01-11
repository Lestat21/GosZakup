using GosZakup.ViewModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GosZakup.View
{
    /// <summary>
    /// Логика взаимодействия для ViewCard.xaml
    /// </summary>
    public partial class Card : Window // вывод карточки закупки
    {
        string x;
        public Card()
        {
            InitializeComponent();
        }

        public Card(string x) : this()
        {
            MainViewTable MainViewTable = new MainViewTable();

            this.x = x;
            var result = from p in MainViewTable.MainVTable() // отбор по условию
                         where p.num_purhchase == x
                         select p;

            var t = result.ToList();

            GR_Card.DataContext = t[0];
        }

        private void BC_Print(object sender, RoutedEventArgs e)   // метод печати карточки
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(Print, TB_num.Text); // параметры - поле для печати и название файла
            }
        }

        private void BC_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
