#region Copyright © 2004-2009 by Robert Misiak
// ChronosXP - Core.cs
// Copyright © 2004-2009 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.Threading;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace ChronosXP
{
	/// <summary>
	/// ChronosXPCore is the location of the application entry.
	/// 
	/// From here, the current planetary hour/day are calculated, and an appropriate glyph is set in a NotifyIcon.  Roughly every second,
	/// a timer (see phTick and RefreshHours) checks to see if the planetary hour has changed, and if so notifies of the changes.  The
	/// lunar phase is also checked here, when applicable, where it is optionally displayed in the NotifyIcon's ToolTip.
	/// 
	/// The four main forms:  Planetary Hours Calendar (Calendar.cs,) Properties (Properties.cs,) and About (About.cs,) and Lunar Phases
	/// (Phases.cs) are all launched from here.  The Planetary Hours Calendar has buttons to open each of these forms, but it comes back
	/// to Core to open them.
	/// 
	/// Core.ErrorMessage is a semi-standard error reporting system that I've set up, which will notify of errors locations, via either
	/// a MessageBox or Balloon window.
	/// 
	/// ChronosXPCore needs to be inherited from Control to recieve WM_HOTKEY via WndProc().
	/// </summary>
	public sealed class ChronosXPCore : System.Windows.Forms.Control
	{
		public const string Copyright = "Core.cs, Copyright © 2004-2009 by Robert Misiak";

		#region Declarations
		private NotifyIcon trayIcon;
		private ContextMenu trayContextMenu;
		private MenuItem itemCalendar, itemAbout, itemExit, itemProperties, itemLicense, itemSeparator, itemHomePage, itemPhases;
		private PlanetaryHours.Planet currentHour;
		private System.Windows.Forms.Timer phClock, trayClock;
		private Thread auxThread, updateThread;
		private Config conf;
		private DateTime lastCleanup = DateTime.Now;
		private LunarPhase localPhases;
		private LunarPhase.Phase currentPhase;
		private bool checkUpdate = true, fastCheckUpdate = false;

        public bool ShutdownRequested = false;
		public PlanetaryHours pHours;
		public Calendar FormCalendar;
		public About FormAbout;
		public PropertiesBox FormProperties;
		public ChronosXP.ShowRichTextBox FormLicense, FormReadMe;
		public Phases FormPhases;
		public bool FormCalendarOpen = false, FormAboutOpen = false, FormPropertiesOpen = false, FormLicenseOpen = false,
			FormReadMeOpen = false, FormPhasesOpen = false;

		#if BETA
			private System.Windows.Forms.Timer betaClock;
			private bool debugnotify = false;
			public bool Photogenic = false;
		#endif
		#endregion

		#region Application Entry and Core constructor
		public ChronosXPCore()
		{
			Application.ApplicationExit += new System.EventHandler (appExit);
			Application.Idle += new System.EventHandler (appIdle);

			conf = new Config (this);
			if (!conf.LoadOK)
			{
                Shutdown();
				return;
			}

            string[] args = Environment.GetCommandLineArgs();

			bool showproperties = false;
			// The command-line arguments should only be used for testing, or internally
			foreach (string arg in args)
			{
				switch (arg.ToLower())
				{
					case "/english":
						conf.Language = Config.CultureEN;
						break;
					case "/nederlands":
					case "/dutch":
						conf.Language = Config.CultureNL;
						break;
					case "/español":
					case "/espanol":
					case "/spanish":
						conf.Language = Config.CultureES;
						break;
					case "/italiano":
					case "/italian":
						conf.Language = Config.CultureIT;
						break;
					case "/français":
					case "/francais":
					case "/french":
						conf.Language = Config.CultureFR;
						break;
					case "/português":
					case "/portugues":
					case "/portuguese":
						conf.Language = Config.CulturePT;
                        break;
                    case "/hungarian":
                    case "/magyar":
                        conf.Language = Config.CultureHU;
                        break;
                    case "/greek":
                        conf.Language = Config.CultureGR;
                        break;
                    case "/hebrew":
                        conf.Language = Config.CultureHE;
                        break;
					case "/standalone":
						conf.RunFromTrayNow = false;
						break;
					case "/tray":
						conf.RunFromTrayNow = true;
						break;
					case "/properties":
						showproperties = true;
						break;
					case "/nogradient":
						conf.UseGradient = false;
						break;
					case "/gradient":
						conf.UseGradient = true;
						break;
					case "/nocheckupdate":
						checkUpdate = false;
						break;
					case "/fastcheckupdate":
						fastCheckUpdate = true;
						break;
				#if BETA
					case "/debugnotify":
						debugnotify = true;
						break;
					// This is to create screen-shots (for the web page) of a BETA, but make it look like a release.  Use with
					// /currentculture=(culture), to set Thread.CurrentCulture (which prints the VisualMonthCalendar in that language)
					// /photogenic implies /debugnotify
					case "/photogenic":
						Photogenic = true;
						debugnotify = true;
						break;
				#endif
					default:
						if (arg.ToLower().StartsWith ("/iconset="))
						{
							string[] zz = arg.Split (new char[] { '=' }, 2);
							if (zz[1].ToLower().Equals("silver") || zz[1].ToLower().Equals("black") || zz[1].ToLower().Equals("multi"))
								conf.Fx.IconSet = zz[1];
							else
							{
								MessageBox.Show (conf.GetString ("Core.InvalidArgument") + ": " + arg, "ChronosXP",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
								Shutdown();
								return;
							}
						}
						else if (arg.ToLower().StartsWith ("/currentculture="))
						{
							string[] zz = arg.Split (new char[] { '=' }, 2);
							try
							{
								conf.CurrentCulture = new CultureInfo (zz[1]);
								Application.CurrentCulture = conf.CurrentCulture;
								Thread.CurrentThread.CurrentUICulture = conf.CurrentCulture;
							}
							catch
							{
								MessageBox.Show ("Unsupported culture: " + zz[1], "ChronosXP",
									MessageBoxButtons.OK, MessageBoxIcon.Error);
                                Shutdown();
								return;
							}
						}
						else
						{
                            continue;
                            //MessageBox.Show (conf.GetString ("Core.InvalidArgument") + ": " + arg, "ChronosXP",
                            //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //Application.Exit();
                            //return;
						}
						break;
				}
			}

			// tray mode?  initialize the NotifyIcon, ContextMenu and Timer, and calculate planetary hours.
			if (conf.RunFromTrayNow)
			{
				initTray();

				pHours = new PlanetaryHours (conf);
				currentHour = pHours.CurrentHour();
				updateTray();
				
				if (conf.Caption == Config.CaptionType.LunarPhase)
				{
					localPhases = new LunarPhase(DateTime.UtcNow);
					currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);
				}

				// This timer checks to see if the planetary hour has changed; if so, notify the user (if applicable) and change the NotifyIcon
				// and it's ToolTip text.
				phClock = new System.Windows.Forms.Timer();
				phClock.Interval = conf.Interval();
				phClock.Tick += new System.EventHandler(phTick);
				phClock.Start();

				// This timer periodically updates the system tray icon;  this is to ensure that the ChronosXP is not hidden with the
				// inactive icons, and also because of a bug that makes the icon distorted after doing a <Windows key>+L
				trayClock = new System.Windows.Forms.Timer();
				trayClock.Interval = 55555;
				trayClock.Tick += new System.EventHandler(trayTick);
				trayClock.Start();

				// When in tray mode, bind Alt+F11 to open the Planetary Hours Calendar (see this.WndProc)
				PInvoke.RegisterHotKey (Handle, 101, PInvoke.MOD_ALT, PInvoke.VK_F11);
			}

			#if WINDOWS
				if (conf.Run == 1 && !Program.SelfTest) // First time ChronosXP is run?  Prompt user to configure locality
				{
					DialogResult res = MessageBox.Show (conf.GetString ("Core.WelcomeText"), conf.GetString ("Core.WelcomeTitle"),
						MessageBoxButtons.YesNo, MessageBoxIcon.Question);
					if (res == DialogResult.Yes)
						ShowProperties();
					else
						MessageBox.Show (conf.GetString ("Core.NoConfigText"), conf.GetString ("Core.NoConfigTitle"),
							MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
			#endif

			#if BETA
			if (conf.BetaExpiry.CompareTo (DateTime.Now) <= 0) // Beta expired?  Prompt user to upgrade
			{
				auxThread = new Thread (new ThreadStart (threadBetaExpired));
				auxThread.Start();

				if (conf.RunFromTrayNow)
				{
					betaClock = new System.Windows.Forms.Timer();
					betaClock.Interval = 2255555;
					betaClock.Tick += new System.EventHandler (betaWarnTick);
					betaClock.Start();
				}
			}
			else
			#endif // BETA
				
			#if WINDOWS
				// First, second or third time ChronosXP is run?  Notify user that its running in the background with a balloon window
				if (conf.Run < 3 && conf.RunFromTrayNow)
				{
					auxThread = new Thread (new ThreadStart (threadBgRunning));
					auxThread.Start();
				}
			#endif // WINDOWS
			
			//if !DEBUG
				CheckUpdates();
			//endif / DEBUG

			// standalone mode?  display the Planetary Hours Calendar
			if (!conf.RunFromTrayNow)
				ShowCalendar();

			// run with /properties argument?  Show the Properties form.
			if (showproperties)
				ShowProperties();
		}

        //[STAThread]
        //public static void Main()
        //{
        //    Application.EnableVisualStyles();
        //    try
        //    {
        //        new ChronosXPCore();
        //        Application.Run();
        //    }
        //    catch (Exception ex)
        //    {
        //        try
        //        {
        //            Application.Run(new ErrorReport(ex));
        //        }
        //        catch
        //        {
        //            new ErrorReport(ex).Show();
        //        }
        //    }
        //}

        public void Shutdown()
        {
            // Program.Main checks this so that a shutdown requested from the constructor
            // (before Application.Run starts) does not leave a message loop running.
            ShutdownRequested = true;
            Application.Exit();
        }
		#endregion

		#region Hooks, threads and timers
		protected override void WndProc (ref Message m)
		{
			if (m.Msg == PInvoke.WM_HOTKEY)
				ShowCalendar();

			base.WndProc (ref m);
		}

		private void trayTick (object sender, System.EventArgs e)
		{
			updateTray();
		}

		/// <summary>
		/// Check to see if the planetary hour (or lunar phase, if applicable) has changed.  If so, notify the user
		/// </summary>
		private void phTick (object sender, System.EventArgs e)
		{
			if (pHours.CurrentHour() == (PlanetaryHours.Planet) (-1))		// Planetary day changed; recalculate hours
			{
				pHours = new PlanetaryHours (conf);
				// Just in case new hour is same as last recorded hour, but the computer had been suspended
				currentHour = (PlanetaryHours.Planet) (-2);  // -2 because pHours.CurrentHour() could be -1 upon error.
			}

			if (currentHour != pHours.CurrentHour())
			{
				currentHour = pHours.CurrentHour();
				NotifyUser();
				updateTray();
				if (currentHour == (PlanetaryHours.Planet) (-1))
					ErrorMessage ("An internal error has occured",
						new Exception ("pHours.CurrentHour returns -1 twice - unable to calculate planetary hours"), conf);
			}
			
			if (conf.Caption == Config.CaptionType.LunarPhase && currentPhase != localPhases.CurrentPhase(DateTime.UtcNow))
			{
				localPhases = new LunarPhase(DateTime.UtcNow);
				currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);				
				updateTray();
			}
		}

		/// <summary>
		/// Check for new versions of ChronosXP in the background.  If a new version exists, prompt the user to upgrade.
		/// </summary>
		private void threadCheckUpdate()
		{
            if (conf.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = conf.CurrentCulture;
            }
			if (!fastCheckUpdate)
				Thread.Sleep (333000);	// Wait 5 minutes before checking for update
			try
			{
				using (WebClient client = new WebClient())
				{
					StreamReader reader = new StreamReader (client.OpenRead (String.Format (Config.URLUpdate, Application.ProductVersion,
					                                                                        Environment.OSVersion.Version,
					                                                                        conf.RunFromTrayNow ? "T" : "S",
					                                                         								Thread.CurrentThread.CurrentUICulture.ToString(),
					                                                                        Thread.CurrentThread.CurrentCulture.ToString(), Config.Platform)));
					string s, location = null, relname = null; int ver = 0;
					while ((s = reader.ReadLine()) != null)
					{
						Match m = Regex.Match(s, Config.UpdatePattern);
						if (m.Success)
						{
							location = m.Groups["location"].Value;
							ver = int.Parse (m.Groups["ver"].Value);
							relname = m.Groups["name"].Value;
							break;
						}
					}
					reader.Close();
					if (ver > Config.InternalVersion)
					{
						Upgrade upg = new Upgrade(conf, location, relname);
						upg.ShowDialog();
					}
				}
			}
			catch // Fail quietly...
			{
			}
		}

		/// <summary>
		/// Notify the user that ChronosXP is running in the background with a balloon window.
		/// </summary>
		private void threadBgRunning()
		{
            if (conf.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = conf.CurrentCulture;
            }
			Thread.Sleep(22000);
			ShowInfoBalloon(conf.GetString ("Core.BackgroundTitle"), conf.GetString ("Core.BackgroundText"));
		}

		#if BETA
		private void threadBetaExpired()
		{
            if (conf.CurrentCulture != null)
            {
                Thread.CurrentThread.CurrentUICulture = conf.CurrentCulture;
            }
			Thread.Sleep (4444);
			DialogResult res = MessageBox.Show (conf.GetString ("Core.BetaExpiredWebPage"), "ChronosXP",
				MessageBoxButtons.YesNo, MessageBoxIcon.Information);
			if (res == DialogResult.Yes)
				conf.LaunchBrowser (Config.URL);
		}

		// Nag the user to upgrade if this beta version is expired
		private void betaWarnTick (object sender, System.EventArgs e)
		{
			ShowErrorBalloon (conf.GetString ("Core.BetaExpiredBalloonTitle"),
				String.Format (conf.GetString ("Core.BetaExpiredBalloonText"), Config.URL));
		}
		#endif

		/// <summary>
		/// Check for a new version of ChronosXP
		/// </summary>
		public void CheckUpdates()
		{
			if (!checkUpdate)
				return;
		#if BETA
			if (conf.CheckUpgrade && updateThread == null)
		#else
			// check for upgrades, only on Mondays if in tray mode.  (it would probably be better to code it to save the date of the last check and work
			// in relation to that)
			if (conf.CheckUpgrade && updateThread == null && (!conf.RunFromTray || DateTime.Today.DayOfWeek == DayOfWeek.Monday))
		#endif
			{
				updateThread = new Thread (new ThreadStart (threadCheckUpdate));
				updateThread.Start();
			}
		}

		/// <summary>
		/// Abort checking for a new version of ChronosXP (this is needed when the user disables checking, but the updateThread is
		/// already running.
		/// </summary>
		public void CancelCheckUpdates()
		{
			if (updateThread != null && updateThread.IsAlive)
				updateThread.Abort();
		}
		#endregion

		#region NotifyIcon and its ContextMenu
		/// <summary>
		/// User double clicked on the tray icon?  show the Calendar form
		/// </summary>
		private void trayIconClick (object sender, System.EventArgs e)
		{
			ShowCalendar();
		}
			
		private void itemCalendarClick (object sender, System.EventArgs e)
		{
			ShowCalendar();
		}
		
		private void itemPhasesClick (object sender, System.EventArgs e)
		{
			ShowPhases();
		}

		private void itemAboutClick (object sender, System.EventArgs e)
		{
			ShowAbout();
		}

		private void itemLicenseClick (object sender, System.EventArgs e)
		{
			ShowLicense();
		}

		private void itemHomePageClick (object sender, System.EventArgs e)
		{
			conf.LaunchBrowser(Config.URL);
		}

		private void itemPropertiesClick (object sender, System.EventArgs e)
		{
			ShowProperties();
		}

		private void itemExitClick (object sender, System.EventArgs e)
		{
			Shutdown();
		}

		/// <summary>
		/// Update the tray icon
		/// </summary>
		private void updateTray()
		{
			if (conf.Caption == Config.CaptionType.LunarPhase && localPhases == null)
			{
				localPhases = new LunarPhase(DateTime.UtcNow);
				currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);
			}
			
			if (currentHour == (PlanetaryHours.Planet) (-1))
			{
				pHours = new PlanetaryHours (conf);
				currentHour = pHours.CurrentHour();
			}

			if (currentHour == (PlanetaryHours.Planet) (-1))
			{
				trayIcon.Icon = new Icon(conf.Fx.ResourceStream("Exclamation.ico"));
				ErrorMessage ("A serious internal error has occured; please report this error to the author of ChronosXP at " + Config.Email,
				              new Exception ("ChronosXP.PlanetaryHours failed twice at " + DateTime.Now.ToString() + "; unable to determine planetary hour."),
					conf);
			}
			else
			{

				try
				{
					if (conf.Fx.IconSet.Equals("Silver") && pHours.CurrentHour() == PlanetaryHours.Planet.Saturn)
						trayIcon.Icon = conf.Fx.GlyphIconSmall("Tray.Saturn");
					else
						trayIcon.Icon = conf.Fx.GlyphIconSmall(pHours.CurrentEnglishHour());
					string stat;
					switch (conf.Caption)
					{
						default:
						case Config.CaptionType.HourNumber:
							stat = pHours.HourOfDay(DateTime.Now);
							break;
						case Config.CaptionType.HouseOfMoment:
							stat = pHours.HouseOfMoment(DateTime.Now);
							break;
						case Config.CaptionType.LunarPhase:
							int i = localPhases.PhaseNum(DateTime.UtcNow);
							if (i == -1)
								i = 7;
							stat = conf.GetString(LunarPhase.PhaseName[i]);
							break;
					}
					string s = pHours.DayString() +  "\r\n" + pHours.CurrentHourString() + "\r\n" + stat;
					// Too bad we're limited to 64 chars, because this would be nice...
				 	//string s = pHours.DayString() +  ", " + pHours.CurrentHourString() + "\r\n" +
				 	//	pHours.HourOfDay(DateTime.Now) + ", " + pHours.HouseOfMoment(DateTime.Now) + "\r\n" +
				 	//	localPhases.PhaseNum(DateTime.UtcNow);
				 	if (s.Length >= 64)
				 		s = s.Remove(60, s.Length - 60) + "...";
				 	trayIcon.Text = s;
				}
				catch (Exception ex)
				{
					trayIcon.Icon = new Icon(conf.Fx.ResourceStream("Exclamation.ico"));
					ErrorMessage ("Unable to set glyph icon.  [CurrentEnglishHour=" + pHours.CurrentEnglishHour() + "]", ex);
				}
			}
		}

		/// <summary>
		/// Create the tray icon
		/// </summary>
		private void initTray()
		{
			trayIcon = new NotifyIcon();

			initContextMenu();

			trayIcon.DoubleClick += new System.EventHandler (trayIconClick);
			//trayIcon.MouseUp += new System.Windows.Forms.MouseEventHandler (trayIconMouseUp);
			trayIcon.Visible = true;
			trayIcon.ContextMenu = trayContextMenu;
		}

		/// <summary>
		/// Create the context menu
		/// </summary>
		private void initContextMenu()
		{
			if (itemCalendar == null)
			{
				itemCalendar = new MenuItem();
				itemCalendar.DefaultItem = true;
				itemCalendar.Shortcut = Shortcut.AltF11;
				itemCalendar.ShowShortcut = true;
				itemCalendar.Click += new System.EventHandler (itemCalendarClick);
			}
			itemCalendar.Text = conf.GetString ("ContextMenu.Calendar");
			
			if (itemPhases == null)
			{
				itemPhases = new MenuItem();
				itemPhases.Click += new System.EventHandler(itemPhasesClick);
			}
			itemPhases.Text = conf.GetString("ContextMenu.LunarPhases");

			if (itemProperties == null)
			{
				itemProperties = new MenuItem();
				itemProperties.Click += new System.EventHandler (itemPropertiesClick);
			}
			itemProperties.Text = conf.GetString("ContextMenu.Properties");

			if (itemAbout == null)
			{
				itemAbout = new MenuItem();
				itemAbout.Click += new System.EventHandler (itemAboutClick);
			}
			itemAbout.Text = conf.GetString("ContextMenu.About");

			if (itemLicense == null)
			{
				itemLicense = new MenuItem();
				itemLicense.Click += new System.EventHandler (itemLicenseClick);
			}
			itemLicense.Text = conf.GetString("ContextMenu.License");

			if (itemHomePage == null)
			{
				itemHomePage = new MenuItem();
				itemHomePage.Click += new System.EventHandler (itemHomePageClick);
			}
			itemHomePage.Text = conf.GetString("ContextMenu.HomePage");

			if (itemSeparator == null)
			{
				itemSeparator = new MenuItem();
				itemSeparator.Text = "-";
			}

			if (itemExit == null)
			{
				itemExit = new MenuItem();
				itemExit.Click += new System.EventHandler (itemExitClick);
			}
			itemExit.Text = conf.GetString("ContextMenu.Exit");

			if (trayContextMenu == null)
			{
				trayContextMenu = new ContextMenu (new MenuItem[] { itemPhases, itemHomePage, itemLicense, itemProperties, itemAbout,
																	  itemSeparator, itemCalendar, itemExit });
			}
			else
			{
				trayContextMenu.MenuItems.Clear();
				trayContextMenu.MenuItems.AddRange (new MenuItem[] { itemPhases, itemHomePage, itemLicense, itemProperties, itemAbout,
																	   itemSeparator, itemCalendar, itemExit });
			}
		}
		#endregion

		#region Win32 (PlaySound/Shell_NotifyIconEx)
		// We use NIIF_NOSOUND here since this function is only used to display balloon windows;  at this time any optional sound is
		// played with PInvoke.PlaySound.
		public void ShowBalloon (string title, string text)
		{
			notifyBalloon (title, text, PInvoke.NIIF_NOSOUND);
		}

		public void ShowInfoBalloon (string title, string text)
		{
			notifyBalloon (title, text, PInvoke.NIIF_INFO);
		}

		public void ShowErrorBalloon (string title, string text)
		{
			notifyBalloon (title, text, PInvoke.NIIF_ERROR);
		}

		private void notifyBalloon (string title, string text, Int32 flags)
		{
			// NotifyIcon.ShowBalloonTip replaces the old reflection into NotifyIcon's private fields plus an ANSI
			// Shell_NotifyIconA call, which mangled non-Latin text and depended on framework internals.
			if (trayIcon == null)
				return;
			try
			{
				trayIcon.ShowBalloonTip (11200, title, text, flags == PInvoke.NIIF_ERROR ? ToolTipIcon.Error : ToolTipIcon.Info);
			}
			catch
			{
			}
		}

		/// <summary>
		/// Play a sound alias, such as "Windows Logon"; called when the planetary hour has changed if the user has configured it
		/// </summary>
		public void PlaySoundAlias (string name)
		{
			PInvoke.PlaySound (name, 0, PInvoke.SND_ASYNC | PInvoke.SND_NOWAIT | PInvoke.SND_ALIAS);
		}

		/// <summary>
		/// Play a wave file (called when the planetary hour has changed if the user has configured it)
		/// </summary>
		public void PlaySoundFile (string name)
		{
			PInvoke.PlaySound (name, 0, PInvoke.SND_ASYNC | PInvoke.SND_NOWAIT | PInvoke.SND_FILENAME);
		}

		/// <summary>
		/// Play a sound when the planetary hour has changed
		/// </summary>
		private void notifySound()
		{
			switch (conf.Sound)
			{
				case Config.SoundTypes.SystemSelect:
				case Config.SoundTypes.WaveSelect:
					break;
				case Config.SoundTypes.System:
					for (int i = 0; i < conf.EventCount; i++)
					{
						if (conf.SoundName.Equals (conf.EventList[i].Name))
						{
							PlaySoundAlias (conf.EventList[i].Label);
							break;
						}
					}
					break;
				case Config.SoundTypes.Wave:
					if (conf.SoundFile != null)
						PlaySoundFile (conf.SoundFile);
					else
						ErrorMessage ("ChronosXP is misconfigured.  If you continue to recieve this error message, reinstall ChronosXP,",
							new Exception("Sound == Config.SoundTypes.Wave while SoundFile == null"), conf);
					break;
			}
		}
		#endregion

		#region ErrorMessage, RefreshHours, NotifyUser, appExit, appIdle
		/// <summary>
		/// Abort any child threads upon exit
		/// </summary>
		private void appExit (object sender, System.EventArgs e)
		{
			try
			{
				if (conf.RunFromTrayNow)
					PInvoke.UnregisterHotKey (Handle, 101);
				if (trayIcon != null)
					trayIcon.Visible = false;
				if (conf.lastNw != null)
					conf.lastNw.Close();
			}
			catch { }
			finally
			{
				if (updateThread != null && updateThread.IsAlive)
					updateThread.Abort();
				if (auxThread != null && auxThread.IsAlive)
					auxThread.Abort();
			}
		}
		
		/// <summary>
		/// Call GC.Collect() periodically when idle.
		/// </summary>
		private void appIdle (object sender, System.EventArgs e)
		{
			if (lastCleanup.AddHours(6).CompareTo(DateTime.Now) > 0)
			{
				lastCleanup = DateTime.Now;
				GC.Collect();
			}
		}

		/// <summary>
		/// Notify the user that the planetary hour has changed with their preferred method, if applicable
		/// </summary>
		public void NotifyUser()
		{
		#if WINDOWS
			if (conf.NotifyHour)
			{
				try
				{
					NotifyWindow nw = new NotifyWindow (pHours, conf);
					nw.Start();
					// ... NotifyWindow will Close and Dispose itself
				}
				catch	// if for some reason the NotifyWindow fails, display a balloon window
				{
					ShowBalloon (pHours.CurrentHourString(), pHours.DayString());
				}
			}

			notifySound();
		#endif
		}

		/// <summary>
		/// This is called after the user clicks "Apply" or "OK" in the Properties form, in case the locality has changed.
		/// </summary>
		public void RefreshHours()
		{
			pHours = new PlanetaryHours (conf);
			if (pHours.CurrentHour() == (PlanetaryHours.Planet) (-1))
				ErrorMessage ("Unable to determine the current planetary hour.",
					new Exception("pHours.GetHour returns null at UTC" + DateTime.UtcNow.ToString()), conf);

			initContextMenu();
			if (conf.Caption == Config.CaptionType.LunarPhase)
			{
				localPhases = new LunarPhase(DateTime.UtcNow);
				currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);
			}
			updateTray();
			if (currentHour != pHours.CurrentHour())
			{
				currentHour = pHours.CurrentHour();
				NotifyUser();
			}
			phClock.Interval = conf.Interval();
		}

		/// <summary>
		/// Generic error message report
		/// </summary>
		// ShowDialog rather than Application.Run: Run throws if a message loop is already running, which
		// turned any error after startup into a second, unhandled exception.
		public void ErrorMessage (string message, Exception ex, Config conf)
		{
			new ErrorReport (message, ex, conf, pHours).ShowDialog();
		}

		public void ErrorMessage (string message, Exception ex)
		{
			new ErrorReport (message, ex, conf, pHours).ShowDialog();
		}

		/// <summary>
		/// Run by Program in /selftest mode once startup has finished: log the computed hours and open every
		/// window, so that CI catches configuration, calculation and form-construction failures.
		/// </summary>
		public void RunSelfTest()
		{
			Program.Log ("Platform=" + Config.Platform + " DefaultPlace=" + conf.DefaultPlace + " DefaultLocality=" + conf.DefaultLocality
				+ " Places=" + conf.PlaceNum);
			PlanetaryHours ph = new PlanetaryHours (conf);
			Program.Log ("Sunrise=" + ph.RS.Sunrise.ToString ("yyyy-MM-dd HH:mm") + " Sunset=" + ph.RS.Sunset.ToString ("yyyy-MM-dd HH:mm")
				+ " Day=" + ph.Day + " CurrentHour=" + ph.CurrentHour());
			for (int i = 0; i < 24; i++)
				Program.Log ("Hour " + (i + 1) + "=" + ph.Hours[i].Hour + " " + ph.Hours[i].StartTime.ToString ("yyyy-MM-dd HH:mm"));
			if (ph.CurrentHour() == (PlanetaryHours.Planet) (-1))
				Program.Fail ("Current planetary hour could not be determined", "");

			ShowCalendar();
			ShowPhases();
			ShowAbout();
			ShowProperties();
			ShowLicense();
			Application.DoEvents();
			Program.Log ("Opened calendar, phases, about, properties and license windows");
			Program.Pass();
		}
		#endregion

		#region Forms
		public void ShowCalendar()
		{
            if (!FormCalendarOpen)
                FormCalendar = new Calendar(conf);
            else if (FormCalendar.WindowState == FormWindowState.Minimized)
                FormCalendar.WindowState = FormWindowState.Normal;
            FormCalendar.Show();
            FormCalendar.Activate();
		}
		
		public void ShowPhases()
		{
			ShowPhases(DateTime.Now);
		}
		
		public void ShowPhases(DateTime when)
		{
			if (!FormPhasesOpen)
				FormPhases = new Phases(when, conf);
			else if (FormPhases.WindowState == FormWindowState.Minimized)
				FormPhases.WindowState = FormWindowState.Normal;
			FormPhases.Show();
			FormPhases.Activate();
		}

		public void ShowAbout()
		{
            if (!FormAboutOpen)
            {
                FormAbout = new About(conf);
            }
            else if (FormAbout.WindowState == FormWindowState.Minimized)
            {
                FormAbout.WindowState = FormWindowState.Normal;
            }
			FormAbout.Show();
			FormAbout.Focus();
		}

		public void ShowLicense()
		{
			if (!FormLicenseOpen)
				FormLicense = new ShowRichTextBox (conf);
			else if (FormLicense.WindowState == FormWindowState.Minimized)
				FormLicense.WindowState = FormWindowState.Normal;
			if (FormLicenseOpen)
			{
				FormLicense.Show();
				FormLicense.Focus();
			}
		}

		public void ShowProperties()
		{
			if (!FormPropertiesOpen)
				FormProperties = new PropertiesBox (conf);
			else if (FormProperties.WindowState == FormWindowState.Minimized)
				FormProperties.WindowState = FormWindowState.Normal;

			FormProperties.Show();
			FormProperties.Focus();
		}
		#endregion
	}
}
