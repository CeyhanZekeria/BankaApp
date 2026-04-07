using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public static class ThemeManager
    {
        // Light theme palette
        private static readonly Color LightFormBg = Color.FromArgb(245, 247, 251);
        private static readonly Color LightCardBg = Color.White;
        private static readonly Color LightSoftPanel = Color.FromArgb(248, 250, 255);
        private static readonly Color LightInputBg = Color.White;
        private static readonly Color LightText = Color.FromArgb(32, 37, 54);
        private static readonly Color LightMutedText = Color.FromArgb(120, 128, 140);
        private static readonly Color LightBorder = Color.FromArgb(230, 233, 240);
        private static readonly Color LightPrimary = Color.FromArgb(63, 114, 255);

        // Dark theme palette - improved hierarchy
        private static readonly Color DarkFormBg = Color.FromArgb(17, 20, 27);
        private static readonly Color DarkCardBg = Color.FromArgb(28, 33, 43);
        private static readonly Color DarkSoftPanel = Color.FromArgb(38, 44, 56);
        private static readonly Color DarkInputBg = Color.FromArgb(34, 39, 50);
        private static readonly Color DarkText = Color.FromArgb(242, 246, 252);
        private static readonly Color DarkMutedText = Color.FromArgb(156, 166, 184);
        private static readonly Color DarkBorder = Color.FromArgb(64, 74, 92);
        private static readonly Color DarkPrimary = Color.FromArgb(78, 125, 255);
        private static readonly Color DarkSecondaryButtonBg = Color.FromArgb(42, 48, 61);

        public static void ApplyTheme(Form form)
        {
            string mode = AppState.ThemeMode;

            if (mode == "Auto")
            {
                int hour = DateTime.Now.Hour;
                mode = (hour >= 7 && hour < 19) ? "Light" : "Dark";
            }

            if (mode == "Dark")
                ApplyDark(form);
            else
                ApplyLight(form);
        }

        private static void ApplyLight(Control parent)
        {
            ApplyThemeRecursive(
                parent,
                isDark: false,
                formBg: LightFormBg,
                cardBg: LightCardBg,
                softPanelBg: LightSoftPanel,
                inputBg: LightInputBg,
                textColor: LightText,
                mutedTextColor: LightMutedText,
                borderColor: LightBorder,
                primaryColor: LightPrimary
            );
        }

        private static void ApplyDark(Control parent)
        {
            ApplyThemeRecursive(
                parent,
                isDark: true,
                formBg: DarkFormBg,
                cardBg: DarkCardBg,
                softPanelBg: DarkSoftPanel,
                inputBg: DarkInputBg,
                textColor: DarkText,
                mutedTextColor: DarkMutedText,
                borderColor: DarkBorder,
                primaryColor: DarkPrimary
            );
        }

        private static void ApplyThemeRecursive(
            Control parent,
            bool isDark,
            Color formBg,
            Color cardBg,
            Color softPanelBg,
            Color inputBg,
            Color textColor,
            Color mutedTextColor,
            Color borderColor,
            Color primaryColor)
        {
            bool keepColor = HasTag(parent, "KeepColor");
            bool softPanel = HasTag(parent, "SoftPanel");
            bool primaryButton = HasTag(parent, "PrimaryButton");
            bool secondaryButton = HasTag(parent, "SecondaryButton");
            bool mutedLabel = HasTag(parent, "MutedText");

            if (parent is Form && !keepColor)
            {
                parent.BackColor = formBg;
                parent.ForeColor = textColor;
            }
            else if (parent is Panel && !keepColor)
            {
                parent.BackColor = softPanel ? softPanelBg : cardBg;
                parent.ForeColor = textColor;
            }
            else if (parent is GroupBox && !keepColor)
            {
                parent.BackColor = cardBg;
                parent.ForeColor = textColor;
            }
            else if (parent is Label && !keepColor)
            {
                parent.BackColor = Color.Transparent;
                parent.ForeColor = mutedLabel ? mutedTextColor : textColor;
            }
            else if ((parent is TextBox || parent is ComboBox || parent is ListBox) && !keepColor)
            {
                parent.BackColor = inputBg;
                parent.ForeColor = textColor;
            }
            else if (parent is CheckBox && !keepColor)
            {
                parent.BackColor = Color.Transparent;
                parent.ForeColor = textColor;
            }
            else if (parent is RadioButton && !keepColor)
            {
                parent.BackColor = Color.Transparent;
                parent.ForeColor = textColor;
            }
            else if (parent is Button btn && !keepColor)
            {
                StyleButton(btn, isDark, primaryButton, secondaryButton, primaryColor, cardBg, textColor, borderColor);
            }

            if (parent is DataGridView dgv && !keepColor)
            {
                StyleGrid(dgv, isDark, cardBg, textColor, borderColor, softPanelBg);
            }

            foreach (Control child in parent.Controls)
            {
                ApplyThemeRecursive(
                    child,
                    isDark,
                    formBg,
                    cardBg,
                    softPanelBg,
                    inputBg,
                    textColor,
                    mutedTextColor,
                    borderColor,
                    primaryColor
                );
            }
        }

        private static void StyleButton(
            Button btn,
            bool isDark,
            bool isPrimary,
            bool isSecondary,
            Color primaryColor,
            Color cardBg,
            Color textColor,
            Color borderColor)
        {
            btn.FlatStyle = FlatStyle.Flat;

            if (isPrimary)
            {
                btn.BackColor = primaryColor;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(primaryColor);
                btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(primaryColor);
                return;
            }

            if (isSecondary)
            {
                btn.BackColor = isDark ? DarkSecondaryButtonBg : cardBg;
                btn.ForeColor = textColor;
                btn.FlatAppearance.BorderSize = 1;
                btn.FlatAppearance.BorderColor = borderColor;
                btn.FlatAppearance.MouseOverBackColor = isDark
                    ? Color.FromArgb(50, 58, 72)
                    : Color.FromArgb(245, 247, 251);
                btn.FlatAppearance.MouseDownBackColor = isDark
                    ? Color.FromArgb(45, 52, 66)
                    : Color.FromArgb(235, 239, 246);
                return;
            }

            if (btn.FlatAppearance.BorderSize == 0)
            {
                btn.BackColor = primaryColor;
                btn.ForeColor = Color.White;
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = ControlPaint.Light(primaryColor);
                btn.FlatAppearance.MouseDownBackColor = ControlPaint.Dark(primaryColor);
                return;
            }

            btn.BackColor = cardBg;
            btn.ForeColor = textColor;
            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = borderColor;
            btn.FlatAppearance.MouseOverBackColor = isDark
                ? Color.FromArgb(34, 40, 52)
                : Color.FromArgb(248, 250, 255);
            btn.FlatAppearance.MouseDownBackColor = isDark
                ? Color.FromArgb(30, 36, 48)
                : Color.FromArgb(238, 242, 255);
        }

        private static void StyleGrid(
            DataGridView dgv,
            bool isDark,
            Color cardBg,
            Color textColor,
            Color borderColor,
            Color softPanelBg)
        {
            dgv.BackgroundColor = cardBg;
            dgv.GridColor = borderColor;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;

            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = softPanelBg;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = textColor;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = softPanelBg;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = textColor;

            dgv.DefaultCellStyle.BackColor = cardBg;
            dgv.DefaultCellStyle.ForeColor = textColor;
            dgv.DefaultCellStyle.SelectionBackColor = isDark
                ? Color.FromArgb(49, 64, 96)
                : Color.FromArgb(238, 242, 255);
            dgv.DefaultCellStyle.SelectionForeColor = textColor;

            dgv.AlternatingRowsDefaultCellStyle.BackColor = isDark
                ? Color.FromArgb(32, 38, 49)
                : Color.FromArgb(252, 252, 252);

            dgv.RowHeadersDefaultCellStyle.BackColor = cardBg;
            dgv.RowHeadersDefaultCellStyle.ForeColor = textColor;
        }

        private static bool HasTag(Control control, string tagName)
        {
            if (control?.Tag == null)
                return false;

            string tag = control.Tag.ToString();
            if (string.IsNullOrWhiteSpace(tag))
                return false;

            string[] parts = tag.Split(',');
            foreach (string part in parts)
            {
                if (part.Trim().Equals(tagName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }
    }
}