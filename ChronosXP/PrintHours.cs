#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - Print.cs
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Summary description for Print.
	/// </summary>
	public sealed partial class PrintHours : System.Windows.Forms.Form
	{
		private ChronosXP.XPLabel xpLabel1;
		private System.Windows.Forms.DateTimePicker dateTimePickerAux;
		private ChronosXP.XPLabel xpLabelAux;
		private ChronosXP.XPLabel xpLabelAnd;
		private ChronosXP.XPCheckBox xpCheckBoxDiary;
		private System.Windows.Forms.ComboBox comboBoxCity;
		private System.Windows.Forms.DateTimePicker dateTimePickerEnd;
		private System.Windows.Forms.RadioButton radioButtonPrintSelective;
		private ChronosXP.XPGroupBox xpGroupBoxTR;
		private System.Windows.Forms.Button buttonPrintPreview;
		private ChronosXP.XPGroupBox xpGroupBox1;
		private ChronosXP.XPGroupBox xPGroupBox2;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private ChronosXP.XPGroupBox xPGroupBox3;
		private ChronosXP.XPCheckBox xpCheckBoxMultiple;
		private System.Windows.Forms.DateTimePicker dateTimePickerStd;
		private System.Windows.Forms.DateTimePicker dateTimePickerStart;
		private System.Windows.Forms.RadioButton radioButtonPrintAll;
		private System.Windows.Forms.Button buttonPageSetup;
		private ChronosXP.XPCheckBox xpCheckBoxGlyphs;
		public const string Copyright = "Print.cs, Copyright  2004-2005 by Robert Misiak";

		private System.ComponentModel.Container components = null;
		private PrintDocument printDocument;
		private PageSetupDialog pageSetup;
		private PrintPreviewDialog printPreview;
		private DateTime when;
		private Place where;
		private Config conf;
		private int dayNo = 0, days = 1, pageNo = 0, pageTotal = 0, printHr = 0, displayHr = 0;
		private Calendar cal;

		public PrintHours (DateTime when, Place where, Config conf, Calendar cal)
		{
			this.when = when;
			this.where = where;
			this.conf = conf;
			this.cal = cal;

			printDocument = new PrintDocument();
			printDocument.PrintPage += new PrintPageEventHandler (printDocument_PrintPage);
			pageSetup = new PageSetupDialog();
			pageSetup.Document = printDocument;
			pageSetup.PageSettings.Landscape = conf.Landscape;
			printPreview = new PrintPreviewDialog();
			printPreview.Document = printDocument;
			printPreview.Text = conf.GetString("Print.PrintPreview");
			printPreview.WindowState = FormWindowState.Maximized;

			InitializeComponent();
			printPreview.Icon = this.Icon;

			dateTimePickerStd.Value = when;
			dateTimePickerAux.Value = when.AddDays(1);
			xpCheckBoxDiary.Checked = conf.Diary;
			xpCheckBoxMultiple.Checked = conf.PrintMultiple;
			xpCheckBoxGlyphs.Checked = conf.PrintGlyphs;

			cal.FormPrintOpen = true;
			/*
			 	foreach (Control c in xPGroupBox3.Controls)
				if (c is DateTimePicker)
				{
					DateTimePicker dtp = c as DateTimePicker;
					dtp.CustomFormat = Application.CurrentCulture.DateTimeFormat.MonthDayPattern + " yyyy";
				}
			*/
			foreach (Control c in xpGroupBoxTR.Controls)
				if (c is DateTimePicker)
				{
					DateTimePicker dtp = c as DateTimePicker;
					dtp.CustomFormat = Application.CurrentCulture.DateTimeFormat.ShortTimePattern;
				}

			if (conf.PrintAll)
				radioButtonPrintAll.Checked = true;
			else
				radioButtonPrintSelective.Checked = true;
			dateTimePickerStart.Value = conf.StartTime;
			dateTimePickerEnd.Value = conf.EndTime;

			for (int i = 0; i < conf.PlaceNum; i++)
				comboBoxCity.Items.Add (conf.Places[i].Name);
			comboBoxCity.Text = where.Name;
		}
		
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (conf.UseGradient && PInvoke.VisualStylesEnabled())
			{
				using (LinearGradientBrush lgb =
						   new LinearGradientBrush (DisplayRectangle, SystemColors.ControlLightLight, SystemColors.Control, LinearGradientMode.Vertical))
					e.Graphics.FillRectangle (lgb, DisplayRectangle);
			}
			else
			{
				e.Graphics.FillRectangle (SystemBrushes.ControlLight, DisplayRectangle);
			}
		}

		// Can't do this in .ctor because of the potential call to Close()
		protected override void OnLoad (System.EventArgs e)
		{
			int nprinters = 0;
			try
			{
				foreach (string printer in PrinterSettings.InstalledPrinters)
					if (printer != null) // required as to not generate a compiler warning
						nprinters++;
			}
			catch { }
			finally
			{
				if (nprinters == 0)
				{
					MessageBox.Show (conf.GetString("Print.NoPrinters"), "ChronosXP", MessageBoxButtons.OK, MessageBoxIcon.Error);
					Close();
				}
			}
		}

		protected override void OnClosing (System.ComponentModel.CancelEventArgs e)
		{
			cal.FormPrintOpen = false;
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
			}
			base.Dispose( disposing );
		}

		private bool inBounds (DateTime when)
		{
			if (radioButtonPrintAll.Checked)
				return true;
			DateTime start = new DateTime (when.Year, when.Month, when.Day, dateTimePickerStart.Value.Hour, dateTimePickerStart.Value.Minute, 0);
			DateTime end = new DateTime (when.Year, when.Month, when.Day, dateTimePickerEnd.Value.Hour, dateTimePickerEnd.Value.Minute, 0);
			if (when.CompareTo (start) >= 0 && when.CompareTo (end) <= 0)
				return true;
			else
				return false;
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintHours));
            this.xpCheckBoxGlyphs = new ChronosXP.XPCheckBox();
            this.buttonPageSetup = new System.Windows.Forms.Button();
            this.radioButtonPrintAll = new System.Windows.Forms.RadioButton();
            this.dateTimePickerStart = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerStd = new System.Windows.Forms.DateTimePicker();
            this.xpCheckBoxMultiple = new ChronosXP.XPCheckBox();
            this.xPGroupBox3 = new ChronosXP.XPGroupBox();
            this.xpLabel1 = new ChronosXP.XPLabel();
            this.dateTimePickerAux = new System.Windows.Forms.DateTimePicker();
            this.xpLabelAux = new ChronosXP.XPLabel();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.xPGroupBox2 = new ChronosXP.XPGroupBox();
            this.comboBoxCity = new System.Windows.Forms.ComboBox();
            this.xpGroupBox1 = new ChronosXP.XPGroupBox();
            this.xpCheckBoxDiary = new ChronosXP.XPCheckBox();
            this.buttonPrintPreview = new System.Windows.Forms.Button();
            this.xpGroupBoxTR = new ChronosXP.XPGroupBox();
            this.xpLabelAnd = new ChronosXP.XPLabel();
            this.dateTimePickerEnd = new System.Windows.Forms.DateTimePicker();
            this.radioButtonPrintSelective = new System.Windows.Forms.RadioButton();
            this.xPGroupBox3.SuspendLayout();
            this.xPGroupBox2.SuspendLayout();
            this.xpGroupBox1.SuspendLayout();
            this.xpGroupBoxTR.SuspendLayout();
            this.SuspendLayout();
            // 
            // xpCheckBoxGlyphs
            // 
            resources.ApplyResources(this.xpCheckBoxGlyphs, "xpCheckBoxGlyphs");
            this.xpCheckBoxGlyphs.Name = "xpCheckBoxGlyphs";
            this.xpCheckBoxGlyphs.CheckedChanged += new System.EventHandler(this.xpCheckBoxGlyphs_CheckedChanged);
            // 
            // buttonPageSetup
            // 
            resources.ApplyResources(this.buttonPageSetup, "buttonPageSetup");
            this.buttonPageSetup.Name = "buttonPageSetup";
            this.buttonPageSetup.Click += new System.EventHandler(this.buttonPageSetup_Click);
            // 
            // radioButtonPrintAll
            // 
            resources.ApplyResources(this.radioButtonPrintAll, "radioButtonPrintAll");
            this.radioButtonPrintAll.Name = "radioButtonPrintAll";
            this.radioButtonPrintAll.CheckedChanged += new System.EventHandler(this.radioButtonPrintAll_CheckedChanged);
            // 
            // dateTimePickerStart
            // 
            resources.ApplyResources(this.dateTimePickerStart, "dateTimePickerStart");
            this.dateTimePickerStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStart.Name = "dateTimePickerStart";
            this.dateTimePickerStart.ShowUpDown = true;
            this.dateTimePickerStart.ValueChanged += new System.EventHandler(this.dateTimePickerStart_ValueChanged);
            // 
            // dateTimePickerStd
            // 
            resources.ApplyResources(this.dateTimePickerStd, "dateTimePickerStd");
            this.dateTimePickerStd.CalendarTitleBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.dateTimePickerStd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerStd.Name = "dateTimePickerStd";
            this.dateTimePickerStd.ShowUpDown = true;
            this.dateTimePickerStd.ValueChanged += new System.EventHandler(this.dateTimePickerStd_ValueChanged);
            // 
            // xpCheckBoxMultiple
            // 
            resources.ApplyResources(this.xpCheckBoxMultiple, "xpCheckBoxMultiple");
            this.xpCheckBoxMultiple.Name = "xpCheckBoxMultiple";
            this.xpCheckBoxMultiple.CheckedChanged += new System.EventHandler(this.xpCheckBoxMultiple_CheckedChanged);
            // 
            // xPGroupBox3
            // 
            this.xPGroupBox3.BackColor = System.Drawing.Color.Transparent;
            this.xPGroupBox3.Controls.Add(this.xpLabel1);
            this.xPGroupBox3.Controls.Add(this.dateTimePickerStd);
            this.xPGroupBox3.Controls.Add(this.xpCheckBoxMultiple);
            this.xPGroupBox3.Controls.Add(this.dateTimePickerAux);
            this.xPGroupBox3.Controls.Add(this.xpLabelAux);
            resources.ApplyResources(this.xPGroupBox3, "xPGroupBox3");
            this.xPGroupBox3.Name = "xPGroupBox3";
            this.xPGroupBox3.TabStop = false;
            // 
            // xpLabel1
            // 
            resources.ApplyResources(this.xpLabel1, "xpLabel1");
            this.xpLabel1.Name = "xpLabel1";
            // 
            // dateTimePickerAux
            // 
            resources.ApplyResources(this.dateTimePickerAux, "dateTimePickerAux");
            this.dateTimePickerAux.CalendarTitleBackColor = System.Drawing.SystemColors.InactiveCaption;
            this.dateTimePickerAux.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerAux.Name = "dateTimePickerAux";
            this.dateTimePickerAux.ShowUpDown = true;
            // 
            // xpLabelAux
            // 
            resources.ApplyResources(this.xpLabelAux, "xpLabelAux");
            this.xpLabelAux.Name = "xpLabelAux";
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // xPGroupBox2
            // 
            this.xPGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.xPGroupBox2.Controls.Add(this.comboBoxCity);
            resources.ApplyResources(this.xPGroupBox2, "xPGroupBox2");
            this.xPGroupBox2.Name = "xPGroupBox2";
            this.xPGroupBox2.TabStop = false;
            // 
            // comboBoxCity
            // 
            this.comboBoxCity.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxCity, "comboBoxCity");
            this.comboBoxCity.Name = "comboBoxCity";
            this.comboBoxCity.Sorted = true;
            this.comboBoxCity.SelectedIndexChanged += new System.EventHandler(this.comboBoxCity_SelectedIndexChanged);
            // 
            // xpGroupBox1
            // 
            this.xpGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBox1.Controls.Add(this.xpCheckBoxDiary);
            this.xpGroupBox1.Controls.Add(this.xpCheckBoxGlyphs);
            resources.ApplyResources(this.xpGroupBox1, "xpGroupBox1");
            this.xpGroupBox1.Name = "xpGroupBox1";
            this.xpGroupBox1.TabStop = false;
            // 
            // xpCheckBoxDiary
            // 
            resources.ApplyResources(this.xpCheckBoxDiary, "xpCheckBoxDiary");
            this.xpCheckBoxDiary.Name = "xpCheckBoxDiary";
            this.xpCheckBoxDiary.CheckedChanged += new System.EventHandler(this.xpCheckBoxDiary_CheckedChanged);
            // 
            // buttonPrintPreview
            // 
            resources.ApplyResources(this.buttonPrintPreview, "buttonPrintPreview");
            this.buttonPrintPreview.Name = "buttonPrintPreview";
            this.buttonPrintPreview.Click += new System.EventHandler(this.buttonPrintPreview_Click);
            // 
            // xpGroupBoxTR
            // 
            this.xpGroupBoxTR.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBoxTR.Controls.Add(this.xpLabelAnd);
            this.xpGroupBoxTR.Controls.Add(this.dateTimePickerEnd);
            this.xpGroupBoxTR.Controls.Add(this.radioButtonPrintSelective);
            this.xpGroupBoxTR.Controls.Add(this.dateTimePickerStart);
            this.xpGroupBoxTR.Controls.Add(this.radioButtonPrintAll);
            resources.ApplyResources(this.xpGroupBoxTR, "xpGroupBoxTR");
            this.xpGroupBoxTR.Name = "xpGroupBoxTR";
            this.xpGroupBoxTR.TabStop = false;
            // 
            // xpLabelAnd
            // 
            resources.ApplyResources(this.xpLabelAnd, "xpLabelAnd");
            this.xpLabelAnd.Name = "xpLabelAnd";
            // 
            // dateTimePickerEnd
            // 
            resources.ApplyResources(this.dateTimePickerEnd, "dateTimePickerEnd");
            this.dateTimePickerEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerEnd.Name = "dateTimePickerEnd";
            this.dateTimePickerEnd.ShowUpDown = true;
            // 
            // radioButtonPrintSelective
            // 
            resources.ApplyResources(this.radioButtonPrintSelective, "radioButtonPrintSelective");
            this.radioButtonPrintSelective.Name = "radioButtonPrintSelective";
            this.radioButtonPrintSelective.CheckedChanged += new System.EventHandler(this.radioButtonPrintSelective_CheckedChanged);
            // 
            // PrintHours
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.xPGroupBox2);
            this.Controls.Add(this.xPGroupBox3);
            this.Controls.Add(this.xpGroupBoxTR);
            this.Controls.Add(this.xpGroupBox1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonPrintPreview);
            this.Controls.Add(this.buttonPageSetup);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintHours";
            this.xPGroupBox3.ResumeLayout(false);
            this.xPGroupBox2.ResumeLayout(false);
            this.xpGroupBox1.ResumeLayout(false);
            this.xpGroupBoxTR.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void xpCheckBoxMultiple_CheckedChanged(object sender, System.EventArgs e)
		{
			conf.PrintMultiple = xpCheckBoxMultiple.Checked;
			xpLabelAux.Enabled = xpCheckBoxMultiple.Checked;
			dateTimePickerAux.Enabled = xpCheckBoxMultiple.Checked;
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonPageSetup_Click(object sender, System.EventArgs e)
		{
			try
			{
				pageSetup.ShowDialog();
				conf.Landscape = pageSetup.PageSettings.Landscape;
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Unable to open Page Setup", ex);
			}
		}

		private void buttonPrintPreview_Click(object sender, System.EventArgs e)
		{
			try
			{
				calcPages();
				printPreview.ShowDialog();
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Print preview unavailable", ex);
			}
		}

		private void printNormal (DateTime when, System.Drawing.Printing.PrintPageEventArgs e, int curpg, int pgtotal)
		{
			PlanetaryHours pHours = new PlanetaryHours (when, where, conf);
			Font hdfont = new Font (Font.Name, 15.0F, FontStyle.Bold);
			Font sfont = new Font (Font.Name, 11.0F);
			Font hrfont = new Font (Font.Name, 10.0F);
			Font hrhdfont = new Font (Font.Name, 11.0F, FontStyle.Bold);

			float picx = e.MarginBounds.Left;
			Image dyim = Image.FromStream (conf.Fx.GlyphGif (pHours.EnglishDay()));
			float x = picx + dyim.Width + 11;
			float y = e.MarginBounds.Top;

			// Previously we used no StringFormat in DrawString and MeasureString, which made text (mainly at the lower-left margin) not formatted correctly.
			// Thanks to a microsoft.public.dotnet.languages.csharp posting by Ron Allen, we now use StringFormat.GenericDefault
			e.Graphics.DrawString (pHours.DayString(), hdfont, Brushes.Black, x, y, StringFormat.GenericDefault);
			y += e.Graphics.MeasureString (pHours.DayString(), hdfont, new PointF(x, y), StringFormat.GenericDefault).Height;

			string whenwhere = when.ToLongDateString() + " - " + where.Name;
			e.Graphics.DrawString (whenwhere, sfont, Brushes.Black, x + 1, y, StringFormat.GenericDefault);
			y += e.Graphics.MeasureString (whenwhere, sfont, new PointF (x, y), StringFormat.GenericDefault).Height + 2;

			string risetext, settext;
			if (!pHours.RS.SunRises)//(!pHours.RiseSet.SunRises)
				risetext = String.Format (conf.Res.GetString ("Calendar.NoSunRiseSet"),
					conf.Res.GetString ("Rise"), when.ToShortDateString(), where.Name);
			else
				risetext = String.Format (conf.Res.GetString ("Calendar.labelSunrise"),
					conf.FormatTime (pHours.Hours[0].StartTime),
					where.UseSystemTime ? "" : (" " + where.ZoneAbbreviation));
			if (!pHours.RS.SunSets)
				settext = String.Format (conf.Res.GetString ("Calendar.NoSunRiseSet"),
					conf.Res.GetString ("Set"), when.ToShortDateString(), where.Name);
			else
				settext = String.Format (conf.Res.GetString ("Calendar.labelSunset"),
					conf.FormatTime (pHours.Hours[12].StartTime),
					where.UseSystemTime ? "": (" " + where.ZoneAbbreviation));

			string riseset = risetext + " - " + settext;
			e.Graphics.DrawString (riseset, sfont, Brushes.Black, x + 1, y, StringFormat.GenericDefault);
			y += e.Graphics.MeasureString (riseset, sfont, new PointF (x, y), StringFormat.GenericDefault).Height + 2;

			float picy = e.MarginBounds.Top + (((y - e.MarginBounds.Top) - dyim.Height) / 2);
			if (y < (e.MarginBounds.Top + dyim.Height))
				y = picy + dyim.Height + 5;

			e.Graphics.DrawImage (dyim, picx, picy);

			x = e.MarginBounds.Left;  y += 11;
			e.Graphics.DrawLine (Pens.Gray, x, y, x + e.MarginBounds.Width, y);
			y += 22;

			e.Graphics.DrawString (conf.Res.GetString ("Calendar.DayHours"), hrhdfont, Brushes.Black, x, y, StringFormat.GenericDefault);
			e.Graphics.DrawString (conf.Res.GetString ("Calendar.NightHours"), hrhdfont, Brushes.Black, e.MarginBounds.Left + (e.MarginBounds.Width / 2), y, StringFormat.GenericDefault);
			y += e.Graphics.MeasureString (conf.Res.GetString ("Calendar.DayHours"), hrhdfont, new PointF (x, y), StringFormat.GenericDefault).Height + 9;
			float savey = y;
			for (int hr = 0; hr < 24; hr++)
			{
				if (hr == 12)
				{
					y = savey;
					x = e.MarginBounds.Left + (e.MarginBounds.Width / 2);
				}
				PlanetaryHours.Planet pl = pHours.Hours[hr].Hour;
				RectangleF rf = new RectangleF (x, y, 16, 16);
				Image im;
				if (conf.PrintGlyphs)
				{
					im = Image.FromStream (conf.Fx.GlyphIconAsStream (Color.Black, pHours.EnglishName (pl)));
					e.Graphics.DrawImage (im, rf);
				}
				else
					im = new Bitmap (1, 1);
				e.Graphics.DrawString (pHours.PlanetName (pl), hrfont, Brushes.Black, x + (conf.PrintGlyphs ? im.Width + 5 : 0), y, StringFormat.GenericDefault);
				e.Graphics.DrawString (conf.FormatTime (pHours.Hours[hr].StartTime), hrfont, Brushes.Black, x + 112, y, StringFormat.GenericDefault);
				if (conf.PrintGlyphs)
					y+= im.Height + 5;
				else
					y += e.Graphics.MeasureString (pHours.PlanetName (pl), hrfont, new PointF (x, y), StringFormat.GenericDefault).Height;
			}

			string ver = Config.FormatVersion();
			SizeF vsz = e.Graphics.MeasureString (ver, hrfont, e.MarginBounds.Width, StringFormat.GenericDefault);
			e.Graphics.DrawString (ver, hrfont, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Bottom - vsz.Height,
				StringFormat.GenericDefault);
			string pg = String.Format (conf.GetString ("Print.PageOf"), curpg, pgtotal);
			SizeF tdsz = e.Graphics.MeasureString (pg, hrfont, e.MarginBounds.Width, StringFormat.GenericDefault);
			e.Graphics.DrawString (pg, hrfont, Brushes.Black, e.MarginBounds.Right - tdsz.Width, e.MarginBounds.Bottom - tdsz.Height, StringFormat.GenericDefault);

			if (curpg != pgtotal)
				e.HasMorePages = true;
			dayNo++;
		}

		private int printDiary (DateTime when, System.Drawing.Printing.PrintPageEventArgs e, int curpg, int pgtotal)
		{
			PlanetaryHours pHours = new PlanetaryHours (when, where, conf);
			Font hdfont = new Font (Font.Name, 15.0F, FontStyle.Bold);
			Font sfont = new Font (Font.Name, 11.0F);
			Font hrfont = new Font (Font.Name, 10.0F);
			Font hrhdfont = new Font (Font.Name, 11.0F, FontStyle.Bold);

			float x, y;  bool hdrPrinted = false;
			if (printHr == 0)
			{
				displayHr = 0;
				hdrPrinted = true;
				float picx = e.MarginBounds.Left;
				Image im = Image.FromStream (conf.Fx.GlyphGif (pHours.EnglishDay()));
				x = picx + im.Width + 11;
				y = e.MarginBounds.Top;

				e.Graphics.DrawString (pHours.DayString(), hdfont, Brushes.Black, x, y, StringFormat.GenericDefault);
				y += e.Graphics.MeasureString (pHours.DayString(), hdfont, new PointF(x, y), StringFormat.GenericDefault).Height;

				string whenwhere = when.ToLongDateString() + " - " + where.Name;
				e.Graphics.DrawString (whenwhere, sfont, Brushes.Black, x + 1, y, StringFormat.GenericDefault);
				y += e.Graphics.MeasureString (whenwhere, sfont, new PointF (x, y), StringFormat.GenericDefault).Height + 2;

				string risetext, settext;
				if (!pHours.RS.SunRises)
					risetext = String.Format (conf.Res.GetString ("Calendar.NoSunRiseSet"),
						conf.Res.GetString ("Rise"), when.ToShortDateString(), where.Name);
				else
					risetext = String.Format (conf.Res.GetString ("Calendar.labelSunrise"),
						conf.FormatTime (pHours.Hours[0].StartTime),
						where.UseSystemTime ? "" : (" " + where.ZoneAbbreviation));
				if (!pHours.RS.SunSets)
					settext = String.Format (conf.Res.GetString ("Calendar.NoSunRiseSet"),
						conf.Res.GetString ("Set"), when.ToShortDateString(), where.Name);
				else
					settext = String.Format (conf.Res.GetString ("Calendar.labelSunset"),
						conf.FormatTime (pHours.Hours[12].StartTime),
						where.UseSystemTime ? "": (" " + where.ZoneAbbreviation));

				string riseset = risetext + " - " + settext;
				e.Graphics.DrawString (riseset, sfont, Brushes.Black, x + 1, y, StringFormat.GenericDefault);
				y += e.Graphics.MeasureString (riseset, sfont, new PointF (x, y), StringFormat.GenericDefault).Height + 2;

				float picy = e.MarginBounds.Top + (((y - e.MarginBounds.Top) - im.Height) / 2);
				if (y < (e.MarginBounds.Top + im.Height))
					y = picy + im.Height + 5;

				e.Graphics.DrawImage (im, picx, picy);

				x = e.MarginBounds.Left;  y += 11;
				e.Graphics.DrawLine (Pens.Gray, x, y, x + e.MarginBounds.Width, y);
				y += 33;
			}
			else
			{
				x = e.MarginBounds.Left;
				y = e.MarginBounds.Top;
				string str = String.Format ("{0} - {1} - {2}", pHours.DayString(), when.ToLongDateString(), where.Name);
				e.Graphics.DrawString (str, hrhdfont, Brushes.Black, x, y, StringFormat.GenericDefault);
				y += e.Graphics.MeasureString (str, hrhdfont, new PointF(x, y), StringFormat.GenericDefault).Height + 22;
			}

			float ght = hrfont.Height * 4;
			if (!hdrPrinted)
				ght += hrfont.Height / 2;
			TimeSpan os = new TimeSpan (0, 0, 1);
			while ((y + ght) <= e.MarginBounds.Bottom && printHr != 24)
			{
				if (inBounds (pHours.Hours[printHr].StartTime))
				{
					displayHr++;
					PlanetaryHours.Planet pl = pHours.Hours[printHr].Hour;
					string displaystr = pHours.HourString(pl) + " (";
					if (printHr == 0)
						displaystr += conf.GetString("Sunrise") + ", ";
					else if (printHr == 12)
						displaystr += conf.GetString("Sunset") + ", ";
					displaystr += conf.FormatTime (pHours.Hours[printHr].StartTime) + " - " +
												 conf.FormatTime (pHours.Hours[printHr + 1].StartTime.Subtract (os)) + ")";
					e.Graphics.DrawString (displaystr, hrfont, Brushes.Black, x, y, StringFormat.GenericDefault);
					float incr = e.Graphics.MeasureString (pHours.PlanetName (pl), hrfont, new PointF (x, y), StringFormat.GenericDefault).Height;
					y += (incr * 2);
					if (conf.PrintGlyphs)
					{
						RectangleF rf = new RectangleF (x, y + 1, 16, 16);
						Image im = Image.FromStream (conf.Fx.GlyphIconAsStream (Color.Black, pHours.EnglishName (pl)));
						e.Graphics.DrawImage (im, rf);
						e.Graphics.DrawLine (Pens.Gray, x + im.Width + 7, y, x + e.MarginBounds.Width, y);
						y += incr;
						e.Graphics.DrawLine (Pens.Gray, x + im.Width + 7, y, x + e.MarginBounds.Width, y);
						y += incr;
					}
					else
					{
						e.Graphics.DrawLine (Pens.Gray, x, y, x + e.MarginBounds.Width, y);
						y += incr;
						e.Graphics.DrawLine (Pens.Gray, x, y, x + e.MarginBounds.Width, y);
						y += incr;
					}
					// The first page has a long header; if not printing the first page, compensate for the absense of this header
					if (hdrPrinted)
						y += incr / 2;
				}
				printHr++;
			}

			string ver = Config.FormatVersion();
			SizeF vsz = e.Graphics.MeasureString (ver, hrfont, e.MarginBounds.Width, StringFormat.GenericDefault);
			e.Graphics.DrawString (ver, hrfont, Brushes.Black, e.MarginBounds.Left, e.MarginBounds.Bottom - vsz.Height,
				StringFormat.GenericDefault);

			// Try to calculate the number of pages.  A better way to do this would instead be, before printing, creating an array of Graphics surfaces
			// with each page.  When the page number is known, go back and write the "Page __ of __" to each surface.  Then, BitBlt() the appropriate
			// Graphics surface to e.Graphics.  I don't envision this algorithm will be innaccurate, but I think it might be possible, especially with the "wrong"
			// size paper.
			int pgs, hrs = 0;
			foreach (PlanetaryHours.PlanetaryHour ph in pHours.Hours)
				if (inBounds(ph.StartTime))
					hrs++;
			if (pgtotal == 0)
			{
				if (printHr == 24)
					pgs = pageNo;
				else if (radioButtonPrintAll.Checked)
				{
					pgs = 24 / (printHr / pageNo);
					if ((printHr * pgs) < 24)
						pgs++;
				}
				else
				{
					if (hrs > 9)
					{
						pgs = (hrs - 9) / 11;
						if ((displayHr * pgs) + 9 < hrs)
							pgs++;
						pgs++;
					}
					else
						pgs = 1;
				}
			}
			else
				pgs = pgtotal;
			string pg = String.Format (conf.GetString ("Print.PageOf"), pageNo, pgs);
			SizeF tdsz = e.Graphics.MeasureString (pg, hrfont, e.MarginBounds.Width, StringFormat.GenericDefault);
			e.Graphics.DrawString (pg, hrfont, Brushes.Black, e.MarginBounds.Right - tdsz.Width, e.MarginBounds.Bottom - tdsz.Height, StringFormat.GenericDefault);

			if (pageNo != pageTotal)
				e.HasMorePages = true;

			if (printHr == 24 || hrs == displayHr)
			{
				printHr = 0;
				dayNo++;
			}

			return pgs;
		}

		private void xpCheckBoxDiary_CheckedChanged(object sender, System.EventArgs e)
		{
			conf.Diary = xpCheckBoxDiary.Checked;
			xpGroupBoxTR.Enabled = xpCheckBoxDiary.Checked;
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Hide();
			conf.StartTime = dateTimePickerStart.Value;
			conf.EndTime = dateTimePickerEnd.Value;
			conf.PrintAll = radioButtonPrintAll.Checked;
			try
			{
				calcPages();
				printDocument.Print();
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Unable to print pages", ex);
			}
			finally
			{
				Close();
			}
		}

		private void calcPages()
		{
			setTitle();
			days = 1;  pageNo = 1;  dayNo = 0;  printHr = 0;
			if (xpCheckBoxMultiple.Checked)
			{
				DateTime inD = new DateTime (dateTimePickerStd.Value.Year, dateTimePickerStd.Value.Month, dateTimePickerStd.Value.Day);
				DateTime compD = new DateTime (dateTimePickerAux.Value.Year, dateTimePickerAux.Value.Month, dateTimePickerAux.Value.Day);
				for (DateTime dt = inD; dt.CompareTo(compD) != 0; dt = dt.AddDays(1))
					days++;
			}
			if (conf.Diary)
			{
				using (Bitmap b = new Bitmap(pageSetup.PageSettings.Bounds.Width, pageSetup.PageSettings.Bounds.Height))
				{
					using (Graphics fx = Graphics.FromImage(b))
					{
						PageSettings ps = pageSetup.PageSettings;
						Rectangle mb = new Rectangle (ps.Margins.Left, ps.Margins.Top, ps.Bounds.Width - ps.Margins.Left - ps.Margins.Right,
							ps.Bounds.Height - ps.Margins.Top - ps.Margins.Bottom);
						pageTotal = days * printDiary (when, new PrintPageEventArgs (fx, mb, ps.Bounds, ps), 1, 0);
						// printDiary() will modifiy some of these values
						days = 1;  pageNo = 0;  dayNo = 0;  printHr = 0;
					}
				}
			}
			else
				pageTotal = days;
		}

		private void printDocument_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			try
			{
				if (conf.Diary)
					printDiary (dateTimePickerStd.Value.AddDays(dayNo), e, pageNo++, pageTotal);
				else
					printNormal (dateTimePickerStd.Value.AddDays(dayNo), e, pageNo++, pageTotal);
			}
			catch (Exception ex)
			{
				conf.Core.ErrorMessage ("Failed drawing pages to print.", ex);
			}
		}

		private void setTitle()
		{
			string str = "ChronosXP (" + dateTimePickerStd.Value.ToShortDateString();
			if (xpCheckBoxMultiple.Checked)
				str += " - " + dateTimePickerAux.Value.ToShortDateString();
			str += ")";
			printDocument.DocumentName = str;
		}

		private void dateTimePickerStd_ValueChanged(object sender, System.EventArgs e)
		{
			// Ahhh... Unagi!
			dateTimePickerAux.MinDate = dateTimePickerStd.Value.AddDays (1);
		}

		private void xpCheckBoxGlyphs_CheckedChanged(object sender, System.EventArgs e)
		{
			conf.PrintGlyphs = xpCheckBoxGlyphs.Checked;
		}

		private void dateTimePickerStart_ValueChanged(object sender, System.EventArgs e)
		{
			//dateTimePickerEnd.MinDate = dateTimePickerStart.Value.AddMinutes(1);
		}

		private void radioButtonPrintSelective_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioButtonPrintSelective.Checked)
			{
				dateTimePickerStart.Enabled = true;
				dateTimePickerEnd.Enabled = true;
				xpLabelAnd.Enabled = true;
			}
		}

		private void radioButtonPrintAll_CheckedChanged(object sender, System.EventArgs e)
		{
			if (radioButtonPrintAll.Checked)
			{
				dateTimePickerStart.Enabled = false;
				dateTimePickerEnd.Enabled = false;
				xpLabelAnd.Enabled = false;
			}
		}

		private void comboBoxCity_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			where = conf.GetPlace (comboBoxCity.Text);
		}
	}
}
