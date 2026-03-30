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

            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);
        }

        private void adminForm_Load_1(object sender, EventArgs e)
        {

        }
    }
}
