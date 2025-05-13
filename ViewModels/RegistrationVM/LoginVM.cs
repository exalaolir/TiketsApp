using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.res;
using TiketsApp.ViewModels.Admin;
using TiketsApp.ViewModels.Base;
using TiketsApp.ViewModels.Saller;
using TiketsApp.ViewModels.UsersVm;
using TiketsApp.Views.AdminViews;
using TiketsApp.Views.RegistrationViews;
using TiketsApp.Views.SallerViews;
using TiketsApp.Views.UserViews;

namespace TiketsApp.ViewModels.RegistrationVM
{
    internal sealed class LoginVM : ValidationViewModel, IPassword
    {
        private readonly Navigation _navigator;
        private string _email;
        private string _pass;
        private bool _dataLoaded;

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [EmailAddress(ErrorMessage = Consts.EmailError)]
        public string Email
        {
            get => _email;
            set
            {
                this.SetValue(ref _email, value);
                Validate(Email);
                _validFields[0] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.PasswordPattern, ErrorMessage = Consts.PassError)]
        public string Password
        {
            get => _pass;
            set
            {
                this.SetValue(ref _pass, value);
                Validate(Password, type: ValidationType.Password);
                _validFields[1] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set => this.SetValue(ref _dataLoaded, value);
        }

        public ICommand NavToUserPageCommand { get; }
        public ICommand NavToSallerPageCommand { get; }

        public ICommand LoginCommand { get; }

        public LoginVM ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            DataLoaded = true;
            _email = string.Empty;
            _pass = string.Empty;

            _validFields = new bool[2];

            NavToUserPageCommand = new Command(() => _navigator.NavigateTo<UserPage>(typeof(UserVM), null));
            NavToSallerPageCommand = new Command(() => _navigator.NavigateTo<SallerPage>(typeof(SallerVM), null));
            LoginCommand = new Command(Login);
        }

        private async void Login ()
        {
            DataLoaded = false;

            await Task.Run(async () =>
            {
                var results = await ValidationServise.ValidateOnLogin(Email, Password);

                var errors = results.Item2;
                if (errors.Count > 0)
                {
                    errors.ForEach(x =>
                    {
                        SetValidationResults(x.result, x.property, x.message!);
                        if (x.property == nameof(Password))
                        {
                            HasPasswordErrors = !x.result;
                            PasswordError = x.property;
                            _validFields[1] = false;
                            this.OnPropertyChanged(nameof(IsButtonEnabled));
                        }
                    });
                    DataLoaded = true;
                    return;
                }

                Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (results.Item1)
                    {
                        case User user:
                            _navigator.NavigateTo<HomePage>(typeof(UserControlVm), user);
                            break;

                        case Models.Saller saller:
                            if (saller.IsNowRegister! == false)
                            {
                                SetValidationResults(false, nameof(Email), Consts.AccauntNotAddedMsg);
                                DataLoaded = true;
                            }
                            else _navigator.NavigateTo<StartPage>(typeof(SallerControlVM), saller);
                            break;

                        case Models.Admin admin:
                            _navigator.NavigateTo<StartAdminPage>(typeof(AdminControlVM), admin);
                            break;
                    }
                });
            });
        }
    }
}
