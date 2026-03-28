using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class LoginForm : Form
    {
        string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";
        private bool isEnglish = true;

        public LoginForm()
        {
            InitializeComponent();

            label4.Text = "";
            label4.Visible = false;
            textBox2.UseSystemPasswordChar = true;

            button2.Width = 50;
            button2.Height = 50;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.BackColor = Color.RoyalBlue;
            button2.ForeColor = Color.White;
            MakeButtonRound(button2);
            button2.Resize += button2_Resize;

            AppState.ApplyFormState(this);
            ThemeManager.ApplyTheme(this);

            this.Resize += (s, e) => AppState.SaveFormState(this);
            this.Move += (s, e) => AppState.SaveFormState(this);
            this.FormClosing += (s, e) => AppState.SaveFormState(this);

            ApplyEnglishLanguage();
        }

        private void MakeButtonRound(Button btn)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, btn.Width, btn.Height);
            btn.Region = new Region(path);
        }

        private void ApplyEnglishLanguage()
        {
            label2.Text = "Login";
            usrnmoremail.Text = "Username / Email";

            passwrd.Text = "Password";
            label3.Text = "Don't have an account?";

            button1.Text = "LOG IN";
            label2.Font = new Font("Segoe UI", 30F);
            linkLabel1.Text = "Create account";
            showbtn.Text = textBox2.UseSystemPasswordChar ? "👁" : "🚫";

            textBox1.PlaceholderText = "Enter username or email";
            textBox2.PlaceholderText = "Enter password";

            button2.Text = "BG";
            isEnglish = true;
        }

        private void ApplyBulgarianLanguage()
        {
            label2.Text = "Добре Дошли";
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 20F);
            usrnmoremail.Text = "Потребител / Имейл";
            passwrd.Text = "Парола";

            button1.Text = "ВХОД";
            linkLabel1.Text = "Създай акаунт";
            label3.Text = "Нямате още профил?";
            showbtn.Text = textBox2.UseSystemPasswordChar ? "👁" : "🚫";

            textBox1.PlaceholderText = "Въведи потребител или имейл";
            textBox2.PlaceholderText = "Въведи парола";

            button2.Text = "EN";
            isEnglish = false;
        }

        private void showbtn_Click(object sender, EventArgs e)
        {
            textBox2.UseSystemPasswordChar = !textBox2.UseSystemPasswordChar;
            showbtn.Text = textBox2.UseSystemPasswordChar ? "👁" : "🚫";
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
                label4.Text = isEnglish
                    ? "Please enter username/email and password."
                    : "Моля, въведете" + "\n" + "потребител/имейл и парола.";
                label4.Visible = true;
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                        SELECT au.ID_USER,
                               au.USERNAME,
                               au.USER_ROLE,
                               c.CLIENT_ID
                        FROM APP_USER au
                        LEFT JOIN CLIENT c ON c.EMAIL = au.EMAIL
                        WHERE (au.USERNAME = :loginValue OR au.EMAIL = :loginValue)
                          AND au.USER_PASSWORD = :password";

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
                                string userRole = reader["USER_ROLE"].ToString().Trim();

                                int clientId = 0;
                                if (reader["CLIENT_ID"] != DBNull.Value)
                                {
                                    clientId = Convert.ToInt32(reader["CLIENT_ID"]);
                                }
                                if (userRole.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                                {
                                    adminForm adminForm = new adminForm(userId, username);
                                    adminForm.Show();
                                    this.Hide();
                                }
                                else
                                {
                                    mainForn mainForm = new mainForn(clientId, userId, username);
                                    mainForm.Show();
                                    this.Hide();
                                }

                            }
                            else
                            {
                                label4.ForeColor = Color.Red;
                                label4.Text = isEnglish
                                    ? "Invalid username/email or password."
                                    : "Невалиден потребител/имейл или парола.";
                                label4.Visible = true;
                            }
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show(
                    isEnglish ? "Database error: " + ex.Message : "Грешка в базата данни: " + ex.Message,
                    isEnglish ? "Error" : "Грешка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    isEnglish ? "Unexpected error: " + ex.Message : "Неочаквана грешка: " + ex.Message,
                    isEnglish ? "Error" : "Грешка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (isEnglish)
                ApplyBulgarianLanguage();
            else
                ApplyEnglishLanguage();
        }

        private void button2_Resize(object sender, EventArgs e)
        {
            MakeButtonRound(button2);
        }

        private void pictureBox1_Click(object sender, EventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void textBox2_TextChanged(object sender, EventArgs e) { }
        private void pictureBox1_Click_1(object sender, EventArgs e) { }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}