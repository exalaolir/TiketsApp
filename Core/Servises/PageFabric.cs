using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TiketsApp.ViewModels.Admin;
using TiketsApp.ViewModels.Base;
using TiketsApp.ViewModels.RegistrationVM;
using TiketsApp.ViewModels.Saller;
using TiketsApp.ViewModels.UsersVm;
using TiketsApp.Views;
using TiketsApp.Views.AdminViews;
using TiketsApp.Views.SallerViews;
using TiketsApp.Views.UserViews;

namespace TiketsApp.Core.Servises
{
    internal sealed class PageFabric
    {

        internal static Page CreatPage ( Type type ) => type switch 
        {
            _ when type == typeof( HomePage ) => new HomePage(),
            _ when type == typeof( LoginPage ) => new LoginPage(),
            _ when type == typeof(Views.RegistrationViews.UserPage) => new Views.RegistrationViews.UserPage(),
            _ when type == typeof(Views.RegistrationViews.SallerPage) => new Views.RegistrationViews.SallerPage(),
            _ when type == typeof(StartPage) => new StartPage(),
            _ when type == typeof(StartAdminPage) => new StartAdminPage(),
            _ when type == typeof(AllUsersPage) => new AllUsersPage(),
            _ when type == typeof(CategoriesPage) => new CategoriesPage(),
            _ when type == typeof(EventsPage) => new EventsPage(),
            _ when type == typeof(NewEventPage) => new NewEventPage(),
            _ when type == typeof(Catalogxaml) => new Catalogxaml(),
            _ when type == typeof(PriviewPage) => new PriviewPage(),
            _ when type == typeof(HabPage) => new HabPage(),
            _ => throw new NotSupportedException()
        };

        internal static ViewModel CreateViewModel ( Type type, object? param, Navigation navigator ) => type switch
        {
            _ when type == typeof(LoginVM) => new LoginVM(param, navigator),
            _ when type == typeof(UserVM) => new UserVM(param, navigator),
            _ when type == typeof(SallerVM) => new SallerVM(param, navigator),
            _ when type == typeof(UserControlVm) => new UserControlVm(param, navigator),
            _ when type == typeof(SallerControlVM) => new SallerControlVM(param, navigator),
            _ when type == typeof(AdminControlVM) => new AdminControlVM(param, navigator),
            _ when type == typeof(AllUsersManagmentViewModel) => new AllUsersManagmentViewModel(param, navigator),
            _ when type == typeof(CategoriesVM) => new CategoriesVM(param, navigator),
            _ when type == typeof(EventsVM) => new EventsVM(param, navigator),
            _ when type == typeof(NewEventVM) => new NewEventVM(param, navigator),
            _ when type == typeof(CatalogVM) => new CatalogVM(param, navigator),
            _ when type == typeof(PriviewVM) => new PriviewVM(param, navigator),
            _ when type == typeof(HabVM) => new HabVM(param, navigator),
            _ => throw new NotSupportedException()
        };
    }
}
