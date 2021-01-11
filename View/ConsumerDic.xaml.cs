using GosZakup.ViewModel;
using System.Linq;
using System.Windows;

namespace GosZakup.View
{
    /// <summary>
    /// Логика взаимодействия для ConsumerDic.xaml
    /// </summary>
    public partial class ConsumerDic : Window
    {
        ViewConsumerDic viewConsumerDic;
        public ConsumerDic()
        {

            InitializeComponent();

        }

        public ConsumerDic(ViewConsumerDic viewConsumerDic) : this()
        {
            this.viewConsumerDic = viewConsumerDic;
            var result = from p in viewConsumerDic.consumerDics() select p;
            grid_ConsumerDic.ItemsSource = result.ToList();
            // TODO прикрутить общую виртуальную таблицу
        }
    }
}
