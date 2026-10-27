#region Copyright  2003-2005 by Robert Misiak
// ChronosXP - PlanetaryHours.cs
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
using System.Resources;
using System.Reflection;
using System.Globalization;

namespace ChronosXP
{
	/// <summary>
	/// Calculate planetary hours times for a given date and place.
	/// </summary>
	public sealed class PlanetaryHours : object
	{
		public const string Copyright = "PlanetaryHours.cs, Copyright  2003-2005 by Robert Misiak";

		public struct PlanetaryHour
		{
			public Planet Hour;
			public DateTime StartTime;
			public override string ToString() { return "{" + Hour.ToString() + "," + StartTime.ToString() + "}"; }
		}

		// These must stay in the order that the planetary hours occur in.
		public enum Planet { Saturn, Jupiter, Mars, Sun, Venus, Mercury, Moon };

		// 25 instead of 24, because the array includes the start time of tomorrow's first hour
		public PlanetaryHour[] Hours = new PlanetaryHour[25];
		public Planet Day;
		public SunTime RS;
		public double DayHourLength, NightHourLength;

		public Config conf;
		public Place where;

		// The planetary days, in order from Sunday - Saturday
		public static Planet[] PlanetaryDays = { Planet.Sun, Planet.Moon, Planet.Mars, Planet.Mercury, Planet.Jupiter, Planet.Venus, Planet.Saturn };

		/// <summary>
		/// Calculates the planetary hours for the planetary day beginning at sunrise during "when". "when" should always be in LocalTime.
		/// </summary>
		public PlanetaryHours(DateTime when, Place where, Config conf) : this(when, where, conf, false) { }

		/// <summary>
		/// Calculate the planetary hours for the current planetary day at the default place.
		/// </summary>
		public PlanetaryHours(Config conf) : this(DateTime.Now, conf.DefaultPlace, conf, true) { }

		/// <summary>
		/// If calchr == true, calulates the planetary hours for the current planetary day contained in "when," with consideration
		/// to whether it is before or after sunrise.
		/// If calchr == false, calculates the planetary hours for the planetary day beginning at sunrise of the given day.
		/// </summary>
		public PlanetaryHours(DateTime when, Place where, Config conf, bool calchr)
		{
			this.where = where;
			this.conf = conf;

			SunTime st = new SunTime(when, where, Config.ZenithDistance);

			// if it is post-midnight and pre-sunset and calchr == true, it is still the previous planetary day so calculate those hours.
			if (calchr && when.CompareTo(st.Sunrise) < 0)
			{
				when = when.Subtract(new TimeSpan(1, 0, 0, 0));
				st = new SunTime(when, where, Config.ZenithDistance);
			}

			RS = st;

			// assign the current planetary day.
			this.Day = PlanetaryDays[(int)when.DayOfWeek];

			SunTime tst = new SunTime(st.Sunrise.AddDays(1), where, Config.ZenithDistance);
			// day hour length = (sunset - sunrise) / 12;  night hour length (tomorrowsunrise - sunset) / 12;  see SunTime.cs
			DayHourLength = st.DayLength / 12;

			if (tst.SunRises)
			{
				double nightLen = 24 - (st.SetDouble - tst.RiseDouble);
				if (nightLen >= 24)
					nightLen -= 24;
				NightHourLength = nightLen / 12;
			}
			else
				NightHourLength = st.NightLength / 12;

			// include tomorrow's sunrise as the 25th element of the array; this is needed by this.HourNum, which is the lowest level
			// member used to calculate the current planetary hour.
			Hours[24].StartTime = tst.Sunrise;

			// First assign a planet to each of the 25 hours in the array.  The first hour is the same planet that rules the day.
			// See http://chronosxp.sourceforge.net/hours.htm for a description of how planetary hours are calculated.
			int idx = (int)Day;
			for (int i = 0; i <= 24; i++)
			{
				Hours[i].Hour = (Planet)idx++;
				if (idx == 7)
					idx = 0;
			}

			// Day hours
			Hours[0].StartTime = st.Sunrise;
			for (int i = 1; i < 12; i++)
				Hours[i].StartTime = Hours[i - 1].StartTime.AddHours(DayHourLength);

			// Night hours
			Hours[12].StartTime = st.Sunset;
			for (int i = 13; i < 24; i++)
				Hours[i].StartTime = Hours[i - 1].StartTime.AddHours(NightHourLength);

			// Failsafe...
			if (Hours[24].StartTime.CompareTo(Hours[23].StartTime) < 0)
				Hours[24].StartTime = Hours[23].StartTime.AddHours(NightHourLength);
		}

		/// <summary>
		/// Return a string such as "Day of the Moon" or "Day of Mercury"
		/// </summary>
		public string DayString()
		{
			string str = conf.GetString("DayException." + EnglishDay());
			if (str == null)
				return String.Format(conf.GetString("DayOf"), FormatPlanet(DayName()));
			else
				return str;
		}

		/// <summary>
		/// The name of the planetary day, such as "Sun" or "Mars"
		/// </summary>
		public string DayName()
		{
			return PlanetName(Day);
		}

		/// <summary>
		/// Returns the name of the planet passed in the argument, in the currently configured language (such as "Mercurius" or "Moon")
		/// </summary>
		public string PlanetName(Planet pl)
		{
			return conf.GetString("Planet." + EnglishName(pl));
		}

		/// <summary>
		/// Returns a string such as "Hour of Venus" or "Uur van de Zon" for the current planetary hour
		/// </summary>
		public string CurrentHourString()
		{
			return HourString(where.CurrentTime());
		}

		/// <summary>
		/// Returns the name of the current planetary hour, in the configured language.
		/// </summary>
		public string CurrentHourName()
		{
			return HourName(where.CurrentTime());
		}

		/// <summary>
		/// Returns a string such as "Hour of Mars" for the given time
		/// </summary>
		public string HourString(DateTime when)
		{
			string s = HourName(when);
			if (s == null)
				return null;
			string str = conf.GetString("HourException." + EnglishHour(when));
			if (str == null)
				return String.Format(conf.GetString("HourOf"), FormatPlanet(s));
			else
				return str;
		}

		public string HourString(Planet pl)
		{
			string str = conf.GetString("HourException." + pl.ToString());
			if (str == null)
				return String.Format(conf.GetString("HourOf"), FormatPlanet(pl));
			else
				return str;
		}

		/// <summary>
		/// The English name of the current planetary day - for use in resource names only, never pass this text to the user.
		/// </summary>
		public string EnglishDay()
		{
			return Day.ToString();
		}

		/// <summary>
		/// Returns the English name of the planet given in the argument;  for use in resource names only, never pass this text to the user.
		/// </summary>
		public string EnglishName(Planet pl)
		{
			return pl.ToString(); 
		}

		/// <summary>
		/// Returns the English name of the hour for the given time.
		/// </summary>
		public string EnglishHour(DateTime when)
		{
			int hr = HourNum(when);
			if (hr == -1)
				return null;
			return EnglishName(Hours[hr].Hour);
		}

		/// <summary>
		/// Returns the English name of the current hour.
		/// </summary>
		public string CurrentEnglishHour()
		{
			return EnglishHour(where.CurrentTime());
		}

		public Planet CurrentHour()
		{
			int i = HourNum(where.CurrentTime());
			if (i == -1)
				return (Planet)(-1);
			return Hours[i].Hour;
		}

		/// <summary>
		/// Returns the localized name of the hour for which the time is given.
		/// </summary>
		public string HourName(DateTime when)
		{
			int i = HourNum(when);
			if (i == -1)
				return null;
			return conf.GetString("Planet." + EnglishName(Hours[i].Hour));
		}

		/// <summary>
		/// Returns the hour number for the given time;  0 (sunrise) - 23 (final hour of night)
		/// </summary>
		public int HourNum(DateTime when)
		{
			int matched = -1;

			for (int i = 0; i < 24; i++)
			{
				if ((when.CompareTo(Hours[i].StartTime)) >= 0 && (when.CompareTo(Hours[i + 1].StartTime) < 0))
				{
					matched = i;
					break;
				}
			}

			return matched;
		}

		/// <summary>
		/// Returns the number (0-23) of the current hour.
		/// </summary>
		public int CurrentHourNum()
		{
			return HourNum(where.CurrentTime());
		}

		/// <summary>
		/// Formats the planet by adding the localized LuminaryPrefix if it is a luminary.  i.e., FormatPlanet("Sun") returns "The Sun",
		/// FormatPlanet("Mercury") simply returns "Mercury".
		/// </summary>
		public string FormatPlanet(string planet)
		{
			if (planet == null)
				return null;
			return conf.GetString("PlanetPrefix." + planet, true) + planet;
		}

		public string FormatPlanet(Planet pl)
		{
			return FormatPlanet(PlanetName(pl));
		}

		/// <summary>
		/// Return a string stating the hour of the day/night, such as "11th Hour of Night"
		/// </summary>
		public string HourOfDay(DateTime when)
		{
			int i = HourNum(when);
			if (i == -1)
				return "HOD: can't figure out which hour";

			string dn;
			if (i < 12)
			{
				dn = conf.GetString("Day");
				i++;
			}
			else
			{
				dn = conf.GetString("Night");
				i -= 11;
			}

			return String.Format(conf.GetString("NHourOfDayNight"), conf.GetString("Abbreviation." + i.ToString()), dn);
		}
		
		/// <summary>
		/// Determine the "house of the moment"; the house in which the Sun is transiting
		/// </summary>
		public string HouseOfMoment(DateTime when)
		{
			int i = HourNum(when);
			if (i == -1)
				return "HOM: Can't figure out which hour";
			int hs = 12;
			for (int x = 0; x < 24; x++)
			{
				if (x == i)
					break;
				if ((x & 1) == 1)
					hs--;
			}
			return String.Format(conf.GetString("Calendar.HouseOfMoment"), conf.GetString("Abbreviation." + hs.ToString()));
		}

		public DateTime roundTime(DateTime dt)
		{
			int min;
			if (dt.Second >= 30)
				min = dt.Minute + 1;
			else
				min = dt.Minute;
			// Explicitly set 0 seconds.
			return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, min, 0);
		}
		
		public override string ToString()
		{
			string res;
			try
			{
				res = "{" + String.Format(@"CurrentHour={0},CurrentHourNum={1},Rise={2},Set={3},NextRise={4},Location={5}",
					Hours[CurrentHourNum()].ToString(), CurrentHourNum(), Hours[0].ToString(), Hours[12].ToString(), Hours[24].ToString(),
					where.ToString()) + "}";
			}
			catch (Exception ex)
			{
				res = "{Unknown/ErrorMsg=" + ex.Message + "}";
			}
			return res;
		}
	}
}
