using Oracle.ManagedDataAccess.Client;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class LoginForm : Form
    {
        string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";

        public LoginForm()
        {
            InitializeComponent();
            label4.Text = "";
            label4.Visible = false;
            textBox2.UseSystemPasswordChar = true;
        }

        private void showbtn_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
            showbtn.Text = textBox2.UseSystemPasswordChar ? "Show" : "Hide";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegisterForm form = new RegisterForm();
            form.Show();
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string loginValue = textBox1.Text.Trim();
            string passwordValue = textBox2.Text.Trim();

            label4.Text = "";
            label4.Visible = false;

            if (string.IsNullOrWhiteSpace(loginValue) || string.IsNullOrWhiteSpace(passwordValue))
            {
                label4.ForeColor = Color.Red;
                label4.Text = "Please enter username/email and password.";
                label4.Visible = true;
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                        SELECT ID_USER, USERNAME
                        FROM APP_USER
                        WHERE (USERNAME = :loginValue OR EMAIL = :loginValue)
                          AND USER_PASSWORD = :password";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":loginValue", OracleDbType.Varchar2).Value = loginValue;
                        cmd.Parameters.Add(":password", OracleDbType.Varchar2).Value = passwordValue;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["ID_USER"]);
                                string username = reader["USERNAME"].ToString();

                                mainForn mainForm = new mainForn(userId, username);
                                mainForm.Show();
                                this.Hide();
                            }
                            else
                            {
                                label4.ForeColor = Color.Red;
                                label4.Text = "Invalid username/email or password.";
                                label4.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click_1(object sender, EventArgs e) { }
    }
}