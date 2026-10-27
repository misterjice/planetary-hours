# ChronosXP 4.1 for modern Windows

ChronosXP (planetary hours tray app, © 2003-2009 Robert Misiak, GPL v2), ported so it builds and runs
on current Windows 10/11 without the old installer or .NET Framework 3.5.

## Download / run

1. Go to the **Actions** tab, open the latest green **Build** run, and download the `ChronosXP-win` artifact.
2. Unzip it anywhere (e.g. `C:\Users\<you>\ChronosXP`). Keep all the files together.
3. Run `ChronosXP.exe`. It lives in the system tray. Alt+F11 opens the calendar.

No installer and no admin rights are needed. Settings are stored under
`HKEY_CURRENT_USER\SOFTWARE\Robert Misiak\ChronosXP`.

## What changed from the original 4.1 source

- **Target framework:** .NET Framework 3.5 → 4.8 (built into Windows 10 1903+ and Windows 11).
- **Registry defaults:** the app used to read its defaults and city list from `HKLM\SOFTWARE\Robert Misiak\ChronosXP`,
  which only the old MSI created. Without it, startup failed with "Failed reading configuration from registry.
  Please re-install ChronosXP." It now falls back to the 32-bit registry view, and failing that seeds the same
  defaults under `HKCU\...\ChronosXP\Defaults`.
- **Startup host:** removed the WPF `App.xaml` wrapper; the app is now a plain WinForms process (`Program.cs`).
- **Tray balloons:** now use `NotifyIcon.ShowBalloonTip` instead of reflection into framework internals.
- **Calculation fix:** latitude/longitude minutes and half-hour time-zone offsets were truncated by integer
  division (e.g. 36°11'N was used as 36°N), skewing sunrise/sunset and therefore the planetary hours.
- **Build:** SDK-style projects (`dotnet build ChronosXP.sln -c Release`), AnyCPU, source converted to UTF-8.
  The Visual Studio 2008 setup projects were removed.

## Building

Requires the .NET SDK 8+ (on Windows, or any OS):

```
dotnet build ChronosXP.sln -c Release
```

Output: `ChronosXP/bin/Release/net48/`.
