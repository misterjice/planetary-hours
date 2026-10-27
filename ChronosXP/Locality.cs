#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - Locality.cs
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// A dialog box where the user can choose the locality used in the Planetary Hours Calendar.  These changes are non-permanent;
	/// they have no effect on the NotifyIcon glyph.  Those (the default place) changes are made in Properties.cs.
	/// 
	/// comboBoxZones.Items contains a list of time zones for use in them.  The time zones have a standard format:
	/// 
	/// +/-nn:nn (full name of time zone) - (timezone abbreviation)
	/// 
	/// They are parsed using regular expressions (who ever would've guessed MS would bring RegEx to Windows?) in Place.cs.
	/// Ideally, I'd like to eliminate this system and make use of the Windows system time zones, in use in Config.cs.
	/// </summary>
	public sealed class Locality : System.Windows.Forms.Form
	{
		private ChronosXP.XPLabel labelMin1;
		private ChronosXP.XPLabel labelDeg2;
		private ChronosXP.XPLabel labelMin2;
		private ChronosXP.XPLinkLabel linkLabelAtlas;
		private System.Windows.Forms.ComboBox comboBoxZones;
		private ChronosXP.XPGroupBox groupBoxLongitude;
		private System.Windows.Forms.RadioButton radioButtonWest;
		private ChronosXP.XPGroupBox groupBoxLatitude;
		private ChronosXP.XPNumericUpDown numericLatDeg;
		private System.Windows.Forms.ComboBox comboBoxCity;
		private ChronosXP.XPLabel labelInfo;
		private ChronosXP.XPNumericUpDown numericLatMin;
		private System.Windows.Forms.RadioButton radioButtonEast;
		private ChronosXP.XPNumericUpDown numericLongMin;
		private System.Windows.Forms.Button buttonOK;
		private ChronosXP.XPGroupBox groupBoxTZ;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.RadioButton radioButtonSouth;
		private ChronosXP.XPNumericUpDown numericLongDeg;
		private System.Windows.Forms.RadioButton radioButtonNorth;
		private ChronosXP.XPGroupBox groupBoxCity;
		private ChronosXP.XPLabel labelDeg1;
		public const string Copyright = "Locality.cs, Copyright  2004 by Robert Misiak";

		#region Form Designer Variables
		private System.ComponentModel.Container components = null;
		#endregion

		private Config conf;
		private Calendar formCalendar;

		public Locality (Config conf, Calendar cal)
		{
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.DoubleBuffer, true);

			InitializeComponent();

			comboBoxZones.Items.Add("Select time zone...");
			comboBoxZones.Items.AddRange(Config.AllZones);
			this.conf = conf;
			formCalendar = cal;
			formCalendar.FormLocalityOpen = true;
			Closing += new System.ComponentModel.CancelEventHandler (this.formClosing);
			for (int i = 0; i < conf.PlaceNum; i++)
				comboBoxCity.Items.Add (conf.Places[i].Name);
			comboBoxCity.Text = conf.DefaultLocality.Name;

			if (!PInvoke.VisualStylesEnabled())
				Config.FixRadioButtons (Controls, FlatStyle.Standard);
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

		private void formClosing (object sender, CancelEventArgs e)
		{
			formCalendar.FormLocalityOpen = false;
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Locality));
            this.labelDeg1 = new ChronosXP.XPLabel();
            this.groupBoxCity = new ChronosXP.XPGroupBox();
            this.comboBoxCity = new System.Windows.Forms.ComboBox();
            this.radioButtonNorth = new System.Windows.Forms.RadioButton();
            this.numericLongDeg = new ChronosXP.XPNumericUpDown();
            this.radioButtonSouth = new System.Windows.Forms.RadioButton();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxTZ = new ChronosXP.XPGroupBox();
            this.comboBoxZones = new System.Windows.Forms.ComboBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.numericLongMin = new ChronosXP.XPNumericUpDown();
            this.radioButtonEast = new System.Windows.Forms.RadioButton();
            this.numericLatMin = new ChronosXP.XPNumericUpDown();
            this.labelInfo = new ChronosXP.XPLabel();
            this.numericLatDeg = new ChronosXP.XPNumericUpDown();
            this.groupBoxLatitude = new ChronosXP.XPGroupBox();
            this.labelMin1 = new ChronosXP.XPLabel();
            this.radioButtonWest = new System.Windows.Forms.RadioButton();
            this.groupBoxLongitude = new ChronosXP.XPGroupBox();
            this.labelMin2 = new ChronosXP.XPLabel();
            this.labelDeg2 = new ChronosXP.XPLabel();
            this.linkLabelAtlas = new ChronosXP.XPLinkLabel();
            this.groupBoxCity.SuspendLayout();
            this.groupBoxTZ.SuspendLayout();
            this.groupBoxLatitude.SuspendLayout();
            this.groupBoxLongitude.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelDeg1
            // 
            this.labelDeg1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelDeg1, "labelDeg1");
            this.labelDeg1.Name = "labelDeg1";
            // 
            // groupBoxCity
            // 
            this.groupBoxCity.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxCity.Controls.Add(this.comboBoxCity);
            resources.ApplyResources(this.groupBoxCity, "groupBoxCity");
            this.groupBoxCity.Name = "groupBoxCity";
            this.groupBoxCity.TabStop = false;
            // 
            // comboBoxCity
            // 
            resources.ApplyResources(this.comboBoxCity, "comboBoxCity");
            this.comboBoxCity.Name = "comboBoxCity";
            this.comboBoxCity.Sorted = true;
            this.comboBoxCity.SelectedIndexChanged += new System.EventHandler(this.comboBoxCity_SelectedIndexChanged);
            // 
            // radioButtonNorth
            // 
            resources.ApplyResources(this.radioButtonNorth, "radioButtonNorth");
            this.radioButtonNorth.Name = "radioButtonNorth";
            this.radioButtonNorth.TabStop = true;
            // 
            // numericLongDeg
            // 
            resources.ApplyResources(this.numericLongDeg, "numericLongDeg");
            this.numericLongDeg.Maximum = 179;
            this.numericLongDeg.Minimum = 0;
            this.numericLongDeg.Name = "numericLongDeg";
            this.numericLongDeg.Value = 0;
            // 
            // radioButtonSouth
            // 
            resources.ApplyResources(this.radioButtonSouth, "radioButtonSouth");
            this.radioButtonSouth.Name = "radioButtonSouth";
            this.radioButtonSouth.TabStop = true;
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
            // groupBoxTZ
            // 
            this.groupBoxTZ.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxTZ.Controls.Add(this.comboBoxZones);
            resources.ApplyResources(this.groupBoxTZ, "groupBoxTZ");
            this.groupBoxTZ.Name = "groupBoxTZ";
            this.groupBoxTZ.TabStop = false;
            // 
            // comboBoxZones
            // 
            this.comboBoxZones.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.comboBoxZones, "comboBoxZones");
            this.comboBoxZones.Name = "comboBoxZones";
            // 
            // buttonOK
            // 
            this.buttonOK.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // numericLongMin
            // 
            resources.ApplyResources(this.numericLongMin, "numericLongMin");
            this.numericLongMin.Maximum = 59;
            this.numericLongMin.Minimum = 0;
            this.numericLongMin.Name = "numericLongMin";
            this.numericLongMin.Value = 0;
            // 
            // radioButtonEast
            // 
            resources.ApplyResources(this.radioButtonEast, "radioButtonEast");
            this.radioButtonEast.Name = "radioButtonEast";
            this.radioButtonEast.TabStop = true;
            // 
            // numericLatMin
            // 
            resources.ApplyResources(this.numericLatMin, "numericLatMin");
            this.numericLatMin.Maximum = 59;
            this.numericLatMin.Minimum = 0;
            this.numericLatMin.Name = "numericLatMin";
            this.numericLatMin.Value = 0;
            // 
            // labelInfo
            // 
            this.labelInfo.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelInfo, "labelInfo");
            this.labelInfo.Name = "labelInfo";
            // 
            // numericLatDeg
            // 
            resources.ApplyResources(this.numericLatDeg, "numericLatDeg");
            this.numericLatDeg.Maximum = 89;
            this.numericLatDeg.Minimum = 0;
            this.numericLatDeg.Name = "numericLatDeg";
            this.numericLatDeg.Value = 0;
            // 
            // groupBoxLatitude
            // 
            this.groupBoxLatitude.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxLatitude.Controls.Add(this.numericLatDeg);
            this.groupBoxLatitude.Controls.Add(this.labelMin1);
            this.groupBoxLatitude.Controls.Add(this.labelDeg1);
            this.groupBoxLatitude.Controls.Add(this.numericLatMin);
            this.groupBoxLatitude.Controls.Add(this.radioButtonNorth);
            this.groupBoxLatitude.Controls.Add(this.radioButtonSouth);
            resources.ApplyResources(this.groupBoxLatitude, "groupBoxLatitude");
            this.groupBoxLatitude.Name = "groupBoxLatitude";
            this.groupBoxLatitude.TabStop = false;
            // 
            // labelMin1
            // 
            this.labelMin1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelMin1, "labelMin1");
            this.labelMin1.Name = "labelMin1";
            // 
            // radioButtonWest
            // 
            resources.ApplyResources(this.radioButtonWest, "radioButtonWest");
            this.radioButtonWest.Name = "radioButtonWest";
            this.radioButtonWest.TabStop = true;
            // 
            // groupBoxLongitude
            // 
            this.groupBoxLongitude.BackColor = System.Drawing.Color.Transparent;
            this.groupBoxLongitude.Controls.Add(this.labelMin2);
            this.groupBoxLongitude.Controls.Add(this.numericLongMin);
            this.groupBoxLongitude.Controls.Add(this.labelDeg2);
            this.groupBoxLongitude.Controls.Add(this.numericLongDeg);
            this.groupBoxLongitude.Controls.Add(this.radioButtonWest);
            this.groupBoxLongitude.Controls.Add(this.radioButtonEast);
            resources.ApplyResources(this.groupBoxLongitude, "groupBoxLongitude");
            this.groupBoxLongitude.Name = "groupBoxLongitude";
            this.groupBoxLongitude.TabStop = false;
            // 
            // labelMin2
            // 
            this.labelMin2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelMin2, "labelMin2");
            this.labelMin2.Name = "labelMin2";
            // 
            // labelDeg2
            // 
            this.labelDeg2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.labelDeg2, "labelDeg2");
            this.labelDeg2.Name = "labelDeg2";
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
            // Locality
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxTZ);
            this.Controls.Add(this.groupBoxLongitude);
            this.Controls.Add(this.groupBoxLatitude);
            this.Controls.Add(this.groupBoxCity);
            this.Controls.Add(this.labelInfo);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.linkLabelAtlas);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "Locality";
            this.groupBoxCity.ResumeLayout(false);
            this.groupBoxTZ.ResumeLayout(false);
            this.groupBoxLatitude.ResumeLayout(false);
            this.groupBoxLatitude.PerformLayout();
            this.groupBoxLongitude.ResumeLayout(false);
            this.groupBoxLongitude.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			if (this.comboBoxZones.Text.Equals ("Select time zone...") && !this.comboBoxCity.Text.Equals (conf.DefaultPlace.Name))
			{
				MessageBox.Show (conf.Res.GetString ("Locality.SelectZone"), "ChronosXP", MessageBoxButtons.OK,
					MessageBoxIcon.Question);
				return;
			}

			Place newPlace;
			if (this.comboBoxZones.Text.Equals ("Select time zone..."))
				newPlace = conf.DefaultPlace;
			else
				newPlace = new Place (this.comboBoxCity.Text, this.numericLongDeg.Value, this.numericLongMin.Value,
					this.radioButtonWest.Checked, this.numericLatDeg.Value, this.numericLatMin.Value, this.radioButtonNorth.Checked,
					this.comboBoxZones.Text);
			conf.AddPlace(newPlace);
			formCalendar.SetLocality (newPlace);

			conf.DefaultLocality = newPlace;
			this.Close();
		}

		private void linkLabelAtlas_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			this.linkLabelAtlas.LinkVisited = true;
			conf.LaunchBrowser (Config.URLAtlas);
		}

		private void comboBoxCity_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (conf.GetPlaceNum (comboBoxCity.Text) != -1)
			{
				Place p = conf.GetPlace (comboBoxCity.Text);
				numericLongMin.Value = p.LongitudeMinutes;
				numericLongDeg.Value = p.LongitudeDegrees;
				numericLatMin.Value = p.LatitudeMinutes;
				numericLatDeg.Value = p.LatitudeDegrees;
				if (p.LongitudeQuadrant == 'W')
					radioButtonWest.Checked = true;
				else
					radioButtonEast.Checked = true;
				if (p.LatitudeQuadrant == 'N')
					radioButtonNorth.Checked = true;
				else
					radioButtonSouth.Checked = true;
				if (p.UseSystemTime || p.DefaultPlace)
					comboBoxZones.Text = "Select time zone...";
				else
				{
					bool matched = false;
					foreach (string zn in comboBoxZones.Items)
					{
						if (zn.Equals (p.Zone))
						{
							matched = true;
							break;
						}
					}
					if (!matched)
						comboBoxZones.Items.Add (p.Zone);
					comboBoxZones.Text = p.Zone;
				}
			}
		}
	}
}
