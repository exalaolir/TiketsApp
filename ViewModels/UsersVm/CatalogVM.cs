using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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

        public Event Event => _newEvent;

        public string Path => _newEvent.Emages[0].Path;

        public string Title => _newEvent.Name;

        public string Start => _newEvent.StartTime.ToString();

        public string Desciption => _newEvent.Description;

        public string Price => string.Format("{0:C2}", _newEvent.Price);

        public ICommand WatchPriview { get; }

        public EventCardVM ( Event newEvent, CatalogVM catalogVM )
        {
            _catalogVM = catalogVM;
            _newEvent = newEvent;

            WatchPriview = _catalogVM.WatchPriviewCommand;
        }
    }

    internal sealed class CatalogVM : ViewModel
    {
        private readonly Navigation _navigator;
        private readonly User? _user;
        private bool _dataLoaded;

        public ObservableCollection<EventCardVM>? Events { get; private set; }

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
                var dto = new PriviewEventDto(_user!, card.Event);

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

                var data = appContext.Events.Where(e => DateTime.Now < e.StartTime).Include(e => e.Emages).Include(e => e.Saller).ToList();

                Application.Current.Dispatcher.Invoke(() =>
                {
                    data.ForEach(e => Events.Add(new EventCardVM(e, this)));
                });
            });

            OnPropertyChanged(nameof(Events));
            DataLoaded = true;
        }
    }
}
