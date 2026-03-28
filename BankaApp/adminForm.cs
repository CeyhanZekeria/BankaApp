using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class adminForm : Form
    {
        private int currentUserId;
        private string currentUsername;

        public adminForm(int userId, string username)
        {
            InitializeComponent();
            currentUserId = userId;
            currentUsername = username;
            AppState.ApplyFormState(this);
            ThemeManager.ApplyTheme(this);

            this.Resize += (s, e) => AppState.SaveFormState(this);
            this.Move += (s, e) => AppState.SaveFormState(this);
            this.FormClosing += (s, e) => AppState.SaveFormState(this);
        }

        private void adminForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
