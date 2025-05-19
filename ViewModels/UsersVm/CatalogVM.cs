using Microsoft.EntityFrameworkCore;
using Panuon.WPF.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.Models.DTO;
using TiketsApp.ViewModels.Base;
using TiketsApp.Views.UserViews;

namespace TiketsApp.ViewModels.UsersVm
{
    internal sealed class EventCardVM : ViewModel
    {
        private readonly Event _newEvent;
        private readonly CatalogVM _catalogVM;
        private Order? _order;
        private readonly string _btnText;

        public Order? Order => _order;

        public Event Event => _newEvent;

        public string Path => _newEvent.Emages[0].Path;

        public string Title => _newEvent.Name;

        public string Start => _newEvent.StartTime.ToString();

        public string Desciption => _newEvent.Description;

        public string BtnText => _btnText;

        public string Price => string.Format("{0:C2}", _newEvent.Price);

        public ICommand WatchPriview { get; }

        public EventCardVM ( Event newEvent, CatalogVM catalogVM, Order? order )
        {
            _catalogVM = catalogVM;
            _newEvent = newEvent;
            _order = order;

            _btnText = order == null || order.Status == Status.RejectByUser ? "Подробнее" : "Изменить бронь";

            WatchPriview = _catalogVM.WatchPriviewCommand;
        }
    }

    internal sealed class CatalogVM : ViewModel
    {
        private readonly Navigation _navigator;
        private readonly User? _user;
        private bool _dataLoaded;

        private decimal _min;

        public decimal Min
        {
            get => _min;
            set
            {
                SetValue(ref _min, value);
            }
        }

        private decimal _max;

        public decimal Max
        {
            get => _max;
            set
            {
                SetValue(ref _max, value);
            }
        }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetValue(ref _searchText, value);
            }
        }

        public ICommand Search => new Command(() =>
        {
            if (_searchText != string.Empty)
            {
                Events = new(Events!.Where(e => Regex.IsMatch(e.Title, $"^{Regex.Escape(_searchText)}", RegexOptions.IgnoreCase)));
            }
            else
            {
                Events = new(_events!);
            }

            OnPropertyChanged(nameof(Events));
        });

        private decimal _currentPrice;

        public decimal CurrentPrice
        {
            get => _currentPrice;
            set
            {
                SetValue(ref _currentPrice, value);
            }
        }

        public ICommand SortByDateDes => new Command(() =>
        {
            Events = new(Events!.OrderByDescending(e => e.Event.StartTime));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand FilterByPrice => new Command(() =>
        {
            Events = new(_events!.Where(e => decimal.Parse(e.Price.TrimEnd([' ', 'B', 'r'])) <= CurrentPrice));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByDate => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e.Event.StartTime));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByName => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e.Event.Name));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByCount => new Command(() =>
        {
            Events = new(Events!.OrderByDescending(e => e.Event.Count));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByPrice => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e.Event.Price));
            OnPropertyChanged(nameof(Events));
        });

        public ObservableCollection<EventCardVM>? Events { get; private set; }

        public ObservableCollection<EventCardVM>? _events { get; private set; }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set
            {
                SetValue(ref _dataLoaded, value);
            }
        }

        public ICommand WatchPriviewCommand { get; }


        public CatalogVM ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            _user = param as User;
            _param = param;

            WatchPriviewCommand = new Command<EventCardVM>(card =>
            {
                Order? order;

                if (card.Order == null || card.Order.Status == Status.RejectByUser)
                    order = null;
                else order = card.Order;

                    var dto = new PriviewEventDto(_user!, card.Event, order);

                _navigator.NavigateTo<PriviewPage>(typeof(PriviewVM), dto); 
            });

            LoadData().ConfigureAwait(false);
        }

        private async Task LoadData ()
        {
            await Task.Run(() =>
            {
                Events = [];

                using AppContext appContext = new();

                var data = appContext.Events.Where(e => DateTime.Now < e.StartTime && !e.IsDeleted)
                .Include(e => e.Emages)
                .Include(e => e.Saller)
                .Include(e =>e.Orders)
                .ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    data.ForEach(e => Events.Add(
                        new EventCardVM(e, 
                        this, 
                        e.Orders.FirstOrDefault(o => o.UserId == _user!.Id))));

                    Max = data.Max(d => d.Price);
                    Min = data.Min(d => d.Price);
                    CurrentPrice = Max;

                    _events = new(Events);
                });
            });

            OnPropertyChanged(nameof(Events));
            DataLoaded = true;
        }
    }
}
