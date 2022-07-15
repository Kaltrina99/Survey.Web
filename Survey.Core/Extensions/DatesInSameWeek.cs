using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Core.Extensions
{
    public static class DatesInSameWeekExtension
    {
        public static bool DatesInSameWeek(this DateTime date1, DateTime date2, DayOfWeek weekStartsOn)
        { 
        var first = date1.AddDays(-GetOffsetedDayofWeek(date1.DayOfWeek, (int)weekStartsOn));
        var second = date2.AddDays(-GetOffsetedDayofWeek(date2.DayOfWeek, (int)weekStartsOn));

            return first.ToLongDateString() == second.ToLongDateString();
        }
        private static int GetOffsetedDayofWeek(DayOfWeek dayOfWeek, int offsetBy)
        {
            return (((int)dayOfWeek - offsetBy + 7) % 7);
        }
    }
}
