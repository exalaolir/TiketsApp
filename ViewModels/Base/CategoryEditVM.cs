using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TiketsApp.res;

namespace TiketsApp.ViewModels.Base
{
    internal abstract class CategoryEditVM : ValidationViewModel
    {
        protected string _name = string.Empty;

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [MaxLength(30, ErrorMessage = Consts.CategoryLenghtMessage)]
        [RegularExpression(Consts.FioPattern, ErrorMessage = Consts.NameError)]
        public string Name
        {
            get => _name;
            set
            {
                this.SetValue(ref _name, value);
                Validate(Name);
                _validFields[0] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public abstract ICommand SaveCommand { get; }
    }
}
