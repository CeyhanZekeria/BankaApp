using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BankaApp
{
    public static class UiStyle
    {
        public static readonly Color BgColor = Color.FromArgb(245, 247, 251);
        public static readonly Color CardColor = Color.White;
        public static readonly Color Primary = Color.FromArgb(63, 114, 255);
        public static readonly Color PrimaryDark = Color.FromArgb(39, 84, 214);
        public static readonly Color Sidebar = Color.FromArgb(20, 24, 38);
        public static readonly Color TextDark = Color.FromArgb(32, 37, 54);
        public static readonly Color TextGray = Color.FromArgb(120, 128, 140);
        public static readonly Color Border = Color.FromArgb(230, 233, 240);
        public static readonly Color Success = Color.FromArgb(34, 197, 94);
        public static readonly Color Danger = Color.FromArgb(239, 68, 68);

        public static void ApplyPageStyle(Form form)
        {
            form.BackColor = BgColor;
            form.ForeColor = TextDark;
            form.Font = new Font("Segoe UI", 10F);
        }

        private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();

            if (radius <= 0)
            {
                path.AddRectangle(rect);
                path.CloseFigure();
                return path;
            }

            int d = radius * 2;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();

            return path;
        }

        public static void RoundControl(Control control, int radius = 18)
        {
            if (control.Width <= 0 || control.Height <= 0)
                return;

            using (GraphicsPath path = CreateRoundedPath(
                new Rectangle(0, 0, control.Width, control.Height), radius))
            {
                control.Region = new Region(path);
            }
        }

        public static void KeepRounded(Control control, int radius = 18)
        {
            void apply(object s, EventArgs e)
            {
                if (control.IsDisposed) return;
                RoundControl(control, radius);
            }

            control.HandleCreated -= apply;
            control.Resize -= apply;

            control.HandleCreated += apply;
            control.Resize += apply;

            apply(control, EventArgs.Empty);
        }

        public static void StyleCard(Panel panel, int radius = 24)
        {
            panel.BackColor = CardColor;
            panel.BorderStyle = BorderStyle.None;
            KeepRounded(panel, radius);
        }

        public static void StyleColoredPanel(Panel panel, Color color, int radius = 24)
        {
            panel.BackColor = color;
            panel.BorderStyle = BorderStyle.None;
            KeepRounded(panel, radius);
        }

        public static void StylePrimaryButton(Button btn)
        {
            btn.BackColor = Primary;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = PrimaryDark;
            btn.FlatAppearance.MouseOverBackColor = PrimaryDark;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            KeepRounded(btn, 16);
        }

        public static void StyleSecondaryButton(Button btn)
        {
            btn.BackColor = CardColor;
            btn.ForeColor = TextDark;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderColor = Border;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(245, 247, 251);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(245, 247, 251);
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            KeepRounded(btn, 16);
        }

        public static void StyleDangerButton(Button btn)
        {
            btn.BackColor = Danger;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(210, 45, 45);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(210, 45, 45);
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            KeepRounded(btn, 16);
        }

        public static void StyleSuccessButton(Button btn)
        {
            btn.BackColor = Success;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(22, 163, 74);
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(22, 163, 74);
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            KeepRounded(btn, 16);
        }

        public static void StyleNavButton(Button btn, bool active = false)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(18, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.MouseDownBackColor = active ? PrimaryDark : Color.FromArgb(35, 40, 55);
            btn.FlatAppearance.MouseOverBackColor = active ? PrimaryDark : Color.FromArgb(35, 40, 55);

            if (active)
            {
                btn.BackColor = Primary;
                btn.ForeColor = Color.White;
                btn.Tag = "active";
            }
            else
            {
                btn.BackColor = Color.Transparent;
                btn.ForeColor = Color.White;
                btn.Tag = "inactive";
            }
        }

        public static void SetActiveNavButton(Button activeBtn, params Button[] allButtons)
        {
            foreach (Button btn in allButtons)
            {
                if (btn == null) continue;
                StyleNavButton(btn, btn == activeBtn);
            }
        }

        public static void AddHoverEffect(Button btn, Color normalColor, Color hoverColor)
        {
            btn.MouseEnter -= HoverEnter;
            btn.MouseLeave -= HoverLeave;

            btn.MouseEnter += HoverEnter;
            btn.MouseLeave += HoverLeave;

            void HoverEnter(object sender, EventArgs e)
            {
                if ((btn.Tag != null && btn.Tag.ToString() == "active"))
                    return;

                btn.BackColor = hoverColor;
            }

            void HoverLeave(object sender, EventArgs e)
            {
                if ((btn.Tag != null && btn.Tag.ToString() == "active"))
                    return;

                btn.BackColor = normalColor;
            }
        }

        public static void StyleInput(Control control)
        {
            control.BackColor = Color.White;
            control.ForeColor = TextDark;
            control.Font = new Font("Segoe UI", 10F);

            if (control is TextBox tb)
            {
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (control is ComboBox cb)
            {
                cb.FlatStyle = FlatStyle.Flat;
                cb.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            else if (control is RichTextBox rtb)
            {
                rtb.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        public static void StyleGrid(DataGridView dgv)
        {
            dgv.EnableHeadersVisualStyles = false;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.GridColor = Border;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowTemplate.Height = 36;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextDark;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 42;

            dgv.DefaultCellStyle.BackColor = Color.White;
            dgv.DefaultCellStyle.ForeColor = TextDark;
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            dgv.DefaultCellStyle.SelectionForeColor = TextDark;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 10F);
            dgv.DefaultCellStyle.Padding = new Padding(4, 0, 4, 0);

            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);
        }

        public static void StyleSectionTitle(Label lbl)
        {
            lbl.ForeColor = TextDark;
            lbl.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
        }

        public static void StyleMutedLabel(Label lbl)
        {
            lbl.ForeColor = TextGray;
            lbl.Font = new Font("Segoe UI", 10F);
        }

        public static void StyleValueLabel(Label lbl)
        {
            lbl.ForeColor = TextDark;
            lbl.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        }
    }
}