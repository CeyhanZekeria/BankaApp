using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class settings : Form
    {
        private int currentClientId;
        private int currentAppUserId;
        private string currentUsername;
        public settings(int clientId, int userId, string username)
        {
            InitializeComponent();

            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;
        }

        private void settings_Load(object sender, EventArgs e)
        {
        }

        private void homeBtn_Click(object sender, EventArgs e)
        {
            mainForn mainForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
            mainForm.Show();
            this.Hide();
        }



        private void profl_Click(object sender, EventArgs e)
        {
            ProfileForm form = new ProfileForm(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            this.Hide();
        }

        private void secrty_Click(object sender, EventArgs e)
        {
            securityForm loginForm = new securityForm(currentClientId, currentAppUserId, currentUsername);
            loginForm.Show();
            this.Hide();
        }
    }
}
