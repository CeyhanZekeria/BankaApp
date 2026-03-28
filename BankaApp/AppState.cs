using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public static class AppState
    {
        public static FormWindowState WindowState = FormWindowState.Normal;
        public static Size WindowSize = new Size(1200, 800);
        public static Point WindowLocation = new Point(100, 100);

        public static string ThemeMode = "Light"; // Light, Dark, Auto

        public static void SaveFormState(Form form)
        {
            if (form.WindowState == FormWindowState.Normal)
            {
                WindowSize = form.Size;
                WindowLocation = form.Location;
            }

            WindowState = form.WindowState;
        }

        public static void ApplyFormState(Form form)
        {
            form.StartPosition = FormStartPosition.Manual;
            form.Size = WindowSize;
            form.Location = WindowLocation;
            form.WindowState = WindowState;
        }
    }
}