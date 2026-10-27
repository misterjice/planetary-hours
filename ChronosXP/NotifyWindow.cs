#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - NotifyWindow.cs
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
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChronosXP
{
	// NotifyWindow:  Display a Windows Messenger-style notification window in the lower right corner of the screen.  This class replaces
	// John O'Byrne's TaskbarNotifier that was used in previous versions.  It paints a different background color for each planetary hour, which
	// the user can configure in the Properties form.  It also has similar semantics as balloon windows:  It will display for about 20 seconds if
	// there is keyboard or mouse activity, otherwise it will continue displaying for up to 24 minutes or until there is user activity.
	public class NotifyWindow : System.Windows.Forms.Form
	{
		public const string Copyright = "NotifyWindow.cs, Copyright 2004 by Robert Misiak";

		#region Declarations
		public bool IsPreview = false;
		private const int padding = 11;
		private Config conf;
		private Image glyphImage;
		private System.Drawing.Font titleFont, ulTitleFont, ulFont;
		private string dayOf, hourOf;
		private SizeF szDay, szHour, szTitle;
		private Rectangle rScreen, rDisplay, rArea, rClose;
		private RectangleF rText, rTitle, rGlobText, rGlobTitle, rGlobClose, rLine1, rLine2;
		private string line1, line2;
		private Color baseColor;
		private bool closePressed = false, waiting = false, titleHover = false, textHover = false, closeHover = false, activityWait, textMouseDown = false,
			titleMouseDown = false;
		private StringFormat tFormat;
		private int hMouseHook = 0, hKeyboardHook = 0;
		private PInvoke.HookProc hpMouse, hpKeyboard;
		private int realHeight;
		private int cHeight = 0, cTop = 0;
		private delegate void dShow();
		private delegate void dAdjust (int height, int top);
		private dShow show, close;
		private dAdjust adjust;
		private Thread viewThread;
		private System.Windows.Forms.Timer viewClock;
		private DateTime saveTime;
		#endregion

		#region Constructor
		public NotifyWindow (PlanetaryHours pHours, Config conf) : this (pHours.DayString(), pHours.CurrentHourString(), conf,
			pHours.CurrentEnglishHour(), conf.PlanetColor (pHours.CurrentHour()), conf.NotifyFont, pHours.CurrentHourNum() == 0 ? true : false,
			conf.Sticky)
		{
		}

		public NotifyWindow (string dayStr, string hourStr, Config conf, string englishHour, Color baseClr, Font useFnt, bool dayFirst) :
			this (dayStr, hourStr, conf, englishHour, baseClr, useFnt, dayFirst, false)
		{
		}

		public NotifyWindow (string dayStr, string hourStr, Config conf, string englishHour, Color baseClr, Font useFnt, bool dayFirst, bool sticky)
		{
			this.conf = conf;

			// If another NotifyWindow is open, close it.
			if (conf.lastNw != null)
				conf.lastNw.Close();
			conf.lastNw = this;

			SetStyle (ControlStyles.UserMouse, true);
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.DoubleBuffer, true);

			ShowInTaskbar = false;
			FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			StartPosition = System.Windows.Forms.FormStartPosition.Manual;

			float titleSize;
			if (useFnt.Size <= 11.25F)
				titleSize = 11.25F;
			else
				titleSize = useFnt.Size;
			titleFont = new Font (useFnt.Name, titleSize, useFnt.Style);
			ulTitleFont = new Font (useFnt.Name, titleSize, useFnt.Style | FontStyle.Underline);
			ulFont = new Font (useFnt, FontStyle.Underline);
			Font = useFnt;
			glyphImage = Image.FromStream (conf.Fx.GlyphGif (englishHour));
			dayOf = dayStr;
			hourOf = hourStr;
			#if MONO
				rScreen = Screen.PrimaryScreen.WorkingArea;
			#else
				rScreen = Screen.GetWorkingArea(Screen.PrimaryScreen.Bounds);
			#endif
			baseColor = baseClr;

			tFormat = new StringFormat();
			tFormat.Alignment = StringAlignment.Center;
			tFormat.LineAlignment = StringAlignment.Center;
			using (Graphics fx = CreateGraphics())
			{
				szDay = fx.MeasureString (dayOf, ulFont, rScreen.Width - (padding / 2), tFormat);
				szHour = fx.MeasureString (hourOf, ulFont, rScreen.Width - (padding / 2), tFormat);
				szTitle = fx.MeasureString ("ChronosXP", ulTitleFont, rScreen.Width - (padding / 2), tFormat);
			}

		/*	Width = realHeight = (int) Math.Ceiling(
				Math.Max
				(
					glyphImage.Width + Math.Max (szDay.Width, szHour.Width) + (padding * 3),
					glyphImage.Height + szTitle.Height + (padding * 3)
				)
			);	*/
			Width = (int) Math.Ceiling (glyphImage.Width + Math.Max (szDay.Width, szHour.Width) + (padding * 3));
			realHeight = (int) Math.Ceiling (glyphImage.Height + szTitle.Height + (padding * 3)) + 4;
			Left = rScreen.Width - Width - padding;
			Top = rScreen.Bottom;  Height = 0;
			rArea = new Rectangle (Left, rScreen.Bottom - realHeight, Width, realHeight);

			SizeF ln1, ln2;
			if (dayFirst)
			{
				line1 = dayOf;  ln1 = szDay;
				line2 = hourOf;  ln2 = szHour;
			}
			else
			{
				line1 = hourOf;  ln1 = szHour;
				line2 = dayOf;  ln2 = szDay;
			}

			float tlen = Math.Max (ln1.Width, ln2.Width);
			rLine1 = new RectangleF (glyphImage.Width + (padding * 2),
				szTitle.Height + (padding * 2) + ((glyphImage.Height - (szDay.Height + szHour.Height)) / 2) - 2.5F, tlen, ln1.Height);
			rLine2 = new RectangleF (rLine1.X, rLine1.Y + szHour.Height, tlen, ln2.Height);
			rText = new RectangleF (rLine1.X, rLine1.Y, rLine1.Width, rLine1.Height + rLine2.Height + 5);
			// rGlob* are RectangleF's Offset'ed to their position on screen, for use with Cursor.Position
			rGlobText = rText;  rGlobText.Offset (Left, Top - realHeight);
			rTitle = new RectangleF (padding, padding, szTitle.Width, szTitle.Height);
			rGlobTitle = rTitle;  rGlobTitle.Offset (Left, Top - realHeight);

			rClose = new Rectangle (Width - (padding * 2), padding, 13, 13);
			rGlobClose = rClose;  rGlobClose.Offset (Left, Top - realHeight);

			rDisplay = new Rectangle (0, 0, Width, realHeight);

			// This is for times when the user does an Alt+Tab - since we are a Form we'll appear there
			Icon = conf.Fx.GlyphIcon (englishHour);
            Text = String.Format("{0}, {1} - {2}", hourOf, dayOf, Config.SoftwareName);

			Application.ApplicationExit += new System.EventHandler (appExit);

            // This is now disabled.  It makes some people's antivirus software think that ChronosXP is a keylogged (which it's not)
			// Check for mouse or keyboard activity; if there is none, display the window for up to 24 minutes (similar to balloon windows)
			// (WH_MOUSE_LL and WH_KEYBOARD_LL require Win2000+)
            //if (sticky && Config.ModernOS())
            //{
            //    activityWait = true;

            //    hpMouse = new PInvoke.HookProc (mouseHookProc);
            //    hMouseHook = PInvoke.SetWindowsHookEx (PInvoke.WH_MOUSE_LL, hpMouse,
            //        Marshal.GetHINSTANCE (Assembly.GetExecutingAssembly().GetModules()[0]), 0);

            //    hpKeyboard = new PInvoke.HookProc (keyboardHookProc);
            //    hKeyboardHook = PInvoke.SetWindowsHookEx (PInvoke.WH_KEYBOARD_LL, hpKeyboard,
            //        Marshal.GetHINSTANCE (Assembly.GetExecutingAssembly().GetModules()[0]), 0);
            //}
            //else
            //{
				activityWait = false;
            //}
		}
		#endregion

		#region Start, Clock tick EventHandler and Threads
		public void Start()
		{
			PInvoke.ShowWindow (Handle, PInvoke.SW_SHOWNOACTIVATE);
			if (hKeyboardHook != 0 && hMouseHook != 0 && activityWait)
				waiting = true;

			show = new dShow (showing);
			close = new dShow (finished);
			adjust = new dAdjust (adjustPos);

			saveTime = DateTime.Now;
			cTop = rScreen.Height;  cHeight = 0;
			viewThread = new Thread (new ThreadStart (beginShow));
			viewThread.Start();
		}

		protected void finished()
		{
			Close();
		}

		protected void adjustPos (int height, int top)
		{
			PInvoke.SetWindowPos (Handle, PInvoke.HWND_TOPMOST, Left, top, Width, height, PInvoke.SWP_NOACTIVATE);
		}

		protected void showing()
		{
			viewClock = new System.Windows.Forms.Timer();
			viewClock.Interval = 11200;
			viewClock.Tick += new System.EventHandler (viewTick);
			viewClock.Start();
		}

		protected void viewTick (object sender, System.EventArgs e)
		{
			if ((!waiting || saveTime.AddMinutes(24).CompareTo(DateTime.Now) < 0) && !rArea.Contains (Cursor.Position))
			{
				viewClock.Stop();
				viewClock.Dispose();
				cTop = rScreen.Height - realHeight;  cHeight = realHeight;
				viewThread = new Thread (new ThreadStart (beginClose));
				viewThread.Start();
			}
		}

		protected void beginShow()
		{
			while (cHeight < realHeight)
			{
				BeginInvoke (adjust, new object[] { cHeight, cTop });
				cHeight += 2;  cTop -= 2;
				Thread.Sleep(9);
			}
			BeginInvoke (adjust, new object[] { realHeight, rScreen.Height - realHeight });
			BeginInvoke (show, null);
		}

		protected void beginClose()
		{
			while (cHeight > 0)
			{
				Thread.Sleep(9);
				BeginInvoke (adjust, new object[] { cHeight, cTop });
				cHeight -= 2;  cTop += 2;
			}
			BeginInvoke (close, null);
		}
		#endregion

		#region Drawing
		// Paint the glyph, title, text and close button.
		protected override void OnPaint (System.Windows.Forms.PaintEventArgs e)
		{
			// Draw the glyph image
			e.Graphics.DrawImage (glyphImage, padding + 2, (int) Math.Ceiling (szTitle.Height) + (padding * 2), glyphImage.Width, glyphImage.Height);

			Font useFont;  Brush useBrush;
			if (titleHover)
				useFont = ulTitleFont;
			else
				useFont = titleFont;
			if (titleMouseDown)
				useBrush = Brushes.Gray;
			else
				useBrush = Brushes.Black;
			// Title string
			e.Graphics.DrawString ("ChronosXP", useFont, useBrush, rTitle, tFormat);

			if (textHover)
				useFont = ulFont;
			else
				useFont = Font;
			if (textMouseDown)
				useBrush = Brushes.Gray;
			else
				useBrush = Brushes.Black;
			// "Day of ____", "Hour of ___" text.
			e.Graphics.DrawString (line1, useFont, useBrush, rLine1, tFormat);
			e.Graphics.DrawString (line2, useFont, useBrush, rLine2, tFormat);

			if (PInvoke.VisualStylesEnabled())
				drawThemeCloseButton(e.Graphics);
			else
				drawLegacyCloseButton(e.Graphics);
		}

		// Draw an XP-style close button using DrawThemeBackground()
		protected void drawThemeCloseButton (Graphics fx)
		{
			IntPtr hTheme = PInvoke.OpenThemeData (Handle, "Window");
			if (hTheme == IntPtr.Zero)
			{
				drawLegacyCloseButton(fx);
				return;
			}
			int stateID;
			if (closePressed)
				stateID = PInvoke.CBS_PUSHED;
			else if (closeHover)
				stateID = PInvoke.CBS_HOT;
			else
				stateID = PInvoke.CBS_NORMAL;
			PInvoke.Rect reClose = new PInvoke.Rect (rClose);
			PInvoke.Rect reClip = reClose; //new PInvoke.Rect (e.Graphics.VisibleClipBounds);
			IntPtr hDC = fx.GetHdc();
			PInvoke.DrawThemeBackground (hTheme, hDC, PInvoke.WP_CLOSEBUTTON, stateID, ref reClose, ref reClip);
			fx.ReleaseHdc(hDC);
			PInvoke.CloseThemeData(hTheme);
		}

		// Draw an old fashioned Windows 95-style close button
		protected void drawLegacyCloseButton (Graphics fx)
		{
			ButtonState bState;
			if (closePressed)
				bState = ButtonState.Pushed;
			else  // the Windows 95 theme doesn't have a "hot" button
				bState = ButtonState.Normal;
			ControlPaint.DrawCaptionButton (fx, rClose, CaptionButton.Close, bState);
		}

		// Paint the background and border.
		protected override void OnPaintBackground (System.Windows.Forms.PaintEventArgs e)
		{
			using (LinearGradientBrush lgb = new LinearGradientBrush (rDisplay, baseColor, Color.White, LinearGradientMode.BackwardDiagonal))
			{
				// The glyph images generally look better on a light background, so use a dramatic blend that will be light underneath the image
				// but quickly transition to 100% baseColor
				Blend bgblend = new Blend();
				bgblend.Factors = new float[] { 0.0F, 0.3F, 0.6F, 0.8F, 0.9F, 1.0F };
				bgblend.Positions = new float[] { 0.0F, 0.2F, 0.4F, 0.6F, 0.8F, 1.0F };
				lgb.Blend = bgblend;
				e.Graphics.FillRectangle (lgb, rDisplay);
			}

			// Draw borders...
			e.Graphics.DrawRectangle (Pens.Silver, 2, 2, Width - 4, realHeight - 4);

			// Top border
			e.Graphics.DrawLine (Pens.Silver, 0, 0, Width, 0);
			e.Graphics.DrawLine (Pens.White, 0, 1, Width - 1, 1);
			e.Graphics.DrawLine (Pens.DarkGray, 3, 3, Width - 4, 3);
			e.Graphics.DrawLine (Pens.DimGray, 4, 4, Width - 5, 4);

			// Left border
			e.Graphics.DrawLine (Pens.Silver, 0, 0, 0, realHeight);
			e.Graphics.DrawLine (Pens.White, 1, 1, 1, realHeight);
			e.Graphics.DrawLine (Pens.DarkGray, 3, 3, 3, realHeight - 4);
			e.Graphics.DrawLine (Pens.DimGray, 4, 4, 4, realHeight - 5);

			// Bottom border
			e.Graphics.DrawLine (Pens.DarkGray, 1, realHeight - 1, Width - 1, realHeight - 1);
			e.Graphics.DrawLine (Pens.White, 3, realHeight - 3, Width - 3, realHeight - 3);
			e.Graphics.DrawLine (Pens.Silver, 4, realHeight - 4, Width - 4, realHeight - 4);
			e.Graphics.DrawLine (Pens.Silver, 5, realHeight - 5, Width - 5, realHeight - 5);

			// Right border
			e.Graphics.DrawLine (Pens.DarkGray, Width - 1, 1, Width - 1, realHeight - 1);
			e.Graphics.DrawLine (Pens.White, Width - 3, 3, Width - 3, realHeight - 3);
			e.Graphics.DrawLine (Pens.Silver, Width - 4, 4, Width - 4, realHeight - 4);
			e.Graphics.DrawLine (Pens.DarkGray, Width - 5, 5, Width - 5, realHeight - 5);
		}
		#endregion

		#region Mouse/Keyboard input
		protected override void OnMouseMove (System.Windows.Forms.MouseEventArgs e)
		{
			if (rGlobTitle.Contains (Cursor.Position) && !textMouseDown && !closePressed)
			{
				Cursor = Cursors.Hand;
				titleHover = true;
				Invalidate();
			}
			else if (rGlobText.Contains (Cursor.Position) && !titleMouseDown && !closePressed)
			{
				Cursor = Cursors.Hand;
				textHover = true;
				Invalidate();
			}
			else if (rGlobClose.Contains (Cursor.Position) && !titleMouseDown && !textMouseDown)
			{
				Cursor = Cursors.Hand;
				closeHover = true;
				Invalidate();
			}
			else if ((textHover || titleHover || closeHover) && (!titleMouseDown && !textMouseDown && !closePressed))
			{
				Cursor = Cursors.Default;
				textHover = false;
				titleHover = false;
				closeHover = false;
				Invalidate();
			}
			base.OnMouseMove (e);
		}
		
		protected override void OnMouseLeave (System.EventArgs e)
		{
			if (!textMouseDown && !titleMouseDown && !closePressed)
			{
				Cursor = Cursors.Default;
				titleHover = false;
				textHover = false;
				closeHover = false;
				Invalidate();
			}
			base.OnMouseLeave(e);
		}
		
		protected override void OnMouseDown (System.Windows.Forms.MouseEventArgs e)
		{
			if (e.Button == MouseButtons.Left)
			{
				//mouseDown = true;
				if (rGlobClose.Contains (Cursor.Position))
				{
					closePressed = true;
					closeHover = false;
					Invalidate();
				}
				else if (rGlobText.Contains (Cursor.Position))
				{
					textMouseDown = true;
					Invalidate();
				}
				else if (rGlobTitle.Contains (Cursor.Position))
				{
					titleMouseDown = true;
					Invalidate();
				}
			}
			base.OnMouseDown (e);
		}
		
		protected override void OnMouseUp (System.Windows.Forms.MouseEventArgs e)
		{
			//mouseDown = false;
			if (e.Button == MouseButtons.Left)
			{
				if (closePressed)
				{
					Cursor = Cursors.Default;
					closePressed = false;
					closeHover = false;
					Invalidate();
					if (rGlobClose.Contains (Cursor.Position))
						Close();
				}
				else if (rGlobTitle.Contains (Cursor.Position) && titleMouseDown)
				{
					Cursor = Cursors.Default;
					titleMouseDown = false;
					Invalidate();
					conf.Core.ShowAbout();
					Close();
				}
				else if (rGlobText.Contains (Cursor.Position) && textMouseDown)
				{
					Cursor = Cursors.Default;
					textMouseDown = false;
					Invalidate();
					conf.Core.ShowCalendar();
					Close();
				}
				else if (titleMouseDown || textMouseDown)
				{
					Cursor = Cursors.Default;
					titleMouseDown = false;
					textMouseDown = false;
					titleHover = false;
					textHover = false;
					Invalidate();
				}
			}
			base.OnMouseUp (e);
		}
		
		protected int mouseHookProc (int nCode, int wParam, IntPtr lParam)
		{
			if (nCode == PInvoke.HC_ACTION && waiting)
				waiting = false;
			return PInvoke.CallNextHookEx (hMouseHook, nCode, wParam, lParam);
		}

		protected int keyboardHookProc (int nCode, int wParam, IntPtr lParam)
		{
			if (nCode == PInvoke.HC_ACTION && waiting)
				waiting = false;
			return PInvoke.CallNextHookEx (hKeyboardHook, nCode, wParam, lParam);
		}
		#endregion

		#region Dispose, appExit and OnClosing
		// Make sure hooks are Unhook'ed
		protected void appExit (object sender, System.EventArgs e)
		{
			Close();
		}

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
			if (hMouseHook != 0)
				PInvoke.UnhookWindowsHookEx (hMouseHook);
			if (hKeyboardHook != 0)
				PInvoke.UnhookWindowsHookEx (hKeyboardHook);
			Application.ApplicationExit -= new System.EventHandler (appExit);
			if (viewClock != null && viewClock.Enabled)
			{
				viewClock.Stop();
				viewClock.Dispose();
			}
			if (viewThread != null && viewThread.IsAlive)
				viewThread.Abort();
			conf.lastNw = null;
			base.OnClosing (e);
		}

		protected override void Dispose (bool disposing)
		{
			if (disposing)
			{
//				titleFont.Dispose();
//				ulTitleFont.Dispose();
//				ulFont.Dispose();
//				tFormat.Dispose();
			}
			base.Dispose (disposing);
		}
		#endregion
	}
}
