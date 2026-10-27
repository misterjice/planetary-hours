#region Copyright © 2003-2005 by Robert Misiak
// ChronosXP - Calendar.cs
// Copyright © 2003-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Text;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Collections;
using System.Resources;
using System.Reflection;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace ChronosXP
{
	/// <summary>
	/// This is the program's main window; the Planetary Hours Calendar
	/// </summary>
	public sealed class Calendar : System.Windows.Forms.Form
	{
		private XPLinkLabel linkLabelPrint;
		private XPLinkLabel linkLabelAbout;
		private XPLinkLabel linkLabelLocality;
		private XPLinkLabel linkLabelPhase;
		private System.Windows.Forms.ListView listViewNightHours;
		private XPLinkLabel linkLabelProperties;
		private System.Windows.Forms.StatusBarPanel statusBarHOD;
		private System.Windows.Forms.StatusBarPanel statusBarDay;
        private ChronosXP.VisualPanel visualPanelLeft;
		private ChronosXP.VisualPanel visualPanelRight;
		private System.Windows.Forms.StatusBarPanel statusBarTime;
		private System.Windows.Forms.ListView listViewDayHours;
		private System.Windows.Forms.StatusBarPanel statusBarLocation;
		private System.Windows.Forms.StatusBarPanel statusBarHour;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private ChronosXP.VisualMonthCalendar visualCalendar;
        private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.ComponentModel.Container components = null;
		public const string Copyright = "Calendar.cs, Copyright © 2003-2009 by Robert Misiak";
		public bool FormLocalityOpen = false, FormPrintOpen = false;
		private Config conf;
		private System.Windows.Forms.Timer sbClock;
		private Locality formLocality;
		private PrintHours formPrint;
		private PlanetaryHours localpHours;
		private Place effectivePlace;
		private PlanetaryHours.Planet currentHour;
		private System.Windows.Forms.Label labelDayOf, labelSunrise, labelSunset;
		private System.Windows.Forms.PictureBox pictureBoxDayGlyph;
		private System.Drawing.Font lvFont, lvBoldFont, hdLgFont, hdSmFont;
		private bool defaultPlace;
		private enum BoxStates { Normal, Hot, Pushed };
		private BoxStates cbState = BoxStates.Normal, tmState = BoxStates.Normal;
		private Rectangle cbArea = new Rectangle(0, 0, 0, 0), tmArea = new Rectangle(0, 0, 0, 0);
		private ContextMenu placeMenu, zoneMenu;
        private LunarPhase localPhases;
        private XPLinkLabel xpLinkLabelDonate;
        private XPLinkLabel xpLinkLabelFAQ;
		private LunarPhase.Phase currentPhase;

        #region Constructor
		// All things bright and beautiful, all creatures great and small...
		// All things wise and wonderful, the Lord God made them all.
		public Calendar (Config conf)
		{
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            
            InitializeComponent();

			this.conf = conf;
			conf.Core.FormCalendarOpen = true;
			Text = String.Format (conf.GetString ("Calendar.Title"), Config.FormatVersion(
				#if BETA
					conf.Core.Photogenic
				#endif
			));

			// We size these and calculate their locations ourself in this.updateLabels, so initialize them here instead of via the Windows Forms
			// Designer interface.
			pictureBoxDayGlyph = new PictureBox();
            //if (!PInvoke.WindowsAeroGlass())
                pictureBoxDayGlyph.BackColor = Color.Transparent;
			hdLgFont = new Font (visualPanelRight.Font.Name, 13.25F);  hdSmFont = new Font (visualPanelRight.Font.Name, 8.25F);
			labelDayOf = new Label();  labelSunrise = new Label();  labelSunset = new Label();
			labelDayOf.Font = hdLgFont;  labelSunrise.Font = hdSmFont;  labelSunset.Font = hdSmFont;
            //if (!PInvoke.WindowsVistaGlass())
            //{
                labelDayOf.BackColor = Color.Transparent;
                labelSunrise.BackColor = Color.Transparent;
                labelSunset.BackColor = Color.Transparent;
            //}
			visualPanelRight.Controls.AddRange (new Control[] { pictureBoxDayGlyph, labelDayOf, labelSunrise, labelSunset });

			// Used in ListView's
			lvFont = new Font (listViewDayHours.Font.Name, 8.25F, FontStyle.Regular);
			lvBoldFont = new Font (listViewDayHours.Font.Name, 8.25F, FontStyle.Bold);

			// conf.UseGradient is "Use advanced window graphics" in the Properties form
			if (conf.UseGradient)
			{
				// Draw our own StatusBarPanel's if conf.UseGradient
				statusBarDay.Style = StatusBarPanelStyle.OwnerDraw;
				statusBarHour.Style = StatusBarPanelStyle.OwnerDraw;
				statusBarHOD.Style = StatusBarPanelStyle.OwnerDraw;
				statusBarLocation.Style = StatusBarPanelStyle.OwnerDraw;
				statusBarTime.Style = StatusBarPanelStyle.OwnerDraw;
				statusBar.DrawItem += new System.Windows.Forms.StatusBarDrawItemEventHandler(drawStatusBarPanel);

				// A correction for the silver ("Metallic") Windows XP visual theme.
				if (PInvoke.VisualStylesEnabled())
				{
					StringBuilder fn = new StringBuilder(), tn = new StringBuilder(56), cn = new StringBuilder();
					try
					{
						PInvoke.GetCurrentThemeName(fn, 0, tn, 56, cn, 0);
						if (tn.ToString().Equals("Metallic"))
						{
							visualCalendar.TitleBarColorRight = Color.Silver;
							visualCalendar.TitleBarColorLeft = Color.White;
						}
					}
					catch { }
				}
				
				Blend titleblend = new Blend();
				titleblend.Factors = new float[] { 0.0F, 0.0F, 0.15F, 0.3F, 0.55F, 0.8F };
				titleblend.Positions = new float[] { 0.0F, 0.2F, 0.4F, 0.6F, 0.8F, 1.0F };
				visualCalendar.TitleBarBlend = titleblend;

				Blend leftblend = new Blend();
				leftblend.Factors = new float[] { 0.4F, 0.6F, 0.6F, 0.8F, 0.8F, 0.9F, 1.0F };
				leftblend.Positions = new float[] { 0.0F, 0.2F, 0.4F, 0.6F, 0.8F, 0.9F, 1.0F };
				visualPanelLeft.Blend = leftblend;

				Blend rightblend = new Blend();
				rightblend.Factors = new float[] { 0.2F, 0.4F, 0.6F, 0.75F, 0.9F };
				rightblend.Positions = new float[]  { 0.0F, 0.25F, 0.5F, 0.75F, 1.0F };
				visualPanelRight.Blend = rightblend;
			}
			else
			{
				visualPanelLeft.VisualStyle = false;
				visualPanelRight.VisualStyle = false;
				visualCalendar.VisualStyle = false;
				visualCalendar.TitleBarColorRight = SystemColors.InactiveCaptionText;
			}

			Closing += new System.ComponentModel.CancelEventHandler(formClosing);
			Activated += new System.EventHandler(formActivated);
			visualCalendar.SelectedDateChanged += new System.EventHandler(visualCalendar_DateChanged);

			SetLocality(conf.DefaultPlace);
			visualCalendar.SelectedDate = localpHours.Hours[0].StartTime;

			sbClock = new System.Windows.Forms.Timer();
			sbClock.Interval = conf.Interval();
			sbClock.Start();
			sbClock.Tick += new System.EventHandler(sbTick);

			visualCalendar.SelectedDate = localpHours.Hours[0].StartTime;
			visualCalendar.ToolTipText = conf.Res.GetString("Calendar.MCToolTip");
			visualCalendar.TitleBarToolTipText = conf.Res.GetString("Calendar.TitleToolTip");

			// when in standalone mode, we replace buttonClose.Text with "Exit"
            //if (!conf.RunFromTrayNow)
            //    buttonClose.Text = conf.Res.GetString("Calendar.Exit");

			statusBar.MouseMove += new MouseEventHandler(sbMouseMove);
			statusBar.MouseDown += new MouseEventHandler(sbMouseDown);
			statusBar.MouseUp += new MouseEventHandler(sbMouseUp);
			statusBar.MouseLeave += new EventHandler(sbMouseLeave);

            if (Config.WindowsVistaOrHigher() && PInvoke.VisualStylesEnabled())
            {
                const int correction = 42;
                Height += correction;
                visualPanelLeft.Height += correction;
                visualPanelRight.Height += correction;
                listViewDayHours.Height += correction;
                listViewNightHours.Height += correction;
                //buttonClose.Top += correction;
            }

            if (Thread.CurrentThread.CurrentUICulture.Name == "he")
            {
                Point dayHours = listViewDayHours.Location;
                Point nightHours = listViewNightHours.Location;
                listViewDayHours.Location = nightHours;
                listViewNightHours.Location = dayHours;
                listViewDayHours.RightToLeft = RightToLeft.Yes;
                listViewNightHours.RightToLeft = RightToLeft.Yes;
            }

			#if !MONO
			foreach (Control c in visualPanelLeft.Controls)
			{
				if (c is LinkLabel)
				{
					LinkLabel ll = c as LinkLabel;
					ll.ActiveLinkColor = SystemColors.InactiveCaptionText;
				}
			}
			#endif
		}
		#endregion

        //protected override void OnLoad(EventArgs e)
        //{
        //    base.OnLoad(e);
        //    if (PInvoke.WindowsVistaGlass())
        //    {
                //int ht = labelDayOf.Height + 4;
                //Height += ht;
                //PInvoke.Margins m = new PInvoke.Margins();
                //m.cxLeftWidth = 0;
                //m.cxRightWidth = 0;
                //m.cyTopHeight = ht;
                //m.cyBottomHeight = 0;
                //visualPanelLeft.Top += ht;
                //visualPanelRight.Top += ht;
                //Panel p = new Panel();
                //Controls.Add(p);
                //p.Size = new Size(Bounds.Width, ht);
                //p.Location = new Point(0, 0);
                //p.Paint += new PaintEventHandler(Panel_Paint);
                //pictureBoxDayGlyph.Visible = false;
                //labelDayOf.Visible = false;
                //labelSunrise.Visible = false;
                //labelSunset.Visible = false;
                //int correction = visualCalendar.Top - listViewNightHours.Top;
                //listViewDayHours.Top = listViewNightHours.Top = visualCalendar.Top;
                //listViewDayHours.Height = listViewNightHours.Height = (visualPanelRight.Height - buttonClose.Height - 52);
                //PInvoke.DwmExtendFrameIntoClientArea(Handle, ref m);
        //        updateView();
        //    }
        //}

        private void Panel_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.Black);
        }

        //protected override void OnPaintBackground(PaintEventArgs e)
        //{
        //    base.OnPaintBackground(e);
        //    if (PInvoke.WindowsVistaGlass())
        //        e.Graphics.Clear(Color.Black);
        //}
		
		#region StatusBar
		// Update the StatusBar text
		private void updateBar()
		{
			statusBarDay.Text = localpHours.DayString();
			statusBarDay.Icon = conf.Fx.GlyphIconSmall (localpHours.EnglishDay());
			statusBarHour.Text = localpHours.CurrentHourString();
			statusBarHour.Icon = conf.Fx.GlyphIconSmall (localpHours.CurrentEnglishHour());
			switch (conf.Caption)
			{
				case Config.CaptionType.HourNumber:
					statusBarHOD.Text = localpHours.HourOfDay(effectivePlace.CurrentTime());
					break;
				case Config.CaptionType.HouseOfMoment:
					statusBarHOD.Text = localpHours.HouseOfMoment(effectivePlace.CurrentTime());
					break;
				case Config.CaptionType.LunarPhase:
					int i = localPhases.PhaseNum(DateTime.UtcNow);
					if (i == -1)
						i = 7;
					statusBarHOD.Text = conf.GetString(LunarPhase.PhaseName[i]);
					break;
			}
			statusBarLocation.Text = effectivePlace.Name;
		}

		// Draw our own StatusBarPanel's
		private void drawStatusBarPanel (object sender, System.Windows.Forms.StatusBarDrawItemEventArgs e)
		{
			// First paint the background
            using (LinearGradientBrush lgb =
                        new LinearGradientBrush(e.Bounds, Color.WhiteSmoke, SystemColors.Control, LinearGradientMode.Horizontal))
                e.Graphics.FillRectangle(lgb, e.Bounds);
	
			// Next place the text (and possibly an icon) centered in the StatusBar
			using (StringFormat sf = new StringFormat())
			{
				sf.LineAlignment = StringAlignment.Center;
				sf.Trimming = StringTrimming.EllipsisCharacter;
				sf.FormatFlags = StringFormatFlags.FitBlackBox | StringFormatFlags.NoWrap;
				
				const int cbheight = 16, cbpadding = 4;
				// VStyles: cbwidth = cbheight - 1 to compensate for what would be the shadow, although DrawThemeBackground() doesn't
				// draw the shadow, and neither do we.
				int cbwidth = PInvoke.VisualStylesEnabled() ? cbheight - 1 : cbheight;
				///
				/// Location Panel (with ComboBox button)
				/// 
				if (e.Panel.Equals(statusBarLocation))
				{
					sf.Alignment = StringAlignment.Near;
					SizeF sz = e.Graphics.MeasureString (e.Panel.Text, statusBar.Font, e.Bounds.Width - cbwidth - cbpadding, sf);
					int rx = e.Bounds.X + ((e.Bounds.Width - (int)Math.Ceiling(sz.Width + cbwidth + cbpadding)) / 2);
					Rectangle rText;
					Rectangle rCB;
					if (PInvoke.VisualStylesEnabled())
					{
						rText = new Rectangle (rx, e.Bounds.Y, (int)Math.Ceiling(sz.Width), e.Bounds.Height);
						rCB = new Rectangle (rx + (int)Math.Ceiling(sz.Width) + cbpadding, e.Bounds.Y + ((e.Bounds.Height - cbheight) / 2),
	                               cbwidth, cbheight);
						drawModernComboBoxButton(e.Graphics, rCB, cbState);
					}
					else
					{
						rText = new Rectangle (rx, e.Bounds.Y, (int)Math.Ceiling(sz.Width), e.Bounds.Height);
						rCB = new Rectangle (rx + (int)Math.Ceiling(sz.Width) + cbpadding, e.Bounds.Y + ((e.Bounds.Height - cbheight) / 2),
	                               cbwidth, cbheight);
						drawLegacyComboBoxButton(e.Graphics, rCB, cbState);
					}
					cbArea = rCB;
					e.Graphics.DrawString (e.Panel.Text, statusBar.Font, SystemBrushes.ControlText, rText, sf);
				}
				///
				/// Time Panel (also with ComboBox button)
				///
				else if (e.Panel.Equals(statusBarTime))
				{
					sf.Alignment = StringAlignment.Near;
					SizeF sz = e.Graphics.MeasureString (e.Panel.Text, statusBar.Font, e.Bounds.Width - cbwidth - (cbpadding / 2), sf);
					int rx = e.Bounds.X + ((e.Bounds.Width - (int)Math.Ceiling(sz.Width + cbwidth + (cbpadding / 2))) / 2);
					Rectangle rText;
					Rectangle rTm;
					if (PInvoke.VisualStylesEnabled())
					{
						rText = new Rectangle (rx, e.Bounds.Y, (int)Math.Ceiling(sz.Width), e.Bounds.Height);
						rTm = new Rectangle (rx + (int)Math.Ceiling(sz.Width) + cbpadding, e.Bounds.Y + ((e.Bounds.Height - cbheight) / 2),
	                               cbwidth, cbheight);
						drawModernComboBoxButton(e.Graphics, rTm, tmState);
					}
					else
					{
						rText = new Rectangle (rx, e.Bounds.Y, (int)Math.Ceiling(sz.Width), e.Bounds.Height);
						rTm = new Rectangle (rx + (int)Math.Ceiling(sz.Width) + cbpadding, e.Bounds.Y + ((e.Bounds.Height - cbheight) / 2),
	                               cbwidth, cbheight);
						drawLegacyComboBoxButton(e.Graphics, rTm, tmState);
					}
					tmArea = rTm;
					e.Graphics.DrawString (e.Panel.Text, statusBar.Font, SystemBrushes.ControlText, rText, sf);
				}
				///
				/// Normal, Centered Text
				/// 
				else if (e.Panel.Icon == null)
				{
					sf.Alignment = StringAlignment.Center;
					e.Graphics.DrawString (e.Panel.Text, statusBar.Font, SystemBrushes.ControlText, e.Bounds, sf);
				}
				/// 
				/// Text with icon
				/// 
				else
				{
					sf.Alignment = StringAlignment.Near;
					SizeF sz = e.Graphics.MeasureString (e.Panel.Text, statusBar.Font, e.Bounds.Width - e.Panel.Icon.Width - 2, sf);
					int reqwidth = (int) sz.Width + e.Panel.Icon.Width + 2;
					int rx = e.Bounds.X + (e.Bounds.Width - reqwidth) / 2;

					Rectangle rectIcon = new Rectangle (rx, e.Bounds.Y + ((e.Bounds.Height - e.Panel.Icon.Height) / 2), e.Panel.Icon.Width, e.Panel.Icon.Height);
					Rectangle rectText = new Rectangle (rx + e.Panel.Icon.Width + 2, e.Bounds.Y, e.Bounds.Width - e.Panel.Icon.Width, e.Bounds.Height);
					e.Graphics.DrawIcon (e.Panel.Icon, rectIcon);
					e.Graphics.DrawString (e.Panel.Text, statusBar.Font, SystemBrushes.ControlText, rectText, sf);
				}
			}
		}
		
		private void drawLegacyComboBoxButton (Graphics fx, Rectangle rt, BoxStates cbState)
		{
			ButtonState bs;
			if (cbState == BoxStates.Pushed)
				bs = ButtonState.Pushed;
			else
				bs = ButtonState.Normal;
			ControlPaint.DrawComboButton(fx, rt, bs);
		}

		private void drawModernComboBoxButton (Graphics fx, Rectangle rt, BoxStates cbState)
		{
			IntPtr hTheme = PInvoke.OpenThemeData(statusBar.Handle, "ComboBox");
			if (hTheme == IntPtr.Zero)
			{
				drawLegacyComboBoxButton(fx, rt, cbState);
				return;
			}
			PInvoke.Rect reT = new PInvoke.Rect(rt);
			PInvoke.Rect reC = new PInvoke.Rect(fx.VisibleClipBounds);
			IntPtr hDC = fx.GetHdc();
			int stateId = PInvoke.CBXS_NORMAL;
			if (cbState == BoxStates.Hot)
				stateId = PInvoke.CBXS_HOT;
			else if (cbState == BoxStates.Pushed)
				stateId = PInvoke.CBXS_PRESSED;
			PInvoke.DrawThemeBackground(hTheme, hDC, PInvoke.CP_DROPDOWNBUTTON, stateId, ref reT, ref reC);
			fx.ReleaseHdc(hDC);
			PInvoke.CloseThemeData(hTheme);
		}
		
		private void placeClick (object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			SetLocality(conf.GetPlace(mi.Text));
		}
		
		private void tmClick (object sender, EventArgs e)
		{
			MenuItem mi = sender as MenuItem;
			Place pl = new Place(effectivePlace.Name, effectivePlace.Longitude, effectivePlace.Latitude, mi.Text);
			conf.AddPlace(pl);
			SetLocality(pl);
		}
		
		private void sbMouseMove (object sender, MouseEventArgs e)
		{
			if (cbState == BoxStates.Normal && cbArea.Contains(e.X, e.Y) && tmState == BoxStates.Normal)
			{
				cbState = BoxStates.Hot;
				statusBar.Invalidate(cbArea);
			}
			else if (cbState == BoxStates.Hot && !cbArea.Contains(e.X, e.Y))
			{
				cbState = BoxStates.Normal;
				statusBar.Invalidate(cbArea);
			}
			if (tmState == BoxStates.Normal && tmArea.Contains(e.X, e.Y) && cbState == BoxStates.Normal)
			{
				tmState = BoxStates.Hot;
				statusBar.Invalidate(tmArea);
			}
			else if (tmState == BoxStates.Hot && !tmArea.Contains(e.X, e.Y))
			{
				tmState = BoxStates.Normal;
				statusBar.Invalidate(tmArea);
			}
		}
		
		private void sbMouseLeave (object sender, EventArgs e)
		{
			if (cbState != BoxStates.Normal)
			{
				cbState = BoxStates.Normal;
				statusBar.Invalidate(cbArea);
			}
			
			if (tmState != BoxStates.Normal)
			{
				tmState = BoxStates.Normal;
				statusBar.Invalidate(tmArea);
			}
		}
		
		private void sbMouseDown (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && cbState == BoxStates.Hot)
			{
				cbState = BoxStates.Pushed;
				statusBar.Invalidate(cbArea);
				statusBar.Focus();
			}
			else if (e.Button == MouseButtons.Left && tmState == BoxStates.Hot)
			{
				tmState = BoxStates.Pushed;
				statusBar.Invalidate(tmArea);
				statusBar.Focus();
			}
		}
		
		private void sbMouseUp (object sender, MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left && cbState == BoxStates.Pushed && cbArea.Contains(e.X, e.Y))
			{
				cbState = BoxStates.Normal;
				statusBar.Invalidate(cbArea);
				placeMenu.Show(statusBar, new Point(cbArea.X + (cbArea.Width / 2), cbArea.Y + cbArea.Height - 4));
			}
			else if (cbState != BoxStates.Normal)
			{
				cbState = BoxStates.Normal;
				statusBar.Invalidate(cbArea);
			}
			else if (e.Button == MouseButtons.Left && tmState == BoxStates.Pushed && tmArea.Contains(e.X, e.Y))
			{
				tmState = BoxStates.Normal;
				statusBar.Invalidate(tmArea);
				zoneMenu.Show(statusBar, new Point(tmArea.X + (tmArea.Width / 2), tmArea.Y + tmArea.Height - 4));
			}
			else if (tmState != BoxStates.Normal)
			{
				tmState = BoxStates.Normal;
				statusBar.Invalidate(tmArea);
			}
		}
		#endregion

		#region SetLocality, sbTick
		/// <summary>
		/// SetLocality: this is called both at form entry, and from the Locality form, to set the location for which planetary hours are
		/// displayed.
		/// </summary>
		public void SetLocality (Place where)
		{
			effectivePlace = where;
			defaultPlace = effectivePlace.DefaultPlace;
			localpHours = new PlanetaryHours (effectivePlace.CurrentTime(), effectivePlace, conf, true);
			currentHour = localpHours.CurrentHour();
			visualCalendar.TodayDate = localpHours.Hours[0].StartTime;
			localPhases = new LunarPhase(DateTime.UtcNow);
			currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);

			if (placeMenu == null)
				placeMenu = new ContextMenu();
			else
				placeMenu.MenuItems.Clear();
			string[] arr = new String[conf.PlaceNum];
			bool matched = false;  int i = 0;
			for (; i < conf.PlaceNum; i++)
			{
				arr[i] = conf.Places[i].Name;
				if (arr[i].Equals(where.Name))
					matched = true;
			}
			if (!matched)	
				arr[i] = where.Name;
			
			MenuItem mid = new MenuItem();
			mid.Click += new System.EventHandler(placeClick);
			mid.Text = conf.DefaultPlace.Name;
			if (conf.DefaultPlace.Equals(effectivePlace))
				mid.DefaultItem = true;
			placeMenu.MenuItems.Add(mid);
			mid = new MenuItem();
			mid.Text = "-";
			placeMenu.MenuItems.Add(mid);
			
			Array.Sort(arr);
			foreach (string st in arr)
			{
				if (conf.GetPlace(st).DefaultPlace)
					continue;
				MenuItem mi = new MenuItem();
				mi.Click += new System.EventHandler(placeClick);
				mi.Text = st;
				if (st.Equals(where.Name))
					mi.DefaultItem = true;
				placeMenu.MenuItems.Add(mi);
			}

			if (zoneMenu == null)
				zoneMenu = new ContextMenu();
			else
				zoneMenu.MenuItems.Clear();
			foreach (string st in Config.AllZones)
			{
				MenuItem mi = new MenuItem();
				mi.Text = st;
				mi.Click += new EventHandler (tmClick);
				if (!effectivePlace.UseSystemTime && effectivePlace.Zone.Equals(st))
					mi.DefaultItem = true;
				zoneMenu.MenuItems.Add(mi);
			}

			updateView();
			updateBar();

			if (effectivePlace.UseSystemTime)
			{
				statusBar.Visible = false;
				statusBar.Panels.Clear();
				statusBar.Panels.AddRange (new StatusBarPanel[] { statusBarDay, statusBarHour, statusBarHOD, statusBarLocation });
				statusBar.Visible = true;
			}
			else
			{
				statusBar.Visible = false;
				statusBar.Panels.Clear();
				statusBar.Panels.AddRange (new StatusBarPanel[] { statusBarDay, statusBarHour, statusBarHOD, statusBarTime, statusBarLocation });
				statusBar.Visible = true;
				statusBarTime.Text = effectivePlace.CurrentTime().ToShortTimeString() + " " + effectivePlace.ZoneAbbreviation;
			}
		}

		/// <summary>
		/// This is called every second/every minute (see Config.Interval) to check if planetary hours have changed; if so, update
		/// the StatusBar and the bolded hour in one of the ListView's (if appropriate)
		/// </summary>
		private void sbTick (object sender, System.EventArgs e)
		{
			try
			{
				// Planetary day changed; recalculate planetary hours
				if (localpHours.CurrentHourNum() == -1)
					localpHours = new PlanetaryHours (effectivePlace.CurrentTime(), effectivePlace, conf, true);

				// Current hour changed - update listView's and statusBar
				if (localpHours.CurrentHour() != currentHour)
				{
					updateView();
					updateBar();
					currentHour = localpHours.CurrentHour();
					visualCalendar.TodayDate = localpHours.Hours[0].StartTime;
				}
				
				if (conf.Caption == Config.CaptionType.LunarPhase && localPhases.CurrentPhase(DateTime.UtcNow) != currentPhase)
				{
					localPhases = new LunarPhase(DateTime.UtcNow);
					currentPhase = localPhases.CurrentPhase(DateTime.UtcNow);
					updateBar();
				}

				// if we aren't using the default place, display a "Current time in ____" label.
				if (!effectivePlace.UseSystemTime)
					statusBarTime.Text = effectivePlace.CurrentTime().ToShortTimeString() + " " + effectivePlace.ZoneAbbreviation;
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Periodic update failed at effective time of " + effectivePlace.CurrentTime().ToString(), ex);
			}
		}
		#endregion

		#region visualCalendar_TitleBarClick, formActivated, formClosing, RefreshForm, Dispose
		private void formActivated (object sender, System.EventArgs e)
		{
			TopMost = true;
			TopMost = false;
		}

		public void RefreshForm()
		{
			sbClock.Interval = conf.Interval();
			if (defaultPlace && effectivePlace.LongitudeAsDouble != conf.DefaultPlace.LongitudeAsDouble)
				SetLocality (conf.DefaultPlace);
			else
			{
				effectivePlace = conf.DefaultPlace;
				updateView();
			}
			updateBar();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
					components.Dispose();
				sbClock.Dispose();
				/*
				lvFont.Dispose();
				lvBoldFont.Dispose();
				hdLgFont.Dispose();
				hdSmFont.Dispose();
				*/
			}
			base.Dispose( disposing );
		}

		// when the form is closing, close the Locality or Date forms if they are opened, and update Core.FormCalendarOpen.
		private void formClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			conf.Core.FormCalendarOpen = false;
			if (FormLocalityOpen)
				formLocality.Close();
			if (FormPrintOpen)
				formPrint.Close();
			sbClock.Stop();
            if (!conf.RunFromTrayNow)
                conf.Core.Shutdown();
		}
		#endregion

		#region Calendar date changed
		/// <summary>
		/// This is called when visualCalendar.SelectedDate is changed (by the associated EventHandler); it is also called by sbTick when
		/// the planetary hour has changed.  It populates listViewDayHours and listViewNightHours with the planetary hours names and
		/// start times for the selected date.  If the current planetary day is displayed, it bolds the date of the current planetary hour.
		/// </summary>
		private void updateView()
		{
			// With the just we shall dwell.
			PlanetaryHours pHours = new PlanetaryHours (visualCalendar.SelectedDate, effectivePlace, conf);
			bool boldHour = false;

			if (visualCalendar.SelectedDate.Day == localpHours.Hours[0].StartTime.Day &&
					visualCalendar.SelectedDate.Month == localpHours.Hours[0].StartTime.Month &&
					visualCalendar.SelectedDate.Year == localpHours.Hours[0].StartTime.Year)
				boldHour = true;

			listViewDayHours.Items.Clear();
			listViewNightHours.Items.Clear();

			// Populate ListViewDayHours and ListViewNightHours
			for (int i = 0; i < 24; i++)
			{
                string[] s = new string[2];
                if (Thread.CurrentThread.CurrentUICulture.Name == "he")
                {
                    s[1] = pHours.PlanetName(pHours.Hours[i].Hour);
                    s[0] = conf.FormatTime(pHours.Hours[i].StartTime);
                }
                else
                {
                    s[0] = pHours.PlanetName(pHours.Hours[i].Hour);
                    s[1] = conf.FormatTime(pHours.Hours[i].StartTime);
                }
				ListViewItem lsi = new ListViewItem (s);
				if (boldHour && localpHours.CurrentHourNum() == i)
					lsi.Font = lvBoldFont;
				else
					lsi.Font = lvFont;
				if (i < 12)
					listViewDayHours.Items.Add (lsi);
				else
					listViewNightHours.Items.Add (lsi);
			}

			// update the "Day of ___" header and graphic.
			updateLabels (pHours);
		}

		/// <summary>
		/// Update the "Day of ____" header and graphic.  Center the text/graphic in the middle of visualPanelRight.
		/// </summary>
        private void updateLabels(PlanetaryHours pHours)
        {
            labelDayOf.Text = pHours.DayString();

            // See Sunrise.rtf for information about conf.ZenithDistance
            if (!pHours.RS.SunRises || !pHours.RS.SunSets)//!pHours.RiseSet.SunRises || !pHours.RiseSet.SunSets)
            {
                if (!pHours.RS.SunRises)//(!pHours.RiseSet.SunRises)
                    labelSunrise.Text = String.Format(conf.Res.GetString("Calendar.NoSunRiseSet"),
                        conf.Res.GetString("Rise"), visualCalendar.SelectedDate.ToShortDateString(), effectivePlace.Name);
                else
                    labelSunrise.Text = String.Format(conf.Res.GetString("Calendar.labelSunrise"),
                        conf.FormatTime(pHours.Hours[0].StartTime),
                        effectivePlace.UseSystemTime ? "" : (" " + effectivePlace.ZoneAbbreviation));
                if (!pHours.RS.SunSets)//(!pHours.RiseSet.SunSets)
                    labelSunset.Text = String.Format(conf.Res.GetString("Calendar.NoSunRiseSet"),
                        conf.Res.GetString("Set"), visualCalendar.SelectedDate.ToShortDateString(), effectivePlace.Name);
                else
                    labelSunset.Text = String.Format(conf.Res.GetString("Calendar.labelSunset"),
                        conf.FormatTime(pHours.Hours[12].StartTime),
                        effectivePlace.UseSystemTime ? "" : (" " + effectivePlace.ZoneAbbreviation));
            }
            else //if (conf.ZenithDistance == 90) // Astrological sunrise/sunset time.
            {
                labelSunrise.Text = String.Format(conf.Res.GetString("Calendar.labelSunrise"),
                    conf.FormatTime(pHours.Hours[0].StartTime),
                    effectivePlace.UseSystemTime ? "" : (" " + effectivePlace.ZoneAbbreviation));
                labelSunset.Text = String.Format(conf.Res.GetString("Calendar.labelSunset"),
                    conf.FormatTime(pHours.Hours[12].StartTime),
                    effectivePlace.UseSystemTime ? "" : (" " + effectivePlace.ZoneAbbreviation));
            }
            //else // other zenith distance
            //{
            //    labelSunrise.Text = String.Format(conf.Res.GetString("Calendar.MorningTwilight"), conf.ZenithDistance,
            //        conf.FormatTime(pHours.Hours[0].StartTime));
            //    labelSunset.Text = String.Format(conf.Res.GetString("Calendar.EveningTwilight"), conf.ZenithDistance,
            //        conf.FormatTime(pHours.Hours[12].StartTime));
            //}

            // display a glyph for the selected planetary day
            //if (pictureBoxDayGlyph != null && pictureBoxDayGlyph.Image != null)
            //	pictureBoxDayGlyph.Image.Dispose();
            Image glyph = Image.FromStream(conf.Fx.GlyphGif(pHours.EnglishDay()));
            pictureBoxDayGlyph.Size = glyph.Size; pictureBoxDayGlyph.Image = glyph;

            // center text and graphic in the top of visualPanelRight (the bottom is measured at labelDayHours.Top)
            // center based on the length of labelDayOf or labelSunrise - whichever is greater
            using (Graphics fx = CreateGraphics())
            {
                SizeF daylen = fx.MeasureString(labelDayOf.Text, labelDayOf.Font);
                SizeF riselen = fx.MeasureString(labelSunrise.Text, labelSunrise.Font);
                SizeF setlen = fx.MeasureString(labelSunset.Text, labelSunset.Font);

                labelDayOf.Width = (int)Math.Ceiling(daylen.Width);
                labelDayOf.Height = (int)Math.Ceiling(daylen.Height);
                labelSunrise.Width = (int)Math.Ceiling(riselen.Width);
                labelSunrise.Height = (int)Math.Ceiling(riselen.Height) + 2;
                labelSunset.Width = (int)Math.Ceiling(setlen.Width);
                labelSunset.Height = (int)Math.Ceiling(setlen.Height) + 2;

                int picx = (int)Math.Ceiling((visualPanelRight.Width - (Math.Max(daylen.Width, riselen.Width) + glyph.Width + 11)) / 2);
                int picy = ((listViewDayHours.Top - glyph.Height) / 2) - 1;
                int textx = picx + glyph.Width + 11;
                int texty = ((listViewDayHours.Top - labelDayOf.Height - labelSunrise.Height - labelSunset.Height - 1) / 2) - 1;

                // position PictureBox and Label's
                pictureBoxDayGlyph.Left = picx; pictureBoxDayGlyph.Top = picy;
                labelDayOf.Left = textx; labelDayOf.Top = texty;
                labelSunrise.Left = textx + 2; labelSunrise.Top = texty + labelDayOf.Height + 1;
                labelSunset.Left = textx + 2; labelSunset.Top = texty + labelDayOf.Height + labelSunrise.Height + 1;

                //if (PInvoke.WindowsVistaGlass())
                //{
                //    picy = ((labelDayOf.Height - glyph.Height) / 2) - 1;
                //    fx.DrawImage(glyph, 0, picy);
                //}

            }
        }

		// when the user selects a different date, repopulate the form's two ListView's and the header labels.
		private void visualCalendar_DateChanged (object sender, System.EventArgs e)
		{
			updateView();
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Calendar));
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarDay = new System.Windows.Forms.StatusBarPanel();
            this.statusBarHour = new System.Windows.Forms.StatusBarPanel();
            this.statusBarHOD = new System.Windows.Forms.StatusBarPanel();
            this.statusBarLocation = new System.Windows.Forms.StatusBarPanel();
            this.statusBarTime = new System.Windows.Forms.StatusBarPanel();
            this.visualPanelLeft = new ChronosXP.VisualPanel();
            this.xpLinkLabelFAQ = new ChronosXP.XPLinkLabel();
            this.xpLinkLabelDonate = new ChronosXP.XPLinkLabel();
            this.linkLabelPhase = new ChronosXP.XPLinkLabel();
            this.linkLabelPrint = new ChronosXP.XPLinkLabel();
            this.linkLabelProperties = new ChronosXP.XPLinkLabel();
            this.visualCalendar = new ChronosXP.VisualMonthCalendar();
            this.linkLabelLocality = new ChronosXP.XPLinkLabel();
            this.linkLabelAbout = new ChronosXP.XPLinkLabel();
            this.visualPanelRight = new ChronosXP.VisualPanel();
            this.listViewDayHours = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.listViewNightHours = new System.Windows.Forms.ListView();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarDay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarHour)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarHOD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarTime)).BeginInit();
            this.visualPanelLeft.SuspendLayout();
            this.visualPanelRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            resources.ApplyResources(this.statusBar, "statusBar");
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarDay,
            this.statusBarHour,
            this.statusBarHOD,
            this.statusBarLocation,
            this.statusBarTime});
            this.statusBar.ShowPanels = true;
            this.statusBar.SizingGrip = false;
            // 
            // statusBarDay
            // 
            resources.ApplyResources(this.statusBarDay, "statusBarDay");
            this.statusBarDay.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            // 
            // statusBarHour
            // 
            resources.ApplyResources(this.statusBarHour, "statusBarHour");
            this.statusBarHour.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            // 
            // statusBarHOD
            // 
            resources.ApplyResources(this.statusBarHOD, "statusBarHOD");
            this.statusBarHOD.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            // 
            // statusBarLocation
            // 
            resources.ApplyResources(this.statusBarLocation, "statusBarLocation");
            this.statusBarLocation.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            // 
            // statusBarTime
            // 
            resources.ApplyResources(this.statusBarTime, "statusBarTime");
            this.statusBarTime.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            // 
            // visualPanelLeft
            // 
            this.visualPanelLeft.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.visualPanelLeft.Blend = null;
            this.visualPanelLeft.Controls.Add(this.xpLinkLabelFAQ);
            this.visualPanelLeft.Controls.Add(this.xpLinkLabelDonate);
            this.visualPanelLeft.Controls.Add(this.linkLabelPhase);
            this.visualPanelLeft.Controls.Add(this.linkLabelPrint);
            this.visualPanelLeft.Controls.Add(this.linkLabelProperties);
            this.visualPanelLeft.Controls.Add(this.visualCalendar);
            this.visualPanelLeft.Controls.Add(this.linkLabelLocality);
            this.visualPanelLeft.Controls.Add(this.linkLabelAbout);
            resources.ApplyResources(this.visualPanelLeft, "visualPanelLeft");
            this.visualPanelLeft.LeftColor = System.Drawing.SystemColors.Highlight;
            this.visualPanelLeft.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.visualPanelLeft.Name = "visualPanelLeft";
            this.visualPanelLeft.RightColor = System.Drawing.SystemColors.InactiveCaption;
            this.visualPanelLeft.VisualStyle = true;
            // 
            // xpLinkLabelFAQ
            // 
            this.xpLinkLabelFAQ.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.xpLinkLabelFAQ, "xpLinkLabelFAQ");
            this.xpLinkLabelFAQ.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.xpLinkLabelFAQ.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.xpLinkLabelFAQ.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.xpLinkLabelFAQ.Name = "xpLinkLabelFAQ";
            this.xpLinkLabelFAQ.TabStop = true;
            this.xpLinkLabelFAQ.UseCompatibleTextRendering = true;
            this.xpLinkLabelFAQ.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.xpLinkLabelFAQ_LinkClicked);
            // 
            // xpLinkLabelDonate
            // 
            this.xpLinkLabelDonate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.xpLinkLabelDonate, "xpLinkLabelDonate");
            this.xpLinkLabelDonate.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.xpLinkLabelDonate.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.xpLinkLabelDonate.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.xpLinkLabelDonate.Name = "xpLinkLabelDonate";
            this.xpLinkLabelDonate.TabStop = true;
            this.xpLinkLabelDonate.UseCompatibleTextRendering = true;
            this.xpLinkLabelDonate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.xpLinkLabelDonate_LinkClicked);
            // 
            // linkLabelPhase
            // 
            this.linkLabelPhase.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelPhase, "linkLabelPhase");
            this.linkLabelPhase.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelPhase.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelPhase.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelPhase.Name = "linkLabelPhase";
            this.linkLabelPhase.TabStop = true;
            this.linkLabelPhase.UseCompatibleTextRendering = true;
            this.linkLabelPhase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPhase_LinkClicked);
            // 
            // linkLabelPrint
            // 
            this.linkLabelPrint.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelPrint, "linkLabelPrint");
            this.linkLabelPrint.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelPrint.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelPrint.Name = "linkLabelPrint";
            this.linkLabelPrint.TabStop = true;
            this.linkLabelPrint.UseCompatibleTextRendering = true;
            this.linkLabelPrint.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelPrint_LinkClicked);
            // 
            // linkLabelProperties
            // 
            this.linkLabelProperties.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelProperties, "linkLabelProperties");
            this.linkLabelProperties.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelProperties.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelProperties.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelProperties.Name = "linkLabelProperties";
            this.linkLabelProperties.TabStop = true;
            this.linkLabelProperties.UseCompatibleTextRendering = true;
            this.linkLabelProperties.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelProperties_LinkClicked);
            // 
            // visualCalendar
            // 
            this.visualCalendar.Blend = null;
            this.visualCalendar.CalGradient = System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal;
            this.visualCalendar.ColorLeft = System.Drawing.SystemColors.ControlLightLight;
            this.visualCalendar.ColorRight = System.Drawing.SystemColors.ControlLightLight;
            this.visualCalendar.DayColor = System.Drawing.SystemColors.Highlight;
            resources.ApplyResources(this.visualCalendar, "visualCalendar");
            this.visualCalendar.InactiveForeColor = System.Drawing.SystemColors.GrayText;
            this.visualCalendar.Name = "visualCalendar";
            this.visualCalendar.SelectedDate = new System.DateTime(2004, 7, 4, 0, 0, 0, 0);
            this.visualCalendar.SelectedDateBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.visualCalendar.SelectedDateForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.visualCalendar.SeparatorColorBottom = System.Drawing.Color.Silver;
            this.visualCalendar.SeparatorColorTop = System.Drawing.Color.White;
            this.visualCalendar.TitleBarBlend = null;
            this.visualCalendar.TitleBarColorLeft = System.Drawing.Color.WhiteSmoke;
            this.visualCalendar.TitleBarColorRight = System.Drawing.SystemColors.InactiveCaption;
            this.visualCalendar.TitleBarForeColor = System.Drawing.SystemColors.ControlText;
            this.visualCalendar.TitleBarToolTipText = null;
            this.visualCalendar.TitleFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.visualCalendar.TitleGradient = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.visualCalendar.TodayDate = new System.DateTime(2004, 7, 4, 0, 0, 0, 0);
            this.visualCalendar.ToolTipText = null;
            this.visualCalendar.UseBaseGradient = true;
            this.visualCalendar.VisualStyle = true;
            // 
            // linkLabelLocality
            // 
            this.linkLabelLocality.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelLocality, "linkLabelLocality");
            this.linkLabelLocality.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelLocality.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelLocality.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelLocality.Name = "linkLabelLocality";
            this.linkLabelLocality.TabStop = true;
            this.linkLabelLocality.UseCompatibleTextRendering = true;
            this.linkLabelLocality.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLocality_LinkClicked);
            // 
            // linkLabelAbout
            // 
            this.linkLabelAbout.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelAbout, "linkLabelAbout");
            this.linkLabelAbout.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelAbout.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkLabelAbout.LinkColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.linkLabelAbout.Name = "linkLabelAbout";
            this.linkLabelAbout.TabStop = true;
            this.linkLabelAbout.UseCompatibleTextRendering = true;
            this.linkLabelAbout.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAbout_LinkClicked);
            // 
            // visualPanelRight
            // 
            this.visualPanelRight.BackColor = System.Drawing.SystemColors.ControlLight;
            this.visualPanelRight.Blend = null;
            this.visualPanelRight.Controls.Add(this.listViewDayHours);
            this.visualPanelRight.Controls.Add(this.listViewNightHours);
            resources.ApplyResources(this.visualPanelRight, "visualPanelRight");
            this.visualPanelRight.LeftColor = System.Drawing.SystemColors.ControlLightLight;
            this.visualPanelRight.LinearGradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            this.visualPanelRight.Name = "visualPanelRight";
            this.visualPanelRight.RightColor = System.Drawing.SystemColors.Control;
            this.visualPanelRight.VisualStyle = true;
            // 
            // listViewDayHours
            // 
            this.listViewDayHours.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            resources.ApplyResources(this.listViewDayHours, "listViewDayHours");
            this.listViewDayHours.FullRowSelect = true;
            this.listViewDayHours.MultiSelect = false;
            this.listViewDayHours.Name = "listViewDayHours";
            this.listViewDayHours.TabStop = false;
            this.listViewDayHours.UseCompatibleStateImageBehavior = false;
            this.listViewDayHours.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // listViewNightHours
            // 
            this.listViewNightHours.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4});
            resources.ApplyResources(this.listViewNightHours, "listViewNightHours");
            this.listViewNightHours.FullRowSelect = true;
            this.listViewNightHours.MultiSelect = false;
            this.listViewNightHours.Name = "listViewNightHours";
            this.listViewNightHours.TabStop = false;
            this.listViewNightHours.UseCompatibleStateImageBehavior = false;
            this.listViewNightHours.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // Calendar
            // 
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.visualPanelLeft);
            this.Controls.Add(this.visualPanelRight);
            this.Controls.Add(this.statusBar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "Calendar";
            ((System.ComponentModel.ISupportInitialize)(this.statusBarDay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarHour)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarHOD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarTime)).EndInit();
            this.visualPanelLeft.ResumeLayout(false);
            this.visualPanelRight.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region Button Click EventHandler's / LinkLabel LinkLabelLinkClickedEventHandler's
		private void buttonClose_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void linkLabelHours_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			conf.LaunchBrowser (Config.URLHours);
		}

		private void linkLabelLocality_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			if (!FormLocalityOpen)
				this.formLocality = new Locality (conf, this);
			else if (formLocality.WindowState == FormWindowState.Minimized)
					formLocality.WindowState = FormWindowState.Normal;
			this.formLocality.Show();
			this.formLocality.Focus();			
		}

		private void linkLabelProperties_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			conf.Core.ShowProperties();
		}
		
		private void linkLabelPhase_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			conf.Core.ShowPhases(visualCalendar.SelectedDate);
		}

		private void linkLabelAbout_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			conf.Core.ShowAbout();
		}

		private void linkLabelProduct_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			conf.LaunchBrowser (Config.URL);
		}

		private void linkLabelPrint_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				if (!FormPrintOpen)
					formPrint = new PrintHours (visualCalendar.SelectedDate, effectivePlace, conf, this);
				formPrint.Show();
				formPrint.Focus();
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Unable to print.", ex);
			}
		}
		#endregion

        private void xpLinkLabelDonate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            conf.LaunchBrowser(Config.URLDonate);
        }

        private void xpLinkLabelFAQ_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            conf.LaunchBrowser(Config.URLFAQ);
        }
	}
}
