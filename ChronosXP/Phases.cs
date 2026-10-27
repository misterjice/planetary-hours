#region Copyright  2005 by Robert Misiak
// ChronosXP - Phases.cs
// Copyright  2005 by Robert Misiak <rmisiak@users.sourceforge.net>
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

namespace ChronosXP
{
	/// <summary>
	/// Description of Phases.
	/// </summary>
	public class Phases : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListView listView;
		private System.Windows.Forms.DateTimePicker dateTimePicker;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.Button buttonOK;
		private ChronosXP.XPLabel labelDate;
		public const string Copyright = "Phases.cs, Copyright  2005 by Robert Misiak";
		private Config conf;

		public Phases(DateTime seed, Config cnf)
		{
			SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();

			conf = cnf;
			conf.Core.FormPhasesOpen = true;
			dateTimePicker.CustomFormat = Application.CurrentCulture.DateTimeFormat.MonthDayPattern + " yyyy";
			dateTimePicker.Value = seed;
			refreshList();
		}
		
		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			base.OnClosing(e);
			conf.Core.FormPhasesOpen = false;
		}
		
		protected override void OnPaintBackground(PaintEventArgs e)
		{
			using (LinearGradientBrush lgb = new LinearGradientBrush(DisplayRectangle, SystemColors.ControlLightLight,
			                                                         SystemColors.Control, LinearGradientMode.Vertical))
				e.Graphics.FillRectangle(lgb, DisplayRectangle);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Phases));
			this.labelDate = new ChronosXP.XPLabel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
			this.listView = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// labelDate
			// 
			this.labelDate.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelDate.Anchor")));
			this.labelDate.AutoSize = ((bool)(resources.GetObject("labelDate.AutoSize")));
			this.labelDate.BackColor = System.Drawing.Color.Transparent;
			this.labelDate.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelDate.Dock")));
			this.labelDate.Enabled = ((bool)(resources.GetObject("labelDate.Enabled")));
			this.labelDate.Font = ((System.Drawing.Font)(resources.GetObject("labelDate.Font")));
			this.labelDate.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelDate.ImageAlign")));
			this.labelDate.ImageIndex = ((int)(resources.GetObject("labelDate.ImageIndex")));
			this.labelDate.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelDate.ImeMode")));
			this.labelDate.Location = ((System.Drawing.Point)(resources.GetObject("labelDate.Location")));
			this.labelDate.Name = "labelDate";
			this.labelDate.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelDate.RightToLeft")));
			this.labelDate.Size = ((System.Drawing.Size)(resources.GetObject("labelDate.Size")));
			this.labelDate.TabIndex = ((int)(resources.GetObject("labelDate.TabIndex")));
			this.labelDate.Text = resources.GetString("labelDate.Text");
			this.labelDate.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelDate.TextAlign")));
			this.labelDate.Visible = ((bool)(resources.GetObject("labelDate.Visible")));
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonOK.Anchor")));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOK.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonOK.Dock")));
			this.buttonOK.Enabled = ((bool)(resources.GetObject("buttonOK.Enabled")));
			this.buttonOK.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonOK.FlatStyle")));
			this.buttonOK.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonOK.ImageAlign")));
			this.buttonOK.ImageIndex = ((int)(resources.GetObject("buttonOK.ImageIndex")));
			this.buttonOK.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonOK.ImeMode")));
			this.buttonOK.Location = ((System.Drawing.Point)(resources.GetObject("buttonOK.Location")));
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonOK.RightToLeft")));
			this.buttonOK.Size = ((System.Drawing.Size)(resources.GetObject("buttonOK.Size")));
			this.buttonOK.TabIndex = ((int)(resources.GetObject("buttonOK.TabIndex")));
			this.buttonOK.Text = resources.GetString("buttonOK.Text");
			this.buttonOK.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonOK.TextAlign")));
			this.buttonOK.Visible = ((bool)(resources.GetObject("buttonOK.Visible")));
			this.buttonOK.Click += new System.EventHandler(this.ButtonOKClick);
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = resources.GetString("columnHeader2.Text");
			this.columnHeader2.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader2.TextAlign")));
			this.columnHeader2.Width = ((int)(resources.GetObject("columnHeader2.Width")));
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = resources.GetString("columnHeader1.Text");
			this.columnHeader1.TextAlign = ((System.Windows.Forms.HorizontalAlignment)(resources.GetObject("columnHeader1.TextAlign")));
			this.columnHeader1.Width = ((int)(resources.GetObject("columnHeader1.Width")));
			// 
			// dateTimePicker
			// 
			this.dateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("dateTimePicker.Anchor")));
			this.dateTimePicker.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("dateTimePicker.Dock")));
			this.dateTimePicker.DropDownAlign = ((System.Windows.Forms.LeftRightAlignment)(resources.GetObject("dateTimePicker.DropDownAlign")));
			this.dateTimePicker.Enabled = ((bool)(resources.GetObject("dateTimePicker.Enabled")));
			this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.dateTimePicker.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("dateTimePicker.ImeMode")));
			this.dateTimePicker.Location = ((System.Drawing.Point)(resources.GetObject("dateTimePicker.Location")));
			this.dateTimePicker.Name = "dateTimePicker";
			this.dateTimePicker.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("dateTimePicker.RightToLeft")));
			this.dateTimePicker.ShowUpDown = true;
			this.dateTimePicker.Size = ((System.Drawing.Size)(resources.GetObject("dateTimePicker.Size")));
			this.dateTimePicker.TabIndex = ((int)(resources.GetObject("dateTimePicker.TabIndex")));
			this.dateTimePicker.Visible = ((bool)(resources.GetObject("dateTimePicker.Visible")));
			this.dateTimePicker.ValueChanged += new System.EventHandler(this.DateTimePickerValueChanged);
			// 
			// listView
			// 
			this.listView.Alignment = ((System.Windows.Forms.ListViewAlignment)(resources.GetObject("listView.Alignment")));
			this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("listView.Anchor")));
			this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
						this.columnHeader1,
						this.columnHeader2});
			this.listView.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("listView.Dock")));
			this.listView.Enabled = ((bool)(resources.GetObject("listView.Enabled")));
			this.listView.Font = ((System.Drawing.Font)(resources.GetObject("listView.Font")));
			this.listView.FullRowSelect = true;
			this.listView.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("listView.ImeMode")));
			this.listView.LabelWrap = ((bool)(resources.GetObject("listView.LabelWrap")));
			this.listView.Location = ((System.Drawing.Point)(resources.GetObject("listView.Location")));
			this.listView.MultiSelect = false;
			this.listView.Name = "listView";
			this.listView.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("listView.RightToLeft")));
			this.listView.Size = ((System.Drawing.Size)(resources.GetObject("listView.Size")));
			this.listView.TabIndex = ((int)(resources.GetObject("listView.TabIndex")));
			this.listView.TabStop = false;
			this.listView.Text = resources.GetString("listView.Text");
			this.listView.View = System.Windows.Forms.View.Details;
			this.listView.Visible = ((bool)(resources.GetObject("listView.Visible")));
			// 
			// Phases
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.CancelButton = this.buttonOK;
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.dateTimePicker);
			this.Controls.Add(this.listView);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "Phases";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.ResumeLayout(false);
		}
		#endregion
		void ButtonOKClick(object sender, System.EventArgs e)
		{
			Close();
		}
		
		void DateTimePickerValueChanged(object sender, System.EventArgs e)
		{
			refreshList();
		}
		
		private void refreshList()
		{
			DateTime searchTime = new DateTime(dateTimePicker.Value.Year, dateTimePicker.Value.Month, dateTimePicker.Value.Day,
			                                   23, 59, 59).ToUniversalTime();
			LunarPhase lp = new LunarPhase(searchTime); //dateTimePicker.Value);
			listView.Items.Clear();
			int cp = lp.PhaseNum(searchTime); //dateTimePicker.Value);
			while (cp == -1)
			{
				lp = new LunarPhase(searchTime.AddDays(-1));//.AddDays(28));//dateTimePicker.Value.AddDays(28));
				cp = lp.PhaseNum(searchTime); //dateTimePicker.Value);
			//	if (cp == -1)
			//		cp = 0;
			}
			for (int i = 0; i < 8; i++)
			{
				string[] s = { conf.GetString(LunarPhase.PhaseName[i]), lp.PhaseTimes[i].ToLocalTime().ToString("ddd") + " " + lp.PhaseTimes[i].ToLocalTime().ToShortDateString() + " " + conf.FormatTime(lp.PhaseTimes[i].ToLocalTime()) };
				ListViewItem lsi = new ListViewItem(s);
				if (cp == i)
					lsi.Font = new Font(listView.Font, FontStyle.Bold);
				listView.Items.Add(lsi);
			}
		}		
	}
}
