using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSCCC
{
    public partial class CursorColor : UserControl
    {
        private object _locker = new object();

        DateTime _start = DateTime.Now;

        Color[] _colors;

        public CursorColor()
        {
            InitializeComponent();

            DoubleBuffered = true;
        }

        public void SetColors(Color[] colors)
        {
            lock (_locker)
            {
                _colors = colors;
            }
        }

        public Color[] GetColors()
        {
            return _colors;
        }

        private void CursorColor_Paint(object sender, PaintEventArgs e)
        {
            if (GetColor(out var color))
            {
                RenderCursor(e.Graphics, ref color);
            }
        }

        private bool GetColor(out Color color)
        {
            lock (_locker)
            {
                color = Color.Black;

                if (_colors == null || _colors.Length == 0)
                {
                    return false;
                }
                else if (_colors.Length == 1)
                {
                    color = _colors[0];

                    return true;
                }

                var time = (DateTime.Now - _start).TotalSeconds;
                var progress = (time * 5) % _colors.Length;
                var whole = (int)Math.Floor(progress);
                var first = _colors[whole];
                var second = _colors[(whole + 1) % _colors.Length];
                var alpha = progress - whole;

                var r = (int)(first.R + (second.R - first.R) * alpha);
                var g = (int)(first.G + (second.G - first.G) * alpha);
                var b = (int)(first.B + (second.B - first.B) * alpha);

                color = Color.FromArgb(r, g, b);

                return true;
            }
        }

        private void RenderCursor(Graphics g, ref Color color)
        {
            g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

            var size = 10;

            using (var brush = new SolidBrush(color))
            {
                g.FillRectangle(brush, 0, 0, Width, size);
                g.FillRectangle(brush, 0, Height - size, Width, size);
                g.FillRectangle(brush, 0, size, size, Height - size * 2);
                g.FillRectangle(brush, Width - size, size, size, Height - size * 2);
            }

            using (var brush = new SolidBrush(Color.FromArgb(100, color.R, color.G, color.B)))
            {
                g.FillRectangle(brush, size, size, Height - size * 2, Height - size * 2);
            }
        }
    }
}
