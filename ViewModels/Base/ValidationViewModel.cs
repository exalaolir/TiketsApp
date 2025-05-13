using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using TiketsApp.Core.Servises;

namespace TiketsApp.ViewModels.Base
{
    public abstract class ValidationViewModel : ViewModel, INotifyDataErrorInfo
    {
        protected enum ValidationType
        {
            Password,
            RepeatPassword,
            TextField
        }

        protected bool[] _validFields = [];

        public bool IsButtonEnabled
        {
            get => !HasErrors && !_validFields.Contains(false);
        }

        protected readonly Dictionary<string, IReadOnlyList<string>> _errors = [];

        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected bool _hasPasswordErrors;
        protected string? _passwordError;

        protected bool _hasRepeatPasswordErrors;
        protected string? _repeatPasswordError;

        public bool HasErrors => _errors.Count > 0;

        public string PasswordError
        {
            get => _passwordError ?? "";
            protected set => this.SetValue(ref _passwordError, value);
        }

        public bool HasPasswordErrors
        {
            get => _hasPasswordErrors;
            set => this.SetValue(ref _hasPasswordErrors, value);
        }
        public bool HasRepeatPasswordErrors { get => _hasRepeatPasswordErrors; set => this.SetValue(ref _hasRepeatPasswordErrors, value); }
        public string? RepeatPasswordError { get => _repeatPasswordError; protected set => this.SetValue(ref _repeatPasswordError, value); }

        public IEnumerable GetErrors ( string? propertyName )
        {
            return _errors.TryGetValue(propertyName ?? "", out var errors) ? errors : Enumerable.Empty<ValidationError>();
        }

        protected void Validate ( object property, ValidationType type = ValidationType.TextField, [CallerMemberName] string? propertyName = null )
        {
            if (propertyName == null)
                throw new ArgumentNullException(property.ToString(), "Ошибка в передаче имени поля");

            var results = ValidationServise.ValidateProperty(property, this, propertyName);

            SetValidationResults(results.Item1, propertyName, results.Item2!);

            if (type == ValidationType.Password)
            {
                HasPasswordErrors = !results.Item1;
                PasswordError = !results.Item1 ? _errors[propertyName][0] : "";
            }

            if (type == ValidationType.RepeatPassword)
            {
                HasRepeatPasswordErrors = !results.Item1;
                RepeatPasswordError = !results.Item1 ? _errors[propertyName][0] : "";
            }
        }

        protected void SetValidationResults ( bool result, string propertyName, IReadOnlyList<string> errorMessages )
        {
            if (!result)
                _errors[propertyName] = errorMessages!;
            else
                _errors.Remove(propertyName);

            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            this.OnPropertyChanged(nameof(HasErrors));
        }
    }
}
