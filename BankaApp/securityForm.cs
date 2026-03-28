using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class securityForm : Form
    {
        private int currentClientId;
        private int currentAppUserId;
        private string currentUsername;
        public securityForm(int clientId, int userId, string username)
        {
            InitializeComponent();

            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;

            AppState.ApplyFormState(this);
            ThemeManager.ApplyTheme(this);

            this.Resize += (s, e) => AppState.SaveFormState(this);
            this.Move += (s, e) => AppState.SaveFormState(this);
            this.FormClosing += (s, e) => AppState.SaveFormState(this);
        }

        private void securityForm_Load(object sender, EventArgs e)
        {

        }

        private void profl_Click(object sender, EventArgs e)
        {
            ProfileForm form = new ProfileForm(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            settings form = new settings(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            this.Hide();
        }
        private void homeBtn_Click_1(object sender, EventArgs e)
        {
            mainForn loginForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
            loginForm.Show();
            this.Hide();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel6_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
