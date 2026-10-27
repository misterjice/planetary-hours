# Installs a ChronosXP setup.exe silently and runs ChronosXP.exe /selftest under several registry
# scenarios. Exits non-zero if any scenario fails, hangs, or computes implausible hours.
param(
    [Parameter(Mandatory)] [string] $Installer,
    [Parameter(Mandatory)] [string] $Arch,
    [string] $ScreenshotDir = "smoke-screenshots"
)
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms, System.Drawing

$installDir = Join-Path $env:RUNNER_TEMP "ChronosXP-$Arch"
$userKey = 'HKCU:\SOFTWARE\Robert Misiak'
$hklmKey = 'HKLM:\SOFTWARE\WOW6432Node\Robert Misiak'
$log = Join-Path ([IO.Path]::GetTempPath()) 'ChronosXP-selftest.log'
New-Item -ItemType Directory -Force $ScreenshotDir | Out-Null

# Times are checked for Waco, TX, so run in US Central time.
tzutil /s "Central Standard Time"

Write-Host "Installing $Installer -> $installDir"
$p = Start-Process $Installer -ArgumentList '/VERYSILENT', '/SUPPRESSMSGBOXES', '/NORESTART', '/CURRENTUSER', "/DIR=$installDir" -Wait -PassThru
if ($p.ExitCode) { throw "Installer exited with $($p.ExitCode)" }
$exe = Join-Path $installDir 'ChronosXP.exe'
if (-not (Test-Path $exe)) { throw "Installer did not create $exe" }

function Reset-Registry {
    Remove-Item $userKey -Recurse -Force -ErrorAction SilentlyContinue
    Remove-Item $hklmKey -Recurse -Force -ErrorAction SilentlyContinue
}

function Add-UserPlace($name, $long, $lat) {
    $k = "$userKey\ChronosXP\Places\$name"
    New-Item $k -Force | Out-Null
    Set-ItemProperty $k Longitude $long
    Set-ItemProperty $k Latitude $lat
}

function Save-Screenshot($name) {
    $b = [System.Windows.Forms.Screen]::PrimaryScreen.Bounds
    $bmp = New-Object System.Drawing.Bitmap $b.Width, $b.Height
    $g = [System.Drawing.Graphics]::FromImage($bmp)
    $g.CopyFromScreen($b.Location, [System.Drawing.Point]::Empty, $b.Size)
    $bmp.Save((Join-Path $ScreenshotDir "$Arch-$name.png"))
    $g.Dispose(); $bmp.Dispose()
}

$scenarios = [ordered]@{
    # Nothing in the registry: first run on a clean machine.
    'fresh' = { }
    # Settings left by an old install whose default place is not in any places list.
    'stale-default-place' = {
        New-Item "$userKey\ChronosXP" -Force | Out-Null
        Set-ItemProperty "$userKey\ChronosXP" Run 33 -Type DWord
        Set-ItemProperty "$userKey\ChronosXP" 'Default Place' 'Nowhere, XX'
        Set-ItemProperty "$userKey\ChronosXP" 'Default Locality' 'Also Nowhere, XX'
    }
    # A user-entered city with coordinates as the default place.
    'custom-city' = {
        Add-UserPlace 'Waco, TX' '97W09' '31N33'
        Set-ItemProperty "$userKey\ChronosXP" 'Default Place' 'Waco, TX'
        Set-ItemProperty "$userKey\ChronosXP" Run 5 -Type DWord
    }
    # An old 32-bit install's HKLM key without a Places list, plus the user's own city.
    'legacy-hklm-no-places' = {
        New-Item "$hklmKey\ChronosXP" -Force | Out-Null
        Set-ItemProperty "$hklmKey\ChronosXP" 'Default Place' 'Las Vegas, NV'
        Set-ItemProperty "$hklmKey\ChronosXP" Culture '*'
        Add-UserPlace 'Waco, TX' '97W09' '31N33'
        Set-ItemProperty "$userKey\ChronosXP" 'Default Place' 'Waco, TX'
        Set-ItemProperty "$userKey\ChronosXP" Run 12 -Type DWord
    }
}

$failed = @()
foreach ($name in $scenarios.Keys) {
    Write-Host "`n=== $Arch / $name ==="
    Reset-Registry
    & $scenarios[$name]
    Remove-Item $log -ErrorAction SilentlyContinue

    $proc = Start-Process $exe -ArgumentList '/selftest' -PassThru
    if (-not $proc.WaitForExit(90000)) {
        Save-Screenshot $name
        Stop-Process $proc -Force
        Write-Host "TIMEOUT: ChronosXP did not finish (likely a dialog is open; see screenshot)"
        if (Test-Path $log) { Get-Content $log }
        $failed += $name
        continue
    }
    $text = if (Test-Path $log) { Get-Content $log -Raw } else { '' }
    Write-Host $text
    $ok = ($proc.ExitCode -eq 0) -and ($text -match 'PASS')

    if ($ok -and $text -match 'Waco') {
        # Waco sunrise ranges from about 06:25 to 07:50 local time over the year.
        if ($text -match 'Sunrise=\d{4}-\d\d-\d\d (\d\d):(\d\d)') {
            $mins = [int]$Matches[1] * 60 + [int]$Matches[2]
            if ($mins -lt 360 -or $mins -gt 480) { Write-Host "Implausible Waco sunrise"; $ok = $false }
        } else { $ok = $false }
    }
    if ($name -ne 'fresh' -and $name -ne 'stale-default-place' -and $text -notmatch 'DefaultPlace=\{Waco, TX') {
        Write-Host "Expected Waco, TX as the default place"; $ok = $false
    }
    if (-not $ok) { Write-Host "FAILED (exit code $($proc.ExitCode))"; $failed += $name } else { Write-Host "passed" }
}

Reset-Registry
if ($failed) { throw "Smoke test failed for ${Arch}: $($failed -join ', ')" }
Write-Host "`nAll $Arch scenarios passed."
