 #region Copyright © 2003-2009 by Robert Misiak
// ChronosXP - Config.cs
// Copyright © 2003-2009 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace ChronosXP
{
	/// <summary>
	/// Config is where all configuration is read and saved.  ChronosXP saves all the information it needs to save to the registry.
	/// For items independant to CXP, the default values are stored under HKLM\SOFTWARE\Robert Misiak\ChronosXP,
	/// and the user vales are stored under HKCU in a key of an identical name.  ChronosXPCore calls new Config, and from
	/// there a pointer to Config is passed to each of the forms, usually stored locally under the variable conf.
	/// </summary>
	public sealed class Config
	{
		public const string Copyright = "Config.cs, Copyright © 2003-2009 by Robert Misiak";
        public const string SoftwareName = "ChronosXP";
        // Built as AnyCPU, so the platform is determined at runtime rather than per build.
        public static readonly string Platform = Environment.Is64BitProcess ? "x64" : "x86";
        public static readonly string Architecture = Environment.Is64BitProcess ? "64-bit" : "32-bit";
        public const string CopyrightString = "Copyright © 2009 by Robert Misiak";
		public const string Email = "rmisiak@users.sourceforge.net";
		public const int InternalVersion = 20090412;	// This is used in the process of checking for upgrades
		public const string Version = "4.1";
		public const string PostalAddress = "ChronosXP\r\nc/o Robert Misiak\r\n2779 Fort Myer Ave\r\nHenderson, NV 89052\r\nUnited States of America";
		#if BETA
			public const string BetaVersion = "4.0-Test-A";
			public DateTime BetaExpiry = new DateTime (2009, 9, 1);
		#endif

		#region URLs, etc.
		// URL's used in various parts of the program
		public const string URL								= @"http://chronosxp.sourceforge.net";
		public const string URLHours					= @"http://chronosxp.sourceforge.net/hours.php";
		public const string URLAtlas					= @"https://www.astro.com/atlas";
        public const string URLDonate = @"http://sourceforge.net/project/project_donations.php?group_id=111901";
        public const string URLFAQ = @"http://chronosxp.sourceforge.net/FAQ.html";
		#if BETA
			public const string URLUpdate				= @"http://chronosxp.sourceforge.net/cgi-bin/checkbetaupgrade.cgi?ver={0}&osv={1}&mode={2}&lang={3}/{4}&platform={5}";
		#else
			public const string URLUpdate				= @"http://chronosxp.sourceforge.net/cgi-bin/checkupgrade.cgi?ver={0}&osv={1}&mode={2}&lang={3}/{4}&platform={5}";
		#endif
		public const string URLTranslate			= @"http://chronosxp.sourceforge.net/translate.php";
		public const string SMTPServer				= "mail.sourceforge.net";
		public const string FeedbackAddress		= "rmisiak@users.sourceforge.net";
		public const string UpdatePattern			= @"^Version=(?<ver>\d+) Location=(?<location>[^ ]+) Name=(?<name>.*)$";
		#endregion

		#region Registry declarations
		// Registry keys
		private const string swKey						= /* HKLM, HKCU */ @"SOFTWARE\Robert Misiak";
		private const string mainKey					= /* HKLM, HKCU */ @"SOFTWARE\Robert Misiak\ChronosXP";
		private const string placesKey				= /* HKLM, HKCU */ @"SOFTWARE\Robert Misiak\ChronosXP\Places";
		private const string defaultsKey			= /* HKCU, used when the installer's HKLM key is absent */ @"SOFTWARE\Robert Misiak\ChronosXP\Defaults";
		private const string zoneKey					= /* HKLM */ @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Time Zones";
		private const string ziKey						= /* HKLM */ @"SYSTEM\CurrentControlSet\Control\TimeZoneInformation";
		private const string eventsKey				= /* HKU */ @".DEFAULT\AppEvents\EventLabels";
		private const string runKey						= /* HKCU (or HKLM) */ @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

		// Registry values
		private const string updatesVal				= "Check Updates";
		private const string localityVal			= "Default Locality";
		private const string placeVal					= "Default Place";
		private const string notifyVal				= "Notify";
		private const string pathVal					= "Path";
		private const string soundVal					= "Sound";
		private const string soundNameVal			= "Sound Name";
		private const string soundFileVal			= "Sound File";
		private const string notifyFontVal		= "Notify Font";
		private const string runVal						= "Run";
		private const string cultureVal				= "Culture";
		private const string gradientVal			= "Use Gradient";
		private const string secondsVal				= "Show Seconds";
		private const string intervalVal			= "Short Interval";
		private const string trayVal					= "Run From Tray";
		private const string stickyVal				= "Sticky";
		private const string landscapeVal			= "Landscape";
		private const string diaryVal					= "Diary";
		private const string printGlyphVal		= "Print Glyphs";
		private const string iconSetVal				= "Icon Set";
		private const string multipleVal			= "Print Multiple";
		private const string printAllVal			= "Print All";
		private const string startTimeVal			= "Start Time";
		private const string endTimeVal				= "End Time";
		private const string captionVal				= "Caption";
		private const string pLongitudeVal		= "Longitude";
		private const string pLatitudeVal			= "Latitude";
		private const string pZoneVal					= "Zone";
		private const string zDisplayVal			= "Display";
		private const string zTZIVal					= "TZI";
		private const string zStdVal					= "Std";
		private const string zDltVal					= "Dlt";
		private const string zStandardVal			= "StandardName";
		private const string zDaylightVal			= "DaylightName";
		private const string zNoAutoDSTVal		= "DisableAutoDaylightTimeSet";
        private const string hebrewVal             = "Hebrew";

		private const string defaultBrowser		= "IExplore";

		public const string CultureEN					= "en";
		public const string CultureNL					= "nl";
		public const string CultureES					= "es";
		public const string CultureIT					= "it";
		public const string CultureFR					= "fr";
		public const string CulturePT                  = "pt";
        public const string CultureHU                  = "hu";
        public const string CultureGR                  = "el";
        public const string CultureHE                   = "he";

		public const int LongInterval					= 55555;
		public const int ShortInterval				    = 1111;

        private bool useHebrew = false;
		#endregion

		#region Struct's
		public struct ZoneData
		{
			public string Name;
			public string Display;
			// I may possibly make use of this in a future version.
			//public PInvoke.TimeZoneInformation44 TZI;
		}
		public struct Events
		{
			public string Label;
			public string Name;
		}
		#endregion

		#region Time Zones
		public static string[] AllZones = {
			"+11:00 Bering Time - BET",
			"+11:00 Nome Time - NT",
			"+10:00 Hawaii-Aleutian Time - HST",
			"+09:00 Yukon Time - YST",
			"+09:00 Alaska Standard Time - AKST",
			"+08:00 Alaska Daylight Time - AKDT",
			"+08:00 Pacific Standard Time - PST",
			"+07:00 Pacific Daylight Time - PDT",
			"+07:00 Pacific War Time - PWT",
			"+07:00 Mountain Standard Time - MST",
			"+06:00 Mountain Daylight Time - MDT",
			"+06:00 Mountain War Time - MWT",
			"+06:00 Central Standard Time - CST",
			"+06:00 Central War Time - CWT",
			"+05:00 Central Daylight Time - CDT",
			"+05:00 Eastern Standard Time - EST",
			"+04:00 Eastern Daylight Time - EDT",
			"+04:00 Eastern War Time - EWT",
			"+04:00 Atlantic Standard Time - AST",
			"+03:30 Newfoundland Time - NST",
			"+03:00 Atlantic Daylight Time - ADT",
			"+03:00 Atlantic War Time - AWT",
			"+03:00 Brazil Zone 2 - BZT2",
			"+02:00 Azores Time - AZ",
			"+01:00 West Africa Time - WAT",
			"+00:00 Greenwich Mean Time - GMT",
			"-01:00 British Summer Time - BST",
			"-01:00 Central European Time - CET",
			"-01:00 Middle European Time - MET",
			"-02:00 British War Time - BWT",
			"-02:00 Eastern European Time - EET",
			"-02:00 Central European Summer Time - CEST",
			"-02:00 Russian Federation Zone 1 - RFTm2",
			"-03:00 Eastern European Summer Time - EEST",
			"-03:00 Baghdad Time - BAT",
			"-03:00 Russian Federation Zone 2 - RFTm3",
			"-04:00 Russian Federation Zone 3 - RFTm4",
			"-05:00 Russian Federation Zone 4 - RFTm5",
			"-05:30 Indian Time - IST",
			"-06:00 Russian Federation Zone 5 - RFTm6",
			"-06:30 North Sumatra Time - NSUT",
			"-07:00 South Sumatra Time - SSUT",
			"-07:00 Russian Federation Zone 6 - RFTm7",
			"-07:30 Java Time - JT",
			"-08:00 Australian Western Time - AWST",
			"-08:00 China Coast Time - CCT",
			"-08:00 Russian Federation Zone 7 - RFTm8",
			"-09:00 Australian Western Summer Time - AWDT",
			"-09:00 Japan Time - JST",
			"-09:00 Russian Federation Zone 8 - RFTm9",
			"-09:30 Australian Central Time - ACST",
			"-09:30 South Australian Time - SAT",
			"-10:00 Australian Eastern Time - AEST",
			"-10:00 Guam Time - GST",
			"-10:00 Russian Federation Zone 9 - RFTm10",
			"-10:30 Australian Central Summer Time - ACDT",
			"-10:30 South Australian Summer Time - SDT",
			"-11:00 Australian Eastern Summer Time - AEDT",
			"-11:00 Russian Federation Zone 10 - RFTm11",
			"-12:00 New Zealand Time - NZT",
			"-12:00 Russian Federation Zone 11 - RFTm12",
			"-13:00 New Zealand Summer Time - NZST"
		};
		#endregion

		#region Public declarations
		public bool Error = false;
		public int Run;
		public Events[] EventList;
		public ChronosXPfx Fx = new ChronosXPfx();
		public ChronosXPCore Core;
		public Place[] Places = new Place[256];
		public int PlaceNum = 0;
		public enum SoundTypes { SystemSelect = 0, System = 1, Wave = 2, WaveSelect = 3 };
        public string Path { get { return Application.StartupPath; } }
		public ZoneData[] ZoneList = new ZoneData[128];
		public int ZoneCount = 0, EventCount = 0;
		public bool ZoneSysApplyDST, RunFromTrayNow, LoadOK;
		public string CurrentZone;
		public ResourceManager Res;
		public CultureInfo CultureEnglish = new CultureInfo(CultureEN);
		public CultureInfo CultureDutch = new CultureInfo(CultureNL);
		public CultureInfo CultureSpanish = new CultureInfo(CultureES);
		public CultureInfo CultureItalian = new CultureInfo(CultureIT);
		public CultureInfo CultureFrench = new CultureInfo(CultureFR);
		public CultureInfo CulturePortuguese = new CultureInfo(CulturePT);
        public CultureInfo CultureHungarian = new CultureInfo(CultureHU);
        public CultureInfo CultureGreek = new CultureInfo(CultureGR);
        public CultureInfo CultureHebrew = new CultureInfo(CultureHE);
		public CultureInfo CurrentCulture = null;
		public const double ZenithDistance = 90.8333D; // Don't mess with this number!!!
		// This must stay in the order of PlanetaryHours.Planet: Saturn, Jupiter, Mars, Sun, Venus, Mercury, Moon.
		// Init default colors here.
		public Color[] PlanetColors = { Color.Tomato, Color.Aquamarine, Color.LightCoral, Color.LemonChiffon,
																	  Color.SandyBrown, Color.SteelBlue, Color.CornflowerBlue };
		public NotifyWindow lastNw = null;
		public readonly bool UseZoneInfo;
		public enum CaptionType { HourNumber, HouseOfMoment, LunarPhase };
		#endregion

		#region Private declarations
		private string pLanguage, pPlace, pLPlace, pSoundName, pSoundFile;
		private string pStartTime = "5:00 AM", pEndTime = "11:00 PM";
		private int pSound;
		private Place pDefaultPlace, pDefaultLocality;
		private bool pStartup, pCheckUpgrade, pUseGradient, pShowSeconds, pShortInterval, pRunFromTray, pNotify, pSticky, pLandscape,
						 		 pDiary, pPrintGlyphs, pPrintMultiple = false, pPrintAll = true;
		private System.Drawing.Font pNotifyFont = new Font ("Tahoma", 8.25F);
		private int pCaption;
		#endregion

		#region Constructor
		public Config (ChronosXPCore core)
		{
			this.Core = core;
		#if WINDOWS
			try
			{
				LoadOK = true;

				RegistryKey userKey = Registry.CurrentUser.OpenSubKey (Config.mainKey, true);
				if (userKey == null)
				{
					RegistryKey regSw = Registry.CurrentUser.OpenSubKey(swKey, true);
					if (regSw == null)
					{
						try {
                            Registry.CurrentUser.CreateSubKey(swKey);
                            regSw = Registry.CurrentUser.OpenSubKey(swKey, true);
                        } catch { }
					}
                    userKey = regSw.CreateSubKey("ChronosXP");
				}

                if (userKey.GetValue(runVal) == null)
                {
                    userKey.SetValue(runVal, 1, RegistryValueKind.DWord);
                    Run = 1;
                }
                else
                {
                    try
                    {
                        Run = (int)userKey.GetValue(runVal);
                        Run++;
                        userKey.SetValue(runVal, Run);
                    }
                    catch
                    {
                        Run = 256;
                    }
                }

				RegistryKey configKey = openDefaultsKey();
                try
                {
                    pPlace = configKey.GetValue(placeVal, "Las Vegas, NV").ToString();
                }
                catch (Exception iex)
                {
                    new ErrorReport(placeVal, iex, this).Show();
                }
                try
                {
                    pLPlace = configKey.GetValue(localityVal, "Las Vegas, NV").ToString();
                }
                catch (Exception iex)
                {
                    new ErrorReport(localityVal, iex, this).Show();
                }
                //try
                //{
                //    Path = configKey.GetValue(pathVal).ToString();
                //}
                //catch (Exception iex)
                //{
                //    new ErrorReport(pathVal, iex, this).Show();
                //}
                try
                {
                    pSound = (int)configKey.GetValue(soundVal, 0);
                }
                catch 
                {
                }
                pUseGradient = true; //(int) configKey.GetValue (gradientVal) == 1 ? true : false;
                try
                {
                    pShowSeconds = (int)configKey.GetValue(secondsVal, 0) == 1 ? true : false;
                }
                catch 
                {
                }
                try
                {
                    pShortInterval = (int)configKey.GetValue(intervalVal, 1) == 1 ? true : false;
                }
                catch 
                {
                }
                try
                {
                    pLandscape = (int)configKey.GetValue(landscapeVal, 0) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pDiary = (int)configKey.GetValue(diaryVal, 0) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pPrintGlyphs = (int)configKey.GetValue(printGlyphVal, 1) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    RunFromTrayNow = pRunFromTray = (int)configKey.GetValue(trayVal, 1) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pSoundName = configKey.GetValue(soundNameVal).ToString();
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pCheckUpgrade = (int)configKey.GetValue(updatesVal, 1) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pCaption = (int)configKey.GetValue(captionVal, 1);
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pLanguage = (string)configKey.GetValue(cultureVal);
                }
                catch (Exception iex)
                {
                    new ErrorReport(cultureVal, iex, this).Show();
                }
                try
                {
                    Fx.IconSet = (string)configKey.GetValue(iconSetVal, "Silver");
                }
                catch (Exception iex)
                {
                }
                try
                {
                    if (pLanguage.Equals("*"))
                    {
                        // do nothing...
                    }
                    else if (pLanguage.Equals(CultureNL))
                        CurrentCulture = CultureDutch;
                    else if (pLanguage.Equals(CultureES))
                        CurrentCulture = CultureSpanish;
                    else if (pLanguage.Equals(CultureIT))
                        CurrentCulture = CultureItalian;
                    else if (pLanguage.Equals(CultureFR))
                        CurrentCulture = CultureFrench;
                    else if (pLanguage.Equals(CulturePT))
                        CurrentCulture = CulturePortuguese;
                    else if (pLanguage.Equals(CultureHU))
                        CurrentCulture = CultureHungarian;
                    else if (pLanguage.Equals(CultureGR))
                        CurrentCulture = CultureGreek;
                    else if (pLanguage.Equals(CultureHE))
                        CurrentCulture = CultureHebrew;
                    else
                        CurrentCulture = CultureEnglish;
                }
                catch (Exception iex)
                {
                    new ErrorReport(pLanguage, iex, this).Show();
                }

                try
                {
                    pNotify = (int)configKey.GetValue(notifyVal, 1) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
                try
                {
                    pSticky = (int)configKey.GetValue(stickyVal, 0) == 1 ? true : false;
                }
                catch (Exception iex)
                {
                }
				
				try
				{
					RegistryKey elabels = Registry.Users.OpenSubKey(eventsKey);
					EventList = new Events[elabels.SubKeyCount];
					foreach (string keyname in elabels.GetSubKeyNames())
					{
						RegistryKey nkey = elabels.OpenSubKey (keyname);
						if (nkey.GetValue("") != null)
						{
							EventList[EventCount].Name = (string) nkey.GetValue("");
							EventList[EventCount++].Label = keyname;
							nkey.Close();
						}
					}
					elabels.Close();
				}
				catch
				{
					EventList = new Events[56];
				}

                try
                {
                    if (userKey.GetValue(placeVal) != null)
                        pPlace = userKey.GetValue(placeVal).ToString();
                    if (userKey.GetValue(localityVal) != null)
                        pLPlace = userKey.GetValue(localityVal).ToString();
                    if (userKey.GetValue(soundVal) != null)
                        this.pSound = (int)userKey.GetValue(soundVal);
                    if (userKey.GetValue(soundNameVal) != null)
                        this.pSoundName = userKey.GetValue(soundNameVal).ToString();
                    if (userKey.GetValue(soundFileVal) == null)
                        this.pSoundFile = null;
                    else
                        this.pSoundFile = userKey.GetValue(soundFileVal).ToString();
                    if (userKey.GetValue(updatesVal) != null)
                        pCheckUpgrade = (int)userKey.GetValue(updatesVal) == 1 ? true : false;
                    //if (userKey.GetValue (gradientVal) != null)
                    //	pUseGradient = (int) userKey.GetValue (gradientVal) == 1 ? true : false;
                    if (userKey.GetValue(secondsVal) != null)
                        pShowSeconds = (int)userKey.GetValue(secondsVal) == 1 ? true : false;
                    if (userKey.GetValue(intervalVal) != null)
                        pShortInterval = (int)userKey.GetValue(intervalVal) == 1 ? true : false;
                    if (userKey.GetValue(trayVal) != null)
                        RunFromTrayNow = pRunFromTray = (int)userKey.GetValue(trayVal) == 1 ? true : false;
                    if (userKey.GetValue(notifyVal) != null)
                        pNotify = (int)userKey.GetValue(notifyVal) == 1 ? true : false;
                    if (userKey.GetValue(stickyVal) != null)
                        pSticky = (int)userKey.GetValue(stickyVal) == 1 ? true : false;
                    if (userKey.GetValue(notifyFontVal) != null)
                    {
                        pNotifyFont.Dispose();
                        pNotifyFont = new Font((string)userKey.GetValue(notifyFontVal),
                            float.Parse((string)userKey.GetValue(notifyFontVal + " Size")),
                            (FontStyle)Enum.Parse(typeof(FontStyle), (string)userKey.GetValue(notifyFontVal + " Style")));
                    }
                    if (userKey.GetValue(landscapeVal) != null)
                        pLandscape = (int)userKey.GetValue(landscapeVal) == 1 ? true : false;
                    if (userKey.GetValue(diaryVal) != null)
                        pDiary = (int)userKey.GetValue(diaryVal) == 1 ? true : false;
                    if (userKey.GetValue(printGlyphVal) != null)
                        pPrintGlyphs = (int)userKey.GetValue(printGlyphVal) == 1 ? true : false;
                    if (userKey.GetValue(multipleVal) != null)
                        pPrintMultiple = (int)userKey.GetValue(multipleVal) == 1 ? true : false;
                    if (userKey.GetValue(printAllVal) != null)
                        pPrintAll = (int)userKey.GetValue(printAllVal) == 1 ? true : false;
                    if (userKey.GetValue(startTimeVal) != null)
                        pStartTime = userKey.GetValue(startTimeVal).ToString();
                    if (userKey.GetValue(endTimeVal) != null)
                        pEndTime = userKey.GetValue(endTimeVal).ToString();
                    if (userKey.GetValue(captionVal) != null)
                        pCaption = (int)userKey.GetValue(captionVal);

                    foreach (PlanetaryHours.Planet pl in PlanetaryHours.PlanetaryDays)
                        if (userKey.GetValue("Color" + pl.ToString()) != null)
                            PlanetColors[(int)pl] = Color.FromName((string)userKey.GetValue("Color" + pl.ToString()));

                    if (userKey.GetValue(cultureVal) != null)
                    {
                        pLanguage = (string)userKey.GetValue(cultureVal);

                        if (pLanguage == "*")
                        {
                            // do nothing
                        }
                        else if (pLanguage.StartsWith(CultureEN))
                            CurrentCulture = CultureEnglish;
                        else if (pLanguage.StartsWith(CultureNL))
                            CurrentCulture = CultureDutch;
                        else if (pLanguage.StartsWith(CultureES))
                            CurrentCulture = CultureSpanish;
                        else if (pLanguage.StartsWith(CultureIT))
                            CurrentCulture = CultureItalian;
                        else if (pLanguage.StartsWith(CultureFR))
                            CurrentCulture = CultureFrench;
                        else if (pLanguage.StartsWith(CulturePT))
                            CurrentCulture = CulturePortuguese;
                        else if (pLanguage.StartsWith(CultureHU))
                            CurrentCulture = CultureHungarian;
                        else if (pLanguage.StartsWith(CultureGR))
                            CurrentCulture = CultureGreek;
                        else if (pLanguage.StartsWith(CultureHE))
                            CurrentCulture = CultureHebrew;
                        else
                            CurrentCulture = new CultureInfo(pLanguage);
                    }
                    if (userKey.GetValue(iconSetVal) != null)
                        Fx.IconSet = (string)userKey.GetValue(iconSetVal);

                    ////if (userKey.GetValue(hebrewVal) != null && Convert.ToInt32(userKey.GetValue(hebrewVal)) == 1)
                    ////{
                    ////    useHebrew = true;
                    ////    Thread.CurrentThread.CurrentUICulture = CultureHebrew;
                    ////}
                }
                catch (Exception iex)
                {
                    new ErrorReport("User Configuration", iex, this).Show();
                }

				userKey.Close();
				configKey.Close();

			}
			catch (Exception ex)
			{
				Core.ErrorMessage ("Failed reading configuration from registry.  Please re-install ChronosXP.", ex, this);
				LoadOK = false;
			}

			try
			{
                if (CurrentCulture != null)
                {
                    Thread.CurrentThread.CurrentUICulture = CurrentCulture;
                }
				Res = new ResourceManager ("ChronosXP.Strings", Assembly.GetExecutingAssembly());

			//	if (Config.ModernOS() && CultureInfo.InstalledUICulture.Name.ToLower().StartsWith(CultureEN))
			//		UseZoneInfo = true;
			//	else
					UseZoneInfo = false;

				RegistryKey skey;
				//if (Config.ModernOS())
					skey = Registry.CurrentUser.OpenSubKey (runKey);
                //else
                //    skey = Registry.LocalMachine.OpenSubKey (runKey);
				if (skey == null)
				{
					Core.ErrorMessage ("An internal error has occured", new Exception(@"Failed opening registry key HKCU\" + runKey), this);
					pStartup = false;
				}
				else
				{
					if (skey.GetValue ("ChronosXP") == null)
						pStartup = false;
					else
						pStartup = true;
					skey.Close();
					// An older install may have left the Run entry pointing at an exe that no longer exists.
					if (pStartup)
						Startup = true;
				}

				loadPlaces();
				// A default place or locality saved by an older install may no longer exist in the places list.
				// Fall back to the installer's defaults (or the first known place) rather than failing on a null
				// Place, and treat it as a first run so the user is asked to set their location.
				pDefaultPlace = GetPlace(DPlace);
				if (pDefaultPlace == null)
				{
					pDefaultPlace = GetPlace("Las Vegas, NV") ?? Places[0];
					pPlace = pDefaultPlace.Name;
					Run = 1;
				}
				pDefaultPlace.SetDefault();
				pDefaultLocality = GetPlace(LPlace);
				if (pDefaultLocality == null)
				{
					pDefaultLocality = GetPlace("New York, NY") ?? pDefaultPlace;
					pLPlace = pDefaultLocality.Name;
				}

				if (UseZoneInfo)
				{
					skey = Registry.LocalMachine.OpenSubKey (zoneKey);
					if (skey != null)
					{
						foreach (string s in skey.GetSubKeyNames())
						{
							RegistryKey vkey = skey.OpenSubKey (s);
							if (vkey != null)
							{
								ZoneList[ZoneCount].Display = vkey.GetValue (zDisplayVal).ToString();
								//ZoneList[ZoneCount].TZI = RegistryTZI (vkey.GetValue (zTZIVal));
								ZoneList[ZoneCount++].Name = s;
							}
						}
						skey.Close();
					}
					else
					{
						Core.ErrorMessage ("Unable to read registry key: HKLM\\" + zoneKey, new Exception(), this);
					}
					skey = Registry.LocalMachine.OpenSubKey (ziKey);
					if (skey != null)
					{
						if (skey.GetValue (zNoAutoDSTVal) == null)
							ZoneSysApplyDST = true;
						else
							ZoneSysApplyDST = false;
						if (skey.GetValue (zStandardVal) != null)
						{
							CurrentZone = skey.GetValue (zStandardVal).ToString();
						}
						else
						{
							Core.ErrorMessage ("Unable to determine time zone",
								new Exception (@"Unable to read registry value HKLM\" + ziKey + @"\" + zStandardVal), this);
							CurrentZone = "";
						}
						skey.Close();
					}
					else
					{
						Core.ErrorMessage ("Unable to determine time zone", new Exception("Unable to read registry key: HKLM\\" + ziKey), this);
						CurrentZone = "";
					}
				}
			}
			catch (Exception ex)
			{
				Core.ErrorMessage ("Configuration Failed.  Please re-install ChronosXP.", ex, this);
				LoadOK = false;
			}
		#else // !WINDOWS
			LoadOK = true;
			UseZoneInfo = false;
			try
			{
				Res = new ResourceManager ("Strings", Assembly.GetExecutingAssembly());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

			pPlace = "Las Vegas, NV";
			pLPlace = "Las Vegas, NV";
			Path = @"C:\";
			pSound = 0;
			pUseGradient = true;
			pShowSeconds = false;
			pShortInterval = true;
			pLandscape = false;
			pDiary = false;
			pPrintGlyphs = true;
			this.pRunFromTray = RunFromTrayNow = false;
			ZenithDistance = 90;
			pSoundName = "Windows Logon";
			pCheckUpgrade = false;
			pLanguage = "en-US";
			CurrentCulture = CultureEnglish;
			pNotify = true;
			pSticky = true;
			pDefaultPlace = new Place("Las Vegas, NV", "115W13", "36N11");
			pDefaultPlace.SetDefault();
			AddPlace(pDefaultPlace);
		#endif // WINDOWS
		}
		#endregion

		#region Time zone related
		public int ZoneIdx (string display)
		{
			for (int i = 0; i < ZoneCount; i++)
			{
				if (ZoneList[i].Display.Equals (display))
					return i;

			}
			return -1;
		}

		public string ZoneDisplayToName (string display)
		{
			string result = null;
			for (int i = 0; i < ZoneCount; i++)
			{
				if (ZoneList[i].Display.Equals (display))
				{
					result = ZoneList[i].Name;
					break;
				}
			}
			return result;
		}
			
		public string ZoneNameToDisplay (string name)
		{
			string result = null;
			for (int i = 0; i < ZoneCount; i++)
			{
				if (ZoneList[i].Name.Equals (name))
				{
					result = ZoneList[i].Display;
					break;
				}
			}
			return result;
		}

		public bool ChangeTimeZone (string zname, bool applydst)
		{
			if (zname == null || !UseZoneInfo)  // See To Do.txt
				return false;

			bool res = false;
			if (!zname.Equals (CurrentZone))
			{
				RegistryKey tkey = Registry.LocalMachine.OpenSubKey (zoneKey + "\\" + zname);
			
				if (tkey != null)
				{
					if (tkey.GetValue (zTZIVal) != null && tkey.GetValue (zStdVal) != null && tkey.GetValue (zDltVal) != null)
					{
						string dlt, std = tkey.GetValue (zStdVal).ToString(); // Standard name, ie "Pacific Standard Time"
						if (tkey.GetValue (zDltVal) == null)
							dlt = "";
						else
							dlt = tkey.GetValue (zDltVal).ToString();   // DST name, ie "Pacific Daylight Time"

						PInvoke.TimeZoneInformation44 sTZI = (PInvoke.TimeZoneInformation44) RegistryTZI (tkey.GetValue (zTZIVal));
						PInvoke.TimeZoneInformation172 lTZI = new PInvoke.TimeZoneInformation172();

						lTZI.Bias = sTZI.Bias;
						lTZI.StandardBias = sTZI.StandardBias;
						lTZI.StandardDate = sTZI.StandardDate;
						lTZI.StandardName = std;
						lTZI.DaylightBias = sTZI.DaylightBias;
						lTZI.DaylightDate = sTZI.DaylightDate;
						lTZI.DaylightName = dlt;

						if (PInvoke.SetTimeZoneInformation (ref lTZI))
						{
							CurrentZone = std;
							res = true;
						}
						else
						{
							Core.ErrorMessage ("Unable to change time zone.", new Exception(), this);
						}

						tkey.Close();
					}
					else
					{
						Core.ErrorMessage ("Couldn't find appropriate time zone data for: " + zname, new Exception(), this);
					}
				}
				else
					Core.ErrorMessage ("Can't find time zone: " + zname, new Exception(), this);
			}
			if (applydst)
			{
				RegistryKey zkey = Registry.LocalMachine.OpenSubKey (ziKey, true);
				if (zkey != null)
				{
					if (zkey.GetValue (zNoAutoDSTVal) != null)
					{
						zkey.DeleteValue (zNoAutoDSTVal);
						res = true;
					}
					zkey.Close();
				}
				else
				{
					Core.ErrorMessage ("Unable to change time zone", new Exception("Unable to write to registry key:  HKLM\\" + ziKey), this);
				}
			}
			else
			{
				RegistryKey zkey = Registry.LocalMachine.OpenSubKey (ziKey, true);
				if (zkey != null)
				{
					if (zkey.GetValue (zNoAutoDSTVal) == null)
					{
						zkey.SetValue (zNoAutoDSTVal, 1);
						res = true;
					}
					zkey.Close();
				}
				else
				{
					Core.ErrorMessage ("An internal error has occured.", new Exception("Unable to write to registry key:  HKLM\\" + ziKey), this);
				}
			}
			return res;
		}

		public object RegistryTZI (object rvalue)
		{
			// From http://groups.google.com/groups?hl=en&selm=OzgmlAQUBHA.1348%40tkmsftngp05
			byte[] avalue = rvalue as byte[];
				int tzisize = avalue.Length;
			IntPtr buffer = Marshal.AllocHGlobal (tzisize);
			Marshal.Copy (avalue, 0, buffer, tzisize);
			PInvoke.TimeZoneInformation44 tzi44 = (PInvoke.TimeZoneInformation44) Marshal.PtrToStructure (buffer,
				typeof (PInvoke.TimeZoneInformation44));
			Marshal.FreeHGlobal (buffer);
			return (object) tzi44;
		}
		#endregion

		#region ChronosXP.Place related
		private void loadPlaces()
		{
			// The installer's places are optional: an old HKLM key may lack them, and the user's own places
			// under HKCU must still load either way.
			RegistryKey dkey = openDefaultsKey();
			RegistryKey nkey = dkey.OpenSubKey ("Places");
			dkey.Close();
			if (nkey != null)
			{
				readPlaces (nkey);
				nkey.Close();
			}

			nkey = Registry.CurrentUser.OpenSubKey (placesKey);
			if (nkey == null)
				nkey = Registry.CurrentUser.CreateSubKey (placesKey);
			readPlaces (nkey);
			nkey.Close();

			if (PlaceNum == 0)
				for (int i = 0; i < builtinPlaces.GetLength (0); i++)
					localAddPlace (new Place (builtinPlaces[i, 0], builtinPlaces[i, 1], builtinPlaces[i, 2], builtinPlaces[i, 3]));
		}

		/// <summary>
		/// Opens the key holding the default settings and built-in places.  The original installer wrote these to
		/// HKLM\\SOFTWARE\\Robert Misiak\\ChronosXP; when that key is absent (no installer, or a 32-bit install whose key
		/// is redirected to WOW6432Node), the same defaults are created under HKCU so no installer or admin rights are needed.
		/// </summary>
		// The places the original installer shipped: name, longitude, latitude, zone.
		private static readonly string[,] builtinPlaces = {
			{ "Las Vegas, NV", "115W13", "36N11", "+07:00 Pacific Daylight Time - PDT" },
			{ "Detroit, MI", "83W03", "42N20", "+04:00 Eastern Daylight Time - EDT" },
			{ "London, England", "0W10", "51N30", "-01:00 British Summer Time - BST" },
			{ "Tokyo, Japan", "139E46", "35N42", "-09:00 Japan Time - JST" },
			{ "Los Angeles, CA", "118W15", "34N3", "+07:00 Pacific Daylight Time - PDT" },
			{ "Sydney, Australia", "151E13", "33S52", "-10:00 Australian Eastern Time - AEST" },
			{ "Athens, Greece", "23E43", "37N58", "-03:00 Eastern European Summer Time - EEST" },
			{ "New York, NY", "74W00", "40N43", "+04:00 Eastern Daylight Time - EDT" } };

		private RegistryKey openDefaultsKey()
		{
			RegistryKey key = Registry.LocalMachine.OpenSubKey (mainKey);
			if (key == null)
			{
				try
				{
					RegistryKey hklm32 = RegistryKey.OpenBaseKey (RegistryHive.LocalMachine, RegistryView.Registry32);
					key = hklm32.OpenSubKey (mainKey);
					hklm32.Close();
				}
				catch { }
			}
			if (key != null)
				return key;

			key = Registry.CurrentUser.CreateSubKey (defaultsKey);
			if (key.GetValue (placeVal) == null)
			{
				key.SetValue (pathVal, Application.StartupPath);
				key.SetValue (updatesVal, 1, RegistryValueKind.DWord);
				key.SetValue (captionVal, 2, RegistryValueKind.DWord);
				key.SetValue (printGlyphVal, 1, RegistryValueKind.DWord);
				key.SetValue (soundNameVal, "Windows Logon");
				key.SetValue (gradientVal, 1, RegistryValueKind.DWord);
				key.SetValue (placeVal, "Las Vegas, NV");
				key.SetValue (intervalVal, 1, RegistryValueKind.DWord);
				key.SetValue (soundVal, 1, RegistryValueKind.DWord);
				key.SetValue (diaryVal, 0, RegistryValueKind.DWord);
				key.SetValue (secondsVal, 0, RegistryValueKind.DWord);
				key.SetValue (hebrewVal, 0, RegistryValueKind.DWord);
				key.SetValue (cultureVal, "*");
				key.SetValue (trayVal, 1, RegistryValueKind.DWord);
				key.SetValue (iconSetVal, "Silver");
				key.SetValue (localityVal, "New York, NY");
				key.SetValue (notifyVal, 1, RegistryValueKind.DWord);
				key.SetValue (landscapeVal, 0, RegistryValueKind.DWord);
				key.SetValue (stickyVal, 1, RegistryValueKind.DWord);

				string[,] places = builtinPlaces;
				RegistryKey pkey = key.CreateSubKey ("Places");
				for (int i = 0; i < places.GetLength (0); i++)
				{
					RegistryKey nkey = pkey.CreateSubKey (places[i, 0]);
					nkey.SetValue (pLongitudeVal, places[i, 1]);
					nkey.SetValue (pLatitudeVal, places[i, 2]);
					nkey.SetValue (pZoneVal, places[i, 3]);
					nkey.Close();
				}
				pkey.Close();
			}
			return key;
		}

		public bool AddPlace (Place where)
		{
			if (!localAddPlace (where))
				return false;
			regAddPlace (where);
			return true;
		}

		public int GetPlaceNum (string name)
		{
			int res = -1;
			for (int i = 0; i < PlaceNum; i++)
			{
				if (Places[i].Name.Equals (name))
					res = i;
			}
			return res;
		}

		public Place GetPlace (string name)
		{
			int i = GetPlaceNum (name);
			if (i == -1)
				return null;
			else
				return Places[i];
		}

		private void readPlaces (RegistryKey nkey)
		{
			foreach (string keyname in nkey.GetSubKeyNames())
			{
				RegistryKey rkey = nkey.OpenSubKey (keyname);
				if (rkey.GetValue (pLongitudeVal) == null || rkey.GetValue (pLatitudeVal) == null)
				{
					Core.ErrorMessage ("ChronosXP is improperly configured", new Exception("Bad registry key: HKLM\\" + placesKey + "\\" + keyname),
						this);
				}
				else
				{
					string longitude = rkey.GetValue (pLongitudeVal).ToString();
					string latitude = rkey.GetValue (pLatitudeVal).ToString();
					if (rkey.GetValue (pZoneVal) == null)
						localAddPlace (new Place (keyname, longitude, latitude));
					else
						localAddPlace (new Place (keyname, longitude, latitude, (string) rkey.GetValue (pZoneVal).ToString()));
					rkey.Close();
				}
			}
		}

		private bool localAddPlace (Place where)
		{
			bool matched = false;
			for (int i = 0; i < PlaceNum; i++)
			{
				if (Places[i].Name.Equals (where.Name))
				{
					Places[i] = where;
					matched = true;
					break;
				}
			}

			if (!matched)
			{
				if (PlaceNum >= 256)
				{
					DialogResult res = MessageBox.Show ("ChronosXP allows a maximum of 256 places to be stored.  Would you like to open the Places Editor to delete unnecessary places?  After you finish, you will need to restart ChronosXP.",
						"Error", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
					if (res == DialogResult.Yes)
						System.Diagnostics.Process.Start(System.IO.Path.Combine(Path, "Places Editor.exe"));
					return false;
				}
				Places[PlaceNum++] = where;
			}

			return true;
		}

		private void regAddPlace (Place where)
		{
			bool matched = false;
			RegistryKey rkey = Registry.CurrentUser.OpenSubKey (placesKey, true);
			if (rkey == null)
			{
				Core.ErrorMessage ("ChronosXP is misconfigured", new Exception("Cannot open registry key HKCU\\" + placesKey), this);
				return;
			}
			foreach (string keyname in rkey.GetSubKeyNames())
			{
				if (keyname.Equals (where.Name))
				{
					RegistryKey nkey = rkey.OpenSubKey (keyname, true);
					nkey.SetValue (pLongitudeVal, where.Longitude);
					nkey.SetValue (pLatitudeVal, where.Latitude);
					if (where.Zone != null)
						nkey.SetValue (pZoneVal, where.Zone);
					matched = true;
					nkey.Close();
					break;
				}
			}

			if (!matched)
			{
				RegistryKey nkey;
				try
				{
					nkey = rkey.CreateSubKey (where.Name);
					nkey.SetValue (pLongitudeVal, where.Longitude);
					nkey.SetValue (pLatitudeVal, where.Latitude);
					if (where.Zone != null)
						nkey.SetValue (pZoneVal, where.Zone);
					nkey.Close();
				}
				catch (Exception ex)
				{
					Core.ErrorMessage ("Cannot create subkey HKCU\\" + placesKey + "\\" + where.Name, ex);
				}
			}
			rkey.Close();
		}
		#endregion

		#region Config variables
		
		public System.Drawing.Font NotifyFont
		{
			get
			{
				return pNotifyFont;
			}
			set
			{
				if (value == null)
					return;
				if (!value.Equals (pNotifyFont))
				{
					pNotifyFont.Dispose();
					pNotifyFont = value;
					RegistryKey rkey = Registry.CurrentUser.OpenSubKey (Config.mainKey, true);
					rkey.SetValue (notifyFontVal, value.Name);
					rkey.SetValue (notifyFontVal + " Size", value.Size.ToString());
					rkey.SetValue (notifyFontVal + " Style", value.Style.ToString());
					rkey.Close();
				}
			}
		}
		
		public CaptionType Caption
		{
			get
			{
				return (CaptionType)pCaption;
			}
			set
			{
				if ((int)value != pCaption)
				{
					pCaption = (int)value;
					setRegistryValue(captionVal, (int)value);
				}
			}
		}
		
		public string IconSet
		{
			get
			{
				return Fx.IconSet;
			}
			set
			{
				string pstr = value;
				string res = pstr.Substring (0, 1).ToUpper() + pstr.Substring (1, pstr.Length - 1).ToLower();
				Fx.IconSet = res;
				setRegistryValue (iconSetVal, res);
			}
		}

		public bool CheckUpgrade
		{
			get
			{
				return pCheckUpgrade;
			}
			set
			{
				if (pCheckUpgrade != value)
				{
					pCheckUpgrade = value;
					setRegistryValue (updatesVal, value == true ? 1 : 0);
				}
			}
		}

		public string LPlace
		{
			get
			{
				return pLPlace;
			}
			set
			{
				if (!value.Equals (pLPlace))
				{
					pLPlace = value;
					setRegistryValue (localityVal, value);
				}
			}
		}

		public bool PrintAll
		{
			get
			{
				return pPrintAll;
			}
			set
			{
				if (value != pPrintAll)
				{
					pPrintAll = value;
					setRegistryValue (printAllVal, value ? 1 : 0);
				}
			}
		}

		public bool PrintMultiple
		{
			get
			{
				return pPrintMultiple;
			}
			set
			{
				if (value != pPrintMultiple)
				{
					pPrintMultiple = value;
					setRegistryValue (multipleVal, value ? 1 : 0);
				}
			}
		}

		public string SoundFile
		{
			get
			{
				return pSoundFile;
			}
			set
			{
				if (!value.Equals (pSoundFile))
				{
					pSoundFile = value;
					setRegistryValue (soundFileVal, value);
				}
			}
		}

		public SoundTypes Sound
		{
			get
			{
				return (SoundTypes) pSound;
			}
			set
			{
				if ((int)value != pSound)
				{
					setRegistryValue (soundVal, (int) value);
					this.pSound = (int) value;
				}
			}
		}

		public string  SoundName
		{
			get
			{
				return pSoundName;
			}
			set
			{
				if (!value.Equals (pSoundName))
				{
					setRegistryValue (soundNameVal, value);
					this.pSoundName = value;
				}
			}
		}

		public string DPlace
		{
			get
			{
				return pPlace;
			}
			set
			{
				if (!value.Equals (pPlace))
				{
					setRegistryValue (placeVal, value);
					pPlace = value;
				}
			}
		}

		/// <summary>
		/// TODO: Stop using this.  Remove all references.
		/// </summary>
		public bool UseGradient
		{
			get
			{
				return true;// pUseGradient;
			}
			set
			{
				if (value != pUseGradient)
				{
					pUseGradient = value;
					setRegistryValue (gradientVal, value == true ? 1 : 0);
				}
			}
		}

		public bool ShowSeconds
		{
			get
			{
				return pShowSeconds;
			}
			set
			{
				if (value != pShowSeconds)
				{
					pShowSeconds = value;
					setRegistryValue (secondsVal, value == true ? 1 : 0);
				}
			}
		}

		public string FormatTime (DateTime when)
		{
			if (pShowSeconds)
				return when.ToLongTimeString();
			else
				return when.ToShortTimeString();
		}

		public bool RunFromTray
		{
			get
			{
				return pRunFromTray;
			}
			set
			{
				if (value != pRunFromTray)
				{
					pRunFromTray = value;
					setRegistryValue (trayVal, value == true ? 1 : 0);
				}
			}
		}

		public bool Landscape
		{
			get
			{
				return pLandscape;
			}
			set
			{
				if (value != pLandscape)
				{
					pLandscape = value;
					setRegistryValue (landscapeVal, value ? 1 : 0);
				}
			}
		}

		public bool Diary
		{
			get
			{
				return pDiary;
			}
			set
			{
				if (value != pDiary)
				{
					pDiary = value;
					setRegistryValue (diaryVal, value ? 1 : 0);
				}
			}
		}

		public bool PrintGlyphs
		{
			get
			{
				return pPrintGlyphs;
			}
			set
			{
				if (value != pPrintGlyphs)
				{
					pPrintGlyphs = value;
					setRegistryValue (printGlyphVal, value ? 1 : 0);
				}
			}
		}


		public bool UseShortInterval
		{
			get
			{
				return pShortInterval;
			}
			set
			{
				if (value != pShortInterval)
				{
					pShortInterval = value;
					setRegistryValue (intervalVal, value == true ? 1 : 0);
				}
			}
		}

		public int Interval()
		{
			if (pShortInterval)
				return ShortInterval;
			else
				return LongInterval;
		}
		
		public bool Sticky
		{
			get
			{
				return pSticky;
			}
			set
			{
				if (value != pSticky)
				{
					pSticky = value;
					setRegistryValue (stickyVal, value == true ? 1 : 0);
				}
			}
		}

		public bool Startup
		{
			get
			{
				return pStartup;
			}
			set
			{
				RegistryKey skey;
                //if (Config.ModernOS())
					skey = Registry.CurrentUser.OpenSubKey (runKey, true);
                //else
                //    skey = Registry.LocalMachine.OpenSubKey (runKey, true);
				if (skey == null)
				{
					Core.ErrorMessage ("The Windows registry is possibly corrupt, or an unsupported version of Windows is being used", 
						new Exception(@"Failed reading registry key HKCU\" + runKey), this);
				}
				else
				{
					if (value == false)
					{
						skey.DeleteValue (SoftwareName, false);
						pStartup = false;
					}
					else
					{
						pStartup = false;
						try
						{
							skey.SetValue("ChronosXP", String.Concat('"', Application.ExecutablePath, '"'));
                                //"\"" + Path + "ChronosXP.exe\"");
							pStartup = true;
						}
						catch (Exception ex)
						{
							Core.ErrorMessage (@"Failed writing registry value HKCU\" + runKey + @"\ChronosXP", ex);
						}
					}
					skey.Close();
				}
			}
		}

		public string Language
		{
			get
			{
				return pLanguage;
			}
			set
			{
				if (!value.Equals (pLanguage))
				{
					switch (value)
					{
                        case "*":
                            setRegistryValue(cultureVal, "*");
                            return;
						case CultureEN:
							pLanguage = CultureEN;
							setRegistryValue (cultureVal, CultureEN);
							Thread.CurrentThread.CurrentUICulture = CultureEnglish;
							CurrentCulture = CultureEnglish;
							break;
						case CultureNL:
							pLanguage = CultureNL;
							setRegistryValue (cultureVal, CultureNL);
							Thread.CurrentThread.CurrentUICulture = CultureDutch;
							CurrentCulture = CultureDutch;
							break;
						case CultureES:
							pLanguage = CultureES;
							setRegistryValue (cultureVal, CultureES);
							Thread.CurrentThread.CurrentUICulture = CultureSpanish;
							CurrentCulture = CultureSpanish;
							break;
						case CultureIT:
							pLanguage = CultureIT;
							setRegistryValue(cultureVal, CultureIT);
							Thread.CurrentThread.CurrentUICulture = CultureItalian;
							CurrentCulture = CultureItalian;
							break;
						case CultureFR:
							pLanguage = CultureFR;
							setRegistryValue(cultureVal, CultureFR);
							Thread.CurrentThread.CurrentUICulture = CultureFrench;
							CurrentCulture = CultureFrench;
							break;
					  case CulturePT:
							pLanguage = CulturePT;
							setRegistryValue(cultureVal, CulturePT);
							Thread.CurrentThread.CurrentUICulture = CulturePortuguese;
							CurrentCulture = CulturePortuguese;
							break;
                        case CultureHU:
                            pLanguage = CultureHU;
                            setRegistryValue(cultureVal, CultureHU);
                            Thread.CurrentThread.CurrentUICulture = CultureHungarian;
                            CurrentCulture = CultureHungarian;
                            break;
                        case CultureGR:
                            pLanguage = CultureGR;
                            setRegistryValue(cultureVal, CultureGR);
                            Thread.CurrentThread.CurrentUICulture = CultureGreek;
                            CurrentCulture = CultureGreek;
                            break;
                        case CultureHE:
                            pLanguage = CultureHE;
                            setRegistryValue(cultureVal, CultureHE);
                            Thread.CurrentThread.CurrentUICulture = CultureHebrew;
                            CurrentCulture = CultureHebrew;
                            break;
						default:
							Core.ErrorMessage ("ChronosXP is misconfigured; unknown culture \"" + value + "\"",
								new Exception("Unsupported culture: " + value), this);
							break;
					}
				}
			}
		}

        public bool Hebrew
        {
            get
            {
                return useHebrew;
            }
            set
            {
                useHebrew = value;
                if (useHebrew)
                {
                    Thread.CurrentThread.CurrentUICulture = CultureHebrew;
                }
                else if (CurrentCulture != null)
                {
                    Thread.CurrentThread.CurrentUICulture = CurrentCulture;
                }
                setRegistryValue(hebrewVal, value ? 1 : 0);
            }
        }

		public Place DefaultPlace
		{
			get
			{
				return pDefaultPlace;
			}
			set
			{
				if (!value.Equals (pDefaultPlace))
				{
					Place p = value;
					if (AddPlace (p))
					{
						pDefaultPlace.UnSetDefault();
						p.SetDefault();
						DPlace = p.Name;
						pDefaultPlace = p;
					}
					else
					{
						Core.ErrorMessage ("Unable to add place \"" + p.Name + "\"", new Exception(), this);
					}
				}
			}
		}

		public bool NotifyHour
		{
			get
			{
				return pNotify;
			}
			set
			{
				if (value != pNotify)
				{
					pNotify = value;
					setRegistryValue (notifyVal, value ? 1 : 0);
				}
			}
		}

		public DateTime StartTime
		{
			get
			{
				return DateTime.Parse(pStartTime);
			}
			set
			{
				pStartTime = value.ToShortTimeString();
				setRegistryValue (startTimeVal, pStartTime);
			}
		}

		public DateTime EndTime
		{
			get
			{
				return DateTime.Parse(pEndTime);
			}
			set
			{
				pEndTime = value.ToShortTimeString();
				setRegistryValue (endTimeVal, pEndTime);
			}
		}

		public Place DefaultLocality
		{
			get
			{
				return pDefaultLocality;
			}
			set
			{
				if (!value.Equals (pDefaultLocality))
				{
					Place p = value;
					if (AddPlace (p))
					{
						pDefaultLocality = p;
						LPlace = p.Name;
					}
					else
					{
						Core.ErrorMessage ("Unable to add place \"" + p.Name + "\"", new Exception(), this);
					}
				}
			}
		}
		#endregion

		#region Others (FormatVersion, setRegistryValue, PlanetColor, SetPlanetColor, LaunchBrowser, ModernOS, FixRadioButtons, OSVersion)
		public string GetString (string str)
		{
			try
			{
				return Res.GetString (str, Thread.CurrentThread.CurrentUICulture);
			}
			catch (Exception ex)
			{
				Core.ErrorMessage ("Missing resource: " + str, ex);
				return "[Missing Resource: " + str + "]";
			}
		}

		/// <param name="ignerr">If true, and the resource does not exist, return an empty string and don't produce an error.</param>
		public string GetString (string str, bool ignerr)
		{
			if (!ignerr)
				return this.GetString (str);
			string res = "";
			try { res = Res.GetString (str, Thread.CurrentThread.CurrentUICulture); } catch { }
			return res;
		}
		
		public static bool WindowsVistaOrHigher()
		{
            return Environment.OSVersion.Version.Major >= 6;
		}
		
		public static string OSVersion()
		{
			string res;
			PInvoke.OSVersionInfo os = new PInvoke.OSVersionInfo();
			os.dwOSVersionInfoSize = Marshal.SizeOf (typeof (PInvoke.OSVersionInfo));
			PInvoke.GetVersionEx (ref os);
			
			switch (os.dwMajorVersion)
			{
				case 4:
				switch (os.dwMinorVersion)
				{
					case 0:
						res = "Windows NT 4.0";
						if (os.szCSDVersion.Length > 0)
							res += " " + os.szCSDVersion;
						break;
					case 10:
						res = "Windows 98";
						if (os.dwBuildNumber > 1998)
							res += " SE";
						break;
					case 90:
						res = "Windows ME";
						break;
					default:
						res = String.Format ("Windows Unknown Version {0}.{1}.{2}", os.dwMajorVersion, os.dwMinorVersion,
							os.dwBuildNumber);
						break;
				}
					break;
				case 5:
				switch (os.dwMinorVersion)
				{
					case 0:
						res = "Windows 2000";
						break;
					case 1:
						res = "Windows XP";
						break;
					case 2:
						res = "Windows 2003 Server";
						break;
					default:
						res = String.Format ("Windows Unknown Version {0}.{1}.{2}", os.dwMajorVersion, os.dwMinorVersion,
							os.dwBuildNumber);
						break;
				}
					if (os.szCSDVersion.Length > 0)
						res += " " + os.szCSDVersion;
					break;
				case 6:
                    if (os.dwMinorVersion == 0)
                        res = "Windows Vista";
                    else
                        res = String.Format("Windows Unknown Version {0}.{1}.{2}", os.dwMajorVersion, os.dwMinorVersion,
                            os.dwBuildNumber);
                    break;
                case 7:
                    if (os.dwMinorVersion == 0)
                        res = "Windows 7";
                    else
                        res = String.Format("Windows Unknown Version {0}.{1}.{2}", os.dwMajorVersion, os.dwMinorVersion,
                            os.dwBuildNumber);
                    break;
				default:
					res = String.Format ("Windows Unknown Version {0}.{1}.{2}", os.dwMajorVersion, os.dwMinorVersion,
						os.dwBuildNumber);
					break;
			}
			return res;
		}

		public static void FixRadioButtons (System.Windows.Forms.Control.ControlCollection cc, FlatStyle fs)
		{
			if (cc == null)
				return;
			foreach (Control c in cc)
			{
				if (c is RadioButton)
				{
					RadioButton rb = c as RadioButton;
					rb.FlatStyle = fs;
				}
				if (c.Controls != null)
					FixRadioButtons (c.Controls, fs);
			}
		}

        // voices start to ring in your head - tell me what do they say?
		public void LaunchBrowser (string url)
		{
			// Open in the user's default browser; Internet Explorer is retired on current Windows.
			try
			{
                System.Diagnostics.Process.Start(url);
			}
			catch (Exception ex)
			{
                Core.ErrorMessage ("Unable to open URL: " + url, ex);
                //				MessageBox.Show("Unable to open URL: " + url + "\r\nIf you are a Firefox user, you can ignore this error, or see " +
//				                "http://johnhaller.com/jh/mozilla/firefox_bug_246078.asp to fix this error.", "ChronosXP", MessageBoxButtons.OK,
//				                MessageBoxIcon.Warning);
			}
		}
		
		#if BETA
		public static string FormatVersion (bool photogenic)
		{
			if (photogenic)
				return String.Format ("{0} {1} ({2})", SoftwareName, Config.Version, Config.Architecture);
			else
				return String.Format ("{0} {1} ({2})", SoftwareName, BetaVersion, Config.Architecture);
		}
		#endif

		public static string FormatVersion()
		{
			#if BETA
                return String.Format("{0} {1} ({2})", SoftwareName, BetaVersion, Config.Architecture);
			#else
				return String.Format ("{0} {1} ({2})", SoftwareName, Config.Version, Config.Architecture);
            #endif
        }

		private void setRegistryValue (string vname, object ovalue)
		{
			try
			{
				RegistryKey rkey = Registry.CurrentUser.OpenSubKey (Config.mainKey, true);
				//if (ovalue is bool)
				//	rkey.SetValue (vname, (bool) ovalue ? 1 : 0);
				//else
				rkey.SetValue (vname, ovalue);
				rkey.Close();
			}
			catch (Exception ex)
			{
				Core.ErrorMessage ("Unable to write registry value: " + Config.mainKey + @"\" + vname + " = " + ovalue.ToString(), ex);
			}
		}

		public Color PlanetColor (PlanetaryHours.Planet pl)
		{
			return PlanetColors[(int)pl];
		}

		public void SetPlanetColor (PlanetaryHours.Planet pl, Color clr)
		{
			if (PlanetColors[(int)pl] != clr)
			{
				setRegistryValue ("Color" + pl.ToString(), clr.Name);
				PlanetColors[(int)pl] = clr;
			}
		}
		#endregion
	}
}
