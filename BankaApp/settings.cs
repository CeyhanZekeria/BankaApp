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
            LanguageManager.LoadLanguage();
            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;



            AppState.ApplyFormState(this);
            ApplyModernUi();
            ApplyLanguage();
            ThemeManager.ApplyTheme(this);
            this.Resize += (s, e) => AppState.SaveFormState(this);
            this.Move += (s, e) => AppState.SaveFormState(this);
            this.FormClosing += (s, e) => AppState.SaveFormState(this);
        }
        private void settings_Load(object sender, EventArgs e)
        {
            cmbTheme.Items.Clear();
            cmbTheme.Items.Add("Light");
            cmbTheme.Items.Add("Dark");
            cmbTheme.Items.Add("Auto");
            cmbTheme.SelectedItem = AppState.ThemeMode;

            cmbLanguage.Items.Clear();
            cmbLanguage.Items.Add("EN");
            cmbLanguage.Items.Add("BG");
            cmbLanguage.SelectedItem = LanguageManager.CurrentLanguage;

            ApplyLanguage();
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void cmbTheme_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (cmbTheme.SelectedItem == null)
                return;

            AppState.ThemeMode = cmbTheme.SelectedItem.ToString();
            ApplyModernUi();
            ThemeManager.ApplyTheme(this);
        }
     
        private void ApplyModernUi()
        {
            BackColor = UiStyle.BgColor;

            // theme tags
            panel1.Tag = "KeepColor";
            panel2.Tag = "SoftPanel";
            cmbTheme.Tag = "KeepColor";

            homeBtn.Tag = "SecondaryButton";
            profl.Tag = "SecondaryButton";
            secrty.Tag = "SecondaryButton";

            // sidebar
            panel1.BackColor = UiStyle.SoftPanel;

            // main content
            panel2.BackColor = UiStyle.SoftPanel;
            UiStyle.RoundControl(panel2, 30);

            // combo
            UiStyle.StyleComboBox(cmbTheme);

            // nav buttons
            UiStyle.StyleSecondaryButton(homeBtn);
            UiStyle.StyleSecondaryButton(profl);
            UiStyle.StyleSecondaryButton(secrty);

            UiStyle.RoundControl(homeBtn, 12);
            UiStyle.RoundControl(profl, 12);
            UiStyle.RoundControl(secrty, 12);

            // optional: active one
            homeBtn.BackColor = UiStyle.Primary;
            homeBtn.ForeColor = Color.White;
            homeBtn.FlatAppearance.BorderSize = 0;
        }

        private void cmbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbLanguage.SelectedItem == null)
                return;

            LanguageManager.SetLanguage(cmbLanguage.SelectedItem.ToString());
            ApplyLanguage();
        }

        private void ApplyLanguage()
        {
            homeBtn.Text = LanguageManager.GetText("home");
            profl.Text = LanguageManager.GetText("profile");
            secrty.Text = LanguageManager.GetText("security");
            themeLbl.Text = LanguageManager.GetText("theme");
            langLbl.Text = LanguageManager.GetText("language");
            btnSave.Text = LanguageManager.GetText("save");
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cmbTheme.SelectedItem != null)
                AppState.ThemeMode = cmbTheme.SelectedItem.ToString();

            if (cmbLanguage.SelectedItem != null)
                LanguageManager.SetLanguage(cmbLanguage.SelectedItem.ToString());

            ThemeManager.ApplyTheme(this);
            ApplyLanguage();

            string msg = LanguageManager.CurrentLanguage == "BG"
                ? "Настройките са запазени успешно!"
                : "Settings saved successfully!";

            string title = LanguageManager.CurrentLanguage == "BG"
                ? "Успех"
                : "Success";

            MessageBox.Show(msg, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

}
