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
using TiketsApp.Core.Iterfases;

namespace TiketsApp.Views.RegistrationViews
{
    /// <summary>
    /// Логика взаимодействия для SallerPage.xaml
    /// </summary>
    public partial class SallerPage : Page
    {
        public SallerPage ()
        {
            InitializeComponent();
        }

        private void SetPassword ( object sender, RoutedEventArgs e )
        {
            if (this.DataContext is IPassword passProperty && sender is PasswordBox box)
            {
                passProperty.Password = box.Password;
            }
        }

        private void SetRepeatPassword ( object sender, RoutedEventArgs e )
        {
            if (this.DataContext is IRepeatPassword passProperty && sender is PasswordBox box)
            {
                passProperty.RepeatPassword = box.Password;
            }
        }
    }
}
