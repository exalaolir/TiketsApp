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

namespace TiketsApp.Views.SallerViews
{
    /// <summary>
    /// Логика взаимодействия для NewEventPage.xaml
    /// </summary>
    public partial class NewEventPage : Page
    {
        public NewEventPage()
        {
            InitializeComponent();
        }

        private void DatePicker_PreviewTextInput ( object sender, TextCompositionEventArgs e )
        {
            e.Handled = true;
        }

        private void DatePicker_PreviewKeyDown ( object sender, KeyEventArgs e )
        {
            if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Escape)
            {
                e.Handled = true;
            }
        }
    }
}
