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
using TiketsApp.ViewModels.Saller;
using TiketsApp.ViewModels.UsersVm;

namespace TiketsApp.Views.UserViews
{
    /// <summary>
    /// Логика взаимодействия для HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        public HomePage ()
        {
            InitializeComponent();
            var navigator = new Navigation(Subframe);

            this.Loaded += ( s, e ) =>
            {
                var vm = DataContext as UserControlVm;
                vm!.Subnavigator = navigator;
                vm.NavigateTuHabCommand.Execute(null);
            };
        }
    }
}
