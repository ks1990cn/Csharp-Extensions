using System;
using System.Collections.Generic;
using System.Text;

namespace Extensions
{
    public static class DateExtensions
    {
        /// <summary>
        /// Adds a day in the client time zone to the input UTC time.
        /// </summary>
        /// <param name="time">The time to add days to, in UTC.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the day add operation.</param>
        /// <param name="numDays">The number of days to add.</param>
        public static DateTime AddClientDays(this DateTime time, TimeZoneInfo clientTimeZone, double numDays)
        {
            var clientOldTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(time, DateTimeKind.Utc), clientTimeZone);
            var clientNewTime = clientOldTime.AddDays(numDays);
            var utcNewTime = TimeZoneInfo.ConvertTimeToUtc(clientNewTime, clientTimeZone);

            return utcNewTime;
        }

        /// <summary>
        /// Adds a month in the client time zone to the input UTC time.
        /// </summary>
        /// <param name="time">The time to add months to, in UTC.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the month add operation.</param>
        /// <param name="numMonths">The number of months to add.</param>
        public static DateTime AddClientMonths(this DateTime time, TimeZoneInfo clientTimeZone, int numMonths)
        {
            var clientOldTime = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone);
            var clientNewTime = clientOldTime.AddMonths(numMonths);
            var utcNewTime = TimeZoneInfo.ConvertTimeToUtc(clientNewTime, clientTimeZone);

            return utcNewTime;
        }

        /// <summary>
        /// Adds a year in the client time zone to the input UTC time.
        /// </summary>
        /// <param name="time">The time to add years to, in UTC.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the year add operation.</param>
        /// <param name="numYears">The number of years to add.</param>
        public static DateTime AddClientYears(this DateTime time, TimeZoneInfo clientTimeZone, int numYears)
        {
            var clientOldTime = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone);
            var clientNewTime = clientOldTime.AddYears(numYears);
            var utcNewTime = TimeZoneInfo.ConvertTimeToUtc(clientNewTime, clientTimeZone);

            return utcNewTime;
        }

        /// <summary>
        /// Given a time in UTC, returns a UTC time representing the start of the day in the given client time zone.
        /// </summary>
        /// <param name="time">The time to find the day start for.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the day start.</param>
        /// <returns></returns>
        public static DateTime ClientDayStart(this DateTime time, TimeZoneInfo clientTimeZone)
        {
            var clientStart = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone).Date;

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(clientStart, clientTimeZone);

            return utcStart;
        }

        /// <summary>
        /// Given a time in UTC, returns a UTC time representing the end of the day (the start of the next day) in the given client time zone.
        /// </summary>
        /// <param name="time">The time to find the day end for.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the day end.</param>
        /// <returns></returns>
        public static DateTime ClientDayEnd(this DateTime time, TimeZoneInfo clientTimeZone)
        {
            var clientStart = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone).Date;

            var clientEnd = clientStart.AddDays(1);

            var utcEnd = TimeZoneInfo.ConvertTimeToUtc(clientEnd, clientTimeZone);

            return utcEnd;
        }

        /// <summary>
        /// Given a time in UTC, returns a UTC time representing the start of the month in the given client time zone.
        /// </summary>
        /// <param name="time">The time to find the month start for.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the month start.</param>
        /// <returns></returns>
        public static DateTime ClientMonthStart(this DateTime time, TimeZoneInfo clientTimeZone)
        {
            var clientTime = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone);

            var clientStart = new DateTime(clientTime.Year, clientTime.Month, 1);

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(clientStart, clientTimeZone);

            return utcStart;
        }

        /// <summary>
        /// Given a time in UTC, returns a UTC time representing the start of the year in the given client time zone.
        /// </summary>
        /// <param name="time">The time to find the year start for.</param>
        /// <param name="clientTimeZone">The time zone that will be used to define the year start.</param>
        /// <returns></returns>
        public static DateTime ClientYearStart(this DateTime time, TimeZoneInfo clientTimeZone)
        {
            var clientTime = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone);

            var clientStart = new DateTime(clientTime.Year, 1, 1);

            var utcStart = TimeZoneInfo.ConvertTimeToUtc(clientStart, clientTimeZone);

            return utcStart;
        }

        /// <summary>
        /// Round a datetime to the closest second
        /// </summary>
        /// <param name="date">the datetime to round</param>
        /// <returns></returns>
        public static DateTime RoundToClosestSecond(this DateTime date)
        {
            var remainingTicks = date.Ticks % TimeSpan.TicksPerSecond;
            if (remainingTicks < TimeSpan.TicksPerSecond / 2)
                return new DateTime(date.Ticks - remainingTicks, date.Kind);
            else
                return new DateTime(date.Ticks - remainingTicks + TimeSpan.TicksPerSecond, date.Kind);
        }

        /// <summary>
        /// Round datetime to closest second if it's only differenceInTicks away from a full second
        /// </summary>
        /// <param name="date">the datetime to round</param>
        /// <param name="differenceInTicks">Should be smaller than half of one second ticks</param>
        /// <returns></returns>
        public static DateTime RoundToSecondIfTicksAway(this DateTime date, long differenceInTicks)
        {
            long ticks = date.Ticks;
            long oneSecondTicks = 10000000;
            long remainder = ticks % oneSecondTicks;
            if ((remainder > 0) && (remainder < differenceInTicks))
            {
                ticks -= remainder;
                return new DateTime(ticks);

            }
            if ((remainder > oneSecondTicks - differenceInTicks) && (remainder < oneSecondTicks))
            {
                ticks += (oneSecondTicks - remainder);
                return new DateTime(ticks);
            }

            return date;
        }
        /// <summary>
        /// Returns a datetime to the closest minute
        /// </summary>
        /// <param name="date">the datetime to round</param>
        /// <returns></returns>
        public static DateTime RoundToClosestMinute(this DateTime date)
        {
            var remainingTicks = date.Ticks % TimeSpan.TicksPerMinute;
            if (remainingTicks < TimeSpan.TicksPerMinute / 2)
                return new DateTime(date.Ticks - remainingTicks, date.Kind);
            else
                return new DateTime(date.Ticks - remainingTicks + TimeSpan.TicksPerMinute, date.Kind);
        }
        public static bool NearEqual(this DateTime self, DateTime otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            var diff = Math.Abs(self.Ticks - otherDate.Ticks);
            return diff < tickEpsilon;
        }

        public static bool LessThan(this DateTime self, DateTime otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self.NearEqual(otherDate, tickEpsilon)) return false;

            return self < otherDate;
        }

        public static bool LessThanOrNearEqual(this DateTime self, DateTime otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self.NearEqual(otherDate, tickEpsilon)) return true;

            return self < otherDate;
        }

        public static bool GreaterThan(this DateTime self, DateTime otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self.NearEqual(otherDate, tickEpsilon)) return false;

            return self > otherDate;
        }

        public static bool GreaterThanOrNearEqual(this DateTime self, DateTime otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self.NearEqual(otherDate, tickEpsilon)) return true;

            return self > otherDate;
        }

        public static bool NearEqual(this DateTime? self, DateTime? otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self == null && otherDate == null) return true;

            return self.GetValueOrDefault().NearEqual(otherDate.GetValueOrDefault(), tickEpsilon);
        }

        public static bool LessThan(this DateTime? self, DateTime? otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self == null && otherDate == null) return false;

            return self.GetValueOrDefault().LessThan(otherDate.GetValueOrDefault(), tickEpsilon);
        }

        public static bool LessThanOrNearEqual(this DateTime? self, DateTime? otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self == null && otherDate == null) return true;

            return self.GetValueOrDefault().LessThanOrNearEqual(otherDate.GetValueOrDefault(), tickEpsilon);
        }

        public static bool GreaterThan(this DateTime? self, DateTime? otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self == null && otherDate == null) return false;

            return self.GetValueOrDefault().GreaterThan(otherDate.GetValueOrDefault(), tickEpsilon);
        }

        public static bool GreaterThanOrNearEqual(this DateTime? self, DateTime? otherDate, long tickEpsilon = TimeSpan.TicksPerSecond)
        {
            if (self == null && otherDate == null) return true;

            return self.GetValueOrDefault().GreaterThanOrNearEqual(otherDate.GetValueOrDefault(), tickEpsilon);
        }

        public static DateTime Max(this DateTime timeA, DateTime timeB)
        {
            return timeA > timeB ? timeA : timeB;
        }

        public static DateTime Min(this DateTime timeA, DateTime timeB)
        {
            return timeA > timeB ? timeB : timeA;
        }

        /// <summary>
        /// Converts the DateTime to the client time zone specified.
        /// The DateTime given should be in UTC.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <param name="clientTimeZone">The time zone to convert to.</param>
        public static DateTime ToClientTime(this DateTime time, TimeZoneInfo clientTimeZone, bool isKindMatter = false)
        {
            if (isKindMatter && time.Kind == DateTimeKind.Local)
            {
                return time;
            }

            if (time.Kind != DateTimeKind.Utc) time = DateTime.SpecifyKind(time, DateTimeKind.Utc);
            var clientTime = TimeZoneInfo.ConvertTimeFromUtc(time, clientTimeZone);

            if (isKindMatter) // making it optional in case someone else is expecting the output of the function in Unspecified
                clientTime = DateTime.SpecifyKind(clientTime, DateTimeKind.Local);

            return clientTime;
        }

        public static DateTime GetNextDayMidnightDateTime(this DateTime today, TimeZoneInfo zone)
        {
            return today
                .ToClientTime(zone)
                .Date
                .AddDays(1)
                .ToUTC(zone);
        }

        /// <summary>
        /// Converts the DateTime to UTC.
        /// The DateTime given should be in the client time zone specified.
        /// </summary>
        /// <param name="time">The time to convert.</param>
        /// <param name="clientTimeZone">The time zone to convert from.</param>
        public static DateTime ToUTC(this DateTime time, TimeZoneInfo clientTimeZone)
        {
            if (time.Kind != DateTimeKind.Unspecified) time = DateTime.SpecifyKind(time, DateTimeKind.Unspecified);
            var utcTime = TimeZoneInfo.ConvertTimeToUtc(time, clientTimeZone);
            return utcTime;
        }

        static readonly double maxSecondsToAdd = ((Int64.MaxValue - 1) / TimeSpan.TicksPerSecond);
        /// <summary>
        /// This function adds the Real number of seconds including sub-milliseconds to the time
        /// </summary>
        /// <param name="time"></param>
        /// <param name="value">A number of whole or fractional seconds</param>
        /// <returns></returns>
        public static DateTime AddSecondsFullPrecision(this DateTime time, double value)
        {
            // Prevent overflow/underflow
            if (time == DateTime.MaxValue && value >= 0) return time;
            if (time == DateTime.MinValue && value <= 0) return time;

            if (value > maxSecondsToAdd)
            {
                DateTime tempTime = time;
                double numberOfSeconds = Math.Floor(value);
                if (value - numberOfSeconds != 0)
                {
                    tempTime = time.AddTicks((long)((value - numberOfSeconds) * TimeSpan.TicksPerSecond));
                }
                return tempTime.AddSeconds(numberOfSeconds);
            }
            else
            {
                return time.AddTicks((long)(value * TimeSpan.TicksPerSecond));
            }
        }

        public static int NumberOfDaysInMonth(this DateTime time)
        {
            return DateTime.DaysInMonth(time.Year, time.Month);
        }
    }
}
