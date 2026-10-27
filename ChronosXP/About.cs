using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace ChronosXP
{
    public partial class About : Form
    {
        public const string Copyright = "About.cs, Copyright © 2008 by Robert Misiak";

        private Config _Config;

        public About(Config config)
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);

            InitializeComponent();

            _Config = config;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        private void About_Load(object sender, EventArgs e)
        {
#if BETA
            if (!_Config.Core.Photogenic)
            {
                labelVersion.Text = String.Format("{0} (Build {1})", Config.BetaVersion, Application.ProductVersion);
            }
            else
            {
#endif // BETA
                labelVersion.Text = String.Format("Version {0}", Config.Version);
#if BETA
            }
#endif // BETA

            labelEdition.Text = _Config.GetString(String.Concat("Edition.", Config.Platform));

            _Config.Core.FormAboutOpen = true;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            LinkLabel linkLabel = sender as LinkLabel;
            string url = linkLabel.Tag as string;
            _Config.LaunchBrowser(url);
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            _Config.Core.FormAboutOpen = false;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Rectangle area = new Rectangle(0, 0, Width, Height);
            using (LinearGradientBrush lgb = new LinearGradientBrush(area, Color.WhiteSmoke, SystemColors.Control, LinearGradientMode.ForwardDiagonal))
            {
                e.Graphics.FillRectangle(lgb, area);
            }
        }

        private void labelCopyright_Click(object sender, EventArgs e)
        {

        }
    }
}
