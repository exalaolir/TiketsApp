using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TiketsApp.Core.Servises;
using TiketsApp.ViewModels.Base;
using TiketsApp.ViewModels.RegistrationVM;
using TiketsApp.Views;
using TiketsApp.Views.AdminViews;

namespace TiketsApp.ViewModels.Admin
{
    internal class AdminControlVM : ControlVM
    {
        private readonly Navigation _navigator;
        private readonly Models.Admin? _admin;

        public ICommand NavigateToManagementCommand { get; }

        public override ICommand ExitCommand { get; }

        public override ICommand BackCommand { get; }

        public override ICommand FrontCommand { get; }

        public override ICommand RefreshCommand { get; }

        public ICommand NavigateToCategoriesCommand { get; }

        public AdminControlVM ( object? param, Navigation navigator ) : base()
        {
            _navigator = navigator;
            _admin = param as Models.Admin;
            _param = param;

            NavigateToManagementCommand = new Command(() =>
            {
                Subnavigator!.NavigateTo<AllUsersPage>(typeof(AllUsersManagmentViewModel), _admin);
                NavBtnsRefresH();
            });

            ExitCommand = new Command(() => _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null));

            NavigateToCategoriesCommand = new Command(() =>
            {
                Subnavigator!.NavigateTo<CategoriesPage>(typeof(CategoriesVM), _admin);
                NavBtnsRefresH();
            });

            BackCommand = new Command(() =>
            {
                Subnavigator!.GoBack();
                NavBtnsRefresH();
            });

            FrontCommand = new Command(() =>
            {
                Subnavigator!.GoFront();
                NavBtnsRefresH();
            });

            RefreshCommand = new Command(() => Subnavigator!.Reload(param));

            _canGoBack = Subnavigator?.CanGoBack ?? false;
            _canGoFront = Subnavigator?.CanGoFront ?? false;
        }


    }
}
