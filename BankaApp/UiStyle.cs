using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BankaApp
{
    public static class UiStyle
    {
        public static Color BgColor = Color.FromArgb(245, 247, 251);
        public static Color CardColor = Color.White;
        public static Color Primary = Color.FromArgb(63, 114, 255);
        public static Color PrimaryDark = Color.FromArgb(39, 84, 214);
        public static Color Sidebar = Color.FromArgb(20, 24, 38);
        public static Color TextDark = Color.FromArgb(32, 37, 54);
        public static Color TextGray = Color.FromArgb(120, 128, 140);
        public static Color Border = Color.FromArgb(230, 233, 240);
        public static Color Success = Color.FromArgb(34, 197, 94);
        public static Color Danger = Color.FromArgb(239, 68, 68);

        public static void RoundControl(Control control, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            Rectangle rect = new Rectangle(0, 0, control.Width, control.Height);

            int d = radius * 2;
            path.StartFigure();
            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            control.Region = new Region(path);
        }

        public static void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleSecondaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = Border;
            btn.BackColor = Color.White;
            btn.ForeColor = TextDark;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
        }

        public static void StyleCardPanel(Panel panel)
        {
            panel.BackColor = CardColor;
        }
    }
}