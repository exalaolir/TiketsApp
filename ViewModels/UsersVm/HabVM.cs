using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.ViewModels.Base;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Diagnostics;
using TiketsApp.ViewModels.Admin;
using System.Windows.Input;
using TiketsApp.Models.DTO;
using TiketsApp.Views.UserViews;
using System.Windows;
using System.Text.Json;

namespace TiketsApp.ViewModels.UsersVm
{
    internal sealed partial class HabVM : ValidationViewModel, IPassword, IRepeatPassword, IDateTimer
    {
        private readonly Navigation? _navigator;
        private User _user;

        public ObservableCollection<Order> Orders { get; private set; }

        private ObservableCollection<Order> _storage;

        public Dictionary<string, decimal> Categories { get; private set; }

        public ISeries[] Series { get; set; }
        public Axis[] XAxes { get; set; }
        public Axis[] YAxes { get; set; }


        private PeriodicTimer _timerOfUpdate;
        private CancellationTokenSource _cts = new();

        public async Task RunTimerAsync ()
        {
            await Task.Delay((60 - DateTime.Now.Second) * 1000);

            _timerOfUpdate = new PeriodicTimer(TimeSpan.FromSeconds(60));

            while (await _timerOfUpdate.WaitForNextTickAsync(_cts.Token))
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    int oldCount = Orders.Count;

                    var ordersLookup = Orders.ToLookup(o => o.Event!.StartTime > DateTime.Now);
                    var futureOrders = ordersLookup[true].ToList();
                    var pastOrders = ordersLookup[false].ToList();

                    if (oldCount != futureOrders.Count())
                    {
                        using AppContext appContext = new();

                        appContext.Orders
                           .Where(o => o.Event!.StartTime <= DateTime.Now)
                           .ExecuteUpdate(setters => setters
                                   .SetProperty(o => o.Status, Status.Succes)
                           );

                        OnPropertyChanged(nameof(History));

                        Orders = new(futureOrders);

                        OnPropertyChanged(nameof(Orders));

                        appContext.SaveChanges();
                    }
                });
            }
        }

        private bool _isMenuEnable;

        public bool IsMenuEnable
        {
            get => _isMenuEnable;
            set
            {
                SetValue(ref _isMenuEnable, value);
            }
        }

        private Order? _currentEvent;

        public Order? CurrentEvent
        {
            get => _currentEvent;
            set
            {
                SetValue(ref _currentEvent, value);
                IsMenuEnable = true;

                DeleteEnable = value!.Event!.IsDeleted || value.Status == Status.Changed;

                if (value!.Event!.IsDeleted) DeleteAccept = new Command(DeleteAcception);

                if (value.Status == Status.Changed) DeleteAccept = new Command(ChangeAcception);

                OnPropertyChanged(nameof(DeleteAccept));
            }
        }

        public ICommand WatcCommand { get; }
        public ICommand RejectCommand { get; }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set
            {
                SetValue(ref _dataLoaded, value);
            }
        }

        private bool _deleteEnable;

        public bool DeleteEnable
        {
            get => _deleteEnable;
            set
            {
                SetValue(ref _deleteEnable, value);
            }
        }

        private int _tabIndex;

        public int TabIndex
        {
            get => _tabIndex;
            set
            {
                SetValue(ref _tabIndex, value);
                if (value == 1) OnPropertyChanged(nameof(History));
            }
        }

        public ICommand? DeleteAccept { get; private set; }

        public ObservableCollection<Order> History
        {
            get
            {
                var data = _storage?.Where(o => o.Event!.StartTime <= DateTime.Now).ToList();

                data?.ForEach(d => d.Status = Status.Succes);

                return new( data ?? []);
            }
        }

        public HabVM ( object? param, Navigation navigator ) : this((User)param!)
        {
            _navigator = navigator;
            _param = param;

            RunTimerAsync().ConfigureAwait(false);

            WatcCommand = new Command(() =>
            {
                DataLoaded = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    using AppContext appContext = new AppContext();
                    var @event = appContext.Events.Where(e => e.Id == CurrentEvent!.EventId).Include(e => e.Emages)
                        .Include(e => e.Saller)
                        .Include(e => e.Orders).First();

                    var dto = new PriviewEventDto(_user, @event!, CurrentEvent);

                    _navigator!.NavigateTo<PriviewPage>(typeof(PriviewVM), dto);
                });
            });

            RejectCommand = new Command(() =>
            {
                DataLoaded = false;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    using AppContext appContext = new AppContext();
                    var order = appContext.Orders.Find(CurrentEvent!.Id);
                    var @event = appContext.Events.Find(CurrentEvent.EventId);

                    //order!.Status = Status.RejectByUser;

                    if (@event!.SeatMap != null)
                    {
                        var seatMap = JsonSerializer.Deserialize<SeatMap>(@event.SeatMap);

                        seatMap![(int)order!.Row!][(int)order.Seat!].IsOwned = false;

                        @event.SeatMap = JsonSerializer.Serialize(seatMap);
                    }

                    @event.Count -= 1;

                    appContext.Orders.Remove(order!);

                    appContext.SaveChanges();

                    _navigator.Reload(_user);
                });
            });

            LoadData().ConfigureAwait(false);
        }

        private void DeleteAcception ()
        {
            DataLoaded = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                using AppContext context = new();

                var @event = context.Events.Find(CurrentEvent!.EventId);

                var orders = context.Orders.Where(o => o.EventId == CurrentEvent!.EventId);


                if(orders.Count() > 1)
                {
                    context.Orders.Remove(orders.First(o => o.EventId == CurrentEvent!.EventId));
                }
                else context.Events.Remove(@event!);
                context.SaveChanges();
                _navigator!.Reload();
            });
        }

        private void ChangeAcception ()
        {
            DataLoaded = false;
            Application.Current.Dispatcher.Invoke(() =>
            {

                using AppContext appContext = new();

                var order = appContext.Orders.Find(CurrentEvent!.Id);
                order!.Status = Status.Whait;
                appContext.SaveChanges();
                _navigator?.Reload();
            });
        }

        public async Task LoadData ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext appContext = new();

                var loadedOrders = appContext.Orders
                .Include(o => o.User)
                .Include(o => o.Event)
                    .ThenInclude(e => e.SubCategory)
                .Include(o => o.Saller);

                _storage = new(loadedOrders);

                Orders = new(_storage.Where(o => o.UserId == _user.Id && o.Event!.StartTime > DateTime.Now));

                Categories = Orders
                    .Where(o => o.Event!.SubCategory != null)
                    .GroupBy(o => o.Event!.SubCategory!.Name)
                    .ToDictionary(
                        o => o.Key,
                        o => o.Sum(e => e.Event!.Price)
                    );

                Categories.Add("", 0);
                Series = new ISeries[]
                {
                        new ColumnSeries<double>
                        {
                            Name = "Цены",
                            Values = Categories.Values.Select(v => (double)v).ToList(),
                            Fill = new SolidColorPaint(SKColors.PaleVioletRed),
                        }
                };

                XAxes = new Axis[]
                {
                    new Axis
                    {
                        Labels = Categories.Keys.ToList(),
                        TextSize = 14
                    }
                };

                YAxes = new Axis[]
                {
                    new Axis
                    {
                        Name = "Цена (Br)",
                        TextSize = 14,
                        NameTextSize = 16
                    }
                };

            });

            OnPropertyChanged(nameof(Orders));
            OnPropertyChanged(nameof(Series));
            OnPropertyChanged(nameof(XAxes));
            OnPropertyChanged(nameof(YAxes));
            DataLoaded = true;
        }

        ~HabVM ()
        {
            Debug.WriteLine($"Уничтожена страница: {GetType().Name}");
        }
    }
}
