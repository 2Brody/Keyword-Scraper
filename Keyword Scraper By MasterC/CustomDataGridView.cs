using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyword_Scraper_By_MasterC
{
    [DesignerCategory("Code")]
    [ToolboxItem(true)]
    public class CustomDataGridView : DataGridView
    {
        private Color _neonBorderColor = Color.FromArgb(138, 43, 226); // رنگ پیش‌فرض حاشیه نئون

        public CustomDataGridView()
        {
            // تنظیمات اولیه DataGridView
            this.DoubleBuffered = true;
            this.EnableHeadersVisualStyles = false;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RowTemplate.Height = 40;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.AllowUserToAddRows = false;
            this.AllowUserToDeleteRows = false;
            this.AllowUserToResizeRows = false;
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.BackgroundColor = Color.White;
            this.BorderStyle = BorderStyle.None;

            // تنظیمات Header
            this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            this.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تنظیمات سلول‌ها
            this.DefaultCellStyle.BackColor = Color.FromArgb(236, 240, 241);
            this.DefaultCellStyle.ForeColor = Color.FromArgb(44, 62, 80);
            this.DefaultCellStyle.Font = new Font("Segoe UI", 12);
            this.DefaultCellStyle.SelectionBackColor = Color.FromArgb(52, 152, 219);
            this.DefaultCellStyle.SelectionForeColor = Color.White;
            this.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            // تنظیمات ردیف‌ها
            this.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(44, 62, 80);
            this.RowHeadersDefaultCellStyle.ForeColor = Color.White;
            this.RowHeadersDefaultCellStyle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            this.RowHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // تنظیمات نئون
            this.Paint += CustomDataGridView_Paint;
        }

        private void CustomDataGridView_Paint(object sender, PaintEventArgs e)
        {
            // رسم حاشیه نئون
            DrawNeonBorder(e.Graphics, this.ClientRectangle, _neonBorderColor, 10);
        }

        private void DrawNeonBorder(Graphics graphics, Rectangle rect, Color neonColor, int glowSize)
        {
            using (GraphicsPath path = new GraphicsPath())
            {
                path.AddRectangle(rect);
                using (PathGradientBrush brush = new PathGradientBrush(path))
                {
                    brush.CenterColor = Color.FromArgb(0, neonColor);
                    brush.SurroundColors = new Color[] { neonColor };
                    brush.FocusScales = new PointF(0.8f, 0.8f);

                    for (int i = 0; i < glowSize; i++)
                    {
                        using (Pen pen = new Pen(Color.FromArgb((int)(255 * (1.0 - (double)i / glowSize)), neonColor), 1))
                        {
                            graphics.DrawRectangle(pen, rect.X - i, rect.Y - i, rect.Width + i * 2, rect.Height + i * 2);
                        }
                    }
                }
            }
        }

        // پروپرتی برای تغییر رنگ حاشیه نئون
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color NeonBorderColor
        {
            get { return _neonBorderColor; }
            set
            {
                _neonBorderColor = value;
                this.Invalidate(); // بازسازی کنترل برای اعمال تغییرات
            }
        }

        // پروپرتی برای تغییر رنگ پس‌زمینه
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CustomBackgroundColor
        {
            get { return this.BackgroundColor; }
            set
            {
                this.BackgroundColor = value;
                this.DefaultCellStyle.BackColor = value;
                this.AlternatingRowsDefaultCellStyle.BackColor = value; // تنظیم رنگ پس‌زمینه ردیف‌های متناوب
                foreach (DataGridViewRow row in this.Rows)
                {
                    row.DefaultCellStyle.BackColor = value;
                }
                this.Refresh();
            }
        }

        // پروپرتی برای تغییر رنگ فورگراند
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color CustomForegroundColor
        {
            get { return this.DefaultCellStyle.ForeColor; }
            set
            {
                this.DefaultCellStyle.ForeColor = value;
                this.ColumnHeadersDefaultCellStyle.ForeColor = value;
                this.RowHeadersDefaultCellStyle.ForeColor = value;
                this.DefaultCellStyle.SelectionForeColor = value;
                foreach (DataGridViewRow row in this.Rows)
                {
                    row.DefaultCellStyle.ForeColor = value;
                }
            }
        }

        // پروپرتی برای تغییر رنگ هدر ستون‌ها
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color ColumnHeaderBackgroundColor
        {
            get { return this.ColumnHeadersDefaultCellStyle.BackColor; }
            set { this.ColumnHeadersDefaultCellStyle.BackColor = value; }
        }

        // پروپرتی برای تغییر رنگ هدر ردیف‌ها
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color RowHeaderBackgroundColor
        {
            get { return this.RowHeadersDefaultCellStyle.BackColor; }
            set { this.RowHeadersDefaultCellStyle.BackColor = value; }
        }
    }
}
