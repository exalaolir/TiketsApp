using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TiketsApp.Core.Servises;
using TiketsApp.ViewModels.RegistrationVM;
using TiketsApp.Views;

namespace TiketsApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow ()
        {
            InitializeComponent();
#pragma warning disable WPF0001
            Application.Current.ThemeMode = ThemeMode.Light;
            this.Loaded += ( s, e ) =>
            {
                Navigation navigator = new(MainFrame);
                navigator.NavigateTo<LoginPage>(typeof(LoginVM), null);
            };
        }
    }
}