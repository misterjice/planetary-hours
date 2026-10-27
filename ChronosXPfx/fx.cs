using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ChronosXP
{
	public class ChronosXPfx
	{
		private Assembly asm;
		private string _iconSet;

		public ChronosXPfx()
		{
			asm = Assembly.GetExecutingAssembly();
		}

		public string IconSet
		{
			get
			{
				return iconSet;
			}
			set
			{
				iconSet = value;
			}
		}

		public Icon GlyphIcon(string planet)
		{
			Icon i;
			try
			{
                i = new Icon(ResourceStream(iconSet + "." + planet + ".ico"));
			}
			catch
			{
				i = new Icon (ResourceStream("Exclamation.ico"));
			}
			return new Icon (i, 48, 48);
		}

		public Icon GlyphIcon (string planet, int sz)
		{
			Icon i;
			try
			{
                i = new Icon(ResourceStream(iconSet + "." + planet + ".ico"));
			}
			catch
			{
				i = new Icon (ResourceStream ("Exclamation.ico"));
			}
			return new Icon (i, sz, sz);
		}

		public Icon GlyphIconSmall (string planet)
		{
			Icon i;
			try
			{
                i = new Icon(ResourceStream(iconSet + "." + planet + ".ico"));
			}
			catch
			{
				i = new Icon (ResourceStream ("Exclamation.ico"));
			}
			return new Icon (i, 16, 16);
		}

        private string iconSet
        {
            get
            {
                if (_iconSet == "Hebrew" && File.Exists(Path.Combine(Application.StartupPath, "Alt.txt")))
                {
                    // rabbi says that this should not be available by default; to enable, select Hebrew and create a file called
                    // Alt.txt in the same directory as ChronosXP.exe
                    return "Kabbalistic";
                }
                else
                {
                    return _iconSet;
                }
            }
            set
            {
                _iconSet = value;
            }
        }

		public Stream GlyphIconAsStream (string planet)
		{
            return ResourceStream(iconSet + "." + planet + ".ico");
		}

		public Stream GlyphIconAsStream (System.Drawing.Color color, string planet)
		{
            return ResourceStream(color.Name + "." + planet + ".ico");
		}

		public Stream GlyphGif (string planet)
		{
            string prefix;
            switch (iconSet)
            {
                case "Hebrew":
                case "Kabbalistic":
                    prefix = iconSet;
                    break;
                default:
                    prefix = "Default";
                    break;
            }
            return ResourceStream(String.Concat(prefix, ".", planet, ".gif"));
		}

		public Stream ResourceStream (string file)
		{
			Stream imgStream = asm.GetManifestResourceStream (String.Concat("ChronosXPfx.", file));
			if (imgStream == null)
				imgStream = asm.GetManifestResourceStream ("Exclamation.ico");
			return imgStream;
		}
	}
}
