using System;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Application entry.  Replaces the old WPF App.xaml host, which only existed to keep the process
	/// alive and pulled in a WPF dependency for no benefit.
	/// </summary>
	internal static class Program
	{
		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			try
			{
				ChronosXPCore core = new ChronosXPCore();
				if (!core.ShutdownRequested)
					Application.Run();
			}
			catch (Exception ex)
			{
				Application.Run (new ErrorReport (ex));
			}
		}
	}
}
