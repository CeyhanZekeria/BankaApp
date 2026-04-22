using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class AddMoneyForm : Form
    {
        private int currentClientId;
        private int currentAppUserId;
        private string currentUsername;
        private AccountService accountService = new AccountService();
        private TransferRequestService transferRequestService = new TransferRequestService();
        public AddMoneyForm(int clientId, int userId, string username)
        {
            InitializeComponent();
            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;

            LanguageManager.LoadLanguage();
            ApplyTranslations();
            FormStateHelper.Attach(this);
            
        }
        private void AddMoneyForm_Load(object sender, EventArgs e)
        {
            txtCVV.UseSystemPasswordChar = true;
            lblStatus.Text = "";
            LoadMyAccounts();
            LoadCurrencies();
            StyleAddMoneyButton();
            StyleCardPanel();
        }

        private void LoadMyAccounts()
        {
            DataTable dt = accountService.LoadUserAccounts(currentClientId);

            if (!dt.Columns.Contains("DisplayText"))
                dt.Columns.Add("DisplayText", typeof(string));

            foreach (DataRow row in dt.Rows)
                row["DisplayText"] = row["ACCOUNT_NO"].ToString();

            cmbMyAccounts.DataSource = dt;
            cmbMyAccounts.DisplayMember = "DisplayText";
            cmbMyAccounts.ValueMember = "ID_Account";
            cmbMyAccounts.SelectedIndex = -1;
        }

        private bool IsValidCardNumber(string cardNumber)
        {
            return Regex.IsMatch(cardNumber.Replace(" ", "").Trim(), @"^\d{16}$");
        }

        private bool IsValidValidThru(string validThru)
        {
            return Regex.IsMatch(validThru.Trim(), @"^(0[1-9]|1[0-2])\/\d{2}$");
        }

        private bool IsValidCVV(string cvv)
        {
            return Regex.IsMatch(cvv.Trim(), @"^\d{3}$");
        }

        private void btnSendRequest_Click(object sender, EventArgs e)
        {
            lblStatus.Text = "";

            if (cmbMyAccounts.SelectedIndex == -1)
            {
                lblStatus.Text = "Please select your account.";
                return;
            }

            string cardNumber = txtCardNumber.Text.Trim();
            string validThru = txtValidThru.Text.Trim();
            string cvv = txtCVV.Text.Trim();
            string amountText = txtAmount.Text.Trim();

            if (!IsValidCardNumber(cardNumber))
            {
                lblStatus.Text = "Invalid card number.";
                return;
            }

            if (!IsValidValidThru(validThru))
            {
                lblStatus.Text = "Invalid valid thru. Use MM/yy.";
                return;
            }

            if (!IsValidCVV(cvv))
            {
                lblStatus.Text = "Invalid CVV.";
                return;
            }

            decimal amount;
            if (!decimal.TryParse(amountText, out amount) || amount <= 0)
            {
                lblStatus.Text = "Invalid amount.";
                return;
            }

            int targetAccountId = Convert.ToInt32(cmbMyAccounts.SelectedValue);

            try
            {
                int sourceUserId = transferRequestService.GetCardOwnerUserId(cardNumber, validThru, cvv);
                if (sourceUserId == 0)
                {
                    lblStatus.Text = "Card not found or details are incorrect.";
                    return;
                }

                int sourceClientId = transferRequestService.GetClientIdByUserId(sourceUserId);
                if (sourceClientId == 0)
                {
                    lblStatus.Text = "Card owner profile not found.";
                    return;
                }

                int sourceAccountId = transferRequestService.GetFirstAccountIdByClientId(sourceClientId);
                if (sourceAccountId == 0)
                {
                    lblStatus.Text = "Card owner has no account.";
                    return;
                }

                int targetClientId = transferRequestService.GetClientIdByAccountId(targetAccountId);
                if (targetClientId == 0)
                {
                    lblStatus.Text = "Target account is invalid.";
                    return;
                }

                if (sourceAccountId == targetAccountId)
                {
                    lblStatus.Text = "Cannot request money from the same account.";
                    return;
                }
                if (cmbCurrency.SelectedIndex == -1)
                {
                    lblStatus.Text = "Please select currency.";
                    return;
                }
                int requestedCurrencyId = Convert.ToInt32(cmbCurrency.SelectedValue);

                transferRequestService.CreateTransferRequest(
                    requestedByClientId: currentClientId,
                    fromClientId: sourceClientId,
                    toClientId: targetClientId,
                    fromAccountId: sourceAccountId,
                    toAccountId: targetAccountId,
                    amount: amount,
                    requestedCurrencyId: requestedCurrencyId,
                    note: "Add money request from dashboard"
                );

                MessageBox.Show("Request sent successfully. Waiting for approval.");
                this.DialogResult = DialogResult.OK;
                this.Close();
                mainForn mainForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
                mainForm.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Request failed.";
                MessageBox.Show("Request failed: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            mainForn mainForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
            mainForm.Show();
            this.Hide();
        }
        private void LoadCurrencies()
        {
            DataTable dt = transferRequestService.LoadCurrencies();

            cmbCurrency.DataSource = dt;
            cmbCurrency.DisplayMember = "CURRENCY_TYPE_NAME";
            cmbCurrency.ValueMember = "ID_CURRENCY_TYPE";
            cmbCurrency.SelectedIndex = -1;
        }
        private void lblStatus_Click(object sender, EventArgs e)
        {

        }
        private void SetRoundedPanel(Panel panel, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(panel.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(panel.Width - radius, panel.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, panel.Height - radius, radius, radius, 90, 90);
            path.CloseFigure();
            panel.Region = new Region(path);
        }

        private void StyleAddMoneyButton()
        {
            btnSendRequest.BackColor = Color.FromArgb(0, 120, 215);
            btnSendRequest.ForeColor = Color.White;
            btnSendRequest.FlatStyle = FlatStyle.Flat;
            btnSendRequest.FlatAppearance.BorderSize = 0;
            btnSendRequest.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            btnSendRequest.Cursor = Cursors.Hand;
        }

        private void StyleCancelButton()
        {
            btnCancel.BackColor = Color.White;
            btnCancel.ForeColor = Color.FromArgb(0, 120, 215);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(0, 120, 215);
            btnCancel.FlatAppearance.BorderSize = 1;
            btnCancel.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btnCancel.Cursor = Cursors.Hand;
        }

        private void StyleInputs()
        {
            txtCardNumber.Font = new Font("Segoe UI", 10);
            txtValidThru.Font = new Font("Segoe UI", 10);
            txtCVV.Font = new Font("Segoe UI", 10);
            txtAmount.Font = new Font("Segoe UI", 10);

            cmbMyAccounts.Font = new Font("Segoe UI", 10);
            cmbCurrency.Font = new Font("Segoe UI", 10);

            txtCVV.UseSystemPasswordChar = true;
        }

        private void StyleCardPanel()
        {
            panel13.BackColor = Color.FromArgb(0, 102, 204);
            SetRoundedPanel(panel13, 25);
        }

        private void ArrangeAddMoneyLayout()
        {
            cmbMyAccounts.Location = new Point(380, 135);
            cmbMyAccounts.Size = new Size(270, 32);

            txtAmount.Location = new Point(410, 300);
            txtAmount.Size = new Size(180, 32);

            cmbCurrency.Location = new Point(605, 300);
            cmbCurrency.Size = new Size(75, 32);

            panel13.Location = new Point(775, 215);
            panel13.Size = new Size(410, 220);

            btnSendRequest.Location = new Point(410, 355);
            btnSendRequest.Size = new Size(195, 42);

            lblStatus.Location = new Point(410, 410);
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.FromArgb(220, 53, 69);
            lblStatus.Font = new Font("Segoe UI", 9, FontStyle.Bold);
        }

        private void ApplyTranslations()
        {
            
        }
    }
}