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
using TiketsApp.ViewModels.Admin;

namespace TiketsApp.Views.AdminViews
{
    /// <summary>
    /// Логика взаимодействия для CategoriesPage.xaml
    /// </summary>
    public partial class CategoriesPage : Page
    {
        public CategoriesPage()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged ( object sender, RoutedPropertyChangedEventArgs<object> e )
        {
            if(this.DataContext is CategoriesVM vm && e.NewValue is Models.Category category)
            {
                vm.SelectedCategory = category;
            }
        }
    }
}
