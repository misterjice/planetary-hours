#region Copyright  2004-2005 by Robert Misiak
// ChronosXP - UxTheme.cs
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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

// UxTheme.cs:  XP Visual Style controls.  These (especially XPNumericUpDown) should be used in conjunction with a .manifest file that
// specifies comctl32.dll version 6.0, so that other controls - like ComboBox - will be in XP Visual Style.  Controls implemented here are
// replacements for those that are not modified, or that don't support full functionality, when using comctl32.dll v6.0.

namespace ChronosXP
{
	#region XPGroupBox
	[ToolboxItem (true)]
	public class XPGroupBox : System.Windows.Forms.GroupBox
	{
		public const string Copyright = "UxTheme.cs, Copyright  2004-2005 by Robert Misiak";

		public XPGroupBox()
		{
			SetStyle (ControlStyles.UserPaint, true);
			SetStyle (ControlStyles.AllPaintingInWmPaint, true);
			SetStyle (ControlStyles.OptimizedDoubleBuffer, true);
		}

		protected override void OnPaint (PaintEventArgs e)
		{
			if (!PInvoke.VisualStylesEnabled())
			{
				base.OnPaint(e);
				return;
			}
			IntPtr hTheme = PInvoke.OpenThemeData(Handle, "Button");
			if (hTheme == IntPtr.Zero)
			{
				base.OnPaint(e);
				return;
			}
			SizeF sz;
			// Text needs to be measured here because e.Graphics will soon be tied up with GetHdc()
			if (Text != null && Text.Length > 0)
				sz = e.Graphics.MeasureString (Text, Font, DisplayRectangle.Width - 17, StringFormat.GenericDefault);
			else
				sz = new SizeF();  // have to set sz or else compiler will complain
			// You'd think the height should be (Height - (Font.Height/2)), but using full Height gives a size consistent with FlatStyle.System XP GroupBox'es
			PInvoke.Rect rBounds = new PInvoke.Rect (0, Font.Height / 2, Width, Height);
			PInvoke.Rect rClip = new PInvoke.Rect (e.Graphics.VisibleClipBounds);
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.DrawThemeBackground (hTheme, hDC, PInvoke.BP_GROUPBOX, (Enabled ? PInvoke.GBS_NORMAL : PInvoke.GBS_DISABLED),
			                             ref rBounds, ref rClip);
			if (Text != null && Text.Length > 0)
			{
				RectangleF rText = new RectangleF (7, 0, sz.Width + 3.2F, sz.Height);
				PInvoke.Rect wrText = new PInvoke.Rect (rText);
				// Redraw the background over the border where text will be displayed
				if (BackColor == Color.Transparent)
				{
					PInvoke.DrawThemeParentBackground(Handle, hDC, ref wrText);
					e.Graphics.ReleaseHdc(hDC);
				}
				else
				{
					e.Graphics.ReleaseHdc(hDC);
					using (SolidBrush sb = new SolidBrush (BackColor))
						e.Graphics.FillRectangle (sb, rText);
				}
				Color textColor;
				if (Enabled)
				{
					PInvoke.ColorRef rColor = new PInvoke.ColorRef();
					PInvoke.GetThemeColor (hTheme, PInvoke.BP_GROUPBOX, PInvoke.GBS_NORMAL, PInvoke.TMT_TEXTCOLOR, ref rColor);
					textColor = rColor.ToColor();
				}
				else  // GetThemeColor (... GBS_DISABLED ...)  always returns the default color - so use GrayText
					textColor = SystemColors.GrayText;
				rText.Inflate(-0.8F, 0);  rText.Offset(0.4F, 0);
				using (SolidBrush sb = new SolidBrush(textColor))
					e.Graphics.DrawString(Text, Font, sb, rText, StringFormat.GenericDefault);
			}
			else
				e.Graphics.ReleaseHdc(hDC);
			PInvoke.CloseThemeData(hTheme);
		}

		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (PInvoke.VisualStylesEnabled() && BackColor == Color.Transparent)
			{
				PInvoke.Rect rFull = new PInvoke.Rect(0, 0, Width, Height);
				IntPtr hDC = e.Graphics.GetHdc();
				PInvoke.DrawThemeParentBackground(Handle, hDC, ref rFull);
				e.Graphics.ReleaseHdc(hDC);
			}
			else
			{
				Color useBackColor = BackColor;
				// If BackColor == Transparent, we must figure out the next Parent BackColor that isn't transparent and paint it ourself, since
				// ControlStyles.UserPaint and ControlStyles.AllPaintingInWmPaint are set.
				if (BackColor == Color.Transparent)
				{
					try
					{
						for (Control c = Parent; true; c = c.Parent)
							if (c.BackColor != Color.Transparent)
							{
								useBackColor = c.BackColor;
								break;
							}
					}
					catch
					{
						useBackColor = SystemColors.Control;
					}
				}
				using (SolidBrush sb = new SolidBrush(useBackColor))
					e.Graphics.FillRectangle(sb, new Rectangle (0, 0, Width, Height));
			}
		}
	}
	#endregion

	#region XPCheckBox
	public class XPCheckBox : System.Windows.Forms.CheckBox
	{
		public enum CheckBoxState { Normal, Hot, Pressed, Disabled };
		public CheckBoxState State = CheckBoxState.Normal;
		[Browsable (true)] public bool UseFocusRectangle = true;
		protected bool pressed = false;

		protected override void OnPaint (System.Windows.Forms.PaintEventArgs e)
		{
			// Apparantly OnPaintBackground isn't used in CheckBox so we must do this here.
			if (BackColor == Color.Transparent && PInvoke.VisualStylesEnabled())
			{
				IntPtr bghDC = e.Graphics.GetHdc();
				PInvoke.Rect rBounds = new PInvoke.Rect (0, 0, Width, Height);
				PInvoke.DrawThemeParentBackground (Handle, bghDC, ref rBounds);
				e.Graphics.ReleaseHdc (bghDC);
			}
			else
			{
				using (SolidBrush sb = new SolidBrush (BackColor))
					e.Graphics.FillRectangle (sb, new Rectangle (0, 0, Width, Height));
			}

			if (!PInvoke.VisualStylesEnabled())
			{
				base.OnPaint(e);
				return;
			}

			IntPtr hTheme = PInvoke.OpenThemeData(Handle, "Button");
			if (hTheme == IntPtr.Zero)
			{
				base.OnPaint(e);
				return;
			}
			RectangleF leftF = new RectangleF(0, 0, 14, Height);
			RectangleF clipF = e.Graphics.VisibleClipBounds;
			PInvoke.Rect rLeft = new PInvoke.Rect(leftF);
			PInvoke.Rect rClip = /*rLeft;*/new PInvoke.Rect(clipF);
			Rectangle rRight = new Rectangle(15, 0, Width - 15, Height);
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.DrawThemeBackground (hTheme, hDC, PInvoke.BP_CHECKBOX, checkBoxState(), ref rLeft, ref rClip);
			PInvoke.CloseThemeData(hTheme);
			e.Graphics.ReleaseHdc(hDC);
			if (Text != null && Text.Length > 0)
			{
				using (StringFormat sf = stringFormatOf (TextAlign))
				using (SolidBrush sb = new SolidBrush (Enabled ? ForeColor : SystemColors.GrayText))
				{
					e.Graphics.DrawString (Text, Font, sb, rRight, sf);
					if (Focused && !pressed)
					{
						SizeF sz = e.Graphics.MeasureString (Text, Font, rRight.Width, sf);
						Rectangle rText = new Rectangle (15, 0, (int) Math.Ceiling (sz.Width), (int) Math.Ceiling (sz.Height));
						if (UseFocusRectangle)
							ControlPaint.DrawFocusRectangle (e.Graphics, rText);
					}
				}
			}
		}

		protected override void OnMouseMove (System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseMove (e);
			if (Enabled)
			{
				if (RectangleToScreen (DisplayRectangle).Contains (Cursor.Position))
				{
					if (pressed)
						State = CheckBoxState.Pressed;
					else
						State = CheckBoxState.Hot;
				}
				else
				{
					if (pressed)
						State = CheckBoxState.Hot;
					else
						State = CheckBoxState.Normal;
				}
			}
			else
				State = CheckBoxState.Disabled;
			Invalidate();
		}

		protected override void OnMouseLeave (System.EventArgs e)
		{
			base.OnMouseLeave (e);
			if (Enabled)
			{
				if (pressed)
					State = CheckBoxState.Hot;
				else
					State = CheckBoxState.Normal;
			}
			else
				State = CheckBoxState.Disabled;
			Invalidate();
		}

		protected override void OnMouseDown (System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseDown (e);
			if (Enabled && e.Button == MouseButtons.Left)
			{
				pressed = true;
				State = CheckBoxState.Pressed;
			}
			Invalidate();
		}

		protected override void OnMouseUp (System.Windows.Forms.MouseEventArgs e)
		{
			base.OnMouseUp (e);
			if (Enabled && DisplayRectangle.Contains (Cursor.Position))
				State = CheckBoxState.Hot;
			else
				State = Enabled ? CheckBoxState.Normal : CheckBoxState.Disabled;
			if (pressed)
				pressed = false;
			Parent.Focus();	// This is so that the focus rectangle wont be displayed after clicking the checkbox.
			Invalidate();
		}

		protected override void OnEnabledChanged (System.EventArgs e)
		{
			base.OnEnabledChanged (e);
			State = Enabled ? CheckBoxState.Normal : CheckBoxState.Disabled;
			Invalidate();
		}

		protected int checkBoxState()
		{
			if (Checked)
			{
				switch (State)
				{
					case CheckBoxState.Normal:
						return PInvoke.CBS_CHECKEDNORMAL;
					case CheckBoxState.Disabled:
						return PInvoke.CBS_CHECKEDDISABLED;
					case CheckBoxState.Hot:
						return PInvoke.CBS_CHECKEDHOT;
					case CheckBoxState.Pressed:
						return PInvoke.CBS_CHECKEDPRESSED;
				}
			}
			else
			{
				switch (State)
				{
					case CheckBoxState.Normal:
						return PInvoke.CBS_UNCHECKEDNORMAL;
					case CheckBoxState.Disabled:
						return PInvoke.CBS_UNCHECKEDDISABLED;
					case CheckBoxState.Hot:
						return PInvoke.CBS_UNCHECKEDHOT;
					case CheckBoxState.Pressed:
						return PInvoke.CBS_UNCHECKEDPRESSED;
				}
			}
			return -1;
		}

		protected StringFormat stringFormatOf (ContentAlignment alignment) 
		{
			StringFormat sf = new StringFormat();
 
			switch (alignment)
			{
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomRight:
					sf.LineAlignment = StringAlignment.Far; 
					break;

				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.MiddleCenter:
					sf.LineAlignment = StringAlignment.Center; 
					break;

				case ContentAlignment.TopCenter:
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopRight:
					sf.LineAlignment = StringAlignment.Near; 
					break;
			}

			switch (alignment)
			{
				case ContentAlignment.BottomCenter:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.TopCenter:
					sf.Alignment = StringAlignment.Center;
					break;

				case ContentAlignment.BottomLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.TopLeft:
					sf.Alignment = StringAlignment.Near;
					break;

				case ContentAlignment.BottomRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.TopRight:
					sf.Alignment = StringAlignment.Far;
					break;
			}

			return sf;
		}
	}
	#endregion

	// The only differences between XPLinkLabel, XPLabel, XPPanel and XPTabPage and the originals are that the replacements call
	// DrawThemeParentBackground when BackColor is Transparent.

	#region XPLinkLabel
	public class XPLinkLabel : System.Windows.Forms.LinkLabel
	{
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (!PInvoke.VisualStylesEnabled() || BackColor != Color.Transparent)
			{
				base.OnPaintBackground (e);
				return;
			}
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.Rect rBounds = new PInvoke.Rect (0, 0, Width, Height);
			PInvoke.DrawThemeParentBackground (Handle, hDC, ref rBounds);
			e.Graphics.ReleaseHdc (hDC);
		}

		protected override void OnPaint (PaintEventArgs e)
		{
			base.OnPaint (e);
			if (!Enabled)
			{
				// When disabled, LinkLabel doesn't draw any of the link text.  Ideally I would check TextAlign here and create an appropriate
				// StringFormat, but I know how it is aligned within ChronosXP and will draw appropriately.
				using (StringFormat sf = new StringFormat())
				{
					sf.Alignment = StringAlignment.Near;
					sf.LineAlignment = StringAlignment.Center;
					e.Graphics.DrawString (Text, Font, Brushes.Gray, new Rectangle (0, 0, Width, Height), sf);
				}
			}
		}
	}
	#endregion

	#region XPLabel
	public class XPLabel : System.Windows.Forms.Label
	{
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (!PInvoke.VisualStylesEnabled() || BackColor != Color.Transparent)
			{
				base.OnPaintBackground (e);
				return;
			}
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.Rect rBounds = new PInvoke.Rect (0, 0, Width, Height);
			PInvoke.DrawThemeParentBackground (Handle, hDC, ref rBounds);
			e.Graphics.ReleaseHdc (hDC);
		}
	}
	#endregion

	#region XPPanel
	public class XPPanel : System.Windows.Forms.Panel
	{
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (!PInvoke.VisualStylesEnabled() || BackColor != Color.Transparent)
			{
				base.OnPaintBackground (e);
				return;
			}
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.Rect rBounds = new PInvoke.Rect (0, 0, Width, Height);
			PInvoke.DrawThemeParentBackground (Handle, hDC, ref rBounds);
			e.Graphics.ReleaseHdc (hDC);
		}
	}
	#endregion

	#region XPTabPage
	public class XPTabPage : System.Windows.Forms.TabPage
	{
		protected override void OnPaintBackground (PaintEventArgs e)
		{
			if (!PInvoke.VisualStylesEnabled())
			{
				using (SolidBrush sb = new SolidBrush (BackColor))
					e.Graphics.FillRectangle (sb, DisplayRectangle);
				return;
			}

			IntPtr hTheme = PInvoke.OpenThemeData (Handle, "Tab");
			if (hTheme == IntPtr.Zero)
			{
				using (SolidBrush sb = new SolidBrush (BackColor))
					e.Graphics.FillRectangle (sb, DisplayRectangle);
				return;
			}

			PInvoke.Rect rDisplayRect = new PInvoke.Rect (DisplayRectangle);
			PInvoke.Rect rClipArea = new PInvoke.Rect (e.Graphics.VisibleClipBounds);
			IntPtr hDC = e.Graphics.GetHdc();
			PInvoke.DrawThemeBackground (hTheme, hDC, PInvoke.TABP_BODY, 0, ref rDisplayRect, ref rClipArea);
			e.Graphics.ReleaseHdc (hDC);
			PInvoke.CloseThemeData (hTheme);
		}
	}
	#endregion

	// XPNumericUpDown probably isn't the best name for this Control since it doesn't support all of the functionality of NumericUpDown, and
	// it doesn't call anything from UxTheme.dll itself either.  It's esentially a TextBox override with a VScrollBar that does allow number editing.
	// Using a .manifest gives it the XP Visual Style, which is why nothing from UxTheme.dll needs to be called.

	#region XPNumericUpDown
	public class XPNumericUpDown : System.Windows.Forms.TextBox
	{
		private System.Windows.Forms.VScrollBar vScrollBar;
		private int iValue, iMinimum, iMaximum;

		public XPNumericUpDown()
		{
			AutoSize = false;
			//Height += 2;
			vScrollBar = new System.Windows.Forms.VScrollBar();
			vScrollBar.Size = new Size (vScrollBar.Size.Width, Height);
			vScrollBar.Location = new Point (Width - vScrollBar.Width - 3, 0);
			vScrollBar.Cursor = Cursors.Default;
			vScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler (vScrollBarChanged);
			Controls.Add (vScrollBar);

			Value = 0;
			Maximum = 100;
			Minimum = 0;
		}

		protected override void OnGotFocus (System.EventArgs e)
		{
			SelectionLength = Text.Length;
			SelectionStart = 0;
		}

		protected override void OnKeyPress (System.Windows.Forms.KeyPressEventArgs e)
		{
			if (e.KeyChar >= '0' && e.KeyChar <= '9')
			{
				if (SelectionLength > 0)
				{
					Text = Text.Replace (SelectedText, e.KeyChar.ToString());
					SelectionLength = 0;
					SelectionStart = 0;
				}
				else
				{
					AppendText (e.KeyChar.ToString());
				}
				SetValue (int.Parse (Text));
				e.Handled = true;
			}
			else if (e.KeyChar >= 32 && e.KeyChar <= 255 && e.KeyChar != 127)
			{
				PInvoke.PlaySound ("Asterisk", 0, PInvoke.SND_ASYNC | PInvoke.SND_NOWAIT | PInvoke.SND_ALIAS);
				e.Handled = true;
			}
		}

		protected override void OnResize (System.EventArgs e)
		{
			base.OnResize (e);
			vScrollBar.Location = new Point (Width - vScrollBar.Width - 3, 0);
		}

		protected override void OnMouseWheel (System.Windows.Forms.MouseEventArgs e)
		{
			SetValue (iValue + (e.Delta > 0 ? 1 : -1));
		}

		protected void vScrollBarChanged (object sender, System.Windows.Forms.ScrollEventArgs e)
		{
			if (e.Type == ScrollEventType.SmallIncrement || e.Type == ScrollEventType.LargeIncrement || e.Type == ScrollEventType.Last)
				SetValue (iValue - 1);
			else if (e.Type == ScrollEventType.SmallDecrement || e.Type == ScrollEventType.LargeDecrement || e.Type == ScrollEventType.First)
				SetValue (iValue + 1);
		}

		public void SetValue (int val)
		{
			if (val > iMaximum)
				val = iMaximum;
			if (val < iMinimum)
				val = iMinimum;
			iValue = val;
			Text = iValue.ToString();
		}

		public int Value
		{
			get
			{
				return iValue;
			}
			set
			{
				SetValue (value);
			}
		}

		public int Maximum
		{
			get
			{
				return iMaximum;
			}
			set
			{
				iMaximum = value;
				if (iMaximum < iValue)
					SetValue (iMaximum);
			}
		}

		public int Minimum
		{
			get
			{
				return iMinimum;
			}
			set
			{
				iMinimum = value;
				if (iMinimum > iValue)
					SetValue (iMinimum);
			}
		}
	}
	#endregion
}
