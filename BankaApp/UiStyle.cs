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
        public static Color Warning = Color.FromArgb(245, 158, 11);

        public static void ApplyFormStyle(Form form)
        {
            form.BackColor = BgColor;
            form.ForeColor = TextDark;
        }

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

        public static void StyleCardPanel(Panel panel)
        {
            panel.BackColor = CardColor;
        }

        public static void StyleColoredPanel(Panel panel, Color backColor)
        {
            panel.BackColor = backColor;
        }

        public static void StylePrimaryButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
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
            btn.UseVisualStyleBackColor = false;
        }

        public static void StyleDangerButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Danger;
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        public static void AddHoverEffect(Button btn, Color normalColor, Color hoverColor)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        public static void StyleTitleLabel(Label lbl)
        {
            lbl.ForeColor = TextDark;
            lbl.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lbl.BackColor = Color.Transparent;
        }

        public static void StyleSubtitleLabel(Label lbl)
        {
            lbl.ForeColor = TextGray;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Regular);
            lbl.BackColor = Color.Transparent;
        }

        public static void StyleBalanceLabel(Label lbl)
        {
            lbl.ForeColor = TextDark;
            lbl.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            lbl.BackColor = Color.Transparent;
        }

        public static void StyleLightTextLabel(Label lbl)
        {
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lbl.BackColor = Color.Transparent;
        }

        public static void StyleTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.FixedSingle;
            txt.BackColor = Color.White;
            txt.ForeColor = TextDark;
            txt.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        public static void StyleReadOnlyTextBox(TextBox txt)
        {
            txt.BorderStyle = BorderStyle.None;
            txt.BackColor = CardColor;
            txt.ForeColor = TextDark;
            txt.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            txt.ReadOnly = true;
        }

        public static void StyleComboBox(ComboBox cmb)
        {
            cmb.FlatStyle = FlatStyle.Flat;
            cmb.BackColor = Color.White;
            cmb.ForeColor = TextDark;
            cmb.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }

        public static void StyleListBox(ListBox list)
        {
            list.BorderStyle = BorderStyle.None;
            list.BackColor = Color.White;
            list.ForeColor = TextDark;
            list.Font = new Font("Segoe UI", 10, FontStyle.Regular);
        }
    }
}