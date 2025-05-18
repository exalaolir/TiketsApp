using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    internal sealed class PriviewVM : ValidationViewModel
    {
        private readonly Navigation _navigator;
        private readonly User? _user;
        private bool _dataLoaded;
        private Event _event;
        private bool _seatMapVisible;
        private bool _seatVisible;
        private SeatMap? _seatMap;
        private int _row;
        private int? _seat;
        private Order? _order;
        private string _btnText;

        public int Count => _event.MaxCount - _event.Count;

        public string Price => string.Format("{0:C2}", _event.Price);

        public string SallerId => _event.Saller!.Number!.ToString();

        public List<int>? Rows { get; }
        public List<int>? Seats { get; private set; }

        public bool SeatMapVisible
        {
            get => _seatMapVisible;
            set
            {
                SetValue(ref _seatMapVisible, value);
            }
        }

        public bool SeatVisible
        {
            get => _seatVisible;
            set
            {
                SetValue(ref _seatVisible, value);
            }
        }

        public bool DataLoaded
        {
            get => _dataLoaded;
            set
            {
                SetValue(ref _dataLoaded, value);
            }
        }

        public int? Seat
        {
            get => _seat;
            set
            {
                SetValue(ref _seat, value);
                if(value != null) _validFields[1] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        private bool _deleteVisible;

        public bool DeleteVisible
        {
            get => _deleteVisible;
            set
            {
                SetValue(ref _deleteVisible, value);
            }
        }

        private bool _changeVisible;

        public bool ChangeVisible
        {
            get => _changeVisible;
            set
            {
                SetValue(ref _changeVisible, value);
            }
        }

        public int Row
        {
            get => _row;
            set
            {
                SetValue(ref _row, value);
                _validFields[0] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
                Seats = new(_seatMap!.SeatsCount);

                _seatMap![_row].ForEach(seat =>
                {
                    if (!seat.IsOwned)
                        Seats.Add(seat.Number);
                });

                if (_order != null && _row == (int)_order.Row!)
                {
                    Seats.Add(_seatMap[(int)_order.Row!][(int)_order.Seat!].Number);
                    Seats.Sort();
                }
                SeatVisible = true;
                Seat = null;
                OnPropertyChanged(nameof(Seats));
            }
        }

        public string BtnText => _btnText;

        public Event Event => _event;

        public ICommand BuyCommand { get; }
        public Command RejectCommand { get; }


        private string _deleteBtnText;

        public string DeleteBtnText
        {
            get => _deleteBtnText;
            set
            {
                SetValue(ref _deleteBtnText, value);
            }
        }

        public PriviewVM ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            _param = param;
            var dto = param as PriviewEventDto;

            _user = dto!.User;
            _event = dto!.Event;
            _order = dto!.Order;

            _seatMapVisible = !(_event.SeatMap == null);

            _validFields = new bool[2];


            if (_seatMapVisible)
            {
                _seatMap = JsonSerializer.Deserialize<SeatMap>(_event.SeatMap!);
                Rows = new(_seatMap!.RowsCount);
                _seatMap.ForEach(row =>
                {
                    if (row.HasEmptySeat()) Rows.Add(row.Number);
                });

                if(_order != null)
                {
                    Row = (int)_order.Row!;   
                    Seat = (int)_order.Seat!;
                    OnPropertyChanged(nameof(Seats));
                }
            }
            else
            {
                _validFields = _validFields.Select(f => f = true).ToArray();
                OnPropertyChanged(nameof(IsButtonEnabled));
            }

            if (_order == null && _event.Count == _event.MaxCount)
            {
                _validFields = new bool[3];
                SeatMapVisible = false;
                SeatVisible = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }

            _btnText = _order == null ? "Забронировать" : "Изменить";

            ChangeVisible = true;

            if (_order != null && !_seatMapVisible) ChangeVisible = false;

            if (_order != null && _order.Status == Status.Changed)
            {
                _btnText = "Принять изменения";
                ChangeVisible = true;
                OnPropertyChanged(nameof(BtnText));
                BuyCommand = new Command(() =>
                {
                    DataLoaded = false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using AppContext appContext = new ();

                        var order = appContext.Orders.Find(_order.Id);
                        order!.Status = Status.Whait;
                        appContext.SaveChanges();
                        Buy();
                    });
                });
            }
            else BuyCommand = new Command(Buy);

            if (_order != null && _order.Status == Status.RejectByUser)
            {
                _deleteBtnText = "Принять отмену бронирования";
                _validFields = new bool[3];
                SeatMapVisible = false;
                SeatVisible = false;
                OnPropertyChanged(nameof(IsButtonEnabled));
                RejectCommand = new Command(() =>
                {
                    DataLoaded = false;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        using AppContext context = new();
                        var @event = context.Events.Find(_event.Id);

                        context.Events.Remove(@event!);
                        context.SaveChanges();
                        _navigator.NavigateTo<HabPage>(typeof(HabVM), _user);
                    });
                });
            }
            else
            {
                _deleteBtnText = "Отменить";
                RejectCommand = new Command(Reject);
            }

            DeleteVisible = _order != null;


            DataLoaded = true;
        }


        private void Reject ()
        {
            DataLoaded = false;
            Application.Current.Dispatcher.Invoke(() =>
            {
                using AppContext appContext = new AppContext();
                var order = appContext.Orders.Find(_order!.Id);
                var @event = appContext.Events.Find(_event!.Id);

                if (@event!.StartTime <= DateTime.Now)
                {
                    Array.Fill(_validFields, false);
                    SeatMapVisible = false;
                    SeatVisible = false;
                    OnPropertyChanged(nameof(IsButtonEnabled));
                }

                if (@event!.SeatMap != null)
                {
                    var seatMap = JsonSerializer.Deserialize<SeatMap>(@event.SeatMap);

                    seatMap![(int)order!.Row!][(int)order.Seat!].IsOwned = false;

                    @event.SeatMap = JsonSerializer.Serialize(seatMap);
                }

                @event.Count -= 1;

                appContext.Orders.Remove(order!);

                appContext.SaveChanges();

                _navigator.NavigateTo<HabPage>(typeof(HabVM), _user);
            });
        }

        private async void Buy ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext context = new();

                var @event = context.Events.Find(_event!.Id);

                if(@event!.StartTime <= DateTime.Now)
                {
                    Array.Fill(_validFields, false);
                    SeatMapVisible = false;
                    SeatVisible = false;
                    OnPropertyChanged(nameof(IsButtonEnabled));
                }

                if (_seatMapVisible)
                {
                    _seatMap![Row][(int)Seat!].IsOwned = true;
                    if(_order != null)
                    {
                        if (_order.Row != Row || _order.Seat != Seat)
                            _seatMap![(int)_order.Row!][(int)_order.Seat!].IsOwned = false;
                    }
                    var newSeatMap = JsonSerializer.Serialize<SeatMap>(_seatMap!);
                    @event!.SeatMap = newSeatMap;
                }

                var order = new Order()
                {
                    User = context.Users.Find(_user!.Id),
                    Event = @event,
                    Saller = context.Sallers.Find(_event.Saller!.Id),
                    Row = _seatMapVisible ? Row : null,
                    Seat = _seatMapVisible ? Seat : null,
                    Cost = _event.Price,
                    Status = Status.Whait,
                };

                if(_order != null)
                {
                    var newOrder = context.Orders.Find(_order.Id);
                    newOrder!.Row = Row;
                    newOrder!.Seat = Seat;
                }
                else
                {
                    context.Orders.Add(order);
                    @event!.Count += 1;
                }

                context.SaveChanges();

                Application.Current.Dispatcher.Invoke(() => _navigator.NavigateTo<HabPage>(typeof(HabVM), _user));
            });
        }
    }
}
