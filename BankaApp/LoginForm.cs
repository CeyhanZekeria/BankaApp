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
            ApplyTranslations();

        }
        private void MakeButtonRound(Button btn)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, btn.Width, btn.Height);
            btn.Region = new Region(path);
        }
        private void ApplyTranslations()
        {
            if (LanguageManager.IsEnglish())
            {
                label2.Text = LanguageManager.GetText("login_title");
                label2.Font = new Font("Segoe UI", 30F);
                label2.AutoSize = true;
            }
            else
            {
                label2.Text = LanguageManager.GetText("login_title");
                label2.Font = new Font("Segoe UI", 20F);
                label2.AutoSize = true;
            }

            usrnmoremail.Text = LanguageManager.GetText("username_email");
            passwrd.Text = LanguageManager.GetText("password");
            label3.Text = LanguageManager.GetText("no_account");

            button1.Text = LanguageManager.GetText("log_in");
            linkLabel1.Text = LanguageManager.GetText("create_account");

            showbtn.Text = textBox2.UseSystemPasswordChar ? "👁" : "🚫";

            textBox1.PlaceholderText = LanguageManager.GetText("ph_login_user");
            textBox2.PlaceholderText = LanguageManager.GetText("ph_login_pass");

            button2.Text = LanguageManager.IsBulgarian() ? "EN" : "BG";
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
            SELECT 
                au.ID_USER AS USER_ID,
                au.USERNAME AS USERNAME,
                au.USER_ROLE AS USER_ROLE,
                au.USER_PASSWORD AS PWD,
                au.EMAIL AS EMAIL,
                c.CLIENT_ID AS CLIENT_ID
            FROM APP_USER au
            LEFT JOIN CLIENT c ON c.EMAIL = au.EMAIL
            WHERE (au.USERNAME = :loginValue OR au.EMAIL = :loginValue)";

                DataTable dt = DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("loginValue", loginValue)
                );

                if (dt.Rows.Count > 0)
                {
                    string storedHash = dt.Rows[0]["PWD"].ToString();

                    if (!PasswordHelper.VerifyPassword(passwordValue, storedHash))
                    {
                        label4.ForeColor = Color.Red;
                        label4.Text = LanguageManager.IsEnglish()
                            ? "Invalid username/email or password."
                            : "Невалиден потребител/имейл или парола.";
                        label4.Visible = true;
                        return;
                    }

                    int userId = Convert.ToInt32(dt.Rows[0]["USER_ID"]);
                    string username = dt.Rows[0]["USERNAME"].ToString();
                    string userRole = dt.Rows[0]["USER_ROLE"].ToString().Trim();
                    string email = dt.Rows[0]["EMAIL"] == DBNull.Value ? "" : dt.Rows[0]["EMAIL"].ToString();

                    int clientId = 0;
                    if (dt.Rows[0]["CLIENT_ID"] != DBNull.Value)
                    {
                        clientId = Convert.ToInt32(dt.Rows[0]["CLIENT_ID"]);
                    }

                    if (userRole.Equals("Client", StringComparison.OrdinalIgnoreCase) &&
                        IsClientBlocked(clientId, email))
                    {
                        label4.ForeColor = Color.Red;
                        label4.Text = LanguageManager.IsEnglish()
                            ? "This account is blocked. Please contact support."
                            : "Този акаунт е блокиран. Моля, свържете се с поддръжката.";
                        label4.Visible = true;
                        return;
                    }

                    if (userRole.Equals("Client", StringComparison.OrdinalIgnoreCase))
                    {
                        mainForn mainForm = new mainForn(clientId, userId, username);
                        mainForm.Show();
                        this.Hide();
                    }
                    else
                    {
                        adminForm adminForm = new adminForm(userId, username, userRole);
                        adminForm.Show();
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
        private bool IsClientBlocked(int clientId, string email)
        {
            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                object result = null;

                if (clientId > 0)
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "SELECT Is_Active FROM Client WHERE Client_ID = :clientId", conn))
                    {
                        cmd.Parameters.Add("clientId", OracleDbType.Int32).Value = clientId;
                        result = cmd.ExecuteScalar();
                    }
                }

                if ((result == null || result == DBNull.Value) && !string.IsNullOrWhiteSpace(email))
                {
                    using (OracleCommand cmd = new OracleCommand(
                        "SELECT Is_Active FROM Client WHERE LOWER(Email) = LOWER(:email)", conn))
                    {
                        cmd.Parameters.Add("email", OracleDbType.Varchar2).Value = email.Trim();
                        result = cmd.ExecuteScalar();
                    }
                }

                if (result == null || result == DBNull.Value)
                    return false;

                return Convert.ToInt32(result) == 0;
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            LanguageManager.ToggleLanguage();
            ApplyTranslations();
        }
        private void button2_Resize(object sender, EventArgs e)
        {
            MakeButtonRound(button2);
        }
    }
}