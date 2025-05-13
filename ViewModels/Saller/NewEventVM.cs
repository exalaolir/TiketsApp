using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text.Json;
using System.Windows.Input;
using System.Windows.Threading;
using TiketsApp.Core.Iterfases;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.Models.DTO;
using TiketsApp.res;
using TiketsApp.ViewModels.Admin;
using TiketsApp.ViewModels.Base;
using TiketsApp.Views.SallerViews;

namespace TiketsApp.ViewModels.Saller
{

    internal sealed class ImagesPriviewVm ( Image image ) : ViewModel
    {
        public Image Image => image;
    }


    internal sealed class ImagesVm : ViewModel
    {
        public ObservableCollection<Image> Images { get; }
        private Image? _image;
        private readonly NewEventVM _newEventVM;

        public int ImgCount => _newEventVM.ImagesCount;

        public Image? Image
        {
            get => _image;
            set
            {
                SetValue(ref _image, value);
                _newEventVM.ImagePriviewVM = new ImagesPriviewVm(value!);
            }
        }

        public ICommand DeleteCommand { get; }
        public ImagesVm ( ObservableCollection<Image> images, NewEventVM newEventVM )
        {
            Images = images;
            _newEventVM = newEventVM;
            DeleteCommand = new Command(() =>
            {
                if (Image != null)
                {
                    _newEventVM.Images!.Remove(Image);
                    if (_newEventVM.Images.Count != 3)
                        _newEventVM.ResetValidFields();

                    OnPropertyChanged(nameof(ImgCount));
                }
            });
        }
    }



    internal sealed partial class NewEventVM : ValidationViewModel, IDateTimer
    {
        private readonly Navigation _navigator;
        private Event? _event;
        private readonly Models.Saller _saller;


        private string _title;
        private string _description;
        private int _maxCount;
        private Category? _rootCategory;
        private Category? _subcategory;
        private bool _subcatigoryVisible;
        private int _rows;
        private int _seats;
        private string _adress;


        private DateTime _startDate;
        private DateTime _endDate;
        private decimal _price;

        public ObservableCollection<Category>? Categories { get; set; }
        public ObservableCollection<Category> Subcategories { get; set; }
        public ObservableCollection<Image>? Images { get; set; }


        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [MaxLength(32, ErrorMessage = Consts.EventLenghtMessage)]
        public string Title
        {
            get => _title;
            set
            {
                this.SetValue(ref _title, value);
                Validate(Title);
                _validFields[0] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [MaxLength(200, ErrorMessage = Consts.EventDescriptionMessage)]
        public string Description
        {
            get => _description;
            set
            {
                this.SetValue(ref _description, value);
                Validate(Description);
                _validFields[1] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required]
        public string Adress
        {
            get => _adress;
            set
            {
                SetValue(ref _adress, value);
                Validate(Adress);
                _validFields[5] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                SetValue(ref _price, value);
                _validFields[6] = true;
                this.OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        public Category? RootCategory
        {
            get => _rootCategory;
            set
            {
                if (value != null)
                {
                    this.SetValue(ref _rootCategory, value);


                    Validate(RootCategory!);
                    _validFields[2] = true;
                    OnPropertyChanged(nameof(IsButtonEnabled));

                    Subcategories = new(value?.ChildCategories ?? []);
                    OnPropertyChanged(nameof(Subcategories));
                    if (value != null) SubcatigoryVisible = true;
                }
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        public Category? Subcategory
        {
            get => _subcategory;
            set
            {
                this.SetValue(ref _subcategory, value);
                Validate(Subcategory!);
                _validFields[3] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [Range(1, int.MaxValue)]
        public int MaxCount
        {
            get => _maxCount;
            set
            {
                this.SetValue(ref _maxCount, value);
                Validate(MaxCount!);
                _validFields[4] = true;
                OnPropertyChanged(nameof(IsButtonEnabled));
            }
        }

        public bool SubcatigoryVisible
        {
            get => _subcatigoryVisible;
            set
            {
                this.SetValue(ref _subcatigoryVisible, value);
            }
        }

        [Required(ErrorMessage = Consts.ReqiredMessage)]
        [Range(1, int.MaxValue)]
        public int Rows
        {
            get => _rows;
            set
            {
                this.SetValue(ref _rows, value);
                MaxCount = Rows * Seats;
            }
        }

        [Required]
        [Range(1, int.MaxValue)]
        public int Seats
        {
            get => _seats;
            set
            {
                this.SetValue(ref _seats, value);
                MaxCount = Rows * Seats;
            }
        }



        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                this.SetValue(ref _startDate, value);
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                this.SetValue(ref _endDate, value);
            }
        }

        public NewEventVM ( object? param, Navigation navigator )
        {
            _param = param;
            _navigator = navigator;

            _timer = new DispatcherTimer
            {
                Interval = IDateTimer.CalculateTimeUntilMidnight()
            };
            _timer.Tick += OnDayChanged;
            _timer.Start();
            _canEditCount = true;
            _currentDate = DateTime.Today;
            CurrentEndDate = _currentDate;


            var dto = param as NewEventDto;
            _saller = dto!.Saller;
            _event = dto.Event;


            _btnText = _event == null ? "Создать" : "Изменить";

            Subcategories = [];

            _title = _event?.Name ?? string.Empty;
            _description = _event?.Description ?? string.Empty;
            _maxCount = _event?.MaxCount ?? 1;

            var seatsMap = string.IsNullOrEmpty(_event?.SeatMap)
                ? null
                : JsonSerializer.Deserialize<Models.SeatMap>(_event.SeatMap);

            _rows = seatsMap?.RowsCount ?? 1;
            _seats = seatsMap?.SeatsCount ?? 1;

            _startDate = _event?.StartTime ?? DateTime.Now;

            _adress = _event?.Adress ?? string.Empty;

            _priceStr = _event?.Price.ToString() ?? "";

            _endDate = _event?.EndTime ?? DateTime.Now.AddDays(1);

            Hours = InitTime(true);
            Minuts = InitTime(false);

            ImagePriviewVM = new Default();

            _startMinuts = _event?.StartTime.Hour ?? Minuts[0];
            _endMinuts = _event?.EndTime.Hour ?? Minuts[0];

            _startHours = _event?.StartTime.Minute ?? Hours[0];
            _endHours = _event?.StartTime.Minute ?? Hours[0];

            if (_event?.SeatMap != null) HasSeataMap = true;


            _validFields = new bool[8];
            if (_event?.MaxCount == null) _validFields[4] = true;
            if (_event != null) _validFields = _validFields.Select(x => x = true).ToArray();


            AddImageCommand = new Command(async () =>
            {
                DataLoaded = false;
                var path = await AddImage();
                if (path != null)
                    Images!.Add(new Image()
                    {
                        Path = path,
                        Name = Path.GetFileName(path),
                    });

                if (ImagesCount == 3)
                    _validFields[7] = true;
                else _validFields[7] = false;


                OnPropertyChanged(nameof(IsButtonEnabled));

                ImageVM = new ImagesVm(Images!, this);
                DataLoaded = true;
            });

            SaveCommand = new Command(Save);

            LoadData().ConfigureAwait(false);
        }

        public ICommand AddImageCommand { get; }
        public ICommand SaveCommand { get; }

        private async Task LoadData ()
        {
            DataLoaded = false;

            await Task.Run(() =>
            {
                using AppContext context = new();

                var loadedData = context.Categories
                    .Where(c => c.Parent == null && c.ChildCategories != null && c.ChildCategories.Count > 0
                    && !c.IsBlocked)
                    .Include(c => c.ChildCategories);

                if (_event != null)
                {
                    var loadedImages = context.Images.Where(i => i.Events.Contains(_event));
                    Images = new(loadedImages);
                    ImageVM = new ImagesVm(Images, this);
                }
                else
                {
                    Images = [];
                    ImageVM = new Admin.Default();
                }
                Categories = new(loadedData);
            });

            Images!.CollectionChanged += OnImagesCollectionChanged;

            OnPropertyChanged(nameof(Categories));
            OnPropertyChanged(nameof(Images));

            if(_event != null)
            {
                RootCategory = _event?.RootCategory;
                Subcategory = _event?.SubCategory;
            }
            DataLoaded = true;
        }

        private async void Save ()
        {
            DataLoaded = false;

            await Task.Run(async () =>
            {
                string adress;
                try
                {
                    adress = await AdressServis.GetCoordsByQuery(Adress);
                }
                catch (Exception ex)
                {
                    SetValidationResults(false, nameof(Adress), [ex.Message]);
                    return;
                }

                var startDate = StartDate.Date + new TimeSpan(StartHourse, StartMinuts, 0);
                var endDate = EndDate.Date + new TimeSpan(EndHourse, EndMinuts, 0);

                if (startDate >= endDate)
                {
                    SetValidationResults(false, nameof(Title), ["Конечная дата не может быть больше начальной"]);
                    return;
                }
                else SetValidationResults(true, nameof(Title), [""]);

                using AppContext appContext = new();

                var newEvent = new Event()
                {
                    Name = Title,
                    Description = Description,
                    RootCategory = appContext.Categories.Find(RootCategory!.Id),
                    SubCategory = appContext.Categories.Find(Subcategory!.Id),
                    MaxCount = MaxCount,
                    Saller = appContext.Sallers.Find(_saller.Id),
                    Emages = Images!.Select(i => new Image()
                    {
                        Name = i.Name,
                        Path = i.Path,
                    }).ToList(),
                    SeatMap = HasSeataMap ? JsonSerializer.Serialize<SeatMap>(new SeatMap(Rows, Seats)) : null,
                    Price = Price,
                    StartTime = startDate,
                    EndTime = endDate,
                    Adress = adress
                };

                var sameField = appContext.Events
                    .Where(e =>
                        (e.Saller == _saller && e.Name == Title) ||
                        e.Adress == adress)
                    .ToList();

                if (_event != null)
                    sameField.RemoveAll(e => e.Id == _event.Id);

                if (sameField.Any())
                {
                    foreach (var conflictingEvent in sameField)
                    {
                        if (conflictingEvent?.Adress == adress && conflictingEvent.EndTime >= newEvent.StartTime)
                        {
                            SetValidationResults(false, nameof(Adress), ["Место уже занято"]);
                            return;
                        }
                        else
                        {
                            SetValidationResults(true, nameof(Adress), [""]);
                        }
                        if (conflictingEvent?.Saller!.Id == _saller.Id && conflictingEvent.Name == Title)
                        {
                            SetValidationResults(false, nameof(Title), ["Название уже занято"]);
                            return;
                        }
                        else SetValidationResults(true, nameof(Title), [""]);

                    }
                }

                if (_event == null)
                {
                    appContext.Events.Add(newEvent);
                    appContext.SaveChanges();
                }
                else
                {
                    var oldEvent = appContext.Events.Find(_event.Id)!;

                    foreach (var item in appContext.Images.Include(i => i.Events))
                    {
                        item.Events.RemoveAll(e => e.Id == _event.Id);
                    }

                    oldEvent.Name = Title;
                    oldEvent.Description = Description;
                    oldEvent.RootCategory = appContext.Categories.Find(RootCategory!.Id);
                    oldEvent.SubCategory = appContext.Categories.Find(Subcategory!.Id);
                    oldEvent.MaxCount = MaxCount;
                    oldEvent.Saller = appContext.Sallers.Find(_saller.Id);
                    oldEvent.Emages = Images!.Select(i => new Image()
                    {
                        Name = i.Name,
                        Path = i.Path,
                    }).ToList();
                    oldEvent.SeatMap = HasSeataMap ? JsonSerializer.Serialize<SeatMap>(new SeatMap(Rows, Seats)) : null;
                    oldEvent.Price = Price;
                    oldEvent.StartTime = startDate;
                    oldEvent.EndTime = endDate;
                    oldEvent.Adress = adress;

                    appContext.SaveChanges();
                }
            });

            if (!HasErrors) _navigator.NavigateTo<EventsPage>(typeof(EventsVM), _saller);
            DataLoaded = true;
        }
    }
}
