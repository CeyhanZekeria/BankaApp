using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class deposit : Form
    {
        private int currentClientId;
        private int currentAccountId;
        private int currentEmployeeId;

        private string currentUsername;
        private string currentUserRole;
        private string currentAccountNo;

        public deposit(int employeeId, string username, string userRole)
            : this(employeeId, username, userRole, 0, 0, "")
        {
        }

        public deposit(int employeeId, string username, string userRole, int clientId, int accountId, string accountNo)
        {
            InitializeComponent();

            currentEmployeeId = employeeId;
            currentUsername = username;
            currentUserRole = userRole;

            currentClientId = clientId;
            currentAccountId = accountId;
            currentAccountNo = accountNo;

            this.Load += deposit_Load;
            LanguageManager.LoadLanguage();
            FormStateHelper.Attach(this);

            this.btnCancel.Click += btnCancel_Click;
            this.btnConfirmDeposit.Click += btnConfirmDeposit_Click;
        }

        private void deposit_Load(object sender, EventArgs e)
        {
            UiStyle.ApplyPageStyle(this);

            UiStyle.StylePrimaryButton(btnConfirmDeposit);
            UiStyle.StyleSecondaryButton(btnCancel);

            UiStyle.StyleInput(txtAmount);
            UiStyle.StyleInput(txtDescription);
            UiStyle.StyleInput(cmbCurrency);

            UiStyle.StyleSectionTitle(labelTitle);

            LoadAccountInfo();
            LoadCurrencies();
        }

        private void LoadAccountInfo()
        {
            if (currentAccountId <= 0)
            {
                MessageBox.Show("No account selected.");
                Close();
                return;
            }

            string query = @"
                SELECT
                    c.Name AS CLIENT_NAME,
                    a.Account_NO AS ACCOUNT_NO,
                    a.Availibility AS AVAILIBILITY,
                    ct.Currency_type_Name AS CURRENCY_NAME
                FROM Account a
                JOIN Client c ON c.Client_ID = a.ID_Client
                JOIN Currency_type ct ON ct.ID_Currency_type = a.ID_Currency_type
                WHERE a.ID_Account = :accountId";

            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add("accountId", OracleDbType.Int32).Value = currentAccountId;

                    using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("Account not found.");
                            Close();
                            return;
                        }

                        DataRow row = dt.Rows[0];

                        lblClientValue.Text = row["CLIENT_NAME"].ToString();
                        lblAccountValue.Text = row["ACCOUNT_NO"].ToString();
                        lblBalanceValue.Text = row["AVAILIBILITY"].ToString() + " " + row["CURRENCY_NAME"].ToString();
                    }
                }
            }
        }

        private void LoadCurrencies()
        {
            string query = @"
                SELECT ID_Currency_type, Currency_type_Name
                FROM Currency_type
                ORDER BY Currency_type_Name";

            using (OracleConnection conn = DatabaseHelper.GetConnection())
            {
                conn.Open();

                using (OracleCommand cmd = new OracleCommand(query, conn))
                using (OracleDataAdapter da = new OracleDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    cmbCurrency.DataSource = dt;
                    cmbCurrency.DisplayMember = "CURRENCY_TYPE_NAME";
                    cmbCurrency.ValueMember = "ID_CURRENCY_TYPE";
                    cmbCurrency.SelectedIndex = -1;
                }
            }
        }

        private void btnConfirmDeposit_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentAccountId <= 0)
                {
                    MessageBox.Show("No account selected.");
                    return;
                }

                if (cmbCurrency.SelectedValue == null)
                {
                    MessageBox.Show("Please select deposit currency.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter valid amount.");
                    return;
                }

                int currencyId = Convert.ToInt32(cmbCurrency.SelectedValue);
                string description = string.IsNullOrWhiteSpace(txtDescription.Text)
                    ? "Admin cash deposit"
                    : txtDescription.Text.Trim();

                using (OracleConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (OracleCommand cmd = new OracleCommand("Deposit_Money", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.Add("p_account_id", OracleDbType.Int32).Value = currentAccountId;
                        cmd.Parameters.Add("p_employee_id", OracleDbType.Int32).Value = currentEmployeeId;
                        cmd.Parameters.Add("p_amount", OracleDbType.Decimal).Value = amount;
                        cmd.Parameters.Add("p_currency_id", OracleDbType.Int32).Value = currencyId;
                        cmd.Parameters.Add("p_description", OracleDbType.Varchar2).Value = description;

                        cmd.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Deposit completed successfully.");
                LoadAccountInfo();
                txtAmount.Clear();
                txtDescription.Clear();
                cmbCurrency.SelectedIndex = -1;
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Oracle error: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}