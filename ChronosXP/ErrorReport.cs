#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - ErrorReport.cs
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
using System.Net;
using System.Net.Sockets;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	public sealed class ErrorReport : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonSend;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox userBox;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Label labelError;
		private System.Windows.Forms.Button buttonCancel;
		private System.ComponentModel.Container components = null;

		public ErrorReport (Exception ex) : this (null, ex, null, null) { }
        public ErrorReport(string msg, Exception ex, Config conf) : this(String.Concat("Error reading from registry: ", msg), ex, conf, null) { }
		public ErrorReport (string msg, Exception ex, Config conf, PlanetaryHours pHours)
		{
			InitializeComponent();

			if (msg == null)
				labelError.Text = "An unknown error has occurred.  ChronosXP cannot continue.";
			else
				labelError.Text = msg;

			textBox.Text = ex.Message + "\r\n";
			
			string st;
			if (ex.StackTrace == null)
				st = new StackTrace (true).ToString();
			else
				st = ex.StackTrace;
			if (ex.Source != null)
				st += "\r\nSource=" + ex.Source;
			if (ex.TargetSite != null)
				st += "\r\nTargetSite=" + ex.TargetSite;
			textBox.Text += st + "\r\n";

			try {	textBox.Text += Config.FormatVersion() + " (Build " + Application.ProductVersion + ")\r\n"; } 
			catch { textBox.Text += "Unknown ChronosXP version??? ProductVersion=" + Application.ProductVersion.ToString() + "\r\n"; }
			try { textBox.Text += Config.OSVersion() + " (" + Environment.OSVersion.ToString() + ")\r\n"; }
			catch { textBox.Text += "Unknown OS version??? OSVersion=" + Environment.OSVersion.ToString() + "\r\n"; }
			textBox.Text += ".NET Framework " + Environment.Version.ToString()
			#if MONO
				+ " (Mono-compiled)" +
			#endif
				+ "\r\n";
			textBox.Text += "Thread.CurrentThread.CurrentCulture=" + System.Threading.Thread.CurrentThread.CurrentCulture.ToString() + "\r\n";
			textBox.Text += "Thread.CurrentThread.CurrentUICulture=" + System.Threading.Thread.CurrentThread.CurrentUICulture.ToString() + "\r\n";
			try
			{
				if (!conf.LoadOK)
					textBox.Text += "Configuration: LoadOK=false\r\n";
			}
			catch { }
            try
            {
                textBox.Text += String.Format("Architecture={0}, Platform={1}\r\n", Config.Architecture, Config.Platform);
            }
            catch { }
			try
			{
				textBox.Text += String.Format ("ZoneSysApplyDST={0}, RunFromTray={1}, RunFromTrayNow={2}, CurrentZone={3}\r\n",
					conf.ZoneSysApplyDST.ToString(), conf.RunFromTray.ToString(), conf.RunFromTrayNow.ToString(),
					conf.CurrentZone);
			} 
			catch { }
			try
			{
				textBox.Text += String.Format ("CurrentCulture={0}, ZenithDistance={1}, Language={2}, Place={3}, LPlace={4}\r\n",
					conf.CurrentCulture, Config.ZenithDistance, conf.Language, conf.DPlace, conf.LPlace);
			} 
			catch {}
			try 
			{
				textBox.Text += String.Format ("SoundName={0}, SoundFile={1}, Sound={2}\r\n",
					conf.SoundName, conf.SoundFile, conf.Sound.ToString());
			}
			catch {}
			try
			{
				textBox.Text += String.Format ("Startup={0}, CheckUpgrade={1}, UseGradient={2}, ShowSeconds={3}\r\n",
					conf.Startup.ToString(), conf.CheckUpgrade.ToString(), conf.UseGradient.ToString(), conf.ShowSeconds.ToString());
			}
			catch {}
			try {
				textBox.Text += String.Format ("ShortInterval={0}, Notify={1}, Sticky={2}\r\n", conf.UseShortInterval.ToString(),
					conf.NotifyHour.ToString(), conf.Sticky.ToString());
			}
			catch {}
			try { textBox.Text += "DefaultPlace=" + conf.DefaultPlace.ToString() + "\r\n"; } catch {}
			try { textBox.Text += "DefaultLocality=" + conf.DefaultLocality.ToString() + "\r\n"; } catch {}
			try { textBox.Text += "User Error=" + labelError.Text + "\r\n"; } catch {}
			try { textBox.Text += "Run=" + conf.Run.ToString() + "\r\n"; } catch {}

			if (pHours == null)
				textBox.Text += "pHours=null\r\n";
			else
				textBox.Text += "pHours=" + pHours.ToString() + "\r\n";
			textBox.Text += "DateTime.Now=" + DateTime.Now.ToString() + "\r\n" +
				"DateTime.UtcNow=" + DateTime.UtcNow.ToString() + "\r\n";

			if (Program.SelfTest)
				Program.Fail (labelError.Text, textBox.Text);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ErrorReport));
			this.buttonCancel = new System.Windows.Forms.Button();
			this.labelError = new System.Windows.Forms.Label();
			this.textBox = new System.Windows.Forms.TextBox();
			this.userBox = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.buttonSend = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonCancel.Location = new System.Drawing.Point(320, 296);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(80, 23);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// labelError
			// 
			this.labelError.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.labelError.Location = new System.Drawing.Point(16, 8);
			this.labelError.Name = "labelError";
			this.labelError.Size = new System.Drawing.Size(376, 24);
			this.labelError.TabIndex = 0;
			this.labelError.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(16, 88);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(376, 72);
			this.textBox.TabIndex = 1;
			this.textBox.Text = "";
			this.textBox.WordWrap = false;
			// 
			// userBox
			// 
			this.userBox.AcceptsReturn = true;
			this.userBox.Location = new System.Drawing.Point(16, 224);
			this.userBox.Multiline = true;
			this.userBox.Name = "userBox";
			this.userBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.userBox.Size = new System.Drawing.Size(376, 64);
			this.userBox.TabIndex = 6;
			this.userBox.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 168);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(376, 56);
			this.label1.TabIndex = 7;
			this.label1.Text = "Optionally, you may add any additional information that you deem appropriate, suc" +
"h as what you were trying to do at the time.  You can include your email address" +
" if you would like the authors to contact you.  You may also leave this box blan" +
"k.";
			// 
			// buttonSend
			// 
			this.buttonSend.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonSend.Location = new System.Drawing.Point(200, 296);
			this.buttonSend.Name = "buttonSend";
			this.buttonSend.Size = new System.Drawing.Size(112, 23);
			this.buttonSend.TabIndex = 4;
			this.buttonSend.Text = "Send Error Report";
			this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(376, 56);
			this.label2.TabIndex = 3;
			this.label2.Text = "Please help us make ChronosXP a better program by reporting this error.  Simply c" +
"lick \"Send Error Report\" and the information shown below will be sent.  Reportin" +
"g this error will help the authors of ChronosXP to fix this problem in future re" +
"leases.";
			// 
			// ErrorReport
			// 
			this.AcceptButton = this.buttonSend;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(408, 328);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.userBox);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonSend);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.labelError);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			//this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ErrorReport";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Internal Error - ChronosXP";
			this.TopMost = true;
			this.ResumeLayout(false);
		}
		#endregion

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void buttonSend_Click(object sender, System.EventArgs e)
		{
			Hide();
			Feedback f = new Feedback (Config.FeedbackAddress, Config.FeedbackAddress, "[ChronosXP Report] " + labelError.Text,
				textBox.Text + "\r\n--\r\n" + userBox.Text, Config.SMTPServer, this);
			f.Send();
		}
	}
}
