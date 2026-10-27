# ChronosXP - Makefile.pl (for ChronosXPfx.dll)
# Copyright (c) 2005 by Robert Misiak <rmisiak@users.sourceforge.net>
# PO Box 70972, Las Vegas, NV 89170, United States of America
#
# This program is free software; you can redistribute it and/or
# modify it under the terms of the GNU General Public License
# as published by the Free Software Foundation; either version 2
# of the License, or (at your option) any later version.
# 
# This program is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
# 
# You should have received a copy of the GNU General Public License
# along with this program; if not, write to the Free Software
# Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

use strict;

my $Compiler = "$ENV{'windir'}\\Microsoft.NET\\Framework64\\v2.0.50727\\csc.exe";
my $Output = "ChronosXPfx.dll";
my $Config = "Debug";
my @Defines = ("WINDOWS", "TRACE");
my @References = ();
my $BinDir = "bin";

unless (-e $Compiler) {
	print "FATAL ERROR: Either the Microsoft .NET Framework SDK 1.1 is not installed,\r\n";
	print "or it is installed in a nonstandard place.  You can download the SDK from\r\n";
	print "http://download.microsoft.com, open the page and do a search on '.NET SDK 1.1'\r\n";
	exit 1;
}

foreach my $arg (@ARGV) {
	if ($arg eq '/debug') {
	  $Config = "Debug";
	  push(@Defines, "DEBUG");
	} elsif ($arg eq '/release') {
	  $Config = "Release";
	  push(@Defines, "USEKEY");
	} elsif ($arg eq '/beta') {
		push(@Defines, "BETA");
	} else {
	  print "Unrecognized argument: $arg\r\n";
	  exit 1;
	}
}

my $OutDir = "$BinDir\\$Config";

print "** ** Making: $OutDir\\$Output\r\n";

mkdir $BinDir unless -d $BinDir;
mkdir $OutDir unless -d $OutDir;

my $BuildArgs = "/target:library /platform:x64 /out:${OutDir}\\$Output /debug ";
$BuildArgs .= "/define:$_ " foreach @Defines;
$BuildArgs .= "/reference:$_ " foreach @References;

opendir(DIRH, ".") || die "opendir .: $!";
foreach (readdir DIRH) {
 	$BuildArgs .= "/resource:$_ " if m/\.ico$/i || m/\.gif$/i;
}
$BuildArgs .= "*.cs";

Execute("$Compiler $BuildArgs");

sub Execute {
	my $CommandLine = shift;
	print ">> >> $CommandLine\r\n";
	system($CommandLine);
}
