using Microsoft.EntityFrameworkCore;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.Models.DTO;
using TiketsApp.ViewModels.Admin;
using TiketsApp.ViewModels.Base;
using TiketsApp.Views.SallerViews;

namespace TiketsApp.ViewModels.Saller
{
    internal sealed class FullIventVM : ViewModel
    {
        private readonly Models.Event _event;
        private readonly EventsVM _eventsVM;

        public Models.Event Event => _event;

        public string Title => _event.Name;

        public string Description => $"Описание: {_event.Description}";

        public string Adress => $"{_event.Adress}";

        public string Cost => $"{string.Format("{0:C2}", _event.Price)}";

        public bool IsEnd => DateTime.Now >= _event.StartTime;

        public bool IsEndReverse => !IsEnd;

        public string Count => $"{_event.Count}/{_event.MaxCount}";

        public string StartDay => $"{_event.StartTime}";

        public string EndDay => $"{_event.EndTime}";

        public string Status => IsEnd ? "Завершено" : "Не завершено";

        public ICommand ChangeCommand => _eventsVM.NavigateToNewEventCommand;

        public ICommand DeleteCommand => _eventsVM.DeleteCommand;

        public FullIventVM ( Models.Event newEevent, EventsVM eventsVM )
        {
            _event = newEevent;
            _eventsVM = eventsVM;
        }
    }

    internal sealed class IventCardVM : ViewModel
    {
        public readonly Models.Event _event;
        private readonly EventsVM _eventsVM;

        public string Title => _event.Name;
        public string Count => $"{_event.Count}/{_event.MaxCount}";

        public string Description => _event.Description;

        public ICommand GetFullInfoCommand { get; }

        public bool IsEnd => DateTime.Now >= _event.StartTime;

        public string Status => IsEnd ? "Завершено" : "Не завершено";

        public IventCardVM ( Models.Event newEevent, EventsVM eventsVM )
        {
            _event = newEevent;
            _eventsVM = eventsVM;

            GetFullInfoCommand = new Command (() =>
            {
                _eventsVM.FullCardInfoVM = new FullIventVM(_event, eventsVM);
                _eventsVM!.Users!.Clear();
                foreach (var item in _event.Orders.Select(o => o.User))
                {
                    _eventsVM!.Users!.Add(item!);
                }
            });
        }
    }


    internal sealed class EventsVM : ViewModel
    {
        private readonly Navigation _navigator;
        private bool _isDataLoaded;
        private readonly Models.Saller _saller;
        private dynamic _fullCardInfoVM;

        public ICommand SortByDateDes => new Command(() =>
        {
            Events = new(Events!.OrderByDescending(e => e._event.StartTime));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByDate => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e._event.StartTime));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByName => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e._event.Name));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByCount => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e._event.Count));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand SortByPrice => new Command(() =>
        {
            Events = new(Events!.OrderBy(e => e._event.Price));
            OnPropertyChanged(nameof(Events));
        });

        public ICommand Search => new Command(() =>
        {
            if (_searchText != string.Empty)
            {
                Events = new(Events!.Where(e => Regex.IsMatch(e.Title, $"^{Regex.Escape(_searchText)}", RegexOptions.IgnoreCase)));
            }
            else
            {
                Events = new(_events);
            }

                OnPropertyChanged(nameof(Events));
        });

        private ObservableCollection<IventCardVM> _events;


        public ObservableCollection<IventCardVM>? Events { get; private set; }

        public ObservableCollection<User>? Users { get;  set; }

        public ICommand NavigateToNewEventCommand { get; }

        public ICommand ChangeCommand { get; }

        public ICommand DeleteCommand { get; }

        private string _searchText;

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetValue(ref _searchText, value);
            }
        }

        public bool DataLoaded
        {
            get => _isDataLoaded;
            set => this.SetValue(ref _isDataLoaded, value);
        }

        public dynamic FullCardInfoVM
        {
            get => _fullCardInfoVM;
            set
            {
                this.SetValue(ref _fullCardInfoVM, value);
            }
        }



        public EventsVM ( object? param, Navigation navigator )
        {
            _searchText = string.Empty;
            _navigator = navigator;
            _param = param;
            _saller = (Models.Saller)param!;

            Users = [];

            _fullCardInfoVM = new Default();

            ChangeCommand = new Command(() =>
            {
                _navigator.NavigateTo<NewEventPage>(typeof(NewEventVM), new NewEventDto(_saller, null));
            });

            LoadData().ConfigureAwait(false);

            NavigateToNewEventCommand = new Command<FullIventVM>(item => 
            {
                var param = item.Event;
                _navigator.NavigateTo<NewEventPage>(typeof(NewEventVM), new NewEventDto(_saller, param));

            });

            DeleteCommand = new Command<FullIventVM>(item =>
            {
                DataLoaded = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    using AppContext appContext = new ();

                    var @event = appContext.Events.Find(item.Event.Id);

                    var category = appContext.Categories.Find(@event!.SubCategoryId);

                    category!.ElementsInCategory = category!.ElementsInCategory > 0 ? category!.ElementsInCategory - 1 : null;

                    appContext.Orders
                        .Where(o => o.EventId == @event!.Id)
                        .ExecuteUpdate(setter => 
                        setter.SetProperty(o => o.Status, Status.RejectByUser
                        ));

                    
                    @event!.IsDeleted = true;

                    if (!appContext.Orders.Where(o => o.EventId == @event!.Id).Any())
                        appContext.Events.Remove(@event);

                    appContext.SaveChanges();
                    _navigator.Reload();
                });
            });
        }

        private async Task LoadData ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext context = new();

                var loadedData = context.Events
                .Where(e => !e.IsDeleted && e.SallerId == _saller.Id)
                .Include(i => i.Emages)
                .Include(i => i.RootCategory)
                .Include(i => i.Orders)
                    .ThenInclude(o => o.User)
                .Include(i => i.SubCategory).OrderByDescending(i => i.StartTime).ToList();

                Events = [];

                loadedData.ForEach(i => Events.Add(new IventCardVM(i, this)));
                _events = new(Events);
            });

            OnPropertyChanged(nameof(Events));  
            DataLoaded = true;
        }
    }
}