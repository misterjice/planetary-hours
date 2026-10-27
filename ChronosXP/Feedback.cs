#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - Feedback.cs
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
using System.ComponentModel;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Summary description for Feedback.
	/// </summary>
	public sealed class Feedback : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ProgressBar progressBar1;
		private System.Windows.Forms.Button buttonOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private string fromAddr, toAddr, subject, text, smtpServer;
		private System.Windows.Forms.Label labelText;
		private ErrorReport er;

		public Feedback (string fromaddr, string toaddr, string subject, string text, string smtpserver, ErrorReport er)
		{
			InitializeComponent();

			this.fromAddr = fromaddr;
			this.toAddr = toaddr;
			this.subject = subject;
			this.text = text;
			this.smtpServer = smtpserver;
			this.er = er;
		}

		private void step()
		{
			progressBar1.PerformStep();
			Application.DoEvents();
		}

		public void Send()
		{
			Show();
			Application.DoEvents();

			string[] lines = text.Split (new char[] { '\n' });
			progressBar1.Maximum = lines.Length + 11;

			TcpClient client = null;
            
			// From http://www.aspemporium.com/aspEmporium/cshrp/howtos/howto.asp?hid=25
			try
			{
				//create connection to mail server
				client = new TcpClient(smtpServer, 25);
				step();
				NetworkStream ns = client.GetStream();
				StreamReader stdIn = new StreamReader(ns);
				StreamWriter stdOut = new StreamWriter(ns);

				//SERVER GREETING
				int responseCode = getResponse(stdIn);
				if (responseCode != 220)
					throw new Exception("SMTP Server " + smtpServer + " unavailable.");
				step();

				//HELO COMMAND
				stdOut.WriteLine("HELO chronosxpuser.com");
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				if (responseCode != 250)
					throw new Exception("HELO failed.");
				step();

				//MAIL COMMAND
				stdOut.WriteLine("MAIL FROM:" + fromAddr);
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				if (responseCode != 250)
					throw new Exception("Sender address rejected by server.");
				step();

				//RCPT COMMAND - first email
				stdOut.WriteLine("RCPT TO:" + toAddr);
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				switch(responseCode)
				{
					case 250:
					case 251:
						break;
					default:
						throw new Exception("Recipient address rejected by server.  (" + responseCode.ToString() + ")");
				}
				step();

				//DATA COMMAND
				stdOut.WriteLine("DATA");
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				if (responseCode != 354)
					throw new Exception("Data command not accepted by server.");
				step();

				//send email that was generated before
				stdOut.WriteLine ("Subject: " + subject);
				stdOut.Flush();
				stdOut.WriteLine ("To: " + toAddr);
				step();
				stdOut.Flush();
				stdOut.WriteLine ("From: " + fromAddr);
				stdOut.Flush();
				step();
				foreach (string line in lines)
				{
					if (line.StartsWith (".") || line.ToLower().StartsWith ("from"))
						stdOut.WriteLine (">" + line.TrimEnd (new char[] { '\r', '\n' }));
					else
						stdOut.WriteLine (line.TrimEnd (new char[] { '\r', '\n' }));
					stdOut.Flush();
					step();
				}
				//send . on a line by itself to signify end of stream
				stdOut.WriteLine(".");
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				if (responseCode != 250)
					throw new Exception("Email not accepted.");
				step();

				//QUIT COMMAND
				stdOut.WriteLine("QUIT");
				stdOut.Flush();
				responseCode = getResponse(stdIn);
				labelText.Text = "Error report sent.  Thank you for helping to improve ChronosXP!";
				step();
			}
			catch (Exception ex)
			{
				progressBar1.Step = progressBar1.Maximum;
				labelText.Text = ex.Message + "  Sending report failed.";
			}
			finally
			{
				//dont forget to close that socket...
				if (client != null)
					client.Close();
				client = null;
			}
			step();
			buttonOK.Enabled = true;
		}

		private static int getResponse (StreamReader stdIn)
		{
			try
			{
				string response = "";
				do
					response += stdIn.ReadLine()+"\r\n";
				while(stdIn.Peek() != -1);

				//get the three digit smtp code returned by the server
				return Convert.ToInt32(response.Substring(0, 3));
			}
			catch
			{
				//catch any errors
				return 0;
			}
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
		private void InitializeComponent()
		{
			this.progressBar1 = new System.Windows.Forms.ProgressBar();
			this.buttonOK = new System.Windows.Forms.Button();
			this.labelText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// progressBar1
			// 
			this.progressBar1.Location = new System.Drawing.Point(16, 48);
			this.progressBar1.Name = "progressBar1";
			this.progressBar1.Size = new System.Drawing.Size(352, 23);
			this.progressBar1.Step = 1;
			this.progressBar1.TabIndex = 0;
			// 
			// buttonOK
			// 
			this.buttonOK.Enabled = false;
			this.buttonOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.buttonOK.Location = new System.Drawing.Point(296, 88);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.TabIndex = 1;
			this.buttonOK.Text = "OK";
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// labelText
			// 
			this.labelText.Location = new System.Drawing.Point(16, 0);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(352, 48);
			this.labelText.TabIndex = 3;
			this.labelText.Text = "Sending error report...";
			this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// Feedback
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(384, 126);
			this.ControlBox = false;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.labelText,
																		  this.buttonOK,
																		  this.progressBar1});
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Feedback";
			this.Text = "ChronosXP";
			this.TopMost = true;
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonOK_Click(object sender, System.EventArgs e)
		{
			Close();
			this.er.Close();
		}
	}
}
