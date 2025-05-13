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
using TiketsApp.Views.UserViews;

namespace TiketsApp.ViewModels.UsersVm
{
    internal class UserControlVm : ControlVM
    {
        private readonly Navigation _navigator;

        public ICommand NavigateToCatalogCommand { get; }

        public UserControlVm ( object? param, Navigation navigator )
        {
            _navigator = navigator;


            ExitCommand = new Command(() => _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null));

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

            NavigateToCatalogCommand = new Command(() => Subnavigator!.NavigateTo<Catalogxaml>(typeof(CatalogVM), param));
        }

        public override ICommand ExitCommand { get; }
        public override ICommand BackCommand { get; }
        public override ICommand FrontCommand { get; }
        public override ICommand RefreshCommand { get; }
    }
}
