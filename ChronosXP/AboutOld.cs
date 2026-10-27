#region Copyright © 2004-2005 by Robert Misiak
// ChronosXP - About.cs
// Copyright © 2004-2005 by Robert Misiak <rmisiak@users.sourceforge.net>
// PO Box 70972, Las Vegas, NV 89170-0972, United States of America
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Program credits form.
	/// </summary>
	public sealed class AboutOld : System.Windows.Forms.Form
	{
		private ChronosXP.XPLabel xpLabel2;
		private ChronosXP.XPLabel xpLabel1;
		private ChronosXP.XPGroupBox xpGroupBoxCredits;
		private ChronosXP.XPLabel xPLabel8;
		private ChronosXP.XPLinkLabel xPLinkLabel1;
		private ChronosXP.XPLinkLabel linkLabelLicense;
		private ChronosXP.XPLabel labelName;
		private ChronosXP.XPLabel xPLabel7;
		private ChronosXP.XPLabel xPLabel5;
		private ChronosXP.XPLabel xPLabel4;
		private ChronosXP.XPLabel label14;
		private System.Windows.Forms.PictureBox pictureBox1;
		private ChronosXP.XPLabel label1;
		private System.Windows.Forms.Button buttonOK;
		private ChronosXP.XPLabel label7;
		private ChronosXP.XPLinkLabel linkLabelURL;
		private ChronosXP.XPLabel labelVer;
			//private ChronosXP.XPLabel xpLabelOS;
		public const string Copyright = "About.cs, Copyright © 2004-2005 by Robert Misiak";
		
		private System.ComponentModel.Container components = null;
		private Config conf;

		public AboutOld (Config conf)
		{
			InitializeComponent();

			// We do our own painting in this form, see About.OnPaintBackground
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.DoubleBuffer, true);

			this.conf = conf;
			conf.Core.FormAboutOpen = true;
			Closing += new System.ComponentModel.CancelEventHandler (formClosing);
			
			#if BETA
				if (!conf.Core.Photogenic)
					labelVer.Text = String.Format ("{0} (Build {1}), {2}", Config.BetaVersion, Application.ProductVersion, Config.CopyrightString);
				else
				{
			#endif
					labelVer.Text = String.Format ("Version {0}, {1}", Config.Version, Config.CopyrightString);
			#if BETA
				}
			#endif

			labelVer.Left = labelName.Left + 2;
			linkLabelLicense.Left = labelName.Left + 2;

			pictureBox1.Image = Image.FromStream(conf.Fx.GlyphGif("Saturn"));
			
			//xpLabelAddress.Text = Config.PostalAddress;
			/*
				xpLabelOS = new XPLabel();
				xpLabelOS.Size = new Size (340, 20);
				xpLabelOS.Location = new Point (8, 336);
				xpLabelOS.BackColor = Color.Transparent;
				#if MONO
					xpLabelOS.Text = String.Format("{0} (with Mono)", Config.OSVersion());
				#else
					xpLabelOS.Text = String.Format("{0} (.NET Framework {1})", Config.OSVersion(), Environment.Version.ToString());
				#endif
				Controls.Add(xpLabelOS);
				*/
		}

		// Paint our own background.
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (conf.UseGradient && PInvoke.VisualStylesEnabled())
				using (LinearGradientBrush lgb = new LinearGradientBrush (DisplayRectangle, SystemColors.ControlLightLight, SystemColors.Control,
						   LinearGradientMode.Vertical))
					e.Graphics.FillRectangle (lgb, DisplayRectangle);
			else
				using (SolidBrush sb = new SolidBrush (BackColor))
					e.Graphics.FillRectangle (sb, DisplayRectangle);
		}

		private void formClosing (object sender, System.ComponentModel.CancelEventArgs e)
		{
			conf.Core.FormAboutOpen = false;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AboutOld));
			this.labelVer = new ChronosXP.XPLabel();
			this.linkLabelURL = new ChronosXP.XPLinkLabel();
			this.label7 = new ChronosXP.XPLabel();
			this.buttonOK = new System.Windows.Forms.Button();
			this.label1 = new ChronosXP.XPLabel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label14 = new ChronosXP.XPLabel();
			this.xPLabel4 = new ChronosXP.XPLabel();
			this.xPLabel5 = new ChronosXP.XPLabel();
			this.xPLabel7 = new ChronosXP.XPLabel();
			this.labelName = new ChronosXP.XPLabel();
			this.linkLabelLicense = new ChronosXP.XPLinkLabel();
			this.xPLinkLabel1 = new ChronosXP.XPLinkLabel();
			this.xPLabel8 = new ChronosXP.XPLabel();
			this.xpGroupBoxCredits = new ChronosXP.XPGroupBox();
			this.xpLabel1 = new ChronosXP.XPLabel();
			this.xpLabel2 = new ChronosXP.XPLabel();
			this.xpGroupBoxCredits.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelVer
			// 
			this.labelVer.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelVer.Anchor")));
			this.labelVer.AutoSize = ((bool)(resources.GetObject("labelVer.AutoSize")));
			this.labelVer.BackColor = System.Drawing.Color.Transparent;
			this.labelVer.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelVer.Dock")));
			this.labelVer.Enabled = ((bool)(resources.GetObject("labelVer.Enabled")));
			this.labelVer.Font = ((System.Drawing.Font)(resources.GetObject("labelVer.Font")));
			this.labelVer.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelVer.ImageAlign")));
			this.labelVer.ImageIndex = ((int)(resources.GetObject("labelVer.ImageIndex")));
			this.labelVer.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelVer.ImeMode")));
			this.labelVer.Location = ((System.Drawing.Point)(resources.GetObject("labelVer.Location")));
			this.labelVer.Name = "labelVer";
			this.labelVer.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelVer.RightToLeft")));
			this.labelVer.Size = ((System.Drawing.Size)(resources.GetObject("labelVer.Size")));
			this.labelVer.TabIndex = ((int)(resources.GetObject("labelVer.TabIndex")));
			this.labelVer.Text = resources.GetString("labelVer.Text");
			this.labelVer.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelVer.TextAlign")));
			this.labelVer.Visible = ((bool)(resources.GetObject("labelVer.Visible")));
			// 
			// linkLabelURL
			// 
			this.linkLabelURL.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("linkLabelURL.Anchor")));
			this.linkLabelURL.AutoSize = ((bool)(resources.GetObject("linkLabelURL.AutoSize")));
			this.linkLabelURL.BackColor = System.Drawing.Color.Transparent;
			this.linkLabelURL.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("linkLabelURL.Dock")));
			this.linkLabelURL.Enabled = ((bool)(resources.GetObject("linkLabelURL.Enabled")));
			this.linkLabelURL.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("linkLabelURL.ImageAlign")));
			this.linkLabelURL.ImageIndex = ((int)(resources.GetObject("linkLabelURL.ImageIndex")));
			this.linkLabelURL.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("linkLabelURL.ImeMode")));
			this.linkLabelURL.LinkArea = ((System.Windows.Forms.LinkArea)(resources.GetObject("linkLabelURL.LinkArea")));
			this.linkLabelURL.Location = ((System.Drawing.Point)(resources.GetObject("linkLabelURL.Location")));
			this.linkLabelURL.Name = "linkLabelURL";
			this.linkLabelURL.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("linkLabelURL.RightToLeft")));
			this.linkLabelURL.Size = ((System.Drawing.Size)(resources.GetObject("linkLabelURL.Size")));
			this.linkLabelURL.TabIndex = ((int)(resources.GetObject("linkLabelURL.TabIndex")));
			this.linkLabelURL.TabStop = true;
			this.linkLabelURL.Tag = "http://chronosxp.sourceforge.net";
			this.linkLabelURL.Text = resources.GetString("linkLabelURL.Text");
			this.linkLabelURL.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("linkLabelURL.TextAlign")));
			this.linkLabelURL.Visible = ((bool)(resources.GetObject("linkLabelURL.Visible")));
			this.linkLabelURL.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelURL_LinkClicked);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label7.Anchor")));
			this.label7.AutoSize = ((bool)(resources.GetObject("label7.AutoSize")));
			this.label7.BackColor = System.Drawing.Color.Transparent;
			this.label7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label7.Dock")));
			this.label7.Enabled = ((bool)(resources.GetObject("label7.Enabled")));
			this.label7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.ImageAlign")));
			this.label7.ImageIndex = ((int)(resources.GetObject("label7.ImageIndex")));
			this.label7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label7.ImeMode")));
			this.label7.Location = ((System.Drawing.Point)(resources.GetObject("label7.Location")));
			this.label7.Name = "label7";
			this.label7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label7.RightToLeft")));
			this.label7.Size = ((System.Drawing.Size)(resources.GetObject("label7.Size")));
			this.label7.TabIndex = ((int)(resources.GetObject("label7.TabIndex")));
			this.label7.Text = resources.GetString("label7.Text");
			this.label7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label7.TextAlign")));
			this.label7.Visible = ((bool)(resources.GetObject("label7.Visible")));
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("buttonOK.Anchor")));
			this.buttonOK.BackColor = System.Drawing.Color.Transparent;
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
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label1.Anchor")));
			this.label1.AutoSize = ((bool)(resources.GetObject("label1.AutoSize")));
			this.label1.BackColor = System.Drawing.Color.Transparent;
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
			// pictureBox1
			// 
			this.pictureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("pictureBox1.Anchor")));
			this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
			this.pictureBox1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("pictureBox1.Dock")));
			this.pictureBox1.Enabled = ((bool)(resources.GetObject("pictureBox1.Enabled")));
			this.pictureBox1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("pictureBox1.ImeMode")));
			this.pictureBox1.Location = ((System.Drawing.Point)(resources.GetObject("pictureBox1.Location")));
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("pictureBox1.RightToLeft")));
			this.pictureBox1.Size = ((System.Drawing.Size)(resources.GetObject("pictureBox1.Size")));
			this.pictureBox1.SizeMode = ((System.Windows.Forms.PictureBoxSizeMode)(resources.GetObject("pictureBox1.SizeMode")));
			this.pictureBox1.TabIndex = ((int)(resources.GetObject("pictureBox1.TabIndex")));
			this.pictureBox1.TabStop = false;
			this.pictureBox1.Text = resources.GetString("pictureBox1.Text");
			this.pictureBox1.Visible = ((bool)(resources.GetObject("pictureBox1.Visible")));
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("label14.Anchor")));
			this.label14.AutoSize = ((bool)(resources.GetObject("label14.AutoSize")));
			this.label14.BackColor = System.Drawing.Color.Transparent;
			this.label14.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("label14.Dock")));
			this.label14.Enabled = ((bool)(resources.GetObject("label14.Enabled")));
			this.label14.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label14.ImageAlign")));
			this.label14.ImageIndex = ((int)(resources.GetObject("label14.ImageIndex")));
			this.label14.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("label14.ImeMode")));
			this.label14.Location = ((System.Drawing.Point)(resources.GetObject("label14.Location")));
			this.label14.Name = "label14";
			this.label14.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("label14.RightToLeft")));
			this.label14.Size = ((System.Drawing.Size)(resources.GetObject("label14.Size")));
			this.label14.TabIndex = ((int)(resources.GetObject("label14.TabIndex")));
			this.label14.Text = resources.GetString("label14.Text");
			this.label14.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("label14.TextAlign")));
			this.label14.Visible = ((bool)(resources.GetObject("label14.Visible")));
			// 
			// xPLabel4
			// 
			this.xPLabel4.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xPLabel4.Anchor")));
			this.xPLabel4.AutoSize = ((bool)(resources.GetObject("xPLabel4.AutoSize")));
			this.xPLabel4.BackColor = System.Drawing.Color.Transparent;
			this.xPLabel4.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xPLabel4.Dock")));
			this.xPLabel4.Enabled = ((bool)(resources.GetObject("xPLabel4.Enabled")));
			this.xPLabel4.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel4.ImageAlign")));
			this.xPLabel4.ImageIndex = ((int)(resources.GetObject("xPLabel4.ImageIndex")));
			this.xPLabel4.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xPLabel4.ImeMode")));
			this.xPLabel4.Location = ((System.Drawing.Point)(resources.GetObject("xPLabel4.Location")));
			this.xPLabel4.Name = "xPLabel4";
			this.xPLabel4.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xPLabel4.RightToLeft")));
			this.xPLabel4.Size = ((System.Drawing.Size)(resources.GetObject("xPLabel4.Size")));
			this.xPLabel4.TabIndex = ((int)(resources.GetObject("xPLabel4.TabIndex")));
			this.xPLabel4.Text = resources.GetString("xPLabel4.Text");
			this.xPLabel4.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel4.TextAlign")));
			this.xPLabel4.Visible = ((bool)(resources.GetObject("xPLabel4.Visible")));
			// 
			// xPLabel5
			// 
			this.xPLabel5.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xPLabel5.Anchor")));
			this.xPLabel5.AutoSize = ((bool)(resources.GetObject("xPLabel5.AutoSize")));
			this.xPLabel5.BackColor = System.Drawing.Color.Transparent;
			this.xPLabel5.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xPLabel5.Dock")));
			this.xPLabel5.Enabled = ((bool)(resources.GetObject("xPLabel5.Enabled")));
			this.xPLabel5.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel5.ImageAlign")));
			this.xPLabel5.ImageIndex = ((int)(resources.GetObject("xPLabel5.ImageIndex")));
			this.xPLabel5.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xPLabel5.ImeMode")));
			this.xPLabel5.Location = ((System.Drawing.Point)(resources.GetObject("xPLabel5.Location")));
			this.xPLabel5.Name = "xPLabel5";
			this.xPLabel5.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xPLabel5.RightToLeft")));
			this.xPLabel5.Size = ((System.Drawing.Size)(resources.GetObject("xPLabel5.Size")));
			this.xPLabel5.TabIndex = ((int)(resources.GetObject("xPLabel5.TabIndex")));
			this.xPLabel5.Text = resources.GetString("xPLabel5.Text");
			this.xPLabel5.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel5.TextAlign")));
			this.xPLabel5.Visible = ((bool)(resources.GetObject("xPLabel5.Visible")));
			// 
			// xPLabel7
			// 
			this.xPLabel7.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xPLabel7.Anchor")));
			this.xPLabel7.AutoSize = ((bool)(resources.GetObject("xPLabel7.AutoSize")));
			this.xPLabel7.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xPLabel7.Dock")));
			this.xPLabel7.Enabled = ((bool)(resources.GetObject("xPLabel7.Enabled")));
			this.xPLabel7.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel7.ImageAlign")));
			this.xPLabel7.ImageIndex = ((int)(resources.GetObject("xPLabel7.ImageIndex")));
			this.xPLabel7.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xPLabel7.ImeMode")));
			this.xPLabel7.Location = ((System.Drawing.Point)(resources.GetObject("xPLabel7.Location")));
			this.xPLabel7.Name = "xPLabel7";
			this.xPLabel7.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xPLabel7.RightToLeft")));
			this.xPLabel7.Size = ((System.Drawing.Size)(resources.GetObject("xPLabel7.Size")));
			this.xPLabel7.TabIndex = ((int)(resources.GetObject("xPLabel7.TabIndex")));
			this.xPLabel7.Text = resources.GetString("xPLabel7.Text");
			this.xPLabel7.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel7.TextAlign")));
			this.xPLabel7.Visible = ((bool)(resources.GetObject("xPLabel7.Visible")));
			// 
			// labelName
			// 
			this.labelName.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("labelName.Anchor")));
			this.labelName.AutoSize = ((bool)(resources.GetObject("labelName.AutoSize")));
			this.labelName.BackColor = System.Drawing.Color.Transparent;
			this.labelName.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("labelName.Dock")));
			this.labelName.Enabled = ((bool)(resources.GetObject("labelName.Enabled")));
			this.labelName.Font = ((System.Drawing.Font)(resources.GetObject("labelName.Font")));
			this.labelName.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelName.ImageAlign")));
			this.labelName.ImageIndex = ((int)(resources.GetObject("labelName.ImageIndex")));
			this.labelName.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("labelName.ImeMode")));
			this.labelName.Location = ((System.Drawing.Point)(resources.GetObject("labelName.Location")));
			this.labelName.Name = "labelName";
			this.labelName.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("labelName.RightToLeft")));
			this.labelName.Size = ((System.Drawing.Size)(resources.GetObject("labelName.Size")));
			this.labelName.TabIndex = ((int)(resources.GetObject("labelName.TabIndex")));
			this.labelName.Text = resources.GetString("labelName.Text");
			this.labelName.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("labelName.TextAlign")));
			this.labelName.Visible = ((bool)(resources.GetObject("labelName.Visible")));
			// 
			// linkLabelLicense
			// 
			this.linkLabelLicense.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("linkLabelLicense.Anchor")));
			this.linkLabelLicense.AutoSize = ((bool)(resources.GetObject("linkLabelLicense.AutoSize")));
			this.linkLabelLicense.BackColor = System.Drawing.Color.Transparent;
			this.linkLabelLicense.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("linkLabelLicense.Dock")));
			this.linkLabelLicense.Enabled = ((bool)(resources.GetObject("linkLabelLicense.Enabled")));
			this.linkLabelLicense.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("linkLabelLicense.ImageAlign")));
			this.linkLabelLicense.ImageIndex = ((int)(resources.GetObject("linkLabelLicense.ImageIndex")));
			this.linkLabelLicense.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("linkLabelLicense.ImeMode")));
			this.linkLabelLicense.LinkArea = ((System.Windows.Forms.LinkArea)(resources.GetObject("linkLabelLicense.LinkArea")));
			this.linkLabelLicense.Location = ((System.Drawing.Point)(resources.GetObject("linkLabelLicense.Location")));
			this.linkLabelLicense.Name = "linkLabelLicense";
			this.linkLabelLicense.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("linkLabelLicense.RightToLeft")));
			this.linkLabelLicense.Size = ((System.Drawing.Size)(resources.GetObject("linkLabelLicense.Size")));
			this.linkLabelLicense.TabIndex = ((int)(resources.GetObject("linkLabelLicense.TabIndex")));
			this.linkLabelLicense.TabStop = true;
			this.linkLabelLicense.Text = resources.GetString("linkLabelLicense.Text");
			this.linkLabelLicense.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("linkLabelLicense.TextAlign")));
			this.linkLabelLicense.Visible = ((bool)(resources.GetObject("linkLabelLicense.Visible")));
			this.linkLabelLicense.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabelLicense_LinkClicked);
			// 
			// xPLinkLabel1
			// 
			this.xPLinkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xPLinkLabel1.Anchor")));
			this.xPLinkLabel1.AutoSize = ((bool)(resources.GetObject("xPLinkLabel1.AutoSize")));
			this.xPLinkLabel1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xPLinkLabel1.Dock")));
			this.xPLinkLabel1.Enabled = ((bool)(resources.GetObject("xPLinkLabel1.Enabled")));
			this.xPLinkLabel1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLinkLabel1.ImageAlign")));
			this.xPLinkLabel1.ImageIndex = ((int)(resources.GetObject("xPLinkLabel1.ImageIndex")));
			this.xPLinkLabel1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xPLinkLabel1.ImeMode")));
			this.xPLinkLabel1.LinkArea = ((System.Windows.Forms.LinkArea)(resources.GetObject("xPLinkLabel1.LinkArea")));
			this.xPLinkLabel1.Location = ((System.Drawing.Point)(resources.GetObject("xPLinkLabel1.Location")));
			this.xPLinkLabel1.Name = "xPLinkLabel1";
			this.xPLinkLabel1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xPLinkLabel1.RightToLeft")));
			this.xPLinkLabel1.Size = ((System.Drawing.Size)(resources.GetObject("xPLinkLabel1.Size")));
			this.xPLinkLabel1.TabIndex = ((int)(resources.GetObject("xPLinkLabel1.TabIndex")));
			this.xPLinkLabel1.TabStop = true;
			this.xPLinkLabel1.Tag = "http://CartaNatal.com";
			this.xPLinkLabel1.Text = resources.GetString("xPLinkLabel1.Text");
			this.xPLinkLabel1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLinkLabel1.TextAlign")));
			this.xPLinkLabel1.Visible = ((bool)(resources.GetObject("xPLinkLabel1.Visible")));
			this.xPLinkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.xpLinkLabel_LinkClicked);
			// 
			// xPLabel8
			// 
			this.xPLabel8.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xPLabel8.Anchor")));
			this.xPLabel8.AutoSize = ((bool)(resources.GetObject("xPLabel8.AutoSize")));
			this.xPLabel8.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xPLabel8.Dock")));
			this.xPLabel8.Enabled = ((bool)(resources.GetObject("xPLabel8.Enabled")));
			this.xPLabel8.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel8.ImageAlign")));
			this.xPLabel8.ImageIndex = ((int)(resources.GetObject("xPLabel8.ImageIndex")));
			this.xPLabel8.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xPLabel8.ImeMode")));
			this.xPLabel8.Location = ((System.Drawing.Point)(resources.GetObject("xPLabel8.Location")));
			this.xPLabel8.Name = "xPLabel8";
			this.xPLabel8.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xPLabel8.RightToLeft")));
			this.xPLabel8.Size = ((System.Drawing.Size)(resources.GetObject("xPLabel8.Size")));
			this.xPLabel8.TabIndex = ((int)(resources.GetObject("xPLabel8.TabIndex")));
			this.xPLabel8.Text = resources.GetString("xPLabel8.Text");
			this.xPLabel8.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xPLabel8.TextAlign")));
			this.xPLabel8.Visible = ((bool)(resources.GetObject("xPLabel8.Visible")));
			// 
			// xpGroupBoxCredits
			// 
			this.xpGroupBoxCredits.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xpGroupBoxCredits.Anchor")));
			this.xpGroupBoxCredits.BackColor = System.Drawing.Color.Transparent;
			this.xpGroupBoxCredits.Controls.Add(this.xPLinkLabel1);
			this.xpGroupBoxCredits.Controls.Add(this.xPLabel8);
			this.xpGroupBoxCredits.Controls.Add(this.xPLabel7);
			this.xpGroupBoxCredits.Controls.Add(this.xPLabel5);
			this.xpGroupBoxCredits.Controls.Add(this.xPLabel4);
			this.xpGroupBoxCredits.Controls.Add(this.xpLabel1);
			this.xpGroupBoxCredits.Controls.Add(this.label14);
			this.xpGroupBoxCredits.Controls.Add(this.label7);
			this.xpGroupBoxCredits.Controls.Add(this.label1);
			this.xpGroupBoxCredits.Controls.Add(this.xpLabel2);
			this.xpGroupBoxCredits.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xpGroupBoxCredits.Dock")));
			this.xpGroupBoxCredits.Enabled = ((bool)(resources.GetObject("xpGroupBoxCredits.Enabled")));
			this.xpGroupBoxCredits.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xpGroupBoxCredits.ImeMode")));
			this.xpGroupBoxCredits.Location = ((System.Drawing.Point)(resources.GetObject("xpGroupBoxCredits.Location")));
			this.xpGroupBoxCredits.Name = "xpGroupBoxCredits";
			this.xpGroupBoxCredits.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xpGroupBoxCredits.RightToLeft")));
			this.xpGroupBoxCredits.Size = ((System.Drawing.Size)(resources.GetObject("xpGroupBoxCredits.Size")));
			this.xpGroupBoxCredits.TabIndex = ((int)(resources.GetObject("xpGroupBoxCredits.TabIndex")));
			this.xpGroupBoxCredits.TabStop = false;
			this.xpGroupBoxCredits.Text = resources.GetString("xpGroupBoxCredits.Text");
			this.xpGroupBoxCredits.Visible = ((bool)(resources.GetObject("xpGroupBoxCredits.Visible")));
			// 
			// xpLabel1
			// 
			this.xpLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xpLabel1.Anchor")));
			this.xpLabel1.AutoSize = ((bool)(resources.GetObject("xpLabel1.AutoSize")));
			this.xpLabel1.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xpLabel1.Dock")));
			this.xpLabel1.Enabled = ((bool)(resources.GetObject("xpLabel1.Enabled")));
			this.xpLabel1.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xpLabel1.ImageAlign")));
			this.xpLabel1.ImageIndex = ((int)(resources.GetObject("xpLabel1.ImageIndex")));
			this.xpLabel1.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xpLabel1.ImeMode")));
			this.xpLabel1.Location = ((System.Drawing.Point)(resources.GetObject("xpLabel1.Location")));
			this.xpLabel1.Name = "xpLabel1";
			this.xpLabel1.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xpLabel1.RightToLeft")));
			this.xpLabel1.Size = ((System.Drawing.Size)(resources.GetObject("xpLabel1.Size")));
			this.xpLabel1.TabIndex = ((int)(resources.GetObject("xpLabel1.TabIndex")));
			this.xpLabel1.Text = resources.GetString("xpLabel1.Text");
			this.xpLabel1.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xpLabel1.TextAlign")));
			this.xpLabel1.Visible = ((bool)(resources.GetObject("xpLabel1.Visible")));
			// 
			// xpLabel2
			// 
			this.xpLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(resources.GetObject("xpLabel2.Anchor")));
			this.xpLabel2.AutoSize = ((bool)(resources.GetObject("xpLabel2.AutoSize")));
			this.xpLabel2.BackColor = System.Drawing.Color.Transparent;
			this.xpLabel2.Dock = ((System.Windows.Forms.DockStyle)(resources.GetObject("xpLabel2.Dock")));
			this.xpLabel2.Enabled = ((bool)(resources.GetObject("xpLabel2.Enabled")));
			this.xpLabel2.ImageAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xpLabel2.ImageAlign")));
			this.xpLabel2.ImageIndex = ((int)(resources.GetObject("xpLabel2.ImageIndex")));
			this.xpLabel2.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("xpLabel2.ImeMode")));
			this.xpLabel2.Location = ((System.Drawing.Point)(resources.GetObject("xpLabel2.Location")));
			this.xpLabel2.Name = "xpLabel2";
			this.xpLabel2.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("xpLabel2.RightToLeft")));
			this.xpLabel2.Size = ((System.Drawing.Size)(resources.GetObject("xpLabel2.Size")));
			this.xpLabel2.TabIndex = ((int)(resources.GetObject("xpLabel2.TabIndex")));
			this.xpLabel2.Text = resources.GetString("xpLabel2.Text");
			this.xpLabel2.TextAlign = ((System.Drawing.ContentAlignment)(resources.GetObject("xpLabel2.TextAlign")));
			this.xpLabel2.Visible = ((bool)(resources.GetObject("xpLabel2.Visible")));
			// 
			// About
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScaleBaseSize")));
			this.AutoScroll = ((bool)(resources.GetObject("$this.AutoScroll")));
			this.AutoScrollMargin = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMargin")));
			this.AutoScrollMinSize = ((System.Drawing.Size)(resources.GetObject("$this.AutoScrollMinSize")));
			this.BackColor = System.Drawing.SystemColors.ControlLight;
			this.CancelButton = this.buttonOK;
			this.ClientSize = ((System.Drawing.Size)(resources.GetObject("$this.ClientSize")));
			this.Controls.Add(this.xpGroupBoxCredits);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.linkLabelLicense);
			this.Controls.Add(this.labelVer);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.linkLabelURL);
			this.Enabled = ((bool)(resources.GetObject("$this.Enabled")));
			this.Font = ((System.Drawing.Font)(resources.GetObject("$this.Font")));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.ImeMode = ((System.Windows.Forms.ImeMode)(resources.GetObject("$this.ImeMode")));
			this.Location = ((System.Drawing.Point)(resources.GetObject("$this.Location")));
			this.MaximizeBox = false;
			this.MaximumSize = ((System.Drawing.Size)(resources.GetObject("$this.MaximumSize")));
			this.MinimumSize = ((System.Drawing.Size)(resources.GetObject("$this.MinimumSize")));
			this.Name = "About";
			this.RightToLeft = ((System.Windows.Forms.RightToLeft)(resources.GetObject("$this.RightToLeft")));
			this.StartPosition = ((System.Windows.Forms.FormStartPosition)(resources.GetObject("$this.StartPosition")));
			this.Text = resources.GetString("$this.Text");
			this.xpGroupBoxCredits.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void linkLabelURL_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelURL.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelURL.Tag);
		}
/*
		private void linkLabelCrystalCloud_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelCrystalCloud.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelCrystalCloud.Tag);
		}
*/
		/*
		private void linkLabelCodeProject_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelCodeProject.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelCodeProject.Tag);
		}*/

		private void linkLabelLicense_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelLicense.LinkVisited = true;
			conf.Core.ShowLicense();
		}
		/*
		private void linkLabelSourceForge_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelSourceForge.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelSourceForge.Tag);
		}
		private void linkLabelSourceCode_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelSourceCode.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelSourceCode.Tag);
		}

		private void linkLabelQA_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			linkLabelQA.LinkVisited = true;
			conf.LaunchBrowser ((string) linkLabelQA.Tag);
		}
*//*
		private void xpLinkLabelSharpDevelop_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			xpLinkLabelSharpDevelop.LinkVisited = true;
			conf.LaunchBrowser((string)xpLinkLabelSharpDevelop.Tag);
		}
		
		private void xpLinkLabelHamburger_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			xpLinkLabelHamburger.LinkVisited = true;
			conf.LaunchBrowser ((string) xpLinkLabelHamburger.Tag);
		}
		*/
		/*
		private void XpLinkLabelSDLinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			xpLinkLabelSD.LinkVisited = true;
			conf.LaunchBrowser ((string) xpLinkLabelSD.Tag);
		}*/
		void xpLinkLabel_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
			XPLinkLabel xpll = sender as XPLinkLabel;
			xpll.LinkVisited = true;
			conf.LaunchBrowser((string)xpll.Tag);
		}
	}
}
