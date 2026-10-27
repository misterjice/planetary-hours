; Inno Setup script for ChronosXP.  Build with:
;   iscc /DArch=x64 /DAppVersion=4.1.x /DSourceDir=<build output> installer\ChronosXP.iss
;   iscc /DArch=x86 ...
#ifndef Arch
  #define Arch "x64"
#endif
#ifndef AppVersion
  #define AppVersion "4.1"
#endif
#ifndef SourceDir
  #define SourceDir "..\ChronosXP\bin\Release\net48"
#endif

[Setup]
AppId={{8C1B5F0E-4E0B-4C8E-9C61-6B2A4E1D7C41}
AppName=ChronosXP
AppVersion={#AppVersion}
AppPublisher=Robert Misiak (ported for current Windows)
AppPublisherURL=https://github.com/misterjice/planetary-hours
DefaultDirName={autopf}\ChronosXP
DefaultGroupName=ChronosXP
UninstallDisplayIcon={app}\ChronosXP.exe
LicenseFile={#SourceDir}\License.rtf
OutputDir=output
OutputBaseFilename=ChronosXP-{#AppVersion}-{#Arch}-setup
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
; Program Files needs admin; the app itself keeps all settings per user under HKCU.
PrivilegesRequired=admin
CloseApplications=force
#if Arch == "x64"
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
#endif

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
Source: "{#SourceDir}\*"; DestDir: "{app}"; Excludes: "*.pdb"; Flags: ignoreversion recursesubdirs

[Icons]
Name: "{group}\ChronosXP"; Filename: "{app}\ChronosXP.exe"
Name: "{group}\Read Me"; Filename: "{app}\Read Me.rtf"
Name: "{group}\Uninstall ChronosXP"; Filename: "{uninstallexe}"
Name: "{autodesktop}\ChronosXP"; Filename: "{app}\ChronosXP.exe"; Tasks: desktopicon

[Run]
Filename: "{app}\ChronosXP.exe"; Description: "{cm:LaunchProgram,ChronosXP}"; Flags: nowait postinstall skipifsilent runasoriginaluser

[UninstallRun]
Filename: "{sys}\taskkill.exe"; Parameters: "/f /im ChronosXP.exe"; Flags: runhidden; RunOnceId: "KillChronosXP"
