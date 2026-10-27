using System;
using System.IO;
using System.Windows.Forms;

namespace ChronosXP
{
	/// <summary>
	/// Application entry.  Replaces the old WPF App.xaml host, which only existed to keep the process
	/// alive and pulled in a WPF dependency for no benefit.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// Set by the /selftest argument (used by CI): any error report or unhandled exception is written to
		/// %TEMP%\ChronosXP-selftest.log and exits with code 1; a clean run exits with code 0.
		/// </summary>
		public static bool SelfTest;
		private static string selfTestLog;

		[STAThread]
		public static void Main (string[] args)
		{
			Application.EnableVisualStyles();
			foreach (string arg in args)
				if (arg.ToLower() == "/selftest")
					SelfTest = true;

			if (SelfTest)
			{
				selfTestLog = Path.Combine (Path.GetTempPath(), "ChronosXP-selftest.log");
				File.Delete (selfTestLog);
				Application.SetUnhandledExceptionMode (UnhandledExceptionMode.CatchException);
				Application.ThreadException += (s, e) => Fail ("Unhandled exception", e.Exception.ToString());
				AppDomain.CurrentDomain.UnhandledException += (s, e) => Fail ("Unhandled exception", e.ExceptionObject.ToString());
				Log ("Self-test started");
			}

			try
			{
				ChronosXPCore core = new ChronosXPCore();
				if (core.ShutdownRequested)
				{
					if (SelfTest)
						Fail ("Startup requested shutdown", "");
					return;
				}
				if (SelfTest)
				{
					Timer t = new Timer();
					t.Interval = 2000;
					t.Tick += (s, e) => { t.Stop(); core.RunSelfTest(); };
					t.Start();
				}
				Application.Run();
			}
			catch (Exception ex)
			{
				if (SelfTest)
					Fail ("Unhandled exception", ex.ToString());
				Application.Run (new ErrorReport (ex));
			}
		}

		public static void Log (string line)
		{
			if (selfTestLog != null)
				File.AppendAllText (selfTestLog, line + Environment.NewLine);
		}

		public static void Fail (string message, string detail)
		{
			Log ("FAIL: " + message + Environment.NewLine + detail);
			Environment.Exit (1);
		}

		public static void Pass()
		{
			Log ("PASS");
			Environment.Exit (0);
		}
	}
}
