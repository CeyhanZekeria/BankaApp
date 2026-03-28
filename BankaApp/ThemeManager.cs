using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public static class ThemeManager
    {
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
            parent.BackColor = Color.White;
            parent.ForeColor = Color.Black;

            foreach (Control control in parent.Controls)
            {
                if (control is Panel)
                {
                    control.BackColor = Color.WhiteSmoke;
                    control.ForeColor = Color.Black;
                }
                else if (control is Button)
                {
                    control.BackColor = Color.RoyalBlue;
                    control.ForeColor = Color.White;
                }
                else if (control is TextBox || control is ComboBox)
                {
                    control.BackColor = Color.White;
                    control.ForeColor = Color.Black;
                }
                else if (control is Label || control is RadioButton || control is CheckBox)
                {
                    control.BackColor = Color.Transparent;
                    control.ForeColor = Color.Black;
                }

                if (control.HasChildren)
                    ApplyLight(control);
            }
        }

        private static void ApplyDark(Control parent)
        {
            parent.BackColor = Color.FromArgb(24, 24, 28);
            parent.ForeColor = Color.White;

            foreach (Control control in parent.Controls)
            {
                if (control is Panel)
                {
                    control.BackColor = Color.FromArgb(36, 36, 42);
                    control.ForeColor = Color.White;
                }
                else if (control is Button)
                {
                    control.BackColor = Color.FromArgb(65, 105, 225);
                    control.ForeColor = Color.White;
                }
                else if (control is TextBox || control is ComboBox)
                {
                    control.BackColor = Color.FromArgb(45, 45, 50);
                    control.ForeColor = Color.White;
                }
                else if (control is Label || control is RadioButton || control is CheckBox)
                {
                    control.BackColor = Color.Transparent;
                    control.ForeColor = Color.White;
                }

                if (control.HasChildren)
                    ApplyDark(control);
            }
        }
    }
}