using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.ViewModels.Base;
using TiketsApp.ViewModels.RegistrationVM;

namespace TiketsApp.ViewModels.Admin
{
    internal sealed class ShortUserCard ( Role role, string descriminator ) : ViewModel
    {
        private readonly Dictionary<string, string> _translator = new()
        {
            {"User", "Пользователь" },
            {"Saller", "Организатор" },
            {"Admin", "Администратор" },
        };
        public Role Role => role;

        public string Fio => $"{role.Surname} {role.Name}";

        public string Type => _translator[descriminator];

        public string VishedText => role switch
        {
            Models.Saller saller => (bool)saller.IsNowRegister! || (bool)saller.BannedByAdmin! ? "" : "Заявка",
            _ => ""
        };

    }


    internal sealed class SallerCard ( Models.Saller saller, Navigation navigator, AllUsersManagmentViewModel mainVM, object navParam ) : ViewModel, IRole
    {
        private const string Descriminator = "Saller";
        private readonly Dictionary<string, string> _translator = new()
    {
        {Descriminator, "Организатор"}
    };

        public Role Role => saller;
        public string Fio => $"ФИ: {saller.Surname} {saller.Name}";
        public string Type => $"Тип: {_translator[Descriminator]}";

        public string Email => $"Email: {saller.Email}";
        public string RegistrationDate => $"Дата регистрации {saller.DateOfRegistration.ToShortDateString()}";

        public bool Banned => (bool)saller.BannedByAdmin!;

        public bool BannedAndRegister => (bool)saller.BannedByAdmin! && (bool)saller.IsNowRegister!;

        public bool IsNowRegister => !(bool)saller.IsNowRegister! && !(bool)saller.BannedByAdmin!;

        public string StatusText => saller switch
        {
            { BannedByAdmin: true } => "Заблокирован",
            { IsNowRegister: false } => "Незарегистрирован",
            _ => "Активный организатор"
        };

        public string? Num => $"Номер в реестре: {saller.Number}";

        public ICommand ConfirmCommand { get; } = new Command(async () =>
        {
            mainVM.DataLoaded = false;
            await Task.Run(() =>
            {
                using AppContext appContext = new();
                appContext.Sallers.Find(saller.Id)!.IsNowRegister = true;

                appContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() => navigator.Reload(navParam));
            });
        });

        public ICommand RejectCommand { get; } = new Command(async () =>
        {
            mainVM.DataLoaded = false;
            await Task.Run(() =>
            {
                using AppContext appContext = new();
                appContext.Sallers.Find(saller.Id)!.BannedByAdmin = true;

                appContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() => navigator.Reload(navParam));
            });
        });
    }

    internal sealed class AdminCard ( Models.Admin admin ) : ViewModel, IRole
    {
        private const string Descriminator = "Admin";
        private readonly Dictionary<string, string> _translator = new()
    {
        {Descriminator, "Администратор"}
    };

        public Role Role => admin;
        public string Fio => $"ФИ: {admin.Surname} {admin.Name}";

        public string Email => $"Email: {admin.Email}";

        public string Type => $"Тип: {_translator[Descriminator]}";

        public string RegistrationDate => $"Дата регистрации {admin.DateOfRegistration.ToShortDateString()}";

        public bool Banned => (bool)admin.BannedByAdmin!;
        public string StatusText => admin switch
        {
            { BannedByAdmin: true } => "Заблокирован",
            _ => "Активный администратор"
        };
    }

    internal sealed class UserCard ( User user ) : ViewModel, IRole
    {
        private const string Descriminator = "User";
        private readonly Dictionary<string, string> _translator = new()
    {
        {Descriminator, "Пользователь"}
    };

        public User User => user;

        public Role Role => user;

        public string Fio => $"ФИ: {user.Surname} {user.Name}";
        public string Type => $"Тип: {_translator[Descriminator]}";

        public string Email => $"Email: {user.Email}";

        public string RegistrationDate => $"Дата регистрации {user.DateOfRegistration.ToShortDateString()}";

        public string Age => $"Возраст {(DateTime.Now - user.Birthday).Days / 365} лет";

        public bool Banned => (bool)user.BannedByAdmin!;
        public string StatusText => user switch
        {
            { BannedByAdmin: true } => "Заблокирован",
            _ => "Активный пользователь"
        };
    }

    internal sealed class Default ()
    {
        public string Message => "Ничего не выбрано";
    }

    internal sealed class AllUsersManagmentViewModel : ViewModel
    {
        private readonly Navigation _navigator;
        private readonly Models.Admin? _admin;
        private bool _isDataLoaded;
        private ShortUserCard? _shortUserCard;
        private dynamic _currentModel;
        

        private bool _userChecked;
        private bool _sallerChecked;
        private bool _adminChecked;

        private bool _banEnable;
        private bool _unbunEnable;


        public ObservableCollection<ShortUserCard>? AllUsers { get; private set; }

        public ObservableCollection<ShortUserCard>? AllUsersConst { get; private set; }

        public bool DataLoaded
        {
            get => _isDataLoaded;
            set => this.SetValue(ref _isDataLoaded, value);
        }

        public bool BanEnable
        {
            get => _banEnable;
            set
            {
                this.SetValue(ref _banEnable, value);
            }
        }

        public bool UnbunEnable
        {
            get => _unbunEnable;
            set
            {
                this.SetValue(ref _unbunEnable, value);
            }
        }

        public bool UserChecked
        {
            get => _userChecked;
            set => this.SetValue(ref _userChecked, value);
        }

        public bool SallerChecked
        {
            get => _sallerChecked;
            set => this.SetValue(ref _sallerChecked, value);
        }

        public bool AdminChecked
        {
            get => _adminChecked;
            set => this.SetValue(ref _adminChecked, value);
        }


        public ShortUserCard? SelectedItem
        {
            get => _shortUserCard;
            set
            {
                this.SetValue(ref _shortUserCard, value);
                ChangeCurrentModel(value!);
            }
        }

        public dynamic CurrentModel
        {
            get => _currentModel;
            set
            {
                SetValue(ref _currentModel, value);
            }
        }

       

        public ICommand SelectCategoris { get; }

        public ICommand SelectMsgs { get; }

        public ICommand SelectBanned { get; }

        public ICommand BunCommand { get; }

        public ICommand UnbunCommand { get; }

        public AllUsersManagmentViewModel ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            _admin = param as Models.Admin;
            _currentModel = new Default();
            _param = param;

            _userChecked = true;
            _sallerChecked = true;
            _adminChecked = true;

            LoadData().ConfigureAwait(false);
            SelectCategoris = new Command(Sort);
            SelectMsgs = new Command(SortMsgs);
            SelectBanned = new Command(SortBanned);
            BunCommand = new Command<IRole?>(Bun);
            UnbunCommand = new Command<IRole?>(Unbun);
        }

        private async void Bun( IRole? item )
        {
            if( item == null ) return;

            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext appContext = new ();
                var dbItem = appContext.AllRoles.Find(item.Role.Id);
                dbItem!.BannedByAdmin = true;
                appContext.SaveChanges();

                int i;

                for (i = 0; i < AllUsers!.Count; i++)
                {
                    if (AllUsers[i].Role.Id == item.Role.Id)
                    {
                        AllUsers[i].Role.BannedByAdmin = true;
                        AllUsersConst![AllUsersConst.IndexOf(AllUsers[i])].Role.BannedByAdmin = true;
                        break;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChangeCurrentModel(AllUsers[i]);
                    AllUsers = new(AllUsers.Where(x => x.Role.BannedByAdmin == false));
                    OnPropertyChanged(nameof(AllUsers));
                });
            });

            DataLoaded = true;
        }

        private async void Unbun ( IRole? item )
        {
            if (item == null) return;

            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext appContext = new();
                var dbItem = appContext.AllRoles.Find(item.Role.Id);
                dbItem!.BannedByAdmin = false;
                appContext.SaveChanges();

                int i;

                for (i = 0;  i < AllUsers!.Count; i++)
                {
                    if (AllUsers[i].Role.Id == item.Role.Id)
                    {
                        AllUsers[i].Role.BannedByAdmin = false;
                        AllUsersConst![AllUsersConst.IndexOf(AllUsers[i])].Role.BannedByAdmin = false;
                        break;
                    }
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    ChangeCurrentModel(AllUsers[i]);
                   
                });
            });

            SortBanned();
        }

        private async void SortBanned ()
        {
            DataLoaded = false;
            await Task.Run(() =>
            {

                AllUsers = new(AllUsersConst!.Where(e => e.Role.BannedByAdmin == true));
            });

            OnPropertyChanged(nameof(AllUsers));
            DataLoaded = true;
        }

        private async void SortMsgs ()
        {
            DataLoaded = false;
            await Task.Run(() =>
            {

                AllUsers = new(AllUsersConst!
                    .Where(e => e.Role is Models.Saller saller && saller.IsNowRegister == false 
                    && saller.BannedByAdmin == false));
            });

            OnPropertyChanged(nameof(AllUsers));
            DataLoaded = true;
        }

        private async void Sort ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                Func<ShortUserCard, bool> filter = p =>
                {
                    bool result = false;
                    if (SallerChecked)
                        result = p.Role is Models.Saller;
                    if (AdminChecked)
                        result = result || p.Role is Models.Admin;
                    if (UserChecked)
                        result = result || p.Role is Models.User;

                    return result;
                };

                AllUsers = new(AllUsersConst!.Where(filter).Where(e => e.Role.BannedByAdmin == false));
            });

            OnPropertyChanged(nameof(AllUsers));
            DataLoaded = true;
        }

        private async Task LoadData ()
        {
            await Task.Run(() =>
            {
                using AppContext context = new();
                AllUsers = new(context.AllRoles
                    .Select(r => new ShortUserCard(r, r.GetType().Name)));
                AllUsersConst = new(AllUsers);
            });

            Sort();
        }

        private void ChangeCurrentModel ( ShortUserCard? item)
        {
            ChangeBanEnabled(item);
            ChangeUnBanEnabled(item);

            switch (item?.Role)
            {
                case Models.Admin admin:
                    CurrentModel = new AdminCard(admin);
                    break;
                case Models.Saller saller:
                    CurrentModel = new SallerCard(saller, _navigator, this, _admin!);
                    break;
                case Models.User user:
                    CurrentModel = new UserCard(user);
                    break;
                default:
                    CurrentModel = new Default();
                    break;
            }
        }

        private void ChangeBanEnabled( ShortUserCard? item)
        {
            if (item?.Role.BannedByAdmin == false)
                BanEnable = true;
            else BanEnable = false;
        }

        private void ChangeUnBanEnabled ( ShortUserCard? item )
        {
            if (item?.Role.BannedByAdmin == true)
                UnbunEnable = true;
            else UnbunEnable = false;
        }
    }
}
