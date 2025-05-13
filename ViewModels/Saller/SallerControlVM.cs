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
using TiketsApp.Views.SallerViews;

namespace TiketsApp.ViewModels.Saller
{
    internal class SallerControlVM : ControlVM
    {
        private readonly Navigation _navigator;

        public SallerControlVM ( object? param, Navigation navigator ) : base()
        {
            _navigator = navigator;

            _canGoBack = Subnavigator?.CanGoBack ?? false;
            _canGoFront = Subnavigator?.CanGoFront ?? false;

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

            NavigateToEventsCommand = new Command(() => Subnavigator!.NavigateTo<EventsPage>(typeof(EventsVM), param));
        }

        public override ICommand ExitCommand { get; }
        public override ICommand BackCommand { get; }
        public override ICommand FrontCommand { get; }
        public override ICommand RefreshCommand { get; }

        public ICommand NavigateToEventsCommand { get; }
    }
}
