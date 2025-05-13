using Azure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;
using TiketsApp.ViewModels.Base;

namespace TiketsApp.Core.Servises
{
    internal class Navigation 
    {
        private readonly Frame _frame;
        private readonly NavigationHistory _history;
        public event EventHandler? NavigationStateChanged;

        public Navigation ( Frame frame )
        {
            _frame = frame;
            _history = new(this);
        }


        private void OnNavigation ()
        {
            NavigationStateChanged?.Invoke(this, EventArgs.Empty);
        }

        internal void NavigateTo<PageType> ( Type viewModelType, object? param ) where PageType : Page
        {
            var page = PageFabric.CreatPage(typeof(PageType));
            var viewModel = PageFabric.CreateViewModel(viewModelType, param, this);

            page.DataContext = viewModel;

            var oldPage = _frame.Content as Page;
            
            if (oldPage != null && oldPage.GetType() == page.GetType()) 
                return;

            _history.AddBack(oldPage);
            _history.ClearFront();
            _frame.Navigate(page);
            OnNavigation();
        }

        internal void GoBack ()
        {
            if (_history.CanGoBack)
            {
                var page = _frame.Content as Page;
                _frame.Navigate(_history.GetBack(page));
            }
        }

        internal void GoFront ()
        {
            if (_history.CanGoForward)
            {
                var page = _frame.Content as Page;
                _history.AddBack(page);
                _frame.Navigate(_history.GetForward());
            }
        }

        internal void Reload ( object? param = null)
        {
            var page = _frame.Content as Page;
            var vm = page!.DataContext as ViewModel;

            var viewModel = PageFabric.CreateViewModel(page!.DataContext.GetType(), vm?.Param, this);

            page!.DataContext = viewModel;

            _frame.Navigate(page);
        }

        internal bool CanGoBack => _history.CanGoBack;
        internal bool CanGoFront => _history.CanGoForward;
    }

    sealed class NavigationHistory
    {
        private Stack<Page> backPages;
        private Stack<Page> frontPages;
        private readonly Navigation _navigator;

        public NavigationHistory ( Navigation navigator )
        {
            backPages = [];
            frontPages = [];
            _navigator = navigator;
        }

        public void AddBack ( Page? page )
        {
            if (page != null)
                backPages.Push(page);
        }

        public Page GetBack (Page? forvard)
        {
            var back = backPages.Pop();
            if(forvard != null) frontPages.Push(forvard);
            return CraetePage(back);
        }

        public bool CanGoForward => frontPages.Count > 0;

        public bool CanGoBack => backPages.Count > 0;

        public Page GetForward () => CraetePage(frontPages.Pop());

        public void ClearFront () => frontPages.Clear();

        private Page CraetePage(Page page )
        {
            var newPage = PageFabric.CreatPage(page.GetType());
            var param = page.DataContext as ViewModel;
            var vm = PageFabric.CreateViewModel(page.DataContext.GetType(), param?.Param, _navigator);
            newPage.DataContext = vm;
            return newPage;
        }
    }
}
