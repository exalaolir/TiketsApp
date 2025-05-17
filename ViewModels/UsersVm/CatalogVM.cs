using Microsoft.EntityFrameworkCore;
using Panuon.WPF.UI;
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
                });
            });

            OnPropertyChanged(nameof(Events));
            DataLoaded = true;
        }
    }
}
