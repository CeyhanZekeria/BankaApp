using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class LoginForm : Form
    {
        string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";
        public LoginForm()
        {
            InitializeComponent();
            label4.Visible = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm form = new RegisterForm();
            form.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                label4.Visible = false;

                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = "SELECT COUNT(*) FROM App_User WHERE Username = :username AND User_Password = :password";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":username", textBox1.Text.Trim());
                        cmd.Parameters.Add(":password", textBox2.Text.Trim());

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            mainForn mainForm = new mainForn();
                            mainForm.Show();
                            this.Hide();
                        }
                        else
                        {
                            label4.ForeColor = Color.Red;
                            label4.Text = "Грешно потребителско име или парола.";
                            label4.Visible = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

