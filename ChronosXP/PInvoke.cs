#region Copyright © 2004-2007 by Robert Misiak
// ChronosXP - PInvoke.cs
// Copyright © 2004-2007 by Robert Misiak <rmisiak@users.sourceforge.net>
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
using System.Text;
using System.Runtime.InteropServices;

namespace ChronosXP
{
	internal sealed class PInvoke
	{
		public const string Copyright = "PInvoke.cs, Copyright © 2004-2007 by Robert Misiak";

		#region SetTimeZoneInformation - kernel32.dll
		[DllImport ("kernel32.dll", SetLastError=true)]
		public static extern bool SetTimeZoneInformation (ref TimeZoneInformation172 TZI);

		[StructLayout (LayoutKind.Sequential)]
		public struct SystemTime
		{
			public short wYear;
			public short wMonth;
			public short wDayOfWeek;
			public short wDay;
			public short wHour;
			public short wMinute;
			public short wSecond;
			public short wMilliseconds;
			public static SystemTime FromDateTime (DateTime src)
			{
				SystemTime st = new SystemTime();
				st.wYear = (short)src.Year;  st.wMonth = (short)src.Month;  st.wDay = (short)src.Day;
				st.wHour = (short)src.Hour;  st.wMinute = (short)src.Minute;  st.wSecond = (short)src.Second;
				st.wMilliseconds = (short)src.Millisecond;
				return st;
			}
		}
		// 44-byte TIME_ZONE_INFORMATION structure (for registry keys under HKLM\SOFTWARE\Microsoft\Windows NT\Time Zones)
		[StructLayout (LayoutKind.Sequential)]
		public struct TimeZoneInformation44
		{
			public Int32 Bias;
			public Int32 StandardBias;
			public Int32 DaylightBias;
			public SystemTime StandardDate;
			public SystemTime DaylightDate;
		}
		// 172-byte TIME_ZONE_INFORMATION structure (for kernel32.dll's SetTimeZoneInformation)
		[StructLayout (LayoutKind.Sequential, CharSet=CharSet.Unicode)]
		public struct TimeZoneInformation172
		{
			public Int32 Bias;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=32)]
			public string StandardName;
			public SystemTime StandardDate;
			public Int32 StandardBias;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=32)]
			public string DaylightName;
			public SystemTime DaylightDate;
			public Int32 DaylightBias;
		}
		#endregion

		#region PlaySound - winmm.dll
		// Used to (optionally) play a sound when the planetary hour changes - see Core.cs
		[DllImport ("winmm.dll", EntryPoint="PlaySound", CharSet=CharSet.Auto)]
		public static extern Int32 PlaySound (String pszSound, Int32 hmod, Int32 flags);

		public const Int32 SND_SYNC = 0x0;
		public const Int32 SND_ASYNC = 0x1;
		public const Int32 SND_NODEFAULT = 0x2;
		public const Int32 SND_MEMORY = 0x4;
		public const Int32 SND_LOOP = 0x8;
		public const Int32 SND_NOSTOP = 0x10;
		public const Int32 SND_NOWAIT = 0x2000;
		public const Int32 SND_ALIAS = 0x10000;
		public const Int32 SND_ALIAS_ID = 0x110000;
		public const Int32 SND_FILENAME = 0x20000;
		public const Int32 SND_RESOURCE = 0x40004;
		#endregion

		#region Shell_NotifyIconA - shell32.dll
		// Used for Balloon window tooltips
		[DllImport ("shell32.dll")]
		public static extern Int32 Shell_NotifyIconA(Int32 dwMessage, ref NotifyIconData pnid); 

		[StructLayout (LayoutKind.Sequential)]
		public struct NotifyIconData
		{
			public Int32 cbSize;
			public IntPtr hwnd;
			public Int32 uID;
			public Int32 uFlags;
			public IntPtr uCallbackMessage;
			public IntPtr hIcon;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=128)]
			public string szTip;
			public Int32 dwState;
			public Int32 dwStateMask;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=256)]
			public string szInfo;
			public Int32 uTimeout;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=64)]
			public string szInfoTitle;
			public Int32 dwInfoFlags;
		}

		public const Int32 NIF_MESSAGE = 0x1;
		public const Int32 NIF_ICON = 0x2;
		public const Int32 NIF_TIP = 0x4;
		public const Int32 NIF_STATE = 0x8;
		public const Int32 NIF_INFO = 0x10;
		public const Int32 NIM_ADD = 0x0;
		public const Int32 NIM_MODIFY = 0x1;
		public const Int32 NIM_DELETE = 0x2;
		public const Int32 NIM_SETVERSION = 0x4;
		public const Int32 NIM_SETFOCUS = 0x3;
		public const Int32 NOTIFYICON_VERSION = 0x5;
		public const Int32 NIIF_NONE = 0x0;
		public const Int32 NIIF_INFO = 0x1;
		public const Int32 NIIF_ERROR = 0x3;
		public const Int32 NIIF_NOSOUND = 0x10;
		public const Int32 NIS_SHAREDICON = 0x2;
		#endregion

		#region SetWindowsHookEx, RegisterHotKey, SetWindowPos, etc. - user32.dll
		// help from http://www.codeproject.com/csharp/globalhook.asp 
		public const Int32 WH_KEYBOARD_LL = 13;
		public const Int32 WH_MOUSE_LL = 14;
		public const Int32 SW_SHOWNOACTIVATE = 4;
		public const Int32 SW_SHOWNA = 8;
		public const Int32 HC_ACTION = 0;
		public const Int32 VK_SHIFT = 0x10;
		public const Int32 VK_CONTROL = 0x11;
		public const Int32 VK_F11 = 0x7A;
		public const Int32 WM_KEYDOWN = 0x0100;
		public const Int32 WM_KEYUP = 0x0101;
		public const Int32 WM_HOTKEY = 0x0312;
		public const Int32 WM_MOUSEMOVE = 0x0200;
		public const Int32 WM_LBUTTONDOWN = 0x0201;
		public const Int32 WM_LBUTTONUP = 0x0202;
		public const Int32 MOD_ALT = 0x0001;
		public const Int32 MOD_CONTROL = 0x0002;
		public const Int32 MOD_SHIFT = 0x0004;
		public const Int32 MOD_WIN = 0x0008;
		public const Int32 HWND_TOPMOST = -1;
		public const Int32 SWP_NOACTIVATE = 0x0010;
		public delegate Int32 HookProc (Int32 nCode, Int32 wParam, IntPtr lParam);

		[StructLayout (LayoutKind.Sequential)]
		public struct KbdLLHookStruct
		{
			public uint vkCode;
			public uint scanCode;
			public uint flags;
			public uint time;
			public ulong dwExtraInfo;
		}

		[DllImport ("user32.dll")]
		public static extern Int32 SetWindowsHookEx (Int32 idHook, HookProc lpfn, IntPtr hMod, uint dwThreadId);

		[DllImport ("user32.dll")]
		public static extern Int32 CallNextHookEx (Int32 idHook, Int32 nCode, Int32 wParam, IntPtr lParam);

		[DllImport ("user32.dll")]
		public static extern bool UnhookWindowsHookEx (Int32 idHook);

		[DllImport ("user32.dll")]
		public static extern bool ShowWindow (IntPtr hWnd, Int32 flags);

		[DllImport ("user32.dll")]
		public static extern bool SetWindowPos (IntPtr hWnd, Int32 hWndInsertAfter, Int32 X, Int32 Y, Int32 cx, Int32 cy, uint uFlags);

		[DllImport ("user32.dll")]
		public static extern bool RegisterHotKey (IntPtr hWnd, Int32 id, uint fsModifiers, uint vk);

		[DllImport ("user32.dll")]
		public static extern bool UnregisterHotKey (IntPtr hWnd, Int32 id);
		#endregion

		#region XP Visual Styles - UxTheme.dll
		// I (Robert Misiak) only have the old version of VS.NET - V 2002.  This version has no support for XP Visual Styles.  You can get around
		// this by creating a .manifest file telling the system to use comctl32.dll version 6.0 (the XP version) if it is available - and then setting the
		// FlatStyle on various controls to System.  There are problems with this - namely GroupBox'es and CheckBox'es cannot have transparent
		// backgrounds.  So I've implemented my own here.  I've read that VS.NET 2003 has some (although apparantly buggy) support for
		// visual styles using Application.EnableVisualStyles() - I'm not sure whether or not it requires FlatStyle.System on visual controls, in which
		// case transparent backgrounds probably wouldn't be supported.  If this is the case, our P/Invoke UxTheme.dll code should probably work
		// there, too.  I've done some work with the VS 2005 Beta which is *great* and none of this will be required for it.  I'm not sure when VS 2005
		// will actually ship though so I'll include this for now.
		//
		// Some good resouces for Visual Style programming:
		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnwxp/html/xptheming.asp
		// http://www.codeproject.com/cs/miscctrl/themedtabpage.asp
		// http://www.csharphelp.com/archives2/archive319.html

		[StructLayout (LayoutKind.Sequential)]
		public struct Rect
		{
			public Int32 Left;
			public Int32 Top;
			public Int32 Right;
			public Int32 Bottom;

			public Rect (System.Drawing.Rectangle bounds)
			{
				Left = bounds.Left;
				Top = bounds.Top;
				Right = bounds.Right;
				Bottom = bounds.Bottom;
			}

			public Rect (System.Drawing.RectangleF bounds)
			{
				Left = (int) Math.Floor (bounds.Left);
				Top = (int) Math.Floor (bounds.Top);
				Right = (int) Math.Ceiling (bounds.Right);
				Bottom = (int) Math.Ceiling (bounds.Bottom);
			}

			public Rect (Int32 left, Int32 top, Int32 right, Int32 bottom)
			{
				Left = left;
				Top = top;
				Right = right;
				Bottom = bottom;
			}
		}

		[StructLayout (LayoutKind.Sequential)]
		public struct ColorRef
		{
			public byte r, g, b;
			private byte unused;
			public System.Drawing.Color ToColor() { return System.Drawing.Color.FromArgb(r, g, b); }
		}

		// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/shellcc/platform/commctls/userex/topics/partsandstates.asp
		public const Int32 TMT_TEXTCOLOR = 3803;
		public const Int32 BP_PUSHBUTTON = 1;
		public const Int32 BP_RADIOBUTTON = 2;
		public const Int32 BP_CHECKBOX = 3;
		public const Int32 BP_GROUPBOX = 4;
		public const Int32 BP_USERBUTTON = 5;
		public const Int32 RP_BAND = 3;
		public const Int32 GBS_NORMAL = 1;
		public const Int32 GBS_DISABLED = 2;
		public const Int32 RBS_UNCHECKEDNORMAL = 1;
		public const Int32 RBS_UNCHECKEDHOT = 2;
		public const Int32 RBS_UNCHECKEDPRESSED = 3;
		public const Int32 RBS_UNCHECKEDDISABLED = 4;
		public const Int32 RBS_CHECKEDNORMAL = 5;
		public const Int32 RBS_CHECKEDHOT = 6;
		public const Int32 RBS_CHECKEDPRESSED = 7;
		public const Int32 RBS_CHECKEDDISABLED = 8;
		public const Int32 CBS_NORMAL = 1;
		public const Int32 CBS_HOT = 2;
		public const Int32 CBS_PUSHED = 3;
		public const Int32 CBS_UNCHECKEDNORMAL = 1;
		public const Int32 CBS_UNCHECKEDHOT = 2;
		public const Int32 CBS_UNCHECKEDPRESSED = 3;
		public const Int32 CBS_UNCHECKEDDISABLED = 4;
		public const Int32 CBS_CHECKEDNORMAL = 5;
		public const Int32 CBS_CHECKEDHOT = 6;
		public const Int32 CBS_CHECKEDPRESSED = 7;
		public const Int32 CBS_CHECKEDDISABLED = 8;
		public const Int32 CBS_MIXEDNORMAL = 9;
		public const Int32 CBS_MIXEDHOT = 10;
		public const Int32 CBS_MIXEDPRESSED = 11;
		public const Int32 CBS_MIXEDDISABLED = 12;
		public const Int32 TABP_TABITEM = 1;
		public const Int32 TABP_TABITEMLEFTEDGE = 2;
		public const Int32 TABP_TABITEMRIGHTEDGE = 3;
		public const Int32 TABP_TABITEMBOTHEDGE = 4;
		public const Int32 TABP_TOPTABITEM = 5;
		public const Int32 TABP_TOPTABITEMLEFTEDGE = 6;
		public const Int32 TABP_TOPTABITEMRIGHTEDGE = 7;
		public const Int32 TABP_TOPTABITEMBOTHEDGE = 8;
		public const Int32 TABP_PANE = 9;
		public const Int32 TABP_BODY = 10;
		public const Int32 WP_CLOSEBUTTON = 18;
		public const Int32 WP_SMALLFRAMELEFT = 10;
		public const Int32 WP_SMALLFRAMERIGHT = 11;
		public const Int32 WP_SMALLFRAMEBOTTOM = 12;
		public const Int32 FS_ACTIVE = 1;
		public const Int32 FS_INACTIVE = 2;
		public const Int32 CP_DROPDOWNBUTTON = 1;
		public const Int32 CBXS_NORMAL = 1;
		public const Int32 CBXS_HOT = 2;
		public const Int32 CBXS_PRESSED = 3;

		[DllImport ("UxTheme.dll")]
		public static extern IntPtr OpenThemeData (IntPtr hWnd, [MarshalAs (UnmanagedType.LPTStr)] string classList);

		[DllImport ("UxTheme.dll")]
		public static extern void CloseThemeData (IntPtr hTheme);

		[DllImport ("UxTheme.dll")]
		public static extern void DrawThemeBackground (IntPtr hTheme, IntPtr hDC, Int32 partId, Int32 stateId, ref Rect rect, ref Rect clipRect);

		[DllImport ("UxTheme.dll")]
		public static extern void DrawThemeParentBackground (IntPtr hWnd, IntPtr hDC, ref Rect rect);

		[DllImport ("UxTheme.dll")]
		public static extern void GetThemeColor (IntPtr hTheme, Int32 partId, Int32 stateId, Int32 propId, ref ColorRef pColor);

		[DllImport ("UxTheme.dll")]
		public static extern Int32 IsThemeActive();
		
		[DllImport ("UxTheme.dll")]
		public static extern Int32 GetCurrentThemeName([MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszThemeFileName, int dwMaxNameChars,
		                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszColorBuff, int cchMaxColorChars,
		                                               [MarshalAs(UnmanagedType.LPWStr)] StringBuilder pszSizeBuff, int cchMaxSizeChars);
		
		// VisualStylesEnabled should always be called first, instead of directly calling IsThemeActive(), to retain compatibility with old versions of
		// Windows - UxTheme.dll is only shipped with Windows XP or higher.		
		public static bool VisualStylesEnabled()
		{
		#if WINDOWS
			try
			{
				if (IsThemeActive() == 1)
					return true;
				else
					return false;
			}
			catch (System.DllNotFoundException)
			{
				return false;
			}
		#else
			return false;
		#endif
		}
		#endregion

		#region OS Version - kernel32.dll
		[StructLayout (LayoutKind.Sequential)]
		public struct OSVersionInfo
		{
			public Int32 dwOSVersionInfoSize;
			public Int32 dwMajorVersion;
			public Int32 dwMinorVersion;
			public Int32 dwBuildNumber;
			public Int32 dwPlatformId;
			[MarshalAs (UnmanagedType.ByValTStr, SizeConst=128)]
			public string szCSDVersion;
		}
		
		[DllImport ("kernel32.dll")]
		public static extern short GetVersionEx (ref OSVersionInfo o);
		#endregion

        #region Windows Vista
        [StructLayout(LayoutKind.Sequential)]
        public struct Margins
        {
            public int cxLeftWidth;
            public int cxRightWidth;
            public int cyTopHeight;
            public int cyBottomHeight;
        }

        [DllImport("DwmApi.dll", PreserveSig = false)]
        public static extern bool DwmIsCompositionEnabled();

        [DllImport("DwmApi.dll", PreserveSig = false)]
        public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref Margins pMarInset);

        public static bool WindowsVistaGlass()
        {
            try
            {
                return DwmIsCompositionEnabled();
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
