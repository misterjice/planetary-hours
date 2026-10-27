#region Copyright  2004-2009 by Robert Misiak
// ChronosXP - License.cs
// Copyright  2004-2009 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Display a file in a RichTextBox (Read Me.rtf or License.rtf)
	/// </summary>
	public sealed partial class ShowRichTextBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button buttonOK;
		public const string Copyright = "RichTextBox.cs, Copyright  2004 by Robert Misiak";
		public const string LicenseFile = "License.rtf";

		private System.ComponentModel.Container components = null;
		private Config conf;

		public ShowRichTextBox (Config conf)
		{
			this.conf = conf;
			InitializeComponent();
			Closing += new System.ComponentModel.CancelEventHandler (formClose);

			string filename;
            //if (type == FileType.License)
            //{
            filename = Path.Combine(conf.Path, LicenseFile);
				this.Text = conf.GetString("License.Title");
				conf.Core.FormLicenseOpen = true;
            //}
            //else
            //{
            //    filename = conf.Path + ReadMeFile;
            //    this.Text = conf.GetString ("Read Me.Title");
            //    conf.Core.FormReadMeOpen = true;
            //}

			if (File.Exists (filename))
			{
				try
				{
					richTextBox1.LoadFile (filename);
				}
				catch
				{
					MessageBox.Show ("Failed opening file: " + filename, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
					conf.Core.FormLicenseOpen = false;
					Dispose();
				}
			}
			else
			{
				MessageBox.Show ("File not found: " + filename, "File Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
				conf.Core.FormReadMeOpen = false;
				Dispose();
			}
			richTextBox1.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler (linkClicked);
		}

		private void linkClicked (object sender, System.Windows.Forms.LinkClickedEventArgs e)
		{
			conf.LaunchBrowser (e.LinkText);
		}

		private void formClose (object sender, System.ComponentModel.CancelEventArgs e)
		{
			conf.Core.FormLicenseOpen = false;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ShowRichTextBox));
			this.buttonOK = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.SuspendLayout();
			// 
			// buttonOK
			// 
			this.buttonOK.BackColor = System.Drawing.Color.Transparent;
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(472, 376);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(72, 24);
			this.buttonOK.TabIndex = 257;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.richTextBox1.Location = new System.Drawing.Point(12, 12);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(529, 352);
			this.richTextBox1.TabIndex = 0;
			this.richTextBox1.Text = "";
			// 
			// RichTextBox
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.BackColor = System.Drawing.SystemColors.Control;
			this.CancelButton = this.buttonOK;
			this.ClientSize = new System.Drawing.Size(554, 412);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.richTextBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "RichTextBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.ResumeLayout(false);
		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
