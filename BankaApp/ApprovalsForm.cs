using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class ApprovalsForm : Form
    {
        private int currentClientId;
        private int currentAppUserId;
        private string currentUsername;

        private TransferRequestService transferRequestService = new TransferRequestService();

        public ApprovalsForm(int clientId, int userId, string username)
        {
            InitializeComponent();

            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;
        }

        private void ApprovalsForm_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "";
            StyleGrid();
            LoadPendingRequests();
        }

        private void LoadPendingRequests()
        {
            try
            {
                DataTable dt = transferRequestService.LoadPendingRequestsForClient(currentClientId);

                if (!dt.Columns.Contains("From_IBAN_Masked"))
                    dt.Columns.Add("From_IBAN_Masked", typeof(string));

                if (!dt.Columns.Contains("To_IBAN_Masked"))
                    dt.Columns.Add("To_IBAN_Masked", typeof(string));

                foreach (DataRow row in dt.Rows)
                {
                    row["From_IBAN_Masked"] = MaskIban(row["FROM_IBAN"].ToString());
                    row["To_IBAN_Masked"] = MaskIban(row["TO_IBAN"].ToString());
                }

                dgvRequests.DataSource = dt;

                if (dgvRequests.Columns.Contains("ID_Request"))
                    dgvRequests.Columns["ID_Request"].Visible = false;

                if (dgvRequests.Columns.Contains("FROM_IBAN"))
                    dgvRequests.Columns["FROM_IBAN"].Visible = false;

                if (dgvRequests.Columns.Contains("TO_IBAN"))
                    dgvRequests.Columns["TO_IBAN"].Visible = false;

                if (dgvRequests.Columns.Contains("REQUESTED_BY"))
                    dgvRequests.Columns["REQUESTED_BY"].HeaderText = "Requested By";

                if (dgvRequests.Columns.Contains("AMOUNT"))
                    dgvRequests.Columns["AMOUNT"].HeaderText = "Amount";

                if (dgvRequests.Columns.Contains("FROM_IBAN_MASKED"))
                    dgvRequests.Columns["FROM_IBAN_MASKED"].HeaderText = "From IBAN";

                if (dgvRequests.Columns.Contains("TO_IBAN_MASKED"))
                    dgvRequests.Columns["TO_IBAN_MASKED"].HeaderText = "To IBAN";

                if (dgvRequests.Columns.Contains("REQUESTED_AT"))
                    dgvRequests.Columns["REQUESTED_AT"].HeaderText = "Requested At";

                if (dgvRequests.Columns.Contains("REQUESTED_CURRENCY"))
                    dgvRequests.Columns["REQUESTED_CURRENCY"].HeaderText = "Currency";

                if (dt.Rows.Count == 0)
                {
                    lblStatus.Text = "No pending requests right now.";
                }
                else
                {
                    lblStatus.Text = "";
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error loading requests.";
                MessageBox.Show("Error loading requests: " + ex.Message);
            }
        }

        private string MaskIban(string iban)
        {
            if (string.IsNullOrWhiteSpace(iban) || iban.Length <= 8)
                return "****";

            return iban.Substring(0, 4) + "****" + iban.Substring(iban.Length - 4);
        }

       

        private int GetSelectedRequestId()
        {
            if (dgvRequests.CurrentRow == null)
                return 0;

            return Convert.ToInt32(dgvRequests.CurrentRow.Cells["ID_Request"].Value);
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            int requestId = GetSelectedRequestId();
            if (requestId == 0)
            {
                lblStatus.Text = "Please select a request.";
                return;
            }

            try
            {
                transferRequestService.ApproveRequest(requestId);
                lblStatus.Text = "Request approved successfully.";
                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Approve failed.";
                MessageBox.Show("Approve failed: " + ex.Message);
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            int requestId = GetSelectedRequestId();
            if (requestId == 0)
            {
                lblStatus.Text = "Please select a request.";
                return;
            }

            try
            {
                transferRequestService.RejectRequest(requestId);
                lblStatus.Text = "Request rejected.";
                LoadPendingRequests();
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Reject failed.";
                MessageBox.Show("Reject failed: " + ex.Message);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            mainForn loginForm = new mainForn(currentClientId, currentAppUserId, currentUsername);
            loginForm.Show();
            this.Hide();
        }


        private void StyleGrid()
        {
            dgvRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvRequests.MultiSelect = false;
            dgvRequests.ReadOnly = true;
            dgvRequests.AllowUserToAddRows = false;
            dgvRequests.AllowUserToDeleteRows = false;
            dgvRequests.RowHeadersVisible = false;
            dgvRequests.BorderStyle = BorderStyle.None;
            dgvRequests.BackgroundColor = Color.White;
            dgvRequests.DefaultCellStyle.SelectionBackColor = Color.FromArgb(235, 240, 255);
            dgvRequests.DefaultCellStyle.SelectionForeColor = Color.Black;
            dgvRequests.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvRequests.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvRequests.RowTemplate.Height = 34;

            btnApprove.BackColor = Color.FromArgb(52, 168, 83);
            btnApprove.ForeColor = Color.White;
            btnApprove.FlatStyle = FlatStyle.Flat;
            btnApprove.FlatAppearance.BorderSize = 0;

            btnReject.BackColor = Color.FromArgb(220, 53, 69);
            btnReject.ForeColor = Color.White;
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.FlatAppearance.BorderSize = 0;

            UiStyle.RoundControl(btnApprove, 12);
            UiStyle.RoundControl(btnReject, 12);
            UiStyle.RoundControl(btnCancel, 12);
        }
    }
}