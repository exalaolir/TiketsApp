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
            _frame.NavigationService.RemoveBackEntry();
            OnNavigation();
        }

        internal void GoBack ()
        {
            if (_history.CanGoBack)
            {
                var page = _frame.Content as Page;
                _frame.Navigate(_history.GetBack(page));
                _frame.NavigationService.RemoveBackEntry();
            }
        }

        internal void GoFront ()
        {
            if (_history.CanGoForward)
            {
                var page = _frame.Content as Page;
                _history.AddBack(page);
                _frame.Navigate(_history.GetForward());
                _frame.NavigationService.RemoveBackEntry();
            }
        }

        internal void Reload ( object? param = null )
        {
            var page = _frame.Content as Page;
            var vm = page!.DataContext as ViewModel;

            var viewModel = PageFabric.CreateViewModel(page!.DataContext.GetType(), vm?.Param, this);

            page!.DataContext = viewModel;

            _frame.Navigate(page);
            _frame.NavigationService.RemoveBackEntry();
        }

        internal bool CanGoBack => _history.CanGoBack;
        internal bool CanGoFront => _history.CanGoForward;
    }

    sealed class NavigationHistory
    {
        private LinkedList<Page> backPages;
        private LinkedList<Page> frontPages;
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
            {
                backPages.AddFirst(page);
                if (backPages.Count >= 5)
                {
                    CleanupPage(backPages.Last());
                    backPages.RemoveLast();
                }
            }
        }

        public Page GetBack ( Page? forvard )
        {
            var back = backPages.First();
            var newPage = CraetePage(back);
            CleanupPage(backPages.First());
            backPages.RemoveFirst();
            if (forvard != null)
            {
                frontPages.AddFirst(forvard);
                if (frontPages.Count >= 5)
                {
                    CleanupPage(frontPages.Last());
                    frontPages.RemoveLast();
                }
            }
            return newPage;
        }

        private static void CleanupPage ( Page page )
        {
            (page as IDisposable)?.Dispose();
            page.DataContext = null;
        }

        public bool CanGoForward => frontPages.Count > 0;

        public bool CanGoBack => backPages.Count > 0;

        public Page GetForward ()
        {
            var res = CraetePage(frontPages.First());
            CleanupPage (frontPages.First());
            frontPages.RemoveFirst();
            return res;
        }

        public void ClearFront ()
        {
            foreach (var page in frontPages)
            {
                CleanupPage(page);
            }

            frontPages.Clear();
        }

        private Page CraetePage ( Page page )
        {
            var newPage = PageFabric.CreatPage(page.GetType());
            var param = page.DataContext as ViewModel;
            var vm = PageFabric.CreateViewModel(page.DataContext.GetType(), param?.Param, _navigator);

            CleanupPage(page);
            newPage.DataContext = vm;
            return newPage;
        }
    }
}

//using System;
//using System.Collections.Generic;
//using System.Windows.Controls;
//using System.Windows.Navigation;
//using TiketsApp.ViewModels.Base;

//namespace TiketsApp.Core.Servises
//{
//    internal class Navigation
//    {
//        private readonly Frame _frame;
//        private readonly NavigationHistory _history;
//        public event EventHandler? NavigationStateChanged;

//        public Navigation ( Frame frame )
//        {
//            _frame = frame;
//            _history = new(this);
//            _frame.Navigated += Frame_Navigated;
//        }

//        private void Frame_Navigated ( object sender, NavigationEventArgs e )
//        {
//            // Очистка журнала навигации Frame
//            while (_frame.CanGoBack)
//            {
//                _frame.RemoveBackEntry();
//            }
//        }

//        private void OnNavigation ()
//        {
//            NavigationStateChanged?.Invoke(this, EventArgs.Empty);
//        }

//        internal void NavigateTo<PageType> ( Type viewModelType, object? param ) where PageType : Page
//        {
//            var page = PageFabric.CreatPage(typeof(PageType));
//            var viewModel = PageFabric.CreateViewModel(viewModelType, param, this);

//            page.DataContext = viewModel;

//            var oldPage = _frame.Content as Page;

//            if (oldPage != null && oldPage.GetType() == page.GetType())
//                return;

//            _history.AddBack(oldPage);
//            _history.ClearFront();

//            // Очистка ресурсов перед навигацией
//            if (_frame.Content is IDisposable disposable)
//            {
//                disposable.Dispose();
//            }

//            _frame.Navigate(page);
//            OnNavigation();
//        }

//        internal void GoBack ()
//        {
//            if (_history.CanGoBack)
//            {
//                var currentPage = _frame.Content as Page;
//                var previousPage = _history.GetBack(currentPage);

//                // Очистка текущей страницы
//                if (currentPage != null)
//                {
//                    CleanupPage(currentPage);
//                }

//                _frame.Navigate(previousPage);
//                OnNavigation();
//            }
//        }

//        internal void GoFront ()
//        {
//            if (_history.CanGoForward)
//            {
//                var currentPage = _frame.Content as Page;
//                _history.AddBack(currentPage);

//                var nextPage = _history.GetForward();
//                _frame.Navigate(nextPage);
//                OnNavigation();
//            }
//        }

//        internal void Reload ( object? param = null )
//        {
//            var page = _frame.Content as Page;
//            if (page == null) return;

//            var vm = page.DataContext as ViewModel;
//            var viewModel = PageFabric.CreateViewModel(page.DataContext.GetType(), vm?.Param ?? param, this);

//            // Очистка старого DataContext
//            if (page.DataContext is IDisposable disposable)
//            {
//                disposable.Dispose();
//            }

//            page.DataContext = viewModel;
//            _frame.Navigate(page);
//        }

//        private void CleanupPage ( Page page )
//        {
//            if (page.DataContext is IDisposable disposableVm)
//            {
//                disposableVm.Dispose();
//            }
//            page.DataContext = null;
//        }

//        internal bool CanGoBack => _history.CanGoBack;
//        internal bool CanGoFront => _history.CanGoForward;
//    }

//    sealed class NavigationHistory
//    {
//        private const int MaxHistorySize = 5; // Ограничение истории навигации
//        private readonly LimitedStack<Page> _backPages;
//        private readonly LimitedStack<Page> _forwardPages;
//        private readonly Navigation _navigator;

//        public NavigationHistory ( Navigation navigator )
//        {
//            _backPages = new LimitedStack<Page>(MaxHistorySize);
//            _forwardPages = new LimitedStack<Page>(MaxHistorySize);
//            _navigator = navigator;
//        }

//        public void AddBack ( Page? page )
//        {
//            if (page != null)
//            {
//                _backPages.Push(page);
//            }
//        }

//        public Page GetBack ( Page? currentPage )
//        {
//            var backPage = _backPages.Pop();

//            if (currentPage != null)
//            {
//                _forwardPages.Push(currentPage);
//            }

//            return CreatePage(backPage);
//        }

//        public bool CanGoForward => _forwardPages.Count > 0;
//        public bool CanGoBack => _backPages.Count > 0;

//        public Page GetForward ()
//        {
//            return CreatePage(_forwardPages.Pop());
//        }

//        public void ClearFront ()
//        {
//            foreach (var page in _forwardPages)
//            {
//                CleanupPage(page);
//            }
//            _forwardPages.Clear();
//        }

//        private Page CreatePage ( Page page )
//        {
//            var newPage = PageFabric.CreatPage(page.GetType());
//            if (page.DataContext is ViewModel vm)
//            {
//                var newVm = PageFabric.CreateViewModel(
//                    page.DataContext.GetType(),
//                    vm.Param,
//                    _navigator);
//                newPage.DataContext = newVm;
//            }
//            return newPage;
//        }

//        private void CleanupPage ( Page page )
//        {
//            if (page.DataContext is IDisposable disposable)
//            {
//                disposable.Dispose();
//            }
//            page.DataContext = null;
//        }
//    }

//    sealed class LimitedStack<T> : IEnumerable<T>
//    {
//        private readonly LinkedList<T> _items = new();
//        private readonly int _maxSize;

//        public LimitedStack ( int maxSize )
//        {
//            if (maxSize <= 0)
//                throw new ArgumentOutOfRangeException(nameof(maxSize));
//            _maxSize = maxSize;
//        }

//        public int Count => _items.Count;

//        public void Push ( T item )
//        {
//            _items.AddFirst(item);
//            if (_items.Count > _maxSize)
//            {
//                var last = _items.Last.Value;
//                if (last is IDisposable disposable)
//                {
//                    disposable.Dispose();
//                }
//                _items.RemoveLast();
//            }
//        }

//        public T Pop ()
//        {
//            if (_items.Count == 0)
//                throw new InvalidOperationException("Stack is empty");

//            var item = _items.First.Value;
//            _items.RemoveFirst();
//            return item;
//        }

//        public void Clear ()
//        {
//            _items.Clear();
//        }

//        public IEnumerator<T> GetEnumerator () => _items.GetEnumerator();

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator () => GetEnumerator();
//    }
//}
