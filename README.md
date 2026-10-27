# ChronosXP 4.1 for modern Windows

ChronosXP (planetary hours tray app, © 2003-2009 Robert Misiak, GPL v2), ported so it builds and runs
on current Windows 10/11 without the old installer or .NET Framework 3.5.

## Download / install

Get the latest from the **Releases** page:

- `ChronosXP-<version>-x64-setup.exe`: installer for 64-bit Windows (almost every PC today).
- `ChronosXP-<version>-x86-setup.exe`: installer for 32-bit Windows.
- `*-portable.zip`: no install; unzip anywhere, keep the files together, run `ChronosXP.exe`.

ChronosXP lives in the system tray. Alt+F11 opens the calendar. Settings and your saved places are stored
per user under `HKEY_CURRENT_USER\SOFTWARE\Robert Misiak\ChronosXP`.

### Places

Any city can be used: Properties > Location takes a name plus latitude/longitude in degrees and minutes
(the "atlas" link opens astro.com to look them up), and the calendar's locality dialog adds places with a
time zone. Places are saved in the registry and survive upgrades.

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
- **Places:** a saved default place or locality missing from the places list no longer crashes startup
  (falls back to the first known place); user places load even if the installer's list is absent.
- **Links** open in the default browser instead of Internet Explorer.
- **Installers:** Inno Setup x86/x64 installers built by CI (`installer/ChronosXP.iss`) and published as releases.
- **Build:** SDK-style projects (`dotnet build ChronosXP.sln -c Release`), AnyCPU, source converted to UTF-8.
  The Visual Studio 2008 setup projects were removed.

## Building

Requires the .NET SDK 8+ (on Windows, or any OS):

```
dotnet build ChronosXP.sln -c Release
```

Output: `ChronosXP/bin/Release/net48/`.
