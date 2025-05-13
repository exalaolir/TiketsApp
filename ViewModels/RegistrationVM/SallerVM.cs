using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.res;
using TiketsApp.ViewModels.Base;
using TiketsApp.Views;
using TiketsApp.Views.RegistrationViews;

namespace TiketsApp.ViewModels.RegistrationVM
{
    internal class SallerVM : ValidationViewModel, IPassword, IRepeatPassword
    {
        private readonly Navigation _navigator;
        private string _name;
        private string _surname;
        private string _email;
        private string _password;
        private string _repeatPassword;
        private string _number;
        private bool _dataLoaded;


        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.FioPattern, ErrorMessage = Consts.NameError)]
        public string Name
        {
            get => _name;
            set
            {
                this.SetValue(ref _name, value);
                Validate(Name);
                _validFields[0] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.FioPattern, ErrorMessage = Consts.NameError)]
        public string Surname
        {
            get => _surname;
            set
            {
                this.SetValue(ref _surname, value);
                Validate(Surname);
                _validFields[1] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.NumPattern, ErrorMessage = Consts.NumberMessage)]
        public string Num
        {
            get => _number;
            set
            {
                this.SetValue(ref _number, value);
                Validate(Num);
                _validFields[2] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [EmailAddress(ErrorMessage = Consts.EmailError)]
        public string Email
        {
            get => _email;
            set
            {
                this.SetValue(ref _email, value);
                Validate(Email);
                _validFields[3] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.PasswordPattern, ErrorMessage = Consts.PassError)]
        public string Password
        {
            get => _password;
            set
            {
                this.SetValue(ref _password, value);
                Validate(Password, type: ValidationType.Password);
                _validFields[4] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [RegularExpression(Consts.PasswordPattern, ErrorMessage = Consts.PassError)]
        public string RepeatPassword
        {
            get => _repeatPassword;
            set
            {
                this.SetValue(ref _repeatPassword, value);
                Validate(RepeatPassword, type: ValidationType.RepeatPassword);
                _validFields[5] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set => this.SetValue(ref _dataLoaded, value);
        }

        public ICommand NavToUserPageCommand { get; }
        public ICommand NavigateToLoginCommand { get; }

        public ICommand RegisterSallerCommand { get; }

        public SallerVM ( object? param, Navigation navigation )
        {
            DataLoaded = true;
            _navigator = navigation;
            _validFields = new bool[6];

            _name = string.Empty;
            _surname = string.Empty;
            _email = string.Empty;
            _password = string.Empty;
            _repeatPassword = string.Empty;
            _number = string.Empty;

            NavigateToLoginCommand = new Command(() => _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null));
            NavToUserPageCommand = new Command(() => _navigator.NavigateTo<UserPage>(typeof(UserVM), null));
            RegisterSallerCommand = new Command(RegisterSaller);
        }

        private async void RegisterSaller ()
        {
            DataLoaded = false;

            await Task.Run(async () =>
            {
                var newSaller = new Models.Saller()
                {
                    Name = _name,
                    Email = _email,
                    Password = Hasher.HashPassword(_password),
                    Surname = _surname,
                    DateOfRegistration = DateTime.Now,
                    Number = _number,
                    BannedByAdmin = false,
                };

                var results = await ValidationServise.ValidateOnRegister<Models.Saller>(newSaller);

                if (results.Count > 0)
                {
                    results.ForEach(x => SetValidationResults(x.result, x.property, x.message!));
                    DataLoaded = true;
                    return;
                }

                using AppContext appContext = new();

                appContext.Sallers.Add(newSaller);
                appContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null);
                });
            });
        }
    }
}
