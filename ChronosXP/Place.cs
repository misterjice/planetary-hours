#region Copyright © 2004-2005 by Robert Misiak
// ChronosXP - Place.cs
// Copyright © 2004-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace ChronosXP
{
	/// <summary>
	/// Class for tracking geographical places, it parses longitudes, latitudes & time zones for easier use
	/// in other parts of ChronosXP.
	/// </summary>
	public class Place
	{
		public const string Copyright = "Place.cs, Copyright © 2004 by Robert Misiak";

		public string Name, Longitude, Latitude, Zone;
		public double LatitudeAsDouble, LongitudeAsDouble;
		public char LongitudeQuadrant, LatitudeQuadrant;
		public int LongitudeDegrees, LongitudeMinutes, LatitudeDegrees, LatitudeMinutes;
		public TimeSpan Offset;
		public bool ZoneWest, DefaultPlace, UseSystemTime;
        private string _ZoneAbbreviation;

		public Place (string name, string longitude, string latitude) : this(name, longitude, latitude, null) { }

		public Place (string name, decimal longdeg, decimal longmin, bool west, decimal latdeg, decimal latmin, bool north, string zone)
			: this(name, String.Format ("{0}{1}{2}", longdeg, west ? "W" : "E", longmin), String.Format ("{0}{1}{2}", latdeg, north ? "N" : "S", latmin), zone)
		{	}

		public Place (string name, decimal longdeg, decimal longmin, bool west, decimal latdeg, decimal latmin, bool north)
			: this(name, String.Format ("{0}{1}{2}", longdeg, west ? "W" : "E", longmin), String.Format ("{0}{1}{2}", latdeg, north ? "N" : "S", latmin), null)
		{ }

		public Place(string name, string longitude, string latitude, string zone)
		{
			if (zone != null)
			{
				Regex r = new Regex (@"^(?<eastwest>.)(?<hours>\d+):(?<minutes>\d+).* (?<zone>\w+)$");
				Match m = r.Match (zone);
				if (m.Success)
				{
					Offset = new TimeSpan (int.Parse (m.Groups ["hours"].Value), int.Parse (m.Groups ["minutes"].Value), 0);
					if (m.Groups ["eastwest"].Value.Equals ("+"))
						ZoneWest = true;
					else
						ZoneWest = false;
					_ZoneAbbreviation = m.Groups ["zone"].Value;
					UseSystemTime = false;
				}
				else
				{
					MessageBox.Show ("Zone parse error on \"" + zone + "\"  [Place._real_Place]", "ChronosXP Internal Error",
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					zone = null;
				}
			}
			else
			{
				UseSystemTime = true;
			}

			int [] longi = parseL (longitude);
			int [] lati = parseL (latitude);
			this.Name = name;
			this.Longitude = longitude;
			this.LongitudeAsDouble = stringToDouble (longitude);
			this.LongitudeDegrees = longi [0];
			this.LongitudeMinutes = longi [1];
			this.LongitudeQuadrant = (char) longi [2];
			this.Latitude = latitude;
			this.LatitudeAsDouble = stringToDouble (latitude);
			this.LatitudeDegrees = lati [0];
			this.LatitudeMinutes = lati [1];
			this.LatitudeQuadrant = (char) lati [2];
			this.Zone = zone;
		}

		public DateTime CurrentTime()
		{
			return LocalTime (DateTime.Now.ToUniversalTime());
		}

		public override string ToString()
		{
			string res = String.Format ("{0},{1},{2}", Name, Latitude, Longitude);
			if (DefaultPlace)
				res += ",Default";
			else
				res += "," + (ZoneWest ? "+" : "-") + Offset.ToString();
			if (UseSystemTime)
				res += ",UseSystemTime";
			return "{" + res + "}";
		}

		public DateTime LocalTime (DateTime when)
		{
			if (UseSystemTime)
				return when.ToLocalTime();
			else if (ZoneWest)
				return when.Subtract(Offset);
			else
				return when.Add(Offset);
		}

		protected double stringToDouble (string str)
		{
			int [] ll = parseL (str);
			double res = (double) ll [0] + (ll[1] / 60);
			if (quadrant (str) == 'W' || quadrant (str) == 'S')
				res = -res;
			return res;
		}

		protected int[] parseL (string ll)
		{
			int [] res = new int [3];
			char [] seps = new char[] { 'N', 'S', 'E', 'W' };
			res [0] = int.Parse (ll.ToUpper().Split(seps)[0]);
			res [1] = int.Parse (ll.ToUpper().Split(seps)[1]);
			res [2] = (int) ll[ll.ToUpper().IndexOfAny (seps)];
			return res;
		}

		protected char quadrant (string ll)
		{
			int [] lld = parseL (ll);
			return (char) lld [2];
		}

		public void SetDefault()
		{
			DefaultPlace = true;
			UseSystemTime = true;
		}

		public void UnSetDefault()
		{
			DefaultPlace = false;
			UseSystemTime = Zone == null ? true : false;
		}

        public string ZoneAbbreviation
        {
            set { _ZoneAbbreviation = value; }
            get
            {
                if (UseSystemTime || _ZoneAbbreviation == null)
                {
                    return "";
                }
                else
                {
                    return _ZoneAbbreviation;
                }
            }
        }
	}
}
