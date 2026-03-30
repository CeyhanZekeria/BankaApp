using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();

            textBox2.UseSystemPasswordChar = true;

            button2.Width = 50;
            button2.Height = 50;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.BackColor = Color.RoyalBlue;
            button2.ForeColor = Color.White;
            MakeButtonRound(button2);
            button2.Resize += button2_Resize;

            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);

            LanguageManager.LoadLanguage();

            if (LanguageManager.IsEnglish())
                ApplyEnglishLanguage();
            else
                ApplyBulgarianLanguage();
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
            LanguageManager.SetLanguage("EN");
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
            LanguageManager.SetLanguage("BG");
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
                label4.Text = LanguageManager.IsEnglish()
                    ? "Please enter username/email and password."
                    : "Моля, въведете" + "\n" + "потребител/имейл и парола.";
                label4.Visible = true;
                return;
            }

            try
            {
                string query = @"
                SELECT au.ID_USER,
               au.USERNAME,
               au.USER_ROLE,
               c.CLIENT_ID
               FROM APP_USER au
               LEFT JOIN CLIENT c ON c.EMAIL = au.EMAIL
                WHERE (au.USERNAME = :loginValue OR au.EMAIL = :loginValue)
                 AND au.USER_PASSWORD = :password";

                DataTable dt = DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("loginValue", loginValue),
                    new OracleParameter("password", passwordValue)
                );

                if (dt.Rows.Count > 0)
                {
                    int userId = Convert.ToInt32(dt.Rows[0]["ID_USER"]);
                    string username = dt.Rows[0]["USERNAME"].ToString();
                    string userRole = dt.Rows[0]["USER_ROLE"].ToString().Trim();

                    int clientId = 0;
                    if (dt.Rows[0]["CLIENT_ID"] != DBNull.Value)
                    {
                        clientId = Convert.ToInt32(dt.Rows[0]["CLIENT_ID"]);
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
                    label4.Text = LanguageManager.IsEnglish()
                        ? "Invalid username/email or password."
                        : "Невалиден потребител/имейл или парола.";
                    label4.Visible = true;
                }
            }
            catch (OracleException exs)
            {
                MessageBox.Show(
                    LanguageManager.IsEnglish() ? "Database error: " + exs.Message : "Грешка в базата данни: " + exs.Message,
                    LanguageManager.IsEnglish() ? "Error" : "Грешка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            catch (Exception exa)
            {
                MessageBox.Show(
                    LanguageManager.IsEnglish() ? "Unexpected error: " + exa.Message : "Неочаквана грешка: " + exa.Message,
                    LanguageManager.IsEnglish() ? "Error" : "Грешка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }



        }

        private void button2_Click(object sender, EventArgs e)
        {
            LanguageManager.ToggleLanguage();

            if (LanguageManager.IsEnglish())
                ApplyEnglishLanguage();
            else
                ApplyBulgarianLanguage();
        }

        private void button2_Resize(object sender, EventArgs e)
        {
            MakeButtonRound(button2);
        }
    }
}