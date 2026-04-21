using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class adminForm : Form
    {
        private int currentUserId;
        private string currentUsername;
        private string currentUserRole;
        private int selectedAccountId = 0;
        private string selectedAccountNo = "";
        private int selectedClientId = 0;

        private AdminService adminService = new AdminService();

        public adminForm(int userId, string username, string userRole)
        {
            InitializeComponent();

            currentUserId = userId;
            currentUsername = username;
            currentUserRole = userRole;

            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.adminForm_FormClosing);
            this.Move += new System.EventHandler(this.adminForm_Move);
            this.Resize += new System.EventHandler(this.adminForm_Resize);
            this.dgvAccounts.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAccounts_CellClick);
            this.btnSearchClient.Click += new System.EventHandler(this.btnSearchClient_Click);
            this.dgvClients.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvClients_CellClick);
            this.btnBlockClient.Click += new System.EventHandler(this.btnBlockClient_Click);
            this.btnUnblockClient.Click += new System.EventHandler(this.btnUnblockClient_Click);
            this.btnDeposit.Click += new System.EventHandler(this.btnDeposit_Click);
            this.btnWithdraw.Click += new System.EventHandler(this.btnWithdraw_Click);
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
        }

        private void adminForm_Load(object sender, EventArgs e)
        {
            AppState.ApplyFormState(this);
            ThemeManager.ApplyTheme(this);
            ApplyModernUi();
            LoadUserInfo();
            LoadSearchOptions();
            SetupClientsGrid();
            SetupAccountsGrid();
            SetupTransactionsGrid();
            ApplyRolePermissions();
            ClearClientDetails();
        }

        private void adminForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void adminForm_Move(object sender, EventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void adminForm_Resize(object sender, EventArgs e)
        {
            AppState.SaveFormState(this);
        }

        private void LoadUserInfo()
        {
            lblUsername.Text = "User: " + currentUsername;
            lblRole.Text = "Role: " + currentUserRole;
        }

        private void LoadSearchOptions()
        {
            cmbSearchBy.Items.Clear();
            cmbSearchBy.Items.Add("Name");
            cmbSearchBy.Items.Add("EGN/LNC");
            cmbSearchBy.Items.Add("Email");
            cmbSearchBy.Items.Add("Phone");
            cmbSearchBy.Items.Add("Username");
            cmbSearchBy.Items.Add("Account No");
            cmbSearchBy.SelectedIndex = 0;

        }

        private void SetupClientsGrid()
        {
            dgvClients.AutoGenerateColumns = true;
            dgvClients.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvClients.MultiSelect = false;
            dgvClients.ReadOnly = true;
            dgvClients.AllowUserToAddRows = false;
            dgvClients.AllowUserToDeleteRows = false;
            dgvClients.AllowUserToResizeRows = false;
            dgvClients.RowHeadersVisible = false;
            dgvClients.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClients.BackgroundColor = Color.White;
            dgvClients.BorderStyle = BorderStyle.None;
        }

        private void SetupAccountsGrid()
        {
            dgvAccounts.AutoGenerateColumns = true;
            dgvAccounts.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvAccounts.MultiSelect = false;
            dgvAccounts.ReadOnly = true;
            dgvAccounts.AllowUserToAddRows = false;
            dgvAccounts.AllowUserToDeleteRows = false;
            dgvAccounts.AllowUserToResizeRows = false;
            dgvAccounts.RowHeadersVisible = false;
            dgvAccounts.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccounts.BackgroundColor = Color.White;
            dgvAccounts.BorderStyle = BorderStyle.None;
        }
        private void SetupTransactionsGrid()
        {
            dgvTransactions.AutoGenerateColumns = true;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.MultiSelect = false;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.AllowUserToResizeRows = false;
            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTransactions.BackgroundColor = Color.White;
            dgvTransactions.BorderStyle = BorderStyle.None;
        }

        private void ApplyRolePermissions()
        {
            string role = currentUserRole == null ? "" : currentUserRole.Trim().ToLower();

            bool isCashier = role == "cashier";
            bool isManager = role == "manager";
            bool isOperator = role == "operator";
            bool isAdmin = role == "admin" || role == "director";

            btnSearchClient.Visible = isCashier || isManager || isOperator || isAdmin;
            btnDeposit.Visible = isCashier || isManager || isAdmin;
            btnWithdraw.Visible = isCashier || isManager || isAdmin;
            btnBlockClient.Visible = isManager || isAdmin;
            btnUnblockClient.Visible = isManager || isAdmin;
        }

        private void ClearClientDetails()
        {
            selectedClientId = 0;
            selectedAccountId = 0;
            selectedAccountNo = "";

            lblClientName.Text = "Name: -";
            lblClientIdentity.Text = "EGN/LNC: -";
            lblClientEmail.Text = "Email: -";
            lblClientPhone.Text = "Phone: -";
            lblClientCountry.Text = "Country: -";
            lblClientStatus.Text = "Status: -";

            dgvAccounts.DataSource = null;
            dgvTransactions.DataSource = null;
        }
        private void LoadTransactionHistory(int accountId)
        {
            DataTable dt = adminService.GetAccountHistory(accountId);
            dgvTransactions.DataSource = dt;

            if (dgvTransactions.Columns["ID_TRANSACTIONS"] != null)
                dgvTransactions.Columns["ID_TRANSACTIONS"].Visible = false;

            if (dgvTransactions.Columns["DATE_TRAN"] != null)
                dgvTransactions.Columns["DATE_TRAN"].DefaultCellStyle.Format = "dd.MM.yyyy HH:mm";
        }
        private void dgvAccounts_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                if (dgvAccounts.Rows[e.RowIndex].Cells["ID_ACCOUNT"].Value == null)
                    return;

                selectedAccountId = Convert.ToInt32(dgvAccounts.Rows[e.RowIndex].Cells["ID_ACCOUNT"].Value);

                if (dgvAccounts.Rows[e.RowIndex].Cells["ACCOUNT_NO"].Value != null)
                    selectedAccountNo = dgvAccounts.Rows[e.RowIndex].Cells["ACCOUNT_NO"].Value.ToString();
                else
                    selectedAccountNo = "";
                LoadTransactionHistory(selectedAccountId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error selecting account: " + ex.Message);
            }
        }
        private void btnSearchClient_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText = txtSearch.Text.Trim();
                string searchBy = cmbSearchBy.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    MessageBox.Show("Please enter search text.");
                    return;
                }

                DataTable dt = adminService.SearchClients(searchBy, searchText);
                dgvClients.DataSource = dt;

                if (dt.Rows.Count == 0)
                {
                    ClearClientDetails();
                    MessageBox.Show("No clients found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while searching clients: " + ex.Message);
            }
        }

        private void dgvClients_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                if (dgvClients.Rows[e.RowIndex].Cells["CLIENT_ID"].Value == null)
                    return;

                selectedClientId = Convert.ToInt32(dgvClients.Rows[e.RowIndex].Cells["CLIENT_ID"].Value);

                LoadClientDetails(selectedClientId);
                LoadClientAccounts(selectedClientId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading client details: " + ex.Message);
            }
        }

        private void LoadClientDetails(int clientId)
        {
            DataTable dt = adminService.GetClientById(clientId);

            if (dt.Rows.Count == 0)
            {
                ClearClientDetails();
                return;
            }

            DataRow row = dt.Rows[0];

            string identityType = row["IDENTITY_TYPE"] == DBNull.Value ? "" : row["IDENTITY_TYPE"].ToString();
            string identityValue = row["EGN"] == DBNull.Value ? "" : row["EGN"].ToString();

            lblClientName.Text = "Name: " + row["NAME"].ToString();
            lblClientIdentity.Text = identityType + ": " + identityValue;
            lblClientEmail.Text = "Email: " + (row["EMAIL"] == DBNull.Value ? "-" : row["EMAIL"].ToString());
            lblClientPhone.Text = "Phone: " + (row["PHONE_NUMBER"] == DBNull.Value ? "-" : row["PHONE_NUMBER"].ToString());
            lblClientCountry.Text = "Country: " + (row["COUNTRY"] == DBNull.Value ? "-" : row["COUNTRY"].ToString());
            lblClientStatus.Text = Convert.ToInt32(row["IS_ACTIVE"]) == 1 ? "Status: Active" : "Status: Blocked";
        }

        private void LoadClientAccounts(int clientId)
        {
            DataTable dt = adminService.GetClientAccounts(clientId);
            dgvAccounts.DataSource = dt;

            selectedAccountId = 0;
            selectedAccountNo = "";
        }

        private void btnBlockClient_Click(object sender, EventArgs e)
        {
            if (selectedClientId == 0)
            {
                MessageBox.Show("Please select a client first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to block this client?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                adminService.BlockClient(selectedClientId);
                LoadClientDetails(selectedClientId);
                MessageBox.Show("Client blocked successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error blocking client: " + ex.Message);
            }
        }

        private void btnUnblockClient_Click(object sender, EventArgs e)
        {
            if (selectedClientId == 0)
            {
                MessageBox.Show("Please select a client first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to unblock this client?",
                "Confirm",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                adminService.UnblockClient(selectedClientId);
                LoadClientDetails(selectedClientId);
                MessageBox.Show("Client unblocked successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error unblocking client: " + ex.Message);
            }
        }

        private void btnDeposit_Click(object sender, EventArgs e)
        {
            if (selectedClientId == 0)
            {
                MessageBox.Show("Please select a client first.");
                return;
            }

            if (selectedAccountId == 0)
            {
                MessageBox.Show("Please select an account first.");
                return;
            }

            this.Hide();

            deposit depositForm = new deposit(
                currentUserId,
                currentUsername,
                currentUserRole,
                selectedClientId,
                selectedAccountId,
                selectedAccountNo
            );

            depositForm.FormClosed += (s, args) =>
            {
                this.Show();
                LoadClientDetails(selectedClientId);
                LoadClientAccounts(selectedClientId);
            };

            depositForm.Show();
        }

        private void btnWithdraw_Click(object sender, EventArgs e)
        {
            if (selectedClientId == 0)
            {
                MessageBox.Show("Please select a client first.");
                return;
            }

            if (selectedAccountId == 0)
            {
                MessageBox.Show("Please select an account first.");
                return;
            }

            this.Hide();

            withdraw withdrawForm = new withdraw(
                currentUserId,
                currentUsername,
                currentUserRole,
                selectedClientId,
                selectedAccountId,
                selectedAccountNo
            );

            withdrawForm.FormClosed += (s, args) =>
            {
                this.Show();
                LoadClientDetails(selectedClientId);
                LoadClientAccounts(selectedClientId);
            };

            withdrawForm.Show();
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            LoginForm login = new LoginForm();
            login.Show();
            this.Hide();
        }
        private void ApplyModernUi()
        {
            UiStyle.ApplyPageStyle(this);

            UiStyle.StyleCard(panel1, 30);

            UiStyle.StylePrimaryButton(btnSearchClient);
            UiStyle.StylePrimaryButton(btnDeposit);
            UiStyle.StylePrimaryButton(btnWithdraw);

            UiStyle.StyleDangerButton(btnBlockClient);
            UiStyle.StyleSuccessButton(btnUnblockClient);
            UiStyle.StyleSecondaryButton(btnLogout);

            UiStyle.StyleGrid(dgvClients);
            UiStyle.StyleGrid(dgvAccounts);

            UiStyle.StyleSectionTitle(label1);
            UiStyle.StyleSectionTitle(label2);
            UiStyle.StyleSectionTitle(label3);
        }
    }
}