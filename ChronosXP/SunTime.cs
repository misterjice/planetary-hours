#region Copyright  2003-2005 by Robert Misiak
// ChronosXP - SunTime.cs
// Copyright  2003-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
// 2779 Fort Myer Ave, Henderson, NV 89052 USA
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
#endregion

using System;
using System.Windows.Forms;

namespace ChronosXP
{
	public sealed class SunTime
	{
		public const string Copyright = "SunTime.cs, Copyright  2003-2005 by Robert Misiak";

		public DateTime Sunrise;
		public DateTime Sunset;
		public double DayLength, NightLength, RiseDouble, SetDouble;
		public bool SunRises = true, SunSets = true;

		/// <summary>
		/// For easy incorporation into other applications; calculate "official" sunrise/sunset times 
		/// </summary>
		public SunTime (DateTime when, Place where) : this (when, where, 90.8333) { }
		
		/// <summary>
		/// Calculate sunrise and sunset times for the contained in "when."  Ensure that the latitude (less than 66) and longitude
		/// (less than 180) are in range before calling.
		/// 
		/// We now use the United Stats Naval Observatory algorithm for calculating sunrise and sunset times.  By default, we use 90 as the
		/// geometric zenith distance of the center of the Sun for sunrise and sunset, which is when the Sun crosses the Ascendant.  However,
		/// the technical (non-astrological) definition of sunrise/sunset is when the zenith distance is 9050 (90.8333 expressed as a floating point
		/// value.)  For an observer at sea level with an unobstructed horizon, the Sun first starts to appear over the horizon at 9050, while at 90
		/// the center of the Sun is at the horizon.  Users can change the zenith distance used here by editing the registry value
		/// HKLM\SOFTWARE\Robert Misiak\ChronosXP\Zenith Distance (set it to a floating point value.)  I intentionally made it difficult to change
		/// this setting in the hopes that only users who know what they are doing will make use of it.  Please be very careful when playing around with
		/// this setting!
		/// 
		/// The zenith distance should be:
		/// 90				default, Sun conjunct ascendant
		/// 90.8333		for the technical definition of sunrise/set
		/// 96				for civil twilight
		/// 102				for nautical twilight
		/// 108				for astronomical twilight
		/// 
		/// If you want to use this code in another program, you can substitute the Place argument with two doubles: one for longitude and one for
		/// latitude.  Then replace the where.LocalTime with DateTime.ToLocalTime().  For the zdist argument (zenith distance,) use one of the
		/// values given above.
		/// </summary>
		public SunTime (DateTime when, Place where, double zdist)
		{
			RiseDouble = riseSet (true, when, where, zdist);
			SetDouble = riseSet (false, when, where, zdist);
			if (RiseDouble == -201)
				SunRises = false;
			if (SetDouble == -201)
				SunSets = false;

			if (!SunRises && !SunSets)  // Sun doesn't rise or set; set DayLength=12, NightLength=12
			{
				DayLength = 12;  NightLength = 12;
				Sunrise = new DateTime (when.Year, when.Month, when.Day, 0, 0, 0, 0);
				Sunset = new DateTime (when.Year, when.Month, when.Day, 12, 0, 0, 0);
			}
			else if (SunRises && !SunSets) // Sun rises, but doesn't set.  DayLength=sunrise to noon, NightLength=12
			{
				double offset;
				if (where.UseSystemTime)
				{
					TimeSpan offts = TimeZone.CurrentTimeZone.GetUtcOffset (when);
					offset = offts.Hours + (offts.Minutes / 60);
				}
				else
				{
					offset = where.Offset.Hours + (where.Offset.Minutes / 60);
					if (where.ZoneWest)
						offset = -offset;
				}
				DayLength = (12 - offset) - RiseDouble;  NightLength = 12;
				Sunrise = convertTime (when, where, RiseDouble);
				Sunset = new DateTime (when.Year, when.Month, when.Day, 12, 0, 0, 0);
			}
			// Sun sets after a period of perpetual light;  DayLength=midnight to sunset, NightLength=sunset to midnight or next sunrise
			else if (!SunRises && SunSets)
			{
				double offset;
				if (where.UseSystemTime)
				{
					TimeSpan offts = TimeZone.CurrentTimeZone.GetUtcOffset (when);
					offset = offts.Hours + (offts.Minutes / 60);
				}
				else
				{
					offset = where.Offset.Hours + (where.Offset.Minutes / 60);
					if (where.ZoneWest)
						offset = -offset;
				}
				DayLength = SetDouble + offset;
				SunTime ost = new SunTime (when.AddDays (1), where, zdist);
				NightLength = (24 - offset) - SetDouble;
				// Sun rises tomorrow?  Calculate the night length to this sunrise.
				if (ost.SunRises)
					NightLength += ost.Sunrise.Hour + (ost.Sunrise.Minute / 60);
				Sunrise = new DateTime (when.Year, when.Month, when.Day, 0, 0, 0, 0);
				Sunset = convertTime (when, where, SetDouble);
				if (Sunset.Hour < 12) // Messy
				{
					DayLength += 24;
					Sunset = Sunset.AddDays (1);
				}
			}
			else // Sun rises and sets
			{
				if (SetDouble < RiseDouble)
					SetDouble += 24;
				DayLength = SetDouble - RiseDouble;
				NightLength = 24 - DayLength;
				Sunrise = convertTime (when, where, RiseDouble);
				Sunset = convertTime (when, where, SetDouble);
			}

			// For example; Sun rises at 7PM and sets at 3AM (extreme latitudes)
			if (Sunrise.CompareTo (Sunset) > 0)
				Sunset = Sunset.AddDays (1);
			if (NightLength >= 24)
				NightLength -= 24;
		}

		/// <summary>
		/// United States Naval Observatory algorithm: Almanac for Computers, 1990, published by Nautical Almanac Office
		/// Returns floating point GMT time.  See http://williams.best.vwh.net/sunrise_sunset_algorithm.htm
		/// calcsunrise argument: true=calculate sunrise, false=calculate sunset
		/// </summary>
		private double riseSet (bool calcsunrise, DateTime when, Place where, double zenith)
		{
			// Convert the longitude to hour value and calculate an approximate time
			double lngHour = where.LongitudeAsDouble / 15;
			double t;
			if (calcsunrise)
				t = when.DayOfYear + ((6 - lngHour) / 24);
			else
				t = when.DayOfYear + ((18 - lngHour) / 24);
			// Calculate the Sun's mean anomaly
			double M = (0.9856 * t) - 3.289;
			// Calculate the Sun's true longitude
			double L = adjust (M + (1.916 * Math.Sin (degreesToRadians (M))) + (0.020 * Math.Sin (degreesToRadians (M * 2))) + 282.634, 360);
			// The Sun's right ascension
			double RA = adjust (radiansToDegrees (Math.Atan (0.91764 * Math.Tan (degreesToRadians (L)))), 360);
			// Right ascension value needs to be in the same quadrant as L, and converted to hours
			double Lquadrant = (Math.Floor (L / 90)) * 90;
			double RAquadrant = (Math.Floor (RA / 90)) * 90;
			RA = (RA + (Lquadrant - RAquadrant)) / 15;
			// The Sun's declination
			double sinDec = 0.39782 * Math.Sin (degreesToRadians (L));
			double cosDec = Math.Cos (Math.Asin (sinDec));
			// The Sun's local hour angle - see the Read Me.rtf file for information about the Sun's zenith
			double cosH = (Math.Cos (degreesToRadians (zenith)) - (sinDec * Math.Sin (degreesToRadians (where.LatitudeAsDouble)))) /
				(cosDec * Math.Cos (degreesToRadians (where.LatitudeAsDouble)));
			if (cosH > 1 || cosH < -1) // the sun does not rise or set
				return -201;
			double H;
			if (calcsunrise)
				H = (360 - radiansToDegrees (Math.Acos (cosH))) / 15;
			else
				H = (radiansToDegrees (Math.Acos (cosH))) / 15;
			// Local mean time
			double T = H + RA - (0.06571 * t) - 6.622;
			// return GMT time
			return adjust (T - lngHour, 24);
		}

		private double degreesToRadians (double degrees)
		{
			return ((degrees * Math.PI) / 180.0);
		}

		private double radiansToDegrees (double radians)
		{
			return ((radians / Math.PI) * 180.0);
		}

		/// <summary>
		/// Adjust a value so that it is in the specified range
		/// </summary>
		private double adjust (double src, double range)
		{
			if (src < 0)
				src += range;
			if (src > range)
				src -= range;
			return src;
		}

		/// <summary>
		/// Constructs a DateTime with the date contained in when and the time contained in tod
		/// </summary>
		private DateTime convertTime (DateTime when, Place where, double tod)
		{
			// We use this method to explicitly set date in the return value to the same date stored in "when" - this is because, under some circumstances,
			// the conversion from GMT changed the date.
			DateTime gmtdt = new DateTime (when.Year, when.Month, when.Day, 0, 0, 0).AddHours (tod);
			DateTime ldt = where.LocalTime (gmtdt);
			return new DateTime (when.Year, when.Month, when.Day, ldt.Hour, ldt.Minute, ldt.Second, ldt.Millisecond);
		}
	}
}
