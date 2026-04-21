using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;

namespace BankaApp
{
    public partial class SendMoneyForm : Form
    {
        private int currentClientId;
        private int currentAppUserId;
        private string currentUsername;
        private AccountService accountService = new AccountService();
        private SendMoneyService sendMoneyService = new SendMoneyService();
        private TransferRequestService transferRequestService = new TransferRequestService();
        public SendMoneyForm(int clientId, int userId, string username)
        {
            InitializeComponent();

            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;
            LanguageManager.LoadLanguage();
            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);
            LoadMyAccounts();
            LoadCurrencies();
        }
        private void SendMoneyForm_Load(object sender, EventArgs e)
        {
            AppState.ApplyFormState(this);
            ThemeManager.ApplyTheme(this);

            LoadMyAccounts();
            LoadCurrencies();
        }
        private void SendMoneyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void SendMoneyForm_Move(object sender, EventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void SendMoneyForm_Resize(object sender, EventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mainForn mainForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
            mainForm.Show();
            this.Hide();
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
        private void LoadCurrencies()
        {
            DataTable dt = transferRequestService.LoadCurrencies();

            cmbCurrency.DataSource = dt;
            cmbCurrency.DisplayMember = "CURRENCY_TYPE_NAME";
            cmbCurrency.ValueMember = "ID_CURRENCY_TYPE";
            cmbCurrency.SelectedIndex = -1;
        }

        private void LoadMyAccounts()
        {
            DataTable dt = accountService.LoadUserAccounts(currentClientId);

            cmbFromAccount.Items.Clear();

            foreach (DataRow row in dt.Rows)
            {
                cmbFromAccount.Items.Add(
                    new ComboBoxItem(
                        row["ACCOUNT_NO"].ToString(),
                        Convert.ToInt32(row["ID_ACCOUNT"])
                    )
                );
            }

            if (cmbFromAccount.Items.Count > 0)
                cmbFromAccount.SelectedIndex = 0;
        }
        private void btnSend_Click(object sender, EventArgs e)
        {
            if (cmbFromAccount.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a source account.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCurrency.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a currency.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtToAccount.Text))
            {
                MessageBox.Show("Please enter destination account / IBAN.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtToAccount.Focus();
                return;
            }

            string amountText = txtAmount.Text.Trim().Replace(',', '.');

            if (!decimal.TryParse(amountText, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid amount greater than 0.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAmount.Focus();
                return;
            }

            string toAccountNo = txtToAccount.Text.Trim();
            string description = txtDescription.Text.Trim();

            ComboBoxItem selected = (ComboBoxItem)cmbFromAccount.SelectedItem;
            int fromAccountId = selected.Value;
            int selectedCurrencyId = Convert.ToInt32(cmbCurrency.SelectedValue);

            int? toAccountId = sendMoneyService.GetAccountIdByAccountNo(toAccountNo);

            if (toAccountId == null)
            {
                MessageBox.Show("Destination account was not found.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtToAccount.Focus();
                return;
            }

            if (fromAccountId == toAccountId.Value)
            {
                MessageBox.Show("You cannot send money to the same account.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                $"Are you sure you want to send {amount:F2} to {toAccountNo}?",
                "Confirm transfer",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                sendMoneyService.SendMoneyWithFx(
                    fromAccountId,
                    toAccountId.Value,
                    amount,
                    selectedCurrencyId,
                    description);

                MessageBox.Show("Transfer completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Transfer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Transfer Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != ',')
            {
                e.Handled = true;
            }

            TextBox textBox = sender as TextBox;

            if ((e.KeyChar == '.' || e.KeyChar == ',') &&
                (textBox.Text.Contains(".") || textBox.Text.Contains(",")))
            {
                e.Handled = true;
            }
        }

    }
}