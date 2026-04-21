using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class withdraw : Form
    {
        private const int DefaultEmployeeId = 1;

        private int currentClientId;
        private int currentAccountId;
        private int currentAppUserId;
        private int currentCurrencyId;

        private string currentUsername;
        private string currentUserRole;
        private string currentAccountNo;

        public withdraw(int userId, string username, string userRole)
            : this(userId, username, userRole, 0, 0, "")
        {
        }

        public withdraw(int userId, string username, string userRole, int clientId, int accountId, string accountNo)
        {
            InitializeComponent();

            currentAppUserId = userId;
            currentUsername = username;
            currentUserRole = userRole;

            currentClientId = clientId;
            currentAccountId = accountId;
            currentAccountNo = accountNo;

            LanguageManager.LoadLanguage();
            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);

            this.Load += withdraw_Load;
            this.btnCancel.Click += btnCancel_Click;
            this.btnConfirmWithdraw.Click += btnConfirmWithdraw_Click;
        }

        private void withdraw_Load(object sender, EventArgs e)
        {
            UiStyle.ApplyPageStyle(this);

            UiStyle.StylePrimaryButton(btnConfirmWithdraw);
            UiStyle.StyleSecondaryButton(btnCancel);

            UiStyle.StyleInput(txtAmount);
            UiStyle.StyleInput(txtDescription);

            UiStyle.StyleSectionTitle(labelTitle);
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
            a.ID_Currency_type AS ID_CURRENCY_TYPE,
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

                        currentCurrencyId = Convert.ToInt32(row["ID_CURRENCY_TYPE"]);
                        lblClientValue.Text = row["CLIENT_NAME"].ToString();
                        lblAccountValue.Text = row["ACCOUNT_NO"].ToString();
                        lblBalanceValue.Text = row["AVAILIBILITY"].ToString() + " " + row["CURRENCY_NAME"].ToString();
                        lblCurrencyValue.Text = row["CURRENCY_NAME"].ToString();
                    }
                }
            }
        }

        private void btnConfirmWithdraw_Click(object sender, EventArgs e)
        {
            try
            {
                if (currentAccountId <= 0)
                {
                    MessageBox.Show("No account selected.");
                    return;
                }

                if (!decimal.TryParse(txtAmount.Text.Trim(), out decimal amount) || amount <= 0)
                {
                    MessageBox.Show("Please enter valid amount.");
                    return;
                }

                string description = string.IsNullOrWhiteSpace(txtDescription.Text)
                    ? "Admin cash withdrawal"
                    : txtDescription.Text.Trim();

                using (OracleConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (OracleTransaction tx = conn.BeginTransaction())
                    {
                        try
                        {
                            int withdrawTypeId;

                            using (OracleCommand cmdType = new OracleCommand(
                                "SELECT ID_Type FROM Type WHERE UPPER(Type) = 'WITHDRAWAL'", conn))
                            {
                                cmdType.Transaction = tx;

                                object result = cmdType.ExecuteScalar();
                                if (result == null)
                                    throw new Exception("Withdrawal type not found.");

                                withdrawTypeId = Convert.ToInt32(result);
                            }

                            using (OracleCommand cmdInsert = new OracleCommand(@"
                                INSERT INTO Transactions
                                (
                                    ID_Transactions,
                                    ID_Type,
                                    ID_Employee,
                                    ID_Account,
                                    Sum_Amount,
                                    Date_Tran,
                                    ID_Currency_Type,
                                    ID_Exchange_Rate,
                                    Description
                                )
                                VALUES
                                (
                                    seq_Transactions.NEXTVAL,
                                    :typeId,
                                    :employeeId,
                                    :accountId,
                                    :amount,
                                    SYSDATE,
                                    :currencyId,
                                    NULL,
                                    :description
                                )", conn))
                            {
                                cmdInsert.Transaction = tx;

                                cmdInsert.Parameters.Add("typeId", OracleDbType.Int32).Value = withdrawTypeId;
                                cmdInsert.Parameters.Add("employeeId", OracleDbType.Int32).Value = DefaultEmployeeId;
                                cmdInsert.Parameters.Add("accountId", OracleDbType.Int32).Value = currentAccountId;
                                cmdInsert.Parameters.Add("amount", OracleDbType.Decimal).Value = amount;
                                cmdInsert.Parameters.Add("currencyId", OracleDbType.Int32).Value = currentCurrencyId;
                                cmdInsert.Parameters.Add("description", OracleDbType.Varchar2).Value = description;

                                cmdInsert.ExecuteNonQuery();
                            }

                            tx.Commit();
                        }
                        catch
                        {
                            tx.Rollback();
                            throw;
                        }
                    }
                }

                MessageBox.Show("Withdrawal completed successfully.");
                LoadAccountInfo();
                txtAmount.Clear();
                txtDescription.Clear();
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