#region Copyright  2003-2004 by Robert Misiak
// ChronosXP - Properties.cs
// Copyright  2003-2004 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ChronosXP
{
	/// <summary>
	/// User configuration dialog box
	/// </summary>
	public sealed class PropertiesBox : System.Windows.Forms.Form
	{
        private System.Windows.Forms.RadioButton radioButtonHungarian;
		private System.Windows.Forms.RadioButton radioButtonItalian;
		private System.Windows.Forms.RadioButton radioButtonFrench;
		private System.Windows.Forms.ComboBox comboBoxJupiter;
		private System.Windows.Forms.RadioButton radioButtonWaveFile;
		private ChronosXP.XPLinkLabel linkLabelMars;
		private System.Windows.Forms.Button buttonCancel;
		private ChronosXP.XPLinkLabel linkLabelMoon;
		private System.Windows.Forms.ComboBox comboBoxZones;
		private ChronosXP.XPCheckBox checkBoxStartup;
		private ChronosXP.XPLabel label10;
		private ChronosXP.XPLabel label11;
		private ChronosXP.XPLabel labelExampleShort;
		private ChronosXP.XPNumericUpDown numericLongMin;
		private System.Windows.Forms.Button buttonOK;
		private ChronosXP.XPLinkLabel linkLabelVenus;
		private ChronosXP.XPGroupBox xpGroupBoxColor;
		private ChronosXP.XPLinkLabel linkLabelSaturn;
		private System.Windows.Forms.ComboBox comboBoxVenus;
		private ChronosXP.XPGroupBox groupBox3;
		private ChronosXP.XPGroupBox groupBox2;
		private System.Windows.Forms.RadioButton radioButtonEnglish;
		private ChronosXP.XPTabPage tabPagePrecision;
		private ChronosXP.XPNumericUpDown numericLongDeg;
		private ChronosXP.XPTabPage tabPageStartup;
		private System.Windows.Forms.TextBox textBoxWaveFile;
		private ChronosXP.XPLabel label3;
		private ChronosXP.XPCheckBox checkBoxUpdate;
		private ChronosXP.XPLinkLabel linkLabelJupiter;
		private ChronosXP.XPLabel label7;
		private ChronosXP.XPLabel label6;
		private ChronosXP.XPTabPage tabPageNotify;
		private ChronosXP.XPLabel label9;
		private ChronosXP.XPLabel label8;
		private ChronosXP.XPLabel xpLabelIconSet;
		private System.Windows.Forms.RadioButton radioButtonPHCPhase;
		private ChronosXP.XPGroupBox xpGroupBoxLongitude;
		private System.Windows.Forms.Button buttonApply;
		private System.Windows.Forms.RadioButton radioButtonMulti;
		private ChronosXP.XPTabPage tabPageLanguage;
		private System.Windows.Forms.RadioButton radioButtonHourNumber;
		private ChronosXP.XPTabPage tabPageDisplay;
		private System.Windows.Forms.RadioButton radioButtonSouth;
		private System.Windows.Forms.ComboBox comboBoxSun;
		private System.Windows.Forms.RadioButton radioButtonSecond;
		private ChronosXP.XPCheckBox checkBoxSound;
		private System.Windows.Forms.RadioButton radioButtonLongTime;
		private System.Windows.Forms.RadioButton radioButton3DSilver;
		private System.Windows.Forms.RadioButton radioButtonWest;
		private ChronosXP.XPLinkLabel linkLabelSun;
		private ChronosXP.XPLabel xpLabelInPHC;
		private System.Windows.Forms.ComboBox comboBoxMercury;
		private System.Windows.Forms.RadioButton radioButtonDutch;
		private ChronosXP.XPLabel labelExampleLong;
		private System.Windows.Forms.RadioButton radioButtonSpanish;
		private System.Windows.Forms.RadioButton radioButtonEast;
		private System.Windows.Forms.RadioButton radioButtonSysSound;
		private ChronosXP.XPNumericUpDown numericLatMin;
		private ChronosXP.XPPanel panel4;
		private System.Windows.Forms.ComboBox comboBoxSaturn;
		private System.Windows.Forms.RadioButton radioButtonShortTime;
		private ChronosXP.XPNumericUpDown numericLatDeg;
		private System.Windows.Forms.ComboBox comboBoxMoon;
		private System.Windows.Forms.ComboBox comboBoxSounds;
		private System.Windows.Forms.ComboBox comboBoxMars;
		private ChronosXP.XPGroupBox xpGroupBoxFont;
		private System.Windows.Forms.RadioButton radioButtonBlack;
		private ChronosXP.XPTabPage tabPageSounds;
		private System.Windows.Forms.Button buttonFonts;
		private ChronosXP.XPCheckBox checkBoxTray;
		private ChronosXP.XPLabel xpLabelColor;
		private ChronosXP.XPCheckBox checkBoxDST;
		private System.Windows.Forms.ComboBox comboBoxCity;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.TabControl tabControl1;
		private ChronosXP.XPPanel xPPanel1;
		private ChronosXP.XPLinkLabel linkLabelMercury;
		private ChronosXP.XPCheckBox checkBoxNotify;
		private System.Windows.Forms.Button buttonChooseFile;
		private System.Windows.Forms.RadioButton radioButtonHouseMoment;
		private System.Windows.Forms.RadioButton radioButtonMinute;
		private ChronosXP.XPLabel xpLabelFont;
		private System.Windows.Forms.FontDialog fontDialog1;
		private ChronosXP.XPLinkLabel linkLabelAtlas;
		private System.Windows.Forms.RadioButton radioButtonPortuguese;
		private System.Windows.Forms.Button buttonPreviewSound;
		private ChronosXP.XPGroupBox xpGroupBoxTimeZone;
		private System.Windows.Forms.RadioButton radioButtonNorth;
		private ChronosXP.XPTabPage tabPageLocation;
		private ChronosXP.XPLabel label4;
		public const string Copyright = "Properties.cs, Copyright  2003-2004 by Robert Misiak";
		private System.ComponentModel.Container components = null;
        private RadioButton radioButtonKabbalistic;
        private RadioButton radioButtonHebrew;
        private RadioButton radioButtonGreek;
        private RadioButton radioButtonDefault;
        private RadioButton radioButtonHebrewEnglish;
		private Config conf;

		#region Constructor
		public PropertiesBox (Config conf)
		{
			InitializeComponent();
			this.conf = conf;
			Closing += new System.ComponentModel.CancelEventHandler (this.formClosing);
			conf.Core.FormPropertiesOpen = true;

			labelExampleShort.Text = conf.Res.GetString ("Properties.Example") + ": " + DateTime.Now.ToShortTimeString();
			labelExampleLong.Text = conf.Res.GetString ("Properties.Example") + ": " + DateTime.Now.ToLongTimeString();

			for (int i = 0; i < conf.PlaceNum; i++)
				comboBoxCity.Items.Add (conf.Places[i].Name);
			for (int i = 0; i < conf.EventCount; i++)
				comboBoxSounds.Items.Add (conf.EventList[i].Name);

			this.comboBoxCity.Text = conf.DefaultPlace.Name;

			switch (conf.Sound)
			{
				case Config.SoundTypes.WaveSelect:
					checkBoxSound.Checked = false;
					radioButtonWaveFile.Checked = true;
					break;
				case Config.SoundTypes.SystemSelect:
					checkBoxSound.Checked = false;
					radioButtonSysSound.Checked = true;
					break;
				case Config.SoundTypes.System:
					checkBoxSound.Checked = true;
					radioButtonSysSound.Checked = true;
					break;
				case Config.SoundTypes.Wave:
					checkBoxSound.Checked = true;
					radioButtonWaveFile.Checked = true;
					break;
			}
			comboBoxSounds.Text = conf.SoundName;

			if (conf.SoundFile != null)
				textBoxWaveFile.Text = conf.SoundFile;

			if (conf.Startup)
				checkBoxStartup.Checked = true;
			else
				checkBoxStartup.Checked = false;

			if (conf.UseZoneInfo)
			{
				for (int z = 0; z < conf.ZoneCount; z++)
					comboBoxZones.Items.Add (conf.ZoneList[z].Display);
				comboBoxZones.Text = conf.ZoneNameToDisplay (conf.CurrentZone);
				checkBoxDST.Checked = conf.ZoneSysApplyDST;
			}
			else
			{
				string zn;
				if (TimeZone.CurrentTimeZone.IsDaylightSavingTime (DateTime.Now))
					zn = TimeZone.CurrentTimeZone.DaylightName;
				else
					zn = TimeZone.CurrentTimeZone.StandardName;
				comboBoxZones.Items.Add (zn);
				comboBoxZones.Text = zn;
				checkBoxDST.Visible = false;
				XPLabel xpl = new XPLabel();
				xpl.BackColor = Color.Transparent;
				xpl.Location = checkBoxDST.Location;
				xpl.Size = checkBoxDST.Size;
				xpl.Text = conf.GetString("Properties.NoZoneInfo");
				xpGroupBoxTimeZone.Controls.Add (xpl);
			}

			checkBoxUpdate.Checked = conf.CheckUpgrade;

			//xpCheckBoxPrintGlyph.Checked = conf.PrintGlyphs;
			//if (conf.Diary)
			//	radioButtonPrintDiary.Checked = true;
			//else
			//	radioButtonPrintStandard.Checked = true;

            if (conf.Language == "*")
            {
                radioButtonDefault.Checked = true;
            }
			else if (conf.Language.Equals (Config.CultureNL))
			{
				radioButtonDutch.Checked = true;
				Point save = radioButtonDutch.Location;
				radioButtonDutch.Location = radioButtonEnglish.Location;
				radioButtonEnglish.Location = save;
			}
			else if (conf.Language.Equals (Config.CultureES))
			{
				radioButtonSpanish.Checked = true;
				Point save = radioButtonSpanish.Location;
				radioButtonSpanish.Location = radioButtonEnglish.Location;
				radioButtonEnglish.Location = save;
			}
			else if (conf.Language.Equals (Config.CultureIT))
			{
				radioButtonItalian.Checked = true;
				Point save = radioButtonItalian.Location;
				radioButtonItalian.Location = radioButtonEnglish.Location;
				radioButtonEnglish.Location = save;
			}
			else if (conf.Language.Equals(Config.CultureFR))
			{
				radioButtonFrench.Checked = true;
				Point save = radioButtonFrench.Location;
				radioButtonFrench.Location = radioButtonEnglish.Location;
				radioButtonEnglish.Location = save;
			}
			else if (conf.Language.Equals(Config.CulturePT))
			{
				radioButtonPortuguese.Checked = true;
				Point save = radioButtonPortuguese.Location;
				radioButtonPortuguese.Location = radioButtonEnglish.Location;
				radioButtonEnglish.Location = save;
			}
            else if (conf.Language.Equals(Config.CultureHU))
            {
                radioButtonHungarian.Checked = true;
                Point save = radioButtonHungarian.Location;
                radioButtonHungarian.Location = radioButtonEnglish.Location;
                radioButtonEnglish.Location = save;
            }
            else if (conf.Language.Equals(Config.CultureGR))
            {
                radioButtonGreek.Checked = true;
                Point save = radioButtonGreek.Location;
                radioButtonGreek.Location = radioButtonEnglish.Location;
                radioButtonEnglish.Location = save;
            }
            else if (conf.Language.Equals(Config.CultureHE))
            {
                radioButtonHebrewEnglish.Checked = true;
            }
            else
                radioButtonEnglish.Checked = true;

			checkBoxTray.Checked = conf.RunFromTray;

			if (conf.UseShortInterval)
				radioButtonSecond.Checked = true;
			else
				radioButtonMinute.Checked = true;

			if (conf.ShowSeconds)
				radioButtonLongTime.Checked = true;
			else
				radioButtonShortTime.Checked = true;
			
			switch (conf.Caption)
			{
				default:
				case Config.CaptionType.HourNumber:
					radioButtonHourNumber.Checked = true;
					break;
				case Config.CaptionType.HouseOfMoment:
					radioButtonHouseMoment.Checked = true;
					break;
				case Config.CaptionType.LunarPhase:
					radioButtonPHCPhase.Checked = true;
					break;
			}
			
			switch (conf.Fx.IconSet)
			{
				case "Silver":
					radioButton3DSilver.Checked = true;
					break;
				case "Black":
					radioButtonBlack.Checked = true;
					break;
				case "Multi":
					radioButtonMulti.Checked = true;
					break;
                case "Kabbalistic":
                case "Hebrew":
                    radioButtonHebrew.Checked = true;
                    break;
                    radioButtonKabbalistic.Checked = true;
                    break;
			}
            //if (!Config.ModernOS())
            //    radioButton3DSilver.Enabled = false;

			KnownColor enumColor = new KnownColor();
			Array colors = Enum.GetValues (enumColor.GetType());
			ComboBox[] plcolors = new ComboBox[] { comboBoxSun, comboBoxMoon, comboBoxMercury, comboBoxVenus, comboBoxMars,
													 comboBoxJupiter, comboBoxSaturn };
			for (int cc = 0; cc < colors.Length; cc++)
			{
				foreach (ComboBox cb in plcolors)
				{
					string s = colors.GetValue(cc).ToString();
					if (!s.Equals ("Transparent"))
						cb.Items.Add (s);
				}
			}
			foreach (ComboBox cb in plcolors)
			{
				cb.DrawItem += new System.Windows.Forms.DrawItemEventHandler (drawComboBoxItem);
				cb.DrawMode = DrawMode.OwnerDrawFixed;
			}
			comboBoxSun.Text = conf.PlanetColor (PlanetaryHours.Planet.Sun).Name;
			comboBoxMoon.Text = conf.PlanetColor(PlanetaryHours.Planet.Moon).Name;
			comboBoxMercury.Text = conf.PlanetColor(PlanetaryHours.Planet.Mercury).Name;
			comboBoxVenus.Text = conf.PlanetColor(PlanetaryHours.Planet.Venus).Name;
			comboBoxMars.Text = conf.PlanetColor(PlanetaryHours.Planet.Mars).Name;
			comboBoxJupiter.Text = conf.PlanetColor (PlanetaryHours.Planet.Jupiter).Name;
			comboBoxSaturn.Text = conf.PlanetColor (PlanetaryHours.Planet.Saturn).Name;
			checkBoxNotify.Checked = conf.NotifyHour;

			xpLabelFont.Text = String.Format ("{0}{1}, {2} pt.", conf.NotifyFont.Name,
				conf.NotifyFont.Style == FontStyle.Regular ? "" : " " + conf.NotifyFont.Style.ToString(),
				Math.Round (conf.NotifyFont.SizeInPoints));
			xpLabelFont.Font = (Font) conf.NotifyFont.Clone();
			fontDialog1.Font = xpLabelFont.Font;

			if (!PInvoke.VisualStylesEnabled())
				Config.FixRadioButtons (Controls, FlatStyle.Standard);

			// This is necessary if a user tries to view a NotifyWindow preview in standalone mode.
			if (conf.Core.pHours == null)
				conf.Core.pHours = new PlanetaryHours (conf);
		}
		#endregion

		#region Misc (addSpaces, formClosing, Dispose, and drawComboBoxItem)
		private void drawComboBoxItem (object sender, System.Windows.Forms.DrawItemEventArgs e)
		{
			ComboBox cb = sender as ComboBox;
			Color useClr, useText;
			Pen useRect;
			string clr = (string) cb.Items[e.Index];
			if (cb.Parent.Enabled)
			{
				useClr = Color.FromName (clr);
				useText = e.ForeColor;
				useRect = Pens.Black;
			}
			else
			{
				useClr = Color.Gainsboro;
				useText = SystemColors.GrayText;
				useRect = Pens.Gray;
			}
			using (SolidBrush sb = new SolidBrush (e.BackColor))
				e.Graphics.FillRectangle (sb, e.Bounds);
			int wh = e.Bounds.Height - 3;
			e.Graphics.DrawRectangle (useRect, e.Bounds.X + 2, e.Bounds.Y + 1, e.Bounds.Height - 2, e.Bounds.Height - 2);
			Rectangle rLeft = new Rectangle (e.Bounds.X + 3, e.Bounds.Y + 2, wh, wh);
			Rectangle rRight = new Rectangle (e.Bounds.X + wh + 3, e.Bounds.Y, e.Bounds.Width - wh - 3, e.Bounds.Height);
			using (SolidBrush sb = new SolidBrush (useClr))
				e.Graphics.FillRectangle (sb, rLeft);
			using (SolidBrush sb = new SolidBrush (useText))
				e.Graphics.DrawString (addSpaces (clr), e.Font, sb, rRight, StringFormat.GenericDefault);
		}

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

		private void formClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			conf.Core.FormPropertiesOpen = false;
			if (conf.lastNw != null && conf.lastNw.IsPreview)
				conf.lastNw.Close();
		}

		/// <summary>
		/// Add a space before capital letters.  ex:  "CornflowerBlue" -> " Cornflower Blue"
		/// </summary>
		private string addSpaces (string inp)
		{
			return Regex.Replace (inp, "([A-Z])", " $1");
		}
		#endregion

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PropertiesBox));
            this.fontDialog1 = new System.Windows.Forms.FontDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonApply = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPageLocation = new ChronosXP.XPTabPage();
            this.xpGroupBoxTimeZone = new ChronosXP.XPGroupBox();
            this.checkBoxDST = new ChronosXP.XPCheckBox();
            this.comboBoxZones = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new ChronosXP.XPGroupBox();
            this.comboBoxCity = new System.Windows.Forms.ComboBox();
            this.groupBox2 = new ChronosXP.XPGroupBox();
            this.label9 = new ChronosXP.XPLabel();
            this.numericLatDeg = new ChronosXP.XPNumericUpDown();
            this.label6 = new ChronosXP.XPLabel();
            this.numericLatMin = new ChronosXP.XPNumericUpDown();
            this.radioButtonSouth = new System.Windows.Forms.RadioButton();
            this.radioButtonNorth = new System.Windows.Forms.RadioButton();
            this.xpGroupBoxLongitude = new ChronosXP.XPGroupBox();
            this.label8 = new ChronosXP.XPLabel();
            this.label7 = new ChronosXP.XPLabel();
            this.numericLongMin = new ChronosXP.XPNumericUpDown();
            this.numericLongDeg = new ChronosXP.XPNumericUpDown();
            this.radioButtonEast = new System.Windows.Forms.RadioButton();
            this.radioButtonWest = new System.Windows.Forms.RadioButton();
            this.linkLabelAtlas = new ChronosXP.XPLinkLabel();
            this.tabPageNotify = new ChronosXP.XPTabPage();
            this.xpLabelColor = new ChronosXP.XPLabel();
            this.buttonFonts = new System.Windows.Forms.Button();
            this.xpGroupBoxFont = new ChronosXP.XPGroupBox();
            this.xpLabelFont = new ChronosXP.XPLabel();
            this.xpGroupBoxColor = new ChronosXP.XPGroupBox();
            this.linkLabelSaturn = new ChronosXP.XPLinkLabel();
            this.linkLabelJupiter = new ChronosXP.XPLinkLabel();
            this.linkLabelMars = new ChronosXP.XPLinkLabel();
            this.linkLabelVenus = new ChronosXP.XPLinkLabel();
            this.linkLabelMercury = new ChronosXP.XPLinkLabel();
            this.linkLabelMoon = new ChronosXP.XPLinkLabel();
            this.linkLabelSun = new ChronosXP.XPLinkLabel();
            this.comboBoxSaturn = new System.Windows.Forms.ComboBox();
            this.comboBoxJupiter = new System.Windows.Forms.ComboBox();
            this.comboBoxMars = new System.Windows.Forms.ComboBox();
            this.comboBoxVenus = new System.Windows.Forms.ComboBox();
            this.comboBoxMercury = new System.Windows.Forms.ComboBox();
            this.comboBoxMoon = new System.Windows.Forms.ComboBox();
            this.comboBoxSun = new System.Windows.Forms.ComboBox();
            this.checkBoxNotify = new ChronosXP.XPCheckBox();
            this.tabPageSounds = new ChronosXP.XPTabPage();
            this.checkBoxSound = new ChronosXP.XPCheckBox();
            this.textBoxWaveFile = new System.Windows.Forms.TextBox();
            this.radioButtonWaveFile = new System.Windows.Forms.RadioButton();
            this.buttonChooseFile = new System.Windows.Forms.Button();
            this.buttonPreviewSound = new System.Windows.Forms.Button();
            this.comboBoxSounds = new System.Windows.Forms.ComboBox();
            this.radioButtonSysSound = new System.Windows.Forms.RadioButton();
            this.tabPageDisplay = new ChronosXP.XPTabPage();
            this.radioButtonKabbalistic = new System.Windows.Forms.RadioButton();
            this.radioButtonHebrew = new System.Windows.Forms.RadioButton();
            this.xPPanel1 = new ChronosXP.XPPanel();
            this.radioButtonPHCPhase = new System.Windows.Forms.RadioButton();
            this.radioButtonHouseMoment = new System.Windows.Forms.RadioButton();
            this.radioButtonHourNumber = new System.Windows.Forms.RadioButton();
            this.radioButtonMulti = new System.Windows.Forms.RadioButton();
            this.radioButtonBlack = new System.Windows.Forms.RadioButton();
            this.radioButton3DSilver = new System.Windows.Forms.RadioButton();
            this.xpLabelIconSet = new ChronosXP.XPLabel();
            this.xpLabelInPHC = new ChronosXP.XPLabel();
            this.tabPageLanguage = new ChronosXP.XPTabPage();
            this.radioButtonHebrewEnglish = new System.Windows.Forms.RadioButton();
            this.radioButtonDefault = new System.Windows.Forms.RadioButton();
            this.radioButtonGreek = new System.Windows.Forms.RadioButton();
            this.radioButtonHungarian = new System.Windows.Forms.RadioButton();
            this.radioButtonPortuguese = new System.Windows.Forms.RadioButton();
            this.radioButtonFrench = new System.Windows.Forms.RadioButton();
            this.radioButtonItalian = new System.Windows.Forms.RadioButton();
            this.radioButtonSpanish = new System.Windows.Forms.RadioButton();
            this.label3 = new ChronosXP.XPLabel();
            this.radioButtonEnglish = new System.Windows.Forms.RadioButton();
            this.radioButtonDutch = new System.Windows.Forms.RadioButton();
            this.tabPagePrecision = new ChronosXP.XPTabPage();
            this.label11 = new ChronosXP.XPLabel();
            this.panel4 = new ChronosXP.XPPanel();
            this.labelExampleLong = new ChronosXP.XPLabel();
            this.labelExampleShort = new ChronosXP.XPLabel();
            this.radioButtonLongTime = new System.Windows.Forms.RadioButton();
            this.radioButtonShortTime = new System.Windows.Forms.RadioButton();
            this.radioButtonSecond = new System.Windows.Forms.RadioButton();
            this.label4 = new ChronosXP.XPLabel();
            this.radioButtonMinute = new System.Windows.Forms.RadioButton();
            this.tabPageStartup = new ChronosXP.XPTabPage();
            this.checkBoxTray = new ChronosXP.XPCheckBox();
            this.checkBoxUpdate = new ChronosXP.XPCheckBox();
            this.checkBoxStartup = new ChronosXP.XPCheckBox();
            this.label10 = new ChronosXP.XPLabel();
            this.tabControl1.SuspendLayout();
            this.tabPageLocation.SuspendLayout();
            this.xpGroupBoxTimeZone.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.xpGroupBoxLongitude.SuspendLayout();
            this.tabPageNotify.SuspendLayout();
            this.xpGroupBoxFont.SuspendLayout();
            this.xpGroupBoxColor.SuspendLayout();
            this.tabPageSounds.SuspendLayout();
            this.tabPageDisplay.SuspendLayout();
            this.xPPanel1.SuspendLayout();
            this.tabPageLanguage.SuspendLayout();
            this.tabPagePrecision.SuspendLayout();
            this.panel4.SuspendLayout();
            this.tabPageStartup.SuspendLayout();
            this.SuspendLayout();
            // 
            // fontDialog1
            // 
            this.fontDialog1.AllowScriptChange = false;
            this.fontDialog1.AllowVerticalFonts = false;
            this.fontDialog1.FontMustExist = true;
            this.fontDialog1.ShowEffects = false;
            // 
            // openFileDialog1
            // 
            resources.ApplyResources(this.openFileDialog1, "openFileDialog1");
            this.openFileDialog1.ShowHelp = true;
            // 
            // buttonApply
            // 
            this.buttonApply.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonApply, "buttonApply");
            this.buttonApply.Name = "buttonApply";
            this.buttonApply.UseVisualStyleBackColor = false;
            this.buttonApply.Click += new System.EventHandler(this.buttonApply_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.BackColor = System.Drawing.Color.Transparent;
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPageLocation);
            this.tabControl1.Controls.Add(this.tabPageNotify);
            this.tabControl1.Controls.Add(this.tabPageSounds);
            this.tabControl1.Controls.Add(this.tabPageDisplay);
            this.tabControl1.Controls.Add(this.tabPageLanguage);
            this.tabControl1.Controls.Add(this.tabPagePrecision);
            this.tabControl1.Controls.Add(this.tabPageStartup);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPageLocation
            // 
            this.tabPageLocation.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLocation.Controls.Add(this.xpGroupBoxTimeZone);
            this.tabPageLocation.Controls.Add(this.groupBox3);
            this.tabPageLocation.Controls.Add(this.groupBox2);
            this.tabPageLocation.Controls.Add(this.xpGroupBoxLongitude);
            this.tabPageLocation.Controls.Add(this.linkLabelAtlas);
            resources.ApplyResources(this.tabPageLocation, "tabPageLocation");
            this.tabPageLocation.Name = "tabPageLocation";
            // 
            // xpGroupBoxTimeZone
            // 
            this.xpGroupBoxTimeZone.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBoxTimeZone.Controls.Add(this.checkBoxDST);
            this.xpGroupBoxTimeZone.Controls.Add(this.comboBoxZones);
            resources.ApplyResources(this.xpGroupBoxTimeZone, "xpGroupBoxTimeZone");
            this.xpGroupBoxTimeZone.Name = "xpGroupBoxTimeZone";
            this.xpGroupBoxTimeZone.TabStop = false;
            // 
            // checkBoxDST
            // 
            this.checkBoxDST.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxDST, "checkBoxDST");
            this.checkBoxDST.Name = "checkBoxDST";
            this.checkBoxDST.UseVisualStyleBackColor = false;
            // 
            // comboBoxZones
            // 
            this.comboBoxZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxZones, "comboBoxZones");
            this.comboBoxZones.Name = "comboBoxZones";
            this.comboBoxZones.Sorted = true;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Transparent;
            this.groupBox3.Controls.Add(this.comboBoxCity);
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // comboBoxCity
            // 
            resources.ApplyResources(this.comboBoxCity, "comboBoxCity");
            this.comboBoxCity.Name = "comboBoxCity";
            this.comboBoxCity.Sorted = true;
            this.comboBoxCity.SelectedIndexChanged += new System.EventHandler(this.comboBoxCity_SelectedIndexChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.numericLatDeg);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.numericLatMin);
            this.groupBox2.Controls.Add(this.radioButtonSouth);
            this.groupBox2.Controls.Add(this.radioButtonNorth);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // numericLatDeg
            // 
            resources.ApplyResources(this.numericLatDeg, "numericLatDeg");
            this.numericLatDeg.Maximum = 89;
            this.numericLatDeg.Minimum = 0;
            this.numericLatDeg.Name = "numericLatDeg";
            this.numericLatDeg.Value = 0;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // numericLatMin
            // 
            resources.ApplyResources(this.numericLatMin, "numericLatMin");
            this.numericLatMin.Maximum = 59;
            this.numericLatMin.Minimum = 0;
            this.numericLatMin.Name = "numericLatMin";
            this.numericLatMin.Value = 0;
            // 
            // radioButtonSouth
            // 
            resources.ApplyResources(this.radioButtonSouth, "radioButtonSouth");
            this.radioButtonSouth.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonSouth.Name = "radioButtonSouth";
            this.radioButtonSouth.TabStop = true;
            this.radioButtonSouth.UseVisualStyleBackColor = false;
            // 
            // radioButtonNorth
            // 
            resources.ApplyResources(this.radioButtonNorth, "radioButtonNorth");
            this.radioButtonNorth.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonNorth.Name = "radioButtonNorth";
            this.radioButtonNorth.TabStop = true;
            this.radioButtonNorth.UseVisualStyleBackColor = false;
            // 
            // xpGroupBoxLongitude
            // 
            this.xpGroupBoxLongitude.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBoxLongitude.Controls.Add(this.label8);
            this.xpGroupBoxLongitude.Controls.Add(this.label7);
            this.xpGroupBoxLongitude.Controls.Add(this.numericLongMin);
            this.xpGroupBoxLongitude.Controls.Add(this.numericLongDeg);
            this.xpGroupBoxLongitude.Controls.Add(this.radioButtonEast);
            this.xpGroupBoxLongitude.Controls.Add(this.radioButtonWest);
            resources.ApplyResources(this.xpGroupBoxLongitude, "xpGroupBoxLongitude");
            this.xpGroupBoxLongitude.Name = "xpGroupBoxLongitude";
            this.xpGroupBoxLongitude.TabStop = false;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // numericLongMin
            // 
            resources.ApplyResources(this.numericLongMin, "numericLongMin");
            this.numericLongMin.Maximum = 59;
            this.numericLongMin.Minimum = 0;
            this.numericLongMin.Name = "numericLongMin";
            this.numericLongMin.Value = 0;
            // 
            // numericLongDeg
            // 
            resources.ApplyResources(this.numericLongDeg, "numericLongDeg");
            this.numericLongDeg.Maximum = 179;
            this.numericLongDeg.Minimum = 0;
            this.numericLongDeg.Name = "numericLongDeg";
            this.numericLongDeg.Value = 0;
            // 
            // radioButtonEast
            // 
            resources.ApplyResources(this.radioButtonEast, "radioButtonEast");
            this.radioButtonEast.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonEast.Name = "radioButtonEast";
            this.radioButtonEast.TabStop = true;
            this.radioButtonEast.UseVisualStyleBackColor = false;
            // 
            // radioButtonWest
            // 
            resources.ApplyResources(this.radioButtonWest, "radioButtonWest");
            this.radioButtonWest.BackColor = System.Drawing.Color.Transparent;
            this.radioButtonWest.Name = "radioButtonWest";
            this.radioButtonWest.TabStop = true;
            this.radioButtonWest.UseVisualStyleBackColor = false;
            // 
            // linkLabelAtlas
            // 
            this.linkLabelAtlas.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.linkLabelAtlas, "linkLabelAtlas");
            this.linkLabelAtlas.Name = "linkLabelAtlas";
            this.linkLabelAtlas.TabStop = true;
            this.linkLabelAtlas.UseCompatibleTextRendering = true;
            this.linkLabelAtlas.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelAtlas_LinkClicked);
            // 
            // tabPageNotify
            // 
            this.tabPageNotify.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageNotify.Controls.Add(this.xpLabelColor);
            this.tabPageNotify.Controls.Add(this.buttonFonts);
            this.tabPageNotify.Controls.Add(this.xpGroupBoxFont);
            this.tabPageNotify.Controls.Add(this.xpGroupBoxColor);
            this.tabPageNotify.Controls.Add(this.checkBoxNotify);
            resources.ApplyResources(this.tabPageNotify, "tabPageNotify");
            this.tabPageNotify.Name = "tabPageNotify";
            // 
            // xpLabelColor
            // 
            this.xpLabelColor.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.xpLabelColor, "xpLabelColor");
            this.xpLabelColor.Name = "xpLabelColor";
            // 
            // buttonFonts
            // 
            resources.ApplyResources(this.buttonFonts, "buttonFonts");
            this.buttonFonts.Name = "buttonFonts";
            this.buttonFonts.Click += new System.EventHandler(this.buttonFonts_Click);
            // 
            // xpGroupBoxFont
            // 
            this.xpGroupBoxFont.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBoxFont.Controls.Add(this.xpLabelFont);
            resources.ApplyResources(this.xpGroupBoxFont, "xpGroupBoxFont");
            this.xpGroupBoxFont.Name = "xpGroupBoxFont";
            this.xpGroupBoxFont.TabStop = false;
            // 
            // xpLabelFont
            // 
            resources.ApplyResources(this.xpLabelFont, "xpLabelFont");
            this.xpLabelFont.Name = "xpLabelFont";
            // 
            // xpGroupBoxColor
            // 
            this.xpGroupBoxColor.BackColor = System.Drawing.Color.Transparent;
            this.xpGroupBoxColor.Controls.Add(this.linkLabelSaturn);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelJupiter);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelMars);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelVenus);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelMercury);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelMoon);
            this.xpGroupBoxColor.Controls.Add(this.linkLabelSun);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxSaturn);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxJupiter);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxMars);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxVenus);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxMercury);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxMoon);
            this.xpGroupBoxColor.Controls.Add(this.comboBoxSun);
            resources.ApplyResources(this.xpGroupBoxColor, "xpGroupBoxColor");
            this.xpGroupBoxColor.Name = "xpGroupBoxColor";
            this.xpGroupBoxColor.TabStop = false;
            // 
            // linkLabelSaturn
            // 
            resources.ApplyResources(this.linkLabelSaturn, "linkLabelSaturn");
            this.linkLabelSaturn.Name = "linkLabelSaturn";
            this.linkLabelSaturn.TabStop = true;
            this.linkLabelSaturn.UseCompatibleTextRendering = true;
            this.linkLabelSaturn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSaturn_LinkClicked);
            // 
            // linkLabelJupiter
            // 
            resources.ApplyResources(this.linkLabelJupiter, "linkLabelJupiter");
            this.linkLabelJupiter.Name = "linkLabelJupiter";
            this.linkLabelJupiter.TabStop = true;
            this.linkLabelJupiter.UseCompatibleTextRendering = true;
            this.linkLabelJupiter.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelJupiter_LinkClicked);
            // 
            // linkLabelMars
            // 
            resources.ApplyResources(this.linkLabelMars, "linkLabelMars");
            this.linkLabelMars.Name = "linkLabelMars";
            this.linkLabelMars.TabStop = true;
            this.linkLabelMars.UseCompatibleTextRendering = true;
            this.linkLabelMars.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMars_LinkClicked);
            // 
            // linkLabelVenus
            // 
            resources.ApplyResources(this.linkLabelVenus, "linkLabelVenus");
            this.linkLabelVenus.Name = "linkLabelVenus";
            this.linkLabelVenus.TabStop = true;
            this.linkLabelVenus.UseCompatibleTextRendering = true;
            this.linkLabelVenus.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelVenus_LinkClicked);
            // 
            // linkLabelMercury
            // 
            resources.ApplyResources(this.linkLabelMercury, "linkLabelMercury");
            this.linkLabelMercury.Name = "linkLabelMercury";
            this.linkLabelMercury.TabStop = true;
            this.linkLabelMercury.UseCompatibleTextRendering = true;
            this.linkLabelMercury.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMercury_LinkClicked);
            // 
            // linkLabelMoon
            // 
            resources.ApplyResources(this.linkLabelMoon, "linkLabelMoon");
            this.linkLabelMoon.Name = "linkLabelMoon";
            this.linkLabelMoon.TabStop = true;
            this.linkLabelMoon.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelMoon_LinkClicked);
            // 
            // linkLabelSun
            // 
            resources.ApplyResources(this.linkLabelSun, "linkLabelSun");
            this.linkLabelSun.Name = "linkLabelSun";
            this.linkLabelSun.TabStop = true;
            this.linkLabelSun.UseCompatibleTextRendering = true;
            this.linkLabelSun.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelSun_LinkClicked);
            // 
            // comboBoxSaturn
            // 
            this.comboBoxSaturn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSaturn, "comboBoxSaturn");
            this.comboBoxSaturn.Name = "comboBoxSaturn";
            // 
            // comboBoxJupiter
            // 
            this.comboBoxJupiter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxJupiter, "comboBoxJupiter");
            this.comboBoxJupiter.Name = "comboBoxJupiter";
            // 
            // comboBoxMars
            // 
            this.comboBoxMars.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMars, "comboBoxMars");
            this.comboBoxMars.Name = "comboBoxMars";
            // 
            // comboBoxVenus
            // 
            this.comboBoxVenus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxVenus, "comboBoxVenus");
            this.comboBoxVenus.Name = "comboBoxVenus";
            // 
            // comboBoxMercury
            // 
            this.comboBoxMercury.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMercury, "comboBoxMercury");
            this.comboBoxMercury.Name = "comboBoxMercury";
            // 
            // comboBoxMoon
            // 
            this.comboBoxMoon.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxMoon, "comboBoxMoon");
            this.comboBoxMoon.Name = "comboBoxMoon";
            // 
            // comboBoxSun
            // 
            this.comboBoxSun.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSun, "comboBoxSun");
            this.comboBoxSun.Name = "comboBoxSun";
            // 
            // checkBoxNotify
            // 
            this.checkBoxNotify.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxNotify, "checkBoxNotify");
            this.checkBoxNotify.Name = "checkBoxNotify";
            this.checkBoxNotify.UseVisualStyleBackColor = false;
            this.checkBoxNotify.CheckedChanged += new System.EventHandler(this.checkBoxNotify_CheckedChanged);
            // 
            // tabPageSounds
            // 
            this.tabPageSounds.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageSounds.Controls.Add(this.checkBoxSound);
            this.tabPageSounds.Controls.Add(this.textBoxWaveFile);
            this.tabPageSounds.Controls.Add(this.radioButtonWaveFile);
            this.tabPageSounds.Controls.Add(this.buttonChooseFile);
            this.tabPageSounds.Controls.Add(this.buttonPreviewSound);
            this.tabPageSounds.Controls.Add(this.comboBoxSounds);
            this.tabPageSounds.Controls.Add(this.radioButtonSysSound);
            resources.ApplyResources(this.tabPageSounds, "tabPageSounds");
            this.tabPageSounds.Name = "tabPageSounds";
            // 
            // checkBoxSound
            // 
            this.checkBoxSound.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxSound, "checkBoxSound");
            this.checkBoxSound.Name = "checkBoxSound";
            this.checkBoxSound.UseVisualStyleBackColor = false;
            this.checkBoxSound.CheckedChanged += new System.EventHandler(this.checkBoxSound_CheckedChanged);
            // 
            // textBoxWaveFile
            // 
            this.textBoxWaveFile.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxWaveFile.Cursor = System.Windows.Forms.Cursors.Default;
            resources.ApplyResources(this.textBoxWaveFile, "textBoxWaveFile");
            this.textBoxWaveFile.Name = "textBoxWaveFile";
            this.textBoxWaveFile.ReadOnly = true;
            this.textBoxWaveFile.TabStop = false;
            // 
            // radioButtonWaveFile
            // 
            this.radioButtonWaveFile.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonWaveFile, "radioButtonWaveFile");
            this.radioButtonWaveFile.Name = "radioButtonWaveFile";
            this.radioButtonWaveFile.UseVisualStyleBackColor = false;
            // 
            // buttonChooseFile
            // 
            this.buttonChooseFile.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonChooseFile, "buttonChooseFile");
            this.buttonChooseFile.Name = "buttonChooseFile";
            this.buttonChooseFile.UseVisualStyleBackColor = false;
            this.buttonChooseFile.Click += new System.EventHandler(this.buttonChooseFile_Click);
            // 
            // buttonPreviewSound
            // 
            this.buttonPreviewSound.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonPreviewSound, "buttonPreviewSound");
            this.buttonPreviewSound.Name = "buttonPreviewSound";
            this.buttonPreviewSound.UseVisualStyleBackColor = false;
            this.buttonPreviewSound.Click += new System.EventHandler(this.buttonPreviewSound_Click);
            // 
            // comboBoxSounds
            // 
            this.comboBoxSounds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxSounds, "comboBoxSounds");
            this.comboBoxSounds.Name = "comboBoxSounds";
            // 
            // radioButtonSysSound
            // 
            this.radioButtonSysSound.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonSysSound, "radioButtonSysSound");
            this.radioButtonSysSound.Name = "radioButtonSysSound";
            this.radioButtonSysSound.UseVisualStyleBackColor = false;
            // 
            // tabPageDisplay
            // 
            this.tabPageDisplay.Controls.Add(this.radioButtonKabbalistic);
            this.tabPageDisplay.Controls.Add(this.radioButtonHebrew);
            this.tabPageDisplay.Controls.Add(this.xPPanel1);
            this.tabPageDisplay.Controls.Add(this.radioButtonMulti);
            this.tabPageDisplay.Controls.Add(this.radioButtonBlack);
            this.tabPageDisplay.Controls.Add(this.radioButton3DSilver);
            this.tabPageDisplay.Controls.Add(this.xpLabelIconSet);
            this.tabPageDisplay.Controls.Add(this.xpLabelInPHC);
            resources.ApplyResources(this.tabPageDisplay, "tabPageDisplay");
            this.tabPageDisplay.Name = "tabPageDisplay";
            // 
            // radioButtonKabbalistic
            // 
            this.radioButtonKabbalistic.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonKabbalistic, "radioButtonKabbalistic");
            this.radioButtonKabbalistic.Name = "radioButtonKabbalistic";
            this.radioButtonKabbalistic.UseVisualStyleBackColor = false;
            // 
            // radioButtonHebrew
            // 
            this.radioButtonHebrew.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonHebrew, "radioButtonHebrew");
            this.radioButtonHebrew.Name = "radioButtonHebrew";
            this.radioButtonHebrew.UseVisualStyleBackColor = false;
            // 
            // xPPanel1
            // 
            this.xPPanel1.BackColor = System.Drawing.Color.Transparent;
            this.xPPanel1.Controls.Add(this.radioButtonPHCPhase);
            this.xPPanel1.Controls.Add(this.radioButtonHouseMoment);
            this.xPPanel1.Controls.Add(this.radioButtonHourNumber);
            resources.ApplyResources(this.xPPanel1, "xPPanel1");
            this.xPPanel1.Name = "xPPanel1";
            // 
            // radioButtonPHCPhase
            // 
            this.radioButtonPHCPhase.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonPHCPhase, "radioButtonPHCPhase");
            this.radioButtonPHCPhase.Name = "radioButtonPHCPhase";
            this.radioButtonPHCPhase.UseVisualStyleBackColor = false;
            // 
            // radioButtonHouseMoment
            // 
            this.radioButtonHouseMoment.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonHouseMoment, "radioButtonHouseMoment");
            this.radioButtonHouseMoment.Name = "radioButtonHouseMoment";
            this.radioButtonHouseMoment.UseVisualStyleBackColor = false;
            // 
            // radioButtonHourNumber
            // 
            this.radioButtonHourNumber.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonHourNumber, "radioButtonHourNumber");
            this.radioButtonHourNumber.Name = "radioButtonHourNumber";
            this.radioButtonHourNumber.UseVisualStyleBackColor = false;
            // 
            // radioButtonMulti
            // 
            this.radioButtonMulti.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonMulti, "radioButtonMulti");
            this.radioButtonMulti.Name = "radioButtonMulti";
            this.radioButtonMulti.UseVisualStyleBackColor = false;
            // 
            // radioButtonBlack
            // 
            this.radioButtonBlack.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonBlack, "radioButtonBlack");
            this.radioButtonBlack.Name = "radioButtonBlack";
            this.radioButtonBlack.UseVisualStyleBackColor = false;
            // 
            // radioButton3DSilver
            // 
            this.radioButton3DSilver.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButton3DSilver, "radioButton3DSilver");
            this.radioButton3DSilver.Name = "radioButton3DSilver";
            this.radioButton3DSilver.UseVisualStyleBackColor = false;
            // 
            // xpLabelIconSet
            // 
            this.xpLabelIconSet.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.xpLabelIconSet, "xpLabelIconSet");
            this.xpLabelIconSet.Name = "xpLabelIconSet";
            // 
            // xpLabelInPHC
            // 
            this.xpLabelInPHC.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.xpLabelInPHC, "xpLabelInPHC");
            this.xpLabelInPHC.Name = "xpLabelInPHC";
            // 
            // tabPageLanguage
            // 
            this.tabPageLanguage.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageLanguage.Controls.Add(this.radioButtonHebrewEnglish);
            this.tabPageLanguage.Controls.Add(this.radioButtonDefault);
            this.tabPageLanguage.Controls.Add(this.radioButtonGreek);
            this.tabPageLanguage.Controls.Add(this.radioButtonHungarian);
            this.tabPageLanguage.Controls.Add(this.radioButtonPortuguese);
            this.tabPageLanguage.Controls.Add(this.radioButtonFrench);
            this.tabPageLanguage.Controls.Add(this.radioButtonItalian);
            this.tabPageLanguage.Controls.Add(this.radioButtonSpanish);
            this.tabPageLanguage.Controls.Add(this.label3);
            this.tabPageLanguage.Controls.Add(this.radioButtonEnglish);
            this.tabPageLanguage.Controls.Add(this.radioButtonDutch);
            resources.ApplyResources(this.tabPageLanguage, "tabPageLanguage");
            this.tabPageLanguage.Name = "tabPageLanguage";
            // 
            // radioButtonHebrewEnglish
            // 
            this.radioButtonHebrewEnglish.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonHebrewEnglish, "radioButtonHebrewEnglish");
            this.radioButtonHebrewEnglish.Name = "radioButtonHebrewEnglish";
            this.radioButtonHebrewEnglish.UseVisualStyleBackColor = false;
            // 
            // radioButtonDefault
            // 
            this.radioButtonDefault.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonDefault, "radioButtonDefault");
            this.radioButtonDefault.Name = "radioButtonDefault";
            this.radioButtonDefault.UseVisualStyleBackColor = false;
            // 
            // radioButtonGreek
            // 
            this.radioButtonGreek.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonGreek, "radioButtonGreek");
            this.radioButtonGreek.Name = "radioButtonGreek";
            this.radioButtonGreek.UseVisualStyleBackColor = false;
            // 
            // radioButtonHungarian
            // 
            this.radioButtonHungarian.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonHungarian, "radioButtonHungarian");
            this.radioButtonHungarian.Name = "radioButtonHungarian";
            this.radioButtonHungarian.UseVisualStyleBackColor = false;
            // 
            // radioButtonPortuguese
            // 
            this.radioButtonPortuguese.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonPortuguese, "radioButtonPortuguese");
            this.radioButtonPortuguese.Name = "radioButtonPortuguese";
            this.radioButtonPortuguese.UseVisualStyleBackColor = false;
            // 
            // radioButtonFrench
            // 
            this.radioButtonFrench.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonFrench, "radioButtonFrench");
            this.radioButtonFrench.Name = "radioButtonFrench";
            this.radioButtonFrench.UseVisualStyleBackColor = false;
            // 
            // radioButtonItalian
            // 
            this.radioButtonItalian.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonItalian, "radioButtonItalian");
            this.radioButtonItalian.Name = "radioButtonItalian";
            this.radioButtonItalian.UseVisualStyleBackColor = false;
            // 
            // radioButtonSpanish
            // 
            this.radioButtonSpanish.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonSpanish, "radioButtonSpanish");
            this.radioButtonSpanish.Name = "radioButtonSpanish";
            this.radioButtonSpanish.UseVisualStyleBackColor = false;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // radioButtonEnglish
            // 
            this.radioButtonEnglish.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonEnglish, "radioButtonEnglish");
            this.radioButtonEnglish.Name = "radioButtonEnglish";
            this.radioButtonEnglish.UseVisualStyleBackColor = false;
            // 
            // radioButtonDutch
            // 
            this.radioButtonDutch.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonDutch, "radioButtonDutch");
            this.radioButtonDutch.Name = "radioButtonDutch";
            this.radioButtonDutch.UseVisualStyleBackColor = false;
            // 
            // tabPagePrecision
            // 
            this.tabPagePrecision.BackColor = System.Drawing.SystemColors.Control;
            this.tabPagePrecision.Controls.Add(this.label11);
            this.tabPagePrecision.Controls.Add(this.panel4);
            this.tabPagePrecision.Controls.Add(this.radioButtonSecond);
            this.tabPagePrecision.Controls.Add(this.label4);
            this.tabPagePrecision.Controls.Add(this.radioButtonMinute);
            resources.ApplyResources(this.tabPagePrecision, "tabPagePrecision");
            this.tabPagePrecision.Name = "tabPagePrecision";
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.Controls.Add(this.labelExampleLong);
            this.panel4.Controls.Add(this.labelExampleShort);
            this.panel4.Controls.Add(this.radioButtonLongTime);
            this.panel4.Controls.Add(this.radioButtonShortTime);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // labelExampleLong
            // 
            this.labelExampleLong.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelExampleLong, "labelExampleLong");
            this.labelExampleLong.Name = "labelExampleLong";
            // 
            // labelExampleShort
            // 
            this.labelExampleShort.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelExampleShort, "labelExampleShort");
            this.labelExampleShort.Name = "labelExampleShort";
            // 
            // radioButtonLongTime
            // 
            this.radioButtonLongTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonLongTime, "radioButtonLongTime");
            this.radioButtonLongTime.Name = "radioButtonLongTime";
            this.radioButtonLongTime.UseVisualStyleBackColor = false;
            // 
            // radioButtonShortTime
            // 
            this.radioButtonShortTime.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonShortTime, "radioButtonShortTime");
            this.radioButtonShortTime.Name = "radioButtonShortTime";
            this.radioButtonShortTime.UseVisualStyleBackColor = false;
            // 
            // radioButtonSecond
            // 
            this.radioButtonSecond.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonSecond, "radioButtonSecond");
            this.radioButtonSecond.Name = "radioButtonSecond";
            this.radioButtonSecond.UseVisualStyleBackColor = false;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // radioButtonMinute
            // 
            this.radioButtonMinute.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.radioButtonMinute, "radioButtonMinute");
            this.radioButtonMinute.Name = "radioButtonMinute";
            this.radioButtonMinute.UseVisualStyleBackColor = false;
            // 
            // tabPageStartup
            // 
            this.tabPageStartup.BackColor = System.Drawing.SystemColors.Control;
            this.tabPageStartup.Controls.Add(this.checkBoxTray);
            this.tabPageStartup.Controls.Add(this.checkBoxUpdate);
            this.tabPageStartup.Controls.Add(this.checkBoxStartup);
            this.tabPageStartup.Controls.Add(this.label10);
            resources.ApplyResources(this.tabPageStartup, "tabPageStartup");
            this.tabPageStartup.Name = "tabPageStartup";
            // 
            // checkBoxTray
            // 
            this.checkBoxTray.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxTray, "checkBoxTray");
            this.checkBoxTray.Name = "checkBoxTray";
            this.checkBoxTray.UseVisualStyleBackColor = false;
            this.checkBoxTray.CheckedChanged += new System.EventHandler(this.CheckBoxTrayCheckedChanged);
            // 
            // checkBoxUpdate
            // 
            this.checkBoxUpdate.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxUpdate, "checkBoxUpdate");
            this.checkBoxUpdate.Name = "checkBoxUpdate";
            this.checkBoxUpdate.UseVisualStyleBackColor = false;
            // 
            // checkBoxStartup
            // 
            this.checkBoxStartup.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.checkBoxStartup, "checkBoxStartup");
            this.checkBoxStartup.Name = "checkBoxStartup";
            this.checkBoxStartup.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // PropertiesBox
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonApply);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "PropertiesBox";
            this.tabControl1.ResumeLayout(false);
            this.tabPageLocation.ResumeLayout(false);
            this.xpGroupBoxTimeZone.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.xpGroupBoxLongitude.ResumeLayout(false);
            this.xpGroupBoxLongitude.PerformLayout();
            this.tabPageNotify.ResumeLayout(false);
            this.xpGroupBoxFont.ResumeLayout(false);
            this.xpGroupBoxColor.ResumeLayout(false);
            this.tabPageSounds.ResumeLayout(false);
            this.tabPageSounds.PerformLayout();
            this.tabPageDisplay.ResumeLayout(false);
            this.xPPanel1.ResumeLayout(false);
            this.tabPageLanguage.ResumeLayout(false);
            this.tabPagePrecision.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.tabPageStartup.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		#region updateRegistry
		private int updateRegistry (bool apply)
		{
			if (radioButtonWaveFile.Checked && textBoxWaveFile.Text.Equals ("") && checkBoxSound.Checked)
			{
				MessageBox.Show ("You've chosen to play a wave file when the planetary hour changes, but you didn't select a file.  Please choose a file.",
					"ChronosXP", MessageBoxButtons.OK, MessageBoxIcon.Information);
				this.buttonChooseFile_Click (null, null);
				return -1;
			}

			conf.SetPlanetColor (PlanetaryHours.Planet.Sun, Color.FromName (comboBoxSun.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Moon, Color.FromName (comboBoxMoon.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Mercury, Color.FromName (comboBoxMercury.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Venus, Color.FromName (comboBoxVenus.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Mars, Color.FromName (comboBoxMars.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Jupiter, Color.FromName (comboBoxJupiter.Text));
			conf.SetPlanetColor (PlanetaryHours.Planet.Saturn, Color.FromName (comboBoxSaturn.Text));

			if (!conf.NotifyFont.Name.Equals (fontDialog1.Font.Name) || conf.NotifyFont.Style != fontDialog1.Font.Style ||
					conf.NotifyFont.Size != fontDialog1.Font.Size)
				conf.NotifyFont = (Font) fontDialog1.Font.Clone();

            if (radioButtonDefault.Checked)
                conf.Language = "*";
            else if (radioButtonDutch.Checked)
                conf.Language = Config.CultureNL;
            else if (radioButtonSpanish.Checked)
                conf.Language = Config.CultureES;
            else if (radioButtonItalian.Checked)
                conf.Language = Config.CultureIT;
            else if (radioButtonFrench.Checked)
                conf.Language = Config.CultureFR;
            else if (radioButtonPortuguese.Checked)
                conf.Language = Config.CulturePT;
            else if (radioButtonHungarian.Checked)
                conf.Language = Config.CultureHU;
            else if (radioButtonGreek.Checked)
                conf.Language = Config.CultureGR;
            else if (radioButtonHebrewEnglish.Checked)
                conf.Language = Config.CultureHE;
            else
                conf.Language = Config.CultureEN;

			conf.NotifyHour = checkBoxNotify.Checked;
			if (radioButtonHourNumber.Checked)
				conf.Caption = Config.CaptionType.HourNumber;
			else if (radioButtonHouseMoment.Checked)
				conf.Caption = Config.CaptionType.HouseOfMoment;
			else if (radioButtonPHCPhase.Checked)
				conf.Caption = Config.CaptionType.LunarPhase;

            if (radioButton3DSilver.Checked)
                conf.IconSet = "Silver";
            else if (radioButtonBlack.Checked)
                conf.IconSet = "Black";
            else if (radioButtonHebrew.Checked)
                conf.IconSet = "Hebrew";
            else if (radioButtonKabbalistic.Checked)
                conf.IconSet = "Kabbalistic";
            else
                conf.IconSet = "Multi";

			conf.DefaultPlace = new Place (comboBoxCity.Text, numericLongDeg.Value, numericLongMin.Value, radioButtonWest.Checked,
				numericLatDeg.Value, numericLatMin.Value, radioButtonNorth.Checked);
			conf.DefaultPlace.SetDefault();

			if (!this.checkBoxSound.Checked)
			{
				if (radioButtonSysSound.Checked)
					conf.Sound = Config.SoundTypes.SystemSelect;
				else if (radioButtonWaveFile.Checked)
					conf.Sound = Config.SoundTypes.WaveSelect;
			}
			else
			{
				if (radioButtonSysSound.Checked)
					conf.Sound = Config.SoundTypes.System;
				else if (radioButtonWaveFile.Checked)
					conf.Sound = Config.SoundTypes.Wave;
			}
			if (!textBoxWaveFile.Text.Equals (""))
				conf.SoundFile = textBoxWaveFile.Text;
			conf.SoundName = comboBoxSounds.Text;

			conf.Startup = checkBoxStartup.Checked;
			conf.CheckUpgrade = checkBoxUpdate.Checked;

			//conf.Diary = radioButtonPrintDiary.Checked;
			//conf.PrintGlyphs = xpCheckBoxPrintGlyph.Checked;

			if (checkBoxUpdate.Checked)
				conf.Core.CheckUpdates();
			else
				conf.Core.CancelCheckUpdates();

			//conf.UseGradient = checkBoxGradient.Checked;
			if (radioButtonLongTime.Checked)
				conf.ShowSeconds = true;
			else
				conf.ShowSeconds = false;

			if (radioButtonSecond.Checked)
				conf.UseShortInterval = true;
			else
				conf.UseShortInterval = false;

			bool traychanged = false;
			if (conf.RunFromTray != checkBoxTray.Checked)
			{
				traychanged = true;
				conf.RunFromTray = checkBoxTray.Checked;
				DialogResult res = MessageBox.Show (conf.Res.GetString ( conf.RunFromTray ? "Properties.Tray" : "Properties.Standalone" ),
					"ChronosXP", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
				if (res == DialogResult.Yes)
				{
					System.Diagnostics.Process.Start (Application.ExecutablePath);
                    conf.Core.Shutdown();
					return 1;
				}
			}

            if (conf.Core.FormCalendarOpen)
                conf.Core.FormCalendar.RefreshForm();

			if (conf.RunFromTrayNow)
				conf.Core.RefreshHours();
			
			if (!conf.UseZoneInfo)
				return 1;

			// When the user changes the timezone, the program must be restarted for accuracy.
			try
			{												// BUG HERE
				if (conf.ChangeTimeZone (conf.ZoneDisplayToName (comboBoxZones.Text), checkBoxDST.Checked))
				{
					if (apply)		// User pressed Apply (as opposed to OK)?  Open the Properties form after restarting.
						System.Diagnostics.Process.Start (Application.ExecutablePath, "/properties");
					else
						System.Diagnostics.Process.Start (Application.ExecutablePath);
                    conf.Core.Shutdown();
					return 1;
				}
//				else
//				{
//					if (conf.RunFromTrayNow && !traychanged)
//						conf.Core.RefreshHours();
//				}
			}
			catch
			{
				if (conf.RunFromTrayNow && !traychanged)
					conf.Core.RefreshHours();
			}

			return 1;
		}
		#endregion

		#region Button Click EventHandler's
		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if (updateRegistry (false) != -1)
				this.Close();
		}

		private void buttonApply_Click(object sender, System.EventArgs e)
		{
			updateRegistry (true);
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonFonts_Click(object sender, System.EventArgs e)
		{
			DialogResult res = fontDialog1.ShowDialog();
			if (res == DialogResult.OK)
			{
				xpLabelFont.Font = fontDialog1.Font;
				xpLabelFont.Text = String.Format ("{0}{1}, {2} pt.", fontDialog1.Font.Name,
					fontDialog1.Font.Style == FontStyle.Regular ? "" : " " + fontDialog1.Font.Style.ToString(),
					Math.Round (fontDialog1.Font.SizeInPoints));
			}
		}

		private void buttonChooseFile_Click(object sender, System.EventArgs e)
		{
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				Regex re = new Regex (@"\.wav$", RegexOptions.IgnoreCase);
				Match m = re.Match (this.openFileDialog1.FileName);
				if (!m.Success)
				{
					MessageBox.Show (conf.Res.GetString ("Properties.OnlyWaveText"), conf.Res.GetString ("Properties.OnlyWaveTitle"),
						MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return;
				}
				this.textBoxWaveFile.Text = this.openFileDialog1.FileName;
				this.radioButtonWaveFile.Checked = true;
			}
		}

		private void buttonPreviewSound_Click(object sender, System.EventArgs e)
		{
			for (int i = 0; i < conf.EventCount; i++)
			{
				if (this.comboBoxSounds.Text.Equals (conf.EventList[i].Name))
				{
					conf.Core.PlaySoundAlias (conf.EventList[i].Label);
					break;
				}
			}
		}
		#endregion

		#region LaunchBrowser LinkLabelLinkClickedEventHandler's
		private void linkLabelAtlas_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelAtlas.LinkVisited = true;
			conf.LaunchBrowser (Config.URLAtlas);
		}

        //private void linkLabelLanguage_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
        //{
        //    linkLabelLanguage.LinkVisited = true;
        //    conf.LaunchBrowser (Config.URLTranslate);
        //}
		#endregion

		#region Controls changed EventHandler's
		private void CheckBoxTrayCheckedChanged(object sender, System.EventArgs e)
		{
			if (checkBoxTray.Checked)
				checkBoxStartup.Enabled = true;
			else
			{
				checkBoxStartup.Checked = false;
				checkBoxStartup.Enabled = false;
			}
		}

		private void comboBoxCity_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			string plc = comboBoxCity.Text;
			if (conf.GetPlaceNum (plc) != -1)
			{
				Place where = conf.GetPlace (plc);
				numericLongDeg.Value = where.LongitudeDegrees;
				numericLongMin.Value = where.LongitudeMinutes;
				numericLatDeg.Value = where.LatitudeDegrees;
				numericLatMin.Value = where.LatitudeMinutes;
				if (where.LongitudeQuadrant == 'W')
					radioButtonWest.Checked = true;
				else
					radioButtonEast.Checked = true;
				if (where.LatitudeQuadrant == 'N')
					radioButtonNorth.Checked = true;
				else
					radioButtonSouth.Checked = true;
			}
		}

		private void checkBoxNotify_CheckedChanged(object sender, System.EventArgs e)
		{
			if (checkBoxNotify.Checked)
			{
				xpGroupBoxColor.Enabled = true;
				xpGroupBoxFont.Enabled = true;
				buttonFonts.Enabled = true;
				xpLabelColor.Enabled = true;
			}
			else
			{
				xpGroupBoxColor.Enabled = false;
				xpGroupBoxFont.Enabled = false;
				buttonFonts.Enabled = false;
				xpLabelColor.Enabled = false;
			}
		}

		private void checkBoxSound_CheckedChanged(object sender, System.EventArgs e)
		{
			if (checkBoxSound.Checked)
			{
				radioButtonSysSound.Enabled = true;
				radioButtonWaveFile.Enabled = true;
				comboBoxSounds.Enabled = true;
				textBoxWaveFile.Enabled = true;
				buttonPreviewSound.Enabled = true;
				buttonChooseFile.Enabled = true;
			}
			else
			{
				radioButtonSysSound.Enabled = false;
				radioButtonWaveFile.Enabled = false;
				comboBoxSounds.Enabled = false;
				textBoxWaveFile.Enabled = false;
				buttonPreviewSound.Enabled = false;
				buttonChooseFile.Enabled = false;
			}
		}
		#endregion

		#region Notifier Preview LinkLabelLinkClickedEventHandler's
		private void linkLabelSun_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			string str = conf.GetString("HourException.Sun");
			if (str == null)
				str = String.Format(conf.Res.GetString("HourOf"), conf.Core.pHours.FormatPlanet(conf.Res.GetString("Planet.Sun")));
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				str, conf, "Sun",
				Color.FromName (comboBoxSun.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelMoon_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			string str = conf.GetString("HourException.Moon");
			if (str == null)
				str = String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Moon")));
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				str, conf, "Moon",
				Color.FromName (comboBoxMoon.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelMercury_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Mercury"))), conf, "Mercury",
				Color.FromName (comboBoxMercury.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelVenus_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Venus"))), conf, "Venus",
				Color.FromName (comboBoxVenus.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelMars_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Mars"))), conf, "Mars",
				Color.FromName (comboBoxMars.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelJupiter_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Jupiter"))), conf, "Jupiter",
				Color.FromName (comboBoxJupiter.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}

		private void linkLabelSaturn_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
		#if WINDOWS
			NotifyWindow nw = new NotifyWindow (conf.Res.GetString ("Calendar.Preview"),
				String.Format (conf.Res.GetString ("HourOf"), conf.Core.pHours.FormatPlanet (conf.Res.GetString ("Planet.Saturn"))), conf, "Saturn",
				Color.FromName (comboBoxSaturn.Text), fontDialog1.Font, false);
			nw.IsPreview = true;
			nw.Start();
		#endif
		}
		#endregion
				
	}
}
