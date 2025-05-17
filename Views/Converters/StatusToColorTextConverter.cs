using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using TiketsApp.Models;

namespace TiketsApp.Views.Converters
{
    internal class StatusToColorTextConverter : IValueConverter
    {
        public object Convert ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if (value is not Status status)
                return GetStatusData("Неизвестно", "SystemFillColorCriticalBrush", parameter);

            return status switch
            {
                Status.Whait => GetStatusData("Ожидание", "SystemFillColorNeutralBrush", parameter),
                Status.Changed => GetStatusData("Изменён", "SystemFillColorCautionBrush", parameter),
                Status.Banned => GetStatusData("Заблокирован", "SystemFillColorCriticalBrush", parameter),
                Status.Succes => GetStatusData("Бронь закончилась", "SystemFillColorSuccessBrush", parameter),
                Status.RejectByUser => GetStatusData("Удалён", "AccentTextFillColorSecondaryBrush", parameter),
                _ => GetStatusData("Неизвестно", "SystemFillColorCriticalBrush", parameter)
            };
        }

        private object GetStatusData ( string text, string resourceName, object parameter )
        {
            // Если параметр равен "Text" - возвращаем только текст
            if (parameter is string strParam && strParam == "Text")
                return text;

            // Если параметр равен "Brush" - возвращаем только кисть
            if (parameter is string strParam2 && strParam2 == "Brush")
                return Application.Current.TryFindResource(resourceName) ?? Brushes.Black;

            // По умолчанию возвращаем кортеж
            return new Tuple<string, Brush>(text,
                (Brush)(Application.Current.TryFindResource(resourceName) ?? Brushes.Black));
        }


        public object ConvertBack ( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
