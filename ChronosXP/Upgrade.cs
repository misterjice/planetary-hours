#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - Upgrade.cs
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace ChronosXP
{
	/// <summary>
	/// Summary description for Upgrade.
	/// </summary>
	public sealed class Upgrade : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonNo;
		private System.Windows.Forms.Button buttonYes;
		private System.Windows.Forms.Label labelRelName;
		private System.Windows.Forms.CheckBox checkBoxCheckUpdates;
		public const string Copyright = "Upgrade.cs, Copyright  2004 by Robert Misiak";

		private System.ComponentModel.Container components = null;
		private Config conf;
		private string url, relname;

		public Upgrade (Config conf, string location, string name)
		{
			InitializeComponent();

			this.conf = conf;
			this.url = location;
			this.relname = name;
			labelRelName.Text = conf.Res.GetString ("Upgrade.Version") + ": " + name;
			checkBoxCheckUpdates.Checked = conf.CheckUpgrade;
			this.Closing += new System.ComponentModel.CancelEventHandler (formClosing);
		}

		private void formClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			if (checkBoxCheckUpdates.Checked != conf.CheckUpgrade)
				conf.CheckUpgrade = checkBoxCheckUpdates.Checked;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Upgrade));
			this.checkBoxCheckUpdates = new System.Windows.Forms.CheckBox();
			this.labelRelName = new System.Windows.Forms.Label();
			this.buttonYes = new System.Windows.Forms.Button();
			this.buttonNo = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// checkBoxCheckUpdates
			// 
			this.checkBoxCheckUpdates.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("checkBoxCheckUpdates.Anchor")));
			this.checkBoxCheckUpdates.Appearance = ((System.Windows.Forms.Appearance)(resources.GetObject("checkBoxCheckUpdates.Appearance")));
			this.checkBoxCheckUpdates.CheckAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxCheckUpdates.CheckAlign")));
			this.checkBoxCheckUpdates.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("checkBoxCheckUpdates.Dock")));
			this.checkBoxCheckUpdates.Enabled = ((bool)(resources.GetObject("checkBoxCheckUpdates.Enabled")));
			this.checkBoxCheckUpdates.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("checkBoxCheckUpdates.FlatStyle")));
			this.checkBoxCheckUpdates.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxCheckUpdates.ImageAlign")));
			this.checkBoxCheckUpdates.ImageIndex = ((int)(resources.GetObject("checkBoxCheckUpdates.ImageIndex")));
			this.checkBoxCheckUpdates.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("checkBoxCheckUpdates.ImeMode")));
			this.checkBoxCheckUpdates.Location = ((System.Drawing.Point)(resources.GetObject("checkBoxCheckUpdates.Location")));
			this.checkBoxCheckUpdates.Name = "checkBoxCheckUpdates";
			this.checkBoxCheckUpdates.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("checkBoxCheckUpdates.RightToLeft")));
			this.checkBoxCheckUpdates.Size = ((System.Drawing.Size)(resources.GetObject("checkBoxCheckUpdates.Size")));
			this.checkBoxCheckUpdates.TabIndex = ((int)(resources.GetObject("checkBoxCheckUpdates.TabIndex")));
			this.checkBoxCheckUpdates.Text = resources.GetString("checkBoxCheckUpdates.Text");
			this.checkBoxCheckUpdates.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("checkBoxCheckUpdates.TextAlign")));
			this.checkBoxCheckUpdates.Visible = ((bool)(resources.GetObject("checkBoxCheckUpdates.Visible")));
			// 
			// labelRelName
			// 
			this.labelRelName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelRelName.Anchor")));
			this.labelRelName.AutoSize = ((bool)(resources.GetObject("labelRelName.AutoSize")));
			this.labelRelName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelRelName.Dock")));
			this.labelRelName.Enabled = ((bool)(resources.GetObject("labelRelName.Enabled")));
			this.labelRelName.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelRelName.ImageAlign")));
			this.labelRelName.ImageIndex = ((int)(resources.GetObject("labelRelName.ImageIndex")));
			this.labelRelName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelRelName.ImeMode")));
			this.labelRelName.Location = ((System.Drawing.Point)(resources.GetObject("labelRelName.Location")));
			this.labelRelName.Name = "labelRelName";
			this.labelRelName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelRelName.RightToLeft")));
			this.labelRelName.Size = ((System.Drawing.Size)(resources.GetObject("labelRelName.Size")));
			this.labelRelName.TabIndex = ((int)(resources.GetObject("labelRelName.TabIndex")));
			this.labelRelName.Text = resources.GetString("labelRelName.Text");
			this.labelRelName.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelRelName.TextAlign")));
			this.labelRelName.Visible = ((bool)(resources.GetObject("labelRelName.Visible")));
			// 
			// buttonYes
			// 
			this.buttonYes.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonYes.Anchor")));
			this.buttonYes.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonYes.Dock")));
			this.buttonYes.Enabled = ((bool)(resources.GetObject("buttonYes.Enabled")));
			this.buttonYes.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonYes.FlatStyle")));
			this.buttonYes.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonYes.ImageAlign")));
			this.buttonYes.ImageIndex = ((int)(resources.GetObject("buttonYes.ImageIndex")));
			this.buttonYes.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonYes.ImeMode")));
			this.buttonYes.Location = ((System.Drawing.Point)(resources.GetObject("buttonYes.Location")));
			this.buttonYes.Name = "buttonYes";
			this.buttonYes.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonYes.RightToLeft")));
			this.buttonYes.Size = ((System.Drawing.Size)(resources.GetObject("buttonYes.Size")));
			this.buttonYes.TabIndex = ((int)(resources.GetObject("buttonYes.TabIndex")));
			this.buttonYes.Text = resources.GetString("buttonYes.Text");
			this.buttonYes.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonYes.TextAlign")));
			this.buttonYes.Visible = ((bool)(resources.GetObject("buttonYes.Visible")));
			this.buttonYes.Click += new System.EventHandler(this.buttonYes_Click);
			// 
			// buttonNo
			// 
			this.buttonNo.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonNo.Anchor")));
			this.buttonNo.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonNo.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("buttonNo.Dock")));
			this.buttonNo.Enabled = ((bool)(resources.GetObject("buttonNo.Enabled")));
			this.buttonNo.FlatStyle = ((System.Windows.Forms.FlatStyle)(resources.GetObject("buttonNo.FlatStyle")));
			this.buttonNo.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonNo.ImageAlign")));
			this.buttonNo.ImageIndex = ((int)(resources.GetObject("buttonNo.ImageIndex")));
			this.buttonNo.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("buttonNo.ImeMode")));
			this.buttonNo.Location = ((System.Drawing.Point)(resources.GetObject("buttonNo.Location")));
			this.buttonNo.Name = "buttonNo";
			this.buttonNo.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("buttonNo.RightToLeft")));
			this.buttonNo.Size = ((System.Drawing.Size)(resources.GetObject("buttonNo.Size")));
			this.buttonNo.TabIndex = ((int)(resources.GetObject("buttonNo.TabIndex")));
			this.buttonNo.Text = resources.GetString("buttonNo.Text");
			this.buttonNo.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("buttonNo.TextAlign")));
			this.buttonNo.Visible = ((bool)(resources.GetObject("buttonNo.Visible")));
			this.buttonNo.Click += new System.EventHandler(this.buttonNo_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label1.Dock")));
			this.label1.Enabled = ((bool)(resources.GetObject("label1.Enabled")));
			this.label1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.ImageAlign")));
			this.label1.ImageIndex = ((int)(resources.GetObject("label1.ImageIndex")));
			this.label1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label1.ImeMode")));
			this.label1.Location = ((System.Drawing.Point)(resources.GetObject("label1.Location")));
			this.label1.Name = "label1";
			this.label1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label1.RightToLeft")));
			this.label1.Size = ((System.Drawing.Size)(resources.GetObject("label1.Size")));
			this.label1.TabIndex = ((int)(resources.GetObject("label1.TabIndex")));
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label1.TextAlign")));
			this.label1.Visible = ((bool)(resources.GetObject("label1.Visible")));
			// 
			// Upgrade
			// 
			this.AcceptButton = this.buttonYes;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.CancelButton = this.buttonNo;
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.ControlBox = false;
			this.Controls.Add(this.buttonNo);
			this.Controls.Add(this.buttonYes);
			this.Controls.Add(this.labelRelName);
			this.Controls.Add(this.checkBoxCheckUpdates);
			this.Controls.Add(this.label1);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "Upgrade";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.TopMost = true;
			this.ResumeLayout(false);
		}
		#endregion

		private void buttonYes_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore.exe", url);
			this.Close();
		}

		private void buttonNo_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
