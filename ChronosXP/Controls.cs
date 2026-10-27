#region Copyright © 2004-2005 by Robert Misiak
// ChronosXP - Controls.cs
// Copyright © 2004-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	// VisualMonthCalendar:  a much prettier replacement for MonthCalendar.  The default MonthCalendar is an ActiveX control (not .NET) so
	// AFAIK it wasn't possible to override something to do our own drawing.  As I really had my heart set on a prettier MonthCalendar for
	// ChronosXP, I decided to just create a new control.  This control doesn't have all of the functionality of MonthCalendar, only that which is
	// needed for ChronosXP.  For example, only one date may be selected, and there is no "ShowToday" option.  There is no Today circle,
	// instead the TodayDate is bolded.  There is no option to add other bolded dates.  Even though this was created for ChronosXP, it should
	// be able to be used with any other application with no modification.

	#region VisualMonthCalendar
	[ToolboxItem (true)]
	public class VisualMonthCalendar : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private System.Windows.Forms.Button buttonRight;
		private System.Windows.Forms.Label titleBar;
		private System.Windows.Forms.Button buttonLeft;
		public const string Copyright = "Controls.cs, Copyright © 2004 by Robert Misiak";

		private System.ComponentModel.Container components = null;
		private DateTime selectedDate, todayDate;
		private Color titleBarColorLeft, titleBarColorRight, colorLeft, colorRight, dayColor, titleBarForeColor, sepColorTop, sepColorBottom;
		private Color foreColor, inactiveForeColor, selectedDateForeColor, selectedDateBackColor;
		private DayLabel[,] dayLabels = new DayLabel [6, 7];
		private ToolTip[,] toolTips = new ToolTip [6, 7];
		private ToolTip titleTip;
		private System.Windows.Forms.Label[] dayNameLabels = new System.Windows.Forms.Label [7];
		private DayLabel selectedDayLabel;
		private string toolTipText;
		private string titleBarTipText;
		private System.Drawing.Drawing2D.LinearGradientMode titleGradient, calGradient;
		private bool visualStyle, useBaseGradient;
		private System.Drawing.Drawing2D.Blend titleBarBlend, calBlend;
		private DateTime lastMonthDate;
		private Font calFont, dayFont, selectedDayFont;

		public event System.EventHandler SelectedDateChanged;

		public VisualMonthCalendar()
		{
			SetStyle (ControlStyles.FixedHeight, true);
			SetStyle (ControlStyles.FixedWidth, true);
			SetStyle (ControlStyles.DoubleBuffer, true);
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.CacheText, true);
			SetStyle (ControlStyles.Selectable, true);

			InitializeComponent();

			// default value for colors, etc.
			colorLeft = SystemColors.ControlLightLight;
			colorRight = SystemColors.Control;
			titleBarColorLeft = Color.WhiteSmoke;
			titleBarColorRight = Color.Silver;
			selectedDateForeColor = SystemColors.ActiveCaptionText;
			selectedDateBackColor = SystemColors.ActiveCaption;
			foreColor = SystemColors.ControlText;
			inactiveForeColor = SystemColors.GrayText;
			dayColor = SystemColors.Highlight;
			titleBarForeColor = SystemColors.ControlText;
			sepColorTop = Color.WhiteSmoke;
			sepColorBottom = Color.Silver;
			calGradient = LinearGradientMode.ForwardDiagonal;
			titleGradient = LinearGradientMode.Horizontal;
			calFont = SystemInformation.MenuFont;
			dayFont = SystemInformation.MenuFont;
			selectedDayFont = new Font (SystemInformation.MenuFont, FontStyle.Bold);
			visualStyle = true;
			useBaseGradient = false;

			// The individual days of the month are displayed via a 2-dimensional array of Label controls.
			for (int w = 0, y = titleBar.Height + 19; w < 6; w++)
			{
				for (int d = 0, x = 2; d < 7; d++)
				{
					dayLabels[w, d] = new DayLabel();
					dayLabels[w, d].BackColor = Color.Transparent;
					dayLabels[w, d].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
					dayLabels[w, d].Size = new Size (28, 16);
					dayLabels[w, d].Location = new Point (x, y);
					dayLabels[w, d].Click += new System.EventHandler (dayClick);
					dayLabels[w, d].TabIndex = 1001 + d + (w * 10);
					x += dayLabels[w, d].Width;
					Controls.Add (dayLabels[w, d]);
				}
				y += dayLabels[w, 0].Height;
			}

			// Initialize day name labels
			for (int c = 0, xp = 2; c < 7; c++)
			{
				dayNameLabels[c] = new System.Windows.Forms.Label();
				dayNameLabels[c].BackColor = Color.Transparent;
				dayNameLabels[c].TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
				dayNameLabels[c].Size = new Size (28, 15);
				dayNameLabels[c].Location = new Point (xp, titleBar.Height + 2);
				Controls.Add (dayNameLabels[c]);
				xp += dayNameLabels[c].Width;
			}

			// default TodayDate and SelectedDate values
			todayDate = DateTime.Today;
			SelectedDate = DateTime.Today;

			dateTimePicker.CustomFormat = Application.CurrentCulture.DateTimeFormat.YearMonthPattern;
			dateTimePicker.LostFocus += new System.EventHandler (dateTimePicker_LostFocus);
			dateTimePicker.Leave += new System.EventHandler (dateTimePicker_LostFocus);
			dateTimePicker.ValueChanged += new System.EventHandler (dateTimePicker_DateChanged);
		}
		
		protected override void OnFontChanged (System.EventArgs e)
		{
			calFont = Font;
			dayFont = Font;
			selectedDayFont = new Font (Font, FontStyle.Bold);
		}

		protected void dayClick (object sender, System.EventArgs e)
		{
			DayLabel l = sender as DayLabel;
			l.Focus();
			DateTime dt = l.Date;
			selectedDayLabel.BackColor = Color.Transparent;
			selectedDayLabel.ForeColor = foreColor;
			if (dt.Month == selectedDate.Month)
			{
				selectedDate = dt;
				selectedDayLabel = l;
				l.BackColor = selectedDateBackColor;
				l.ForeColor = selectedDateForeColor;
			}
			else
			{
				selectedDate = dt;
				drawCalendar();
			}

			dateTimePicker.Value = selectedDate;
			if (SelectedDateChanged != null)
				SelectedDateChanged (this, new System.EventArgs());
		}

		[BrowsableAttribute (false)]
		public DateTime TodayDate
		{
			get
			{
				return todayDate;
			}
			set
			{
				todayDate = value;
				drawCalendar();
			}
		}

		[BrowsableAttribute (false)]
		public DateTime SelectedDate
		{
			get
			{
				return selectedDate;
			}
			set
			{
				DateTime dt;
				try
				{
					dt = value;
				}
				catch
				{
					MessageBox.Show ("Invalid Date", "ChronosXP", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				if (dt.Year > 9998 || dt.Year < 1753)
				{
					MessageBox.Show ("Invalid Date;  Dates must be between Januaray 1, 1753 and December 31, 9998.", "ChronosXP",
						MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}
				selectedDate = dt;
				drawCalendar();
				dateTimePicker.Value = selectedDate;
				if (SelectedDateChanged != null)
					SelectedDateChanged (this, new System.EventArgs());
			}
		}

		protected void drawCalendar()
		{
			titleBar.Text = selectedDate.ToString(Application.CurrentCulture.DateTimeFormat.YearMonthPattern);
			titleBar.ForeColor = titleBarForeColor;
			titleBar.Font = calFont;
			buttonLeft.Font = calFont;
			buttonRight.Font = calFont;
			DateTime stime = new DateTime (selectedDate.Year, selectedDate.Month, 1, 12, 0, 0, 0);

			if ((int) stime.DayOfWeek == 0)
				stime = stime.Subtract (new TimeSpan (7, 0, 0, 0));
			else
				stime = stime.Subtract (new TimeSpan ((int) stime.DayOfWeek, 0, 0, 0));

			for (int w = 0; w < 6; w++)
			{
				for (int d = 0; d < 7; d++, stime = stime.AddDays(1))
				{
					if (w == 0)			// This is necessary to display the day names in the user's language.
					{
						dayNameLabels[d].Text = stime.ToString("ddd");
						dayNameLabels[d].ForeColor = dayColor;
						dayNameLabels[d].Font = calFont;
					}

					if (stime.Day == selectedDate.Day && stime.Month == selectedDate.Month && stime.Year == selectedDate.Year)
					{
						selectedDayLabel = dayLabels[w, d];
						dayLabels[w, d].BackColor = selectedDateBackColor;
						dayLabels[w, d].ForeColor = selectedDateForeColor;
					}
					else if (stime.Month != selectedDate.Month)
					{
						dayLabels[w, d].ForeColor = inactiveForeColor;
						dayLabels[w, d].BackColor = Color.Transparent;
						if (stime.Day >= 28)
							lastMonthDate = stime;
					}
					else
					{
						dayLabels[w, d].ForeColor = foreColor;
						dayLabels[w, d].BackColor = Color.Transparent;
					}
					dayLabels[w, d].Font = calFont;

					if (stime.Day == todayDate.Day && stime.Month == todayDate.Month && stime.Year == todayDate.Year &&
							stime.Month == selectedDate.Month)
						dayLabels[w, d].Font = selectedDayFont;
					else
						dayLabels[w, d].Font = dayFont;

					dayLabels[w, d].Text = stime.Day.ToString();
					dayLabels[w, d].Date = stime;
				}
			}
		}

		protected override void OnPaintBackground (System.Windows.Forms.PaintEventArgs e)
		{
			Rectangle basecal = new Rectangle (DisplayRectangle.X, DisplayRectangle.Y + titleBar.Height, DisplayRectangle.Width,
				DisplayRectangle.Height - titleBar.Height);
			Rectangle titlebar = new Rectangle (DisplayRectangle.X, DisplayRectangle.Y, DisplayRectangle.Width, titleBar.Height);

			if (visualStyle)
			{
				if (useBaseGradient)
				{
					using (LinearGradientBrush lgbbase = new LinearGradientBrush (basecal, colorLeft, colorRight, calGradient))
					{
						if (calBlend != null)
							lgbbase.Blend = calBlend;
						e.Graphics.FillRectangle (lgbbase, basecal);
					}
				}
				else
				{
					using (SolidBrush sb = new SolidBrush (colorRight))
						e.Graphics.FillRectangle (sb, basecal);
				}

				using (LinearGradientBrush lgbtitle = new LinearGradientBrush (titlebar, titleBarColorLeft, titleBarColorRight, titleGradient))
				{
					if (titleBarBlend != null)
						lgbtitle.Blend = titleBarBlend;
					e.Graphics.FillRectangle (lgbtitle, titlebar);
				}

				Rectangle separator = new Rectangle (DisplayRectangle.X + 2, DisplayRectangle.Y + titleBar.Height + 16, DisplayRectangle.Width - 4, 2);
				using (LinearGradientBrush lgbsep = new LinearGradientBrush (separator, sepColorTop, sepColorBottom, LinearGradientMode.Vertical))
					e.Graphics.FillRectangle (lgbsep, separator);
			}
			else
			{
				e.Graphics.FillRectangle (new SolidBrush (colorRight), basecal);
				e.Graphics.FillRectangle (new SolidBrush (titleBarColorRight), titlebar);
				using (SolidBrush sb = new SolidBrush (sepColorBottom))
					using (Pen lnPen = new Pen (sb, 0.75F))
						e.Graphics.DrawLine (lnPen, DisplayRectangle.X + 2, DisplayRectangle.Y + titleBar.Height + 17,
							DisplayRectangle.Width - 4, DisplayRectangle.Y + titleBar.Height + 17);
			}
            e.Graphics.DrawRectangle(SystemPens.ControlDarkDark, 0, 0, Width - 1, Height - 1);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
				foreach (ToolTip tt in toolTips)
					tt.Dispose();
				if (titleTip != null)
					titleTip.Dispose();
				foreach (DayLabel dl in dayLabels)
					dl.Dispose();
				foreach (Label dnl in dayNameLabels)
					dnl.Dispose();
//				titleBar.Dispose();
//				calFont.Dispose();
//				dayFont.Dispose();
//				selectedDayFont.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.buttonLeft = new System.Windows.Forms.Button();
			this.titleBar = new System.Windows.Forms.Label();
			this.buttonRight = new System.Windows.Forms.Button();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.SuspendLayout();
			// 
			// buttonLeft
			// 
			this.buttonLeft.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonLeft.Location = new System.Drawing.Point(8, 4);
			this.buttonLeft.Name = "buttonLeft";
			this.buttonLeft.Size = new System.Drawing.Size(16, 23);
			this.buttonLeft.TabIndex = 1;
			this.buttonLeft.TabStop = false;
			this.buttonLeft.Text = "<";
			this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
			// 
			// titleBar
			// 
			this.titleBar.BackColor = System.Drawing.Color.Transparent;
			this.titleBar.Font = new System.Drawing.Font("Times New Roman", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.titleBar.Location = new System.Drawing.Point(0, 0);
			this.titleBar.Name = "titleBar";
			this.titleBar.Size = new System.Drawing.Size(200, 32);
			this.titleBar.TabIndex = 0;
			this.titleBar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.titleBar.Click += new System.EventHandler(this.titleBar_Click);
			// 
			// buttonRight
			// 
			this.buttonRight.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonRight.Location = new System.Drawing.Point(176, 4);
			this.buttonRight.Name = "buttonRight";
			this.buttonRight.Size = new System.Drawing.Size(16, 23);
			this.buttonRight.TabIndex = 2;
			this.buttonRight.TabStop = false;
			this.buttonRight.Text = ">";
			this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.CustomFormat = "MMMM yyyy";
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker.Location = new System.Drawing.Point(44, 6);
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.ShowUpDown = true;
			this.dateTimePicker.Size = new System.Drawing.Size(116, 20);
			this.dateTimePicker.TabIndex = 3;
			this.dateTimePicker.Visible = false;
			// 
			// VisualMonthCalendar
			// 
			this.Controls.Add(this.dateTimePicker);
			this.Controls.Add(this.buttonRight);
			this.Controls.Add(this.buttonLeft);
			this.Controls.Add(this.titleBar);
			this.Name = "VisualMonthCalendar";
			this.Size = new System.Drawing.Size(200, 152);
			this.ResumeLayout(false);
		}
		#endregion

		[BrowsableAttribute (false)]
		public System.Drawing.Drawing2D.Blend Blend
		{
			get
			{
				return calBlend;
			}
			set
			{
				calBlend = value;
				Invalidate();
			}
		}

		[BrowsableAttribute (false)]
		public System.Drawing.Drawing2D.Blend TitleBarBlend
		{
			get
			{
				return titleBarBlend;
			}
			set
			{
				titleBarBlend = value;
				Invalidate();
			}
		}

		[BrowsableAttribute (true)]
		public Color TitleBarForeColor
		{
			get
			{
				return titleBarForeColor;
			}
			set
			{
				titleBarForeColor = value;
				drawCalendar();
			}
		}
		
		public bool UseBaseGradient
		{
			get
			{
				return useBaseGradient;
			}
			set
			{
				useBaseGradient = true;
				Invalidate();
			}
		}

		public Color SeparatorColorTop
		{
			get
			{
				return sepColorTop;
			}
			set
			{
				sepColorTop = value;
				Invalidate();
			}
		}

		public Color SeparatorColorBottom
		{
			get
			{
				return sepColorBottom;
			}
			set
			{
				sepColorBottom = value;
				Invalidate();
			}
		}

		private void buttonRight_Click(object sender, System.EventArgs e)
		{
			selectedDayLabel.BackColor = Color.Transparent;
			selectedDayLabel.ForeColor = foreColor;
			SelectedDate = SelectedDate.AddMonths (1);
		}

		private void buttonLeft_Click(object sender, System.EventArgs e)
		{
			selectedDayLabel.BackColor = Color.Transparent;
			selectedDayLabel.ForeColor = foreColor;
			DateTime dt = selectedDayLabel.Date;
			if (dt.Day > lastMonthDate.Day)
				SelectedDate = lastMonthDate;
			else
				SelectedDate = SelectedDate.Subtract (new TimeSpan (lastMonthDate.Day, 0, 0, 0));
		}

		private void titleBar_Click(object sender, System.EventArgs e)
		{
			dateTimePicker.Visible = true;
			dateTimePicker.Focus();
		}

		private void dateTimePicker_LostFocus (object sender, System.EventArgs e)
		{
			dateTimePicker.Visible = false;
		}

		private void dateTimePicker_DateChanged (object sender, System.EventArgs e)
		{
			SelectedDate = dateTimePicker.Value;
		}

		public string ToolTipText
		{
			get
			{
				return toolTipText;
			}
			set
			{
				toolTipText = value;
				for (int w = 0; w < 6; w++)
				{
					for (int d = 0; d < 7; d++)
					{
						toolTips[w, d] = new ToolTip();
						toolTips[w, d].ShowAlways = true;
						toolTips[w, d].SetToolTip (dayLabels[w, d], toolTipText);
					}
				}
			}
		}

		public string TitleBarToolTipText
		{
			get
			{
				return titleBarTipText;
			}
			set
			{
				if (titleTip != null)
					titleTip.Dispose();
				titleBarTipText = value;
				titleTip = new ToolTip();
				titleTip.ShowAlways = true;
				titleTip.ReshowDelay = 0;
				titleTip.SetToolTip (titleBar, titleBarTipText);
			}
		}

		public Color ColorLeft
		{
			get
			{
				return colorLeft;
			}
			set
			{
				colorLeft = value;
				Invalidate();
			}
		}

		public Color ColorRight
		{
			get
			{
				return colorRight;
			}
			set
			{
				colorRight = value;
				Invalidate();
			}
		}

		public Color TitleBarColorLeft
		{
			get
			{
				return titleBarColorLeft;
			}
			set
			{
				titleBarColorLeft = value;
				Invalidate();
			}
		}

		public Color TitleBarColorRight
		{
			get
			{
				return titleBarColorRight;
			}
			set
			{
				titleBarColorRight = value;
				Invalidate();
			}
		}

		public Color SelectedDateForeColor
		{
			get
			{
				return selectedDateForeColor;
			}
			set
			{
				selectedDateForeColor = value;
				drawCalendar();
			}
		}

		public Color SelectedDateBackColor
		{
			get
			{
				return selectedDateBackColor;
			}
			set
			{
				selectedDateBackColor = value;
				drawCalendar();
			}
		}

		public override Color ForeColor
		{
			get
			{
				return foreColor;
			}
			set
			{
				foreColor = value;
				drawCalendar();
			}
		}

		public Color InactiveForeColor
		{
			get
			{
				return inactiveForeColor;
			}
			set
			{
				inactiveForeColor = value;
				drawCalendar();
			}
		}

		public bool VisualStyle
		{
			get
			{
				return visualStyle;
			}
			set
			{
				visualStyle = value;
				Invalidate();
				drawCalendar();
			}
		}

		public Font TitleFont
		{
			get
			{
				return titleBar.Font;
			}
			set
			{
				titleBar.Font = value;
			}
		}

		public Color DayColor
		{
			get
			{
				return dayColor;
			}
			set
			{
				dayColor = value;
				drawCalendar();
			}
		}

		public System.Drawing.Drawing2D.LinearGradientMode CalGradient
		{
			get
			{
				return calGradient;
			}
			set
			{
				calGradient = value;
				Invalidate();
			}
		}

		public System.Drawing.Drawing2D.LinearGradientMode TitleGradient
		{
			get
			{
				return titleGradient;
			}
			set
			{
				titleGradient = value;
				Invalidate();
			}
		}
	}
	#endregion

	// DayLabel: required by VisualMonthCalendar
	#region DayLabel
	public class DayLabel : System.Windows.Forms.Label
	{
		public DateTime Date;

		public DayLabel()
		{
			SetStyle (ControlStyles.Selectable, true);
			BackColor = Color.Transparent;
			TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			Size = new Size (28, 16);
		}
	}
	#endregion

	#region VisualPanel
	public class VisualPanel : System.Windows.Forms.Panel
	{
		private Color colorLeft, colorRight;
		private System.Drawing.Drawing2D.LinearGradientMode lgm;
		private bool visualStyle;
		private System.Drawing.Drawing2D.Blend blend;

		public VisualPanel()
		{
			SetStyle (ControlStyles.DoubleBuffer, true);
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.ResizeRedraw, true);

			colorLeft = SystemColors.ControlLightLight;
			colorRight = SystemColors.Control;
			lgm = LinearGradientMode.Horizontal;
			visualStyle = true;
		}

		protected override void OnPaintBackground (System.Windows.Forms.PaintEventArgs e)
		{
			if (visualStyle)
			{
				using (LinearGradientBrush lgb = new LinearGradientBrush (DisplayRectangle, colorLeft, colorRight, lgm))
				{
					if (blend != null)
						lgb.Blend = blend;
					e.Graphics.FillRectangle (lgb, DisplayRectangle);
				}
			}
			else
			{
				using (SolidBrush sb = new SolidBrush (BackColor))
					e.Graphics.FillRectangle (sb, DisplayRectangle);
			}
		}

		public System.Drawing.Drawing2D.Blend Blend
		{
			get
			{
				return blend;
			}
			set
			{
				blend = value;
				Invalidate();
			}
		}

		public bool VisualStyle
		{
			get
			{
				return visualStyle;
			}
			set
			{
				visualStyle = value;
				Invalidate();
			}
		}

		public Color LeftColor
		{
			get
			{
				return colorLeft;
			}
			set
			{
				colorLeft = value;
				Invalidate();
			}
		}

		public Color RightColor
		{
			get
			{
				return colorRight;
			}
			set
			{
				colorRight = value;
				Invalidate();
			}
		}

		public System.Drawing.Drawing2D.LinearGradientMode LinearGradientMode
		{
			get
			{
				return lgm;
			}
			set
			{
				lgm = value;
				Invalidate();
			}
		}
	}
		#endregion
}
