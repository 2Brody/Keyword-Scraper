using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyword_Scraper_By_MasterC
{
    public class CustomTabControl : TabControl
    {
        private Color _highlightColor = Color.Red;
        private Color _defaultBackColor = Color.FromArgb(239, 239, 239);
        private Color _defaultForeColor = Color.Black;

        public Color HighlightColor
        {
            get { return _highlightColor; }
            set { _highlightColor = value; Invalidate(); }
        }

        public Color DefaultBackColor
        {
            get { return _defaultBackColor; }
            set { _defaultBackColor = value; Invalidate(); }
        }

        public Color DefaultForeColor
        {
            get { return _defaultForeColor; }
            set { _defaultForeColor = value; Invalidate(); }
        }

        public CustomTabControl()
        {
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
            this.DrawItem += new DrawItemEventHandler(DrawTab);
            this.SizeMode = TabSizeMode.Fixed;
            this.ItemSize = new Size(100, 40);
        }

        private void DrawTab(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            TabPage tabPage = this.TabPages[e.Index];
            Rectangle tabBounds = this.GetTabRect(e.Index);

            if (e.State == DrawItemState.Selected)
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(tabBounds, _highlightColor, Color.White, LinearGradientMode.Vertical))
                {
                    g.FillRectangle(brush, tabBounds);
                }
            }
            else
            {
                g.FillRectangle(new SolidBrush(_defaultBackColor), tabBounds);
            }

            TextRenderer.DrawText(g, tabPage.Text, tabPage.Font, tabBounds, _defaultForeColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ResumeLayout(false);

        }
    }
}
