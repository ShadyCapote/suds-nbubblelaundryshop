using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Suds_NBubbleLaundryShop.CustomControl
{
   public class RoundedButton : Button
        {
            public int BorderRadius { get; set; } = 15;

            public RoundedButton()
            {
                this.FlatStyle = FlatStyle.Flat;
                this.FlatAppearance.BorderSize = 0;
                this.Resize += (s, e) => ApplyRoundedCorners();
            }
            private void ApplyRoundedCorners()
            {
                int r = Math.Min(BorderRadius, Math.Min(Width, Height) / 2);

                GraphicsPath path = new GraphicsPath();

                path.AddArc(0, 0, r * 2, r * 2, 180, 90);
                path.AddArc(Width - r * 2, 0, r * 2, r * 2, 270, 90);
                path.AddArc(Width - r * 2, Height - r * 2, r * 2, r * 2, 0, 90);
                path.AddArc(0, Height - r * 2, r * 2, r * 2, 90, 90);
                path.CloseFigure();

                this.Region?.Dispose();
                this.Region = new Region(path);
                path.Dispose();
            }
        }
}
