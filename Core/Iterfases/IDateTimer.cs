using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TiketsApp.Core.Iterfases
{
    internal interface IDateTimer
    {
        protected static TimeSpan CalculateTimeUntilMidnight ()
        {
            var now = DateTime.Now;
            var midnight = now.Date.AddDays(1);
            return midnight - now;
        }

        protected void OnDayChanged ( object? sender, EventArgs e );
    }
}
