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
using TiketsApp.Core.Servises;
using TiketsApp.ViewModels.Admin;
using TiketsApp.ViewModels.Saller;

namespace TiketsApp.Views.SallerViews
{
    /// <summary>
    /// Логика взаимодействия для StartPage.xaml
    /// </summary>
    public partial class StartPage : Page
    {
        public StartPage ()
        {
            InitializeComponent();
            var navigator = new Navigation(Subframe);
            this.Loaded += ( s, e ) =>
            {
                var vm = DataContext as SallerControlVM;
                vm!.Subnavigator = navigator;
                vm.NavigateToEventsCommand.Execute(null);
            };
        }
    }
}
