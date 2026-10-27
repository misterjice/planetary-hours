#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - LunarPhase.cs
// Copyright  2004-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
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

namespace ChronosXP
{
	// Calculate lunar phase times; thanks to Martin Lewicki (from an alt.astrology.moderated posting) as well as the script at
	// http://www.fourmilab.ch/earthview/pacalc.html (by John Walker)
	sealed class LunarPhase
	{
		public const string Copyright = "LunarPhase.cs, Copyright  2004-2005 by Robert Misiak";

		/// <summary>
		/// UT lunar phase time.
		/// </summary>
		public readonly DateTime NewMoon, FullMoon, FirstQuarter, LastQuarter,
										WaxingGibbous, WaningGibbous, WaxingCrescent, WaningCrescent;

		/// <summary>
		/// Julian lunar phase time.
		/// </summary>
		public readonly double JulianNewMoon, JulianFullMoon, JulianFirstQuarter, JulianLastQuarter,
													 JulianWaxingGibbous, JulianWaningGibbous, JulianWaxingCrescent, JulianWaningCrescent;
		
		public enum Phase { PreNew, New, WaxingCrescent, FirstQuarter, WaxingGibbous, Full, WaningGibbous, LastQuarter, WaningCrescent };
		
		// LunarPhase.cs should be easily incorprated into any other application; however you will want to change
		// these values to the appropriate names.  The names used here are keys for Strings resources.
		public static string[] PhaseName = new string[] { "LunarPhase.New", "LunarPhase.WaxingCrescent", "LunarPhase.FirstQuarter",
																											"LunarPhase.WaxingGibbous", "LunarPhase.Full", "LunarPhase.WaningGibbous",
																											"LunarPhase.LastQuarter", "LunarPhase.WaningCrescent" };
		public readonly DateTime[] PhaseTimes = new DateTime[8];
		public readonly double[] JulianTimes = new double[8];

		// Used as base date for the algorithm, and later to convert from Julian.  Why not midnight/Julian 0.5?
		private const double baseJulian = 2415020.75933;
		private DateTime baseTime = new DateTime(1900, 1, 1, 6, 13, 26);

		/// <summary>
		/// Calculate lunar phase times.  Times stored are UT.
		/// </summary>
		/// <param name="searchTime">A DateTime containing a time for which to report the nearest lunar phases</param>
		public LunarPhase(DateTime searchTime)
		{
			double[] k = new double[8], t = new double[8];
			double ks = Math.Floor((DateAsDouble(searchTime) - 1900) * 12.3685);
			for (int i = 0; i < 8; i++)
			{
				k[i] = ks + 0.125 * i;
				t[i] = k[i] / 1236.85;
			}

			double[] m = new double[8], mprime = new double[8], f = new double[8];
			// First calculate mean phase times
			for (int i = 0; i < 8; i++)
			{
				JulianTimes[i] = baseJulian  // 2415020.75933 = Julian Date for 1-1-1900 6:13:26 UT
					+ 29.53058868 * k[i]  // 29.53058868 = Days in lunar cycle
					+ 0.0001178 * Math.Pow(t[i], 2)
					- 0.000000155 * Math.Pow(t[i], 3)
					+ 0.00033 * DtrSin(166.56 + 132.87 * t[i] - 0.009173 * Math.Pow(t[i], 2));
				m[i] = 359.2242  // 359.2242 = Sun's mean anomaly
					+ 29.10535608 * k[i]
					- 0.0000333 * Math.Pow(t[i], 2)
					- 0.00000347 * Math.Pow(t[i], 3);
				mprime[i] = 306.0253  // 306.0253 = Moon's mean anomaly
					+ 385.81691806 * k[i]
					+ 0.0107306 * Math.Pow(t[i], 2)
					+ 0.00001236 * Math.Pow(t[i], 3);
				f[i] = 21.2964  // 21.2964 = Moon's argument of latitude
					+ 390.67050646 * k[i]
					- 0.0016528 * Math.Pow(t[i], 2)
					- 0.00000239 * Math.Pow(t[i], 3);
			}

			// Calculate actual full and new moon times
			for (int i = 0; i <= 4; i += 4)
			{
				JulianTimes[i] += (0.1734 - 0.000393 * t[i]) * DtrSin(m[i])
					+ 0.0021 * DtrSin(2 * m[i])
					- 0.4068 * DtrSin(mprime[i])
					+ 0.0161 * DtrSin(2 * mprime[i])
					- 0.0004 * DtrSin(3 * mprime[i])
					+ 0.0104 * DtrSin(2 * f[i])
					- 0.0051 * DtrSin(m[i] + mprime[i])
					- 0.0074 * DtrSin(m[i] - mprime[i])
					+ 0.0004 * DtrSin(2 * f[i] + m[i])
					- 0.0004 * DtrSin(2 * f[i] - m[i])
					- 0.0006 * DtrSin(2 * f[i] + mprime[i])
					+ 0.0010 * DtrSin(2 * f[i] - mprime[i])
					+ 0.0005 * DtrSin(m[i] + 2 * mprime[i]);
			}

			// Calculate first qtr, last quarter, and waxing/waning gibbous and crescent times
			for (int i = 1; i < 8; i++)
			{
				if (i == 4) // Full moon; already calculated
					continue;
				JulianTimes[i] += (0.1721 - 0.0004 * t[i]) * DtrSin(m[i])
					+ 0.0021 * DtrSin(2 * m[i])
					- 0.6280 * DtrSin(mprime[i])
					+ 0.0089 * DtrSin(2 * mprime[i])
					- 0.0004 * DtrSin(3 * mprime[i])
					+ 0.0079 * DtrSin(2 * f[i])
					- 0.0119 * DtrSin(m[i] + mprime[i])
					- 0.0047 * DtrSin(m[i] - mprime[i])
					+ 0.0003 * DtrSin(2 * f[i] + m[i])
					- 0.0004 * DtrSin(2 * f[i] - m[i])
					- 0.0006 * DtrSin(2 * f[i] + mprime[i])
					+ 0.0021 * DtrSin(2 * f[i] - mprime[i])
					+ 0.0003 * DtrSin(m[i] + 2 * mprime[i])
					+ 0.0004 * DtrSin(m[i] - 2 * mprime[i])
					- 0.0003 * DtrSin(2 * m[i] + mprime[i]);
				if (i < 4)  // Waxing
					JulianTimes[i] += 0.0028 - 0.0004 * DtrCos(m[i]) + 0.0003 * DtrCos(mprime[i]);
				else if (i > 4) // Waning
					JulianTimes[i] += -0.0028 + 0.0004 * DtrCos(m[i]) - 0.0003 * DtrCos(mprime[i]);
			}

			for (int i = 0; i < 8; i++)
				PhaseTimes[i] = JulianToDateTime(JulianTimes[i]);
			
			NewMoon = PhaseTimes[0];
			WaxingCrescent = PhaseTimes[1];
			FirstQuarter = PhaseTimes[2];
			WaxingGibbous = PhaseTimes[3];
			FullMoon = PhaseTimes[4];
			WaningGibbous = PhaseTimes[5];
			LastQuarter = PhaseTimes[6];
			WaningCrescent = PhaseTimes[7];
			
			JulianNewMoon = JulianTimes[0];
			JulianWaxingCrescent = JulianTimes[1];
			JulianFirstQuarter = JulianTimes[2];
			JulianWaxingGibbous = JulianTimes[3];
			JulianFullMoon = JulianTimes[4];
			JulianWaningGibbous = JulianTimes[5];
			JulianLastQuarter = JulianTimes[6];
			JulianWaningCrescent = JulianTimes[7];
		}
		
		/// <summary>
		/// Returns the current lunar phase.
		/// </summary>
		/// <param name="when">UT DateTime</param>
		/// <returns></returns>
		public Phase CurrentPhase(DateTime when)
		{
			if (when.CompareTo(NewMoon) < 0)
				return Phase.PreNew;
			else if (when.CompareTo(NewMoon) >= 0 && when.CompareTo(WaxingCrescent) < 0)
				return Phase.New;
			else if (when.CompareTo(WaxingCrescent) >= 0 && when.CompareTo(FirstQuarter) < 0)
				return Phase.WaxingCrescent;
			else if (when.CompareTo(FirstQuarter) >= 0 && when.CompareTo(WaxingGibbous) < 0)
				return Phase.FirstQuarter;
			else if (when.CompareTo(WaxingGibbous) >= 0 && when.CompareTo(FullMoon) < 0)
				return Phase.WaxingGibbous;
			else if (when.CompareTo(FullMoon) >= 0 && when.CompareTo(WaningGibbous) < 0)
				return Phase.Full;
			else if (when.CompareTo(WaningGibbous) >= 0 && when.CompareTo(LastQuarter) < 0)
				return Phase.WaningGibbous;
			else if (when.CompareTo(LastQuarter) >= 0 && when.CompareTo(WaningCrescent) < 0)
				return Phase.LastQuarter;
			else
				return Phase.WaningCrescent;
		}
		
		public int PhaseNum(DateTime when)
		{
			if (when.CompareTo(NewMoon) < 0)
				return -1;
			else if (when.CompareTo(NewMoon) >= 0 && when.CompareTo(WaxingCrescent) < 0)
				return 0;
			else if (when.CompareTo(WaxingCrescent) >= 0 && when.CompareTo(FirstQuarter) < 0)
				return 1;
			else if (when.CompareTo(FirstQuarter) >= 0 && when.CompareTo(WaxingGibbous) < 0)
				return 2;
			else if (when.CompareTo(WaxingGibbous) >= 0 && when.CompareTo(FullMoon) < 0)
				return 3;
			else if (when.CompareTo(FullMoon) >= 0 && when.CompareTo(WaningGibbous) < 0)
				return 4;
			else if (when.CompareTo(WaningGibbous) >= 0 && when.CompareTo(LastQuarter) < 0)
				return 5;
			else if (when.CompareTo(LastQuarter) >= 0 && when.CompareTo(WaningCrescent) < 0)
				return 6;
			else
				return 7;
		}

		/// <summary>
		/// Returns the date represented by a floating point value (in which the year is whole)
		/// </summary>
		private double DateAsDouble(DateTime when)
		{
			double daysInYear;
			if (DateTime.IsLeapYear(when.Year))
				daysInYear = 366;
			else
				daysInYear = 365;
			return when.Year + ((1 / daysInYear) * when.DayOfYear);
		}

		/// <summary>
		/// Convert a Julian date to DateTime.
		/// </summary>
		private DateTime JulianToDateTime(double julian)
		{
			return baseTime.AddDays(julian - baseJulian);
		}

		private double DtrSin(double x)
		{
			return Math.Sin(DegreesToRadians(x));
		}

		private double DtrCos(double x)
		{
			return Math.Cos(DegreesToRadians(x));
		}

		// Redundant within ChronosXP; but we'll keep it here for easy incorporation elsewhere.
		private double DegreesToRadians(double x)
		{
			return (x * Math.PI) / 180;
		}
	}
}
