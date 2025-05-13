using System.Windows.Input;
using TiketsApp.Core.Servises;

namespace TiketsApp.ViewModels.Base
{
    internal abstract class ControlVM : ViewModel
    {
        protected bool _canGoBack;
        protected bool _canGoFront;
        protected Navigation? _subnavigator;

        public Navigation? Subnavigator 
        { 
            get => _subnavigator;
            set
            {
                _subnavigator = value;
                if(value != null)
                {
                    value.NavigationStateChanged += NavBtnsRefresH;
                }
            }
        }
        
        protected ControlVM ()
        {
          
        }

        public bool CanGoBack
        {
            get => _canGoBack;
            set => SetValue(ref _canGoBack, value);
        }

        public bool CanGoFront
        {
            get => _canGoFront;
            set => SetValue(ref _canGoFront, value);
        }

        public abstract ICommand ExitCommand { get; }

        public abstract ICommand BackCommand { get; }

        public abstract ICommand FrontCommand { get; }

        public abstract ICommand RefreshCommand { get; }

        protected void NavBtnsRefresH (object? sender = null, EventArgs? e = null)
        {
            CanGoBack = Subnavigator!.CanGoBack;
            CanGoFront = Subnavigator!.CanGoFront;
        }
    }
}
