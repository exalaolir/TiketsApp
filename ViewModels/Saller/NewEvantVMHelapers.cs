using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Threading;
using TiketsApp.Core.Iterfases;
using TiketsApp.ViewModels.Base;

namespace TiketsApp.ViewModels.Saller
{
    internal sealed partial class NewEventVM : ValidationViewModel, IDateTimer
    {
        private bool _isDataLoaded;

        private DateTime _currentDate;
        private DateTime _currentEndDate;
        private readonly DispatcherTimer _timer;
        private bool _hasSeataMap;
        private bool _canEditCount;
        private int _startHours;
        private int _startMinuts;
        private dynamic? _imageVM;
        private bool _addImagesEnable = true;
        private dynamic? _imagePreviewVM;
        private string _btnText;

        private int _endHours;
        private int _endMinuts;

        public List<int> Hours { get; }
        public List<int> Minuts { get; }

        public bool AddImagesEnable
        {
            get => !(Images?.Count >= 3);
        }
        public int ImagesCount => Images?.Count ?? 0;

        public void ResetValidFields ()
        {
            _validFields[7] = false;
            OnPropertyChanged(nameof(IsButtonEnabled));
        }
        public string BtnText
        {
            get => _btnText;
            set
            {
                SetValue(ref _btnText, value);
            }
        }

        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                this.SetValue(ref _currentDate, value);
                CurrentEndDate = value.AddDays(1);
            }
        }

        public DateTime CurrentEndDate
        {
            get => _currentEndDate;
            set => this.SetValue(ref _currentEndDate, value);
        }

        public dynamic? ImageVM
        {
            get => _imageVM;
            set => this.SetValue(ref _imageVM, value);
        }

        public dynamic? ImagePriviewVM
        {
            get => _imagePreviewVM;
            set => this.SetValue(ref _imagePreviewVM, value);
        }

        public bool DataLoaded
        {
            get => _isDataLoaded;
            set => this.SetValue(ref _isDataLoaded, value);
        }

        public bool HasSeataMap
        {
            get => _hasSeataMap;
            set
            {
                this.SetValue(ref _hasSeataMap, value);
                CanEditCount = !value;
            }
        }

        public bool CanEditCount
        {
            get => _canEditCount;
            set
            {
                this.SetValue(ref _canEditCount, value);
            }
        }

        public void OnDayChanged ( object? sender, EventArgs e )
        {
            CurrentDate = DateTime.Today;
            _timer.Interval = IDateTimer.CalculateTimeUntilMidnight();
        }

        public int EndHourse
        {
            get => _endHours;
            set => this.SetValue(ref _endHours, value);
        }

        public int EndMinuts
        {
            get => _endMinuts;
            set => this.SetValue(ref _endMinuts, value);
        }


        public int StartHourse
        {
            get => _startHours;
            set => this.SetValue(ref _startHours, value);
        }

        public int StartMinuts
        {
            get => _startMinuts;
            set => this.SetValue(ref _startMinuts, value);
        }

        private static List<int> InitTime (bool day )
        {

            var initList = new List<int>(day ? 24 : 60);
            int limit = day ? 24 : 59;

            for (int counter = 0; counter <= limit; counter++)
            {
                initList.Add(counter);
            }

            return initList;
        }

        private string? _priceStr;
        
        public string PriceStr
        {
            get => _priceStr ?? "";
            set
            {
                if (value.Contains('.')) value = value.Replace('.', ',');
                string firstPart = "";
                if (value.Length >= 2) firstPart = value.Substring(0, 2);
                if (decimal.TryParse(value, out var result) && !Regex.IsMatch(firstPart, @"^0\d$"))
                {
                    (bool, string, IReadOnlyList<string>) res = result < (decimal)0.1 || result > (decimal)1000000.0 ? (false, nameof(PriceStr), ["Неверный диапазон цены"]) : (true, nameof(PriceStr), [""]);

                    if (value.Contains(',') && value.IndexOf(',') != value.Length - 3)
                        res = (false, nameof(PriceStr), ["Нужно 2 знака после запятой"]);

                    SetValidationResults(res.Item1, res.Item2, res.Item3);

                    Price = result;
                }
                else SetValidationResults(false, nameof(PriceStr), ["Неверный формат"]);
                _priceStr = value;
                OnPropertyChanged(nameof(PriceStr));
            }
        }

        private async Task<string?> AddImage ()
        {
            return await Task.Run(() =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Images (*.jpg, *.png)|*.jpg;*.png";
                dialog.AddExtension = true;

                return dialog.ShowDialog() == true ? dialog.FileName : null;
            });
        }

        private void OnImagesCollectionChanged ( object? sender, NotifyCollectionChangedEventArgs? e )
        {
            OnPropertyChanged(nameof(AddImagesEnable));
            OnPropertyChanged(nameof(ImagesCount));
        }
    }
}
