using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.res;
using TiketsApp.ViewModels.Base;
using TiketsApp.Views;
using TiketsApp.Views.RegistrationViews;

namespace TiketsApp.ViewModels.RegistrationVM
{
    internal class UserVM : ValidationViewModel, IPassword, IRepeatPassword, IDateTimer
    {
        private readonly Navigation _navigator;
        private DateTime _currentDate;
        private readonly DispatcherTimer _timer;

        private string _name;
        private string _surname;
        private string _email;
        private string _password;
        private string _repeatPassword;
        private DateTime _birthday;
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

        public DateTime Birthday
        {
            get => _birthday;
            set
            {
                SetValue(ref _birthday, value);
                _validFields[2] = true; 
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
                if (Password.Length >= 8)
                    Validate(RepeatPassword, type: ValidationType.RepeatPassword, nameof(RepeatPassword));
                _validFields[4] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [Compare(nameof(Password), ErrorMessage = Consts.RepeatPassError)]
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

        public ICommand NavigateToLoginCommand { get; }
        public ICommand NavToSallerPageCommand { get; }

        public ICommand RegisterUserCommand { get; }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set => this.SetValue(ref _currentDate, value);
        }

        public UserVM(object? param, Navigation navigator) 
        {
            _navigator = navigator;
            _currentDate = DateTime.Today;
            _birthday = CurrentDate.AddYears(-18);
            _timer = new DispatcherTimer
            {
                Interval = IDateTimer.CalculateTimeUntilMidnight()
            };
            _timer.Tick += OnDayChanged;
            _timer.Start();

            DataLoaded = true;

            _name = string.Empty;
            _surname = string.Empty;
            _email = string.Empty;
            _password = string.Empty;
            _repeatPassword = string.Empty;

            _validFields = new bool[6];
            _validFields[2] = true;

            NavigateToLoginCommand = new Command(() => _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null));
            NavToSallerPageCommand = new Command(() => _navigator.NavigateTo<SallerPage>(typeof(SallerVM), null));
            RegisterUserCommand = new Command(RegisterUser);
        }

        public void OnDayChanged ( object? sender, EventArgs e )
        {
            CurrentDate = DateTime.Today;
            _timer.Interval = IDateTimer.CalculateTimeUntilMidnight();
        }

        private async void RegisterUser ()
        {
            DataLoaded = false;

            await Task.Run(async () =>
            {
                var newUser = new User()
                {
                    Name = _name,
                    Email = _email,
                    Password = Hasher.HashPassword(_password),
                    Surname = _surname,
                    DateOfRegistration = DateTime.Now,
                    Birthday = _birthday,
                    BannedByAdmin = false,
                };

                var results = await ValidationServise.ValidateOnRegister<User>(newUser);

                if (results.Count > 0)
                {
                    results.ForEach(x => SetValidationResults(x.result, x.property, x.message!));
                    DataLoaded = true;
                    return;
                }

                using AppContext appContext = new();

                appContext.Users.Add(newUser);
                appContext.SaveChanges();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    _navigator.NavigateTo<LoginPage>(typeof(LoginVM), null);
                });
            });
        }
    }
}
