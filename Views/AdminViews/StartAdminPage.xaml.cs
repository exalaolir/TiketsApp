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
using System.Windows.Shapes;
using TiketsApp.Core.Servises;
using TiketsApp.ViewModels.Admin;

namespace TiketsApp.Views.AdminViews
{
    /// <summary>
    /// Логика взаимодействия для StartAdminPage.xaml
    /// </summary>
    public partial class StartAdminPage : Page
    {
        public StartAdminPage ()
        {
            InitializeComponent();
            var navigator = new Navigation(Subframe);
            this.Loaded += ( s, e ) =>
            {
                var vm = DataContext as AdminControlVM;
                vm!.Subnavigator = navigator;
                vm.NavigateToManagementCommand.Execute( null );
            };
        }
    }
}
