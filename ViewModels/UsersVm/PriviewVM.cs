using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiketsApp.Core.Servises;
using TiketsApp.Models;
using TiketsApp.Models.DTO;
using TiketsApp.ViewModels.Base;

namespace TiketsApp.ViewModels.UsersVm
{
    internal sealed class PriviewVM : ViewModel
    {
        private readonly Navigation _navigator;
        private readonly User? _user;
        private bool _dataLoaded;
        private Event _event;

        public bool DataLoaded
        {
            get => _dataLoaded;
            set
            {
                SetValue(ref _dataLoaded, value);
            }
        }

        public Event Event => _event;

        public PriviewVM ( object? param, Navigation navigator )
        {
            _navigator = navigator;
            _param = param;
            var dto = param as PriviewEventDto;

            _user = dto!.User;
            _event = dto!.Event;

            DataLoaded = true;
        }
    }
}
