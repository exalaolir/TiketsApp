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

namespace TiketsApp.Views.UserViews
{
    /// <summary>
    /// Логика взаимодействия для HabPage.xaml
    /// </summary>
    public partial class HabPage : Page
    {
        public HabPage ()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
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

        private void DatePicker_PreviewTextInput ( object sender, TextCompositionEventArgs e )
        {
            e.Handled = true;
        }

        private void OnLoaded ( object sender, RoutedEventArgs e )
        {
            // Подписка на события
            passwordBox.PasswordChanged += SetPassword;
            repeatPasswordBox.PasswordChanged += SetRepeatPassword;
            datePicker.PreviewTextInput += DatePicker_PreviewTextInput;
            datePicker.PreviewKeyDown += DatePicker_PreviewKeyDown;
        }

        private void OnUnloaded ( object sender, RoutedEventArgs e )
        {
            // Отписка от событий
            passwordBox.PasswordChanged -= SetPassword;
            repeatPasswordBox.PasswordChanged -= SetRepeatPassword;
            datePicker.PreviewTextInput -= DatePicker_PreviewTextInput;
            datePicker.PreviewKeyDown -= DatePicker_PreviewKeyDown;

            // Отписка от Loaded/Unloaded
            Loaded -= OnLoaded;
            Unloaded -= OnUnloaded;

            if (DataContext is IDisposable disposable)
            {
                disposable.Dispose();
                //DataContext = null;
            }
        }


        private void DatePicker_PreviewKeyDown ( object sender, KeyEventArgs e )
        {
            if (e.Key != Key.Tab && e.Key != Key.Enter && e.Key != Key.Escape)
            {
                e.Handled = true;
            }
        }

        ~HabPage ()
        {
            Debug.WriteLine($"Уничтожена страница: {GetType().Name}");
        }
    }
}
