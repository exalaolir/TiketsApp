using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace TiketsApp.Views
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage ()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        private void OnLoaded ( object sender, RoutedEventArgs e )
        {
            // Подписываемся на события
            passwordBox.PasswordChanged += SetPassword;
        }

        private void OnUnloaded ( object sender, RoutedEventArgs e )
        {
            passwordBox.PasswordChanged -= SetPassword;

            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;

            DataContext = null;
        }

        private void SetPassword ( object sender, RoutedEventArgs e )
        {
            if (this.DataContext is IPassword passProperty && sender is PasswordBox box)
            {
                passProperty.Password = box.Password;
            }
        }

        ~LoginPage ()
        {
            Debug.WriteLine($"Уничтожена страница: {GetType().Name}");
        }
    }
}
