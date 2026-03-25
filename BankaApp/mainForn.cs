using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client;


namespace BankaApp
{
    public partial class mainForn : Form
    {
        private string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";
        private List<decimal> exchangeRates = new List<decimal>();
        private string chartPairText = "Lei = EUR";
        private int currentUserId;
        private string currentUsername;
        public mainForn(int userId, string username)
        {
            InitializeComponent();

            currentUserId = userId;
            currentUsername = username;

            usrName.Text = currentUsername.ToUpper();
            SetupTransactionsGrid();
            SetupTransactionFilters();
            LoadRecentTransactions();


            LoadExchangeRatesForChart(2, 6, "EUR -> AED");
            SetRoundedPanel(panel1, 30);
            SetRoundedPanel(panel2, 30);
            SetRoundedPanel(panel3, 30);
            SetRoundedPanel(panel4, 30);
            SetRoundedPanel(panel5, 30);
            SetRoundedPanel(panel6, 30);
            SetRoundedPanel(panel7, 30);
            SetRoundedPanel(panel12, 30);
            SetRoundedPanel(panel13, 30);
            SetRoundedPanel(panel14, 30);
            SetRoundedPanel(panel15, 30);

            panel1.Resize += panel1_Resize;
            panel2.Resize += panel2_Resize;
            panel3.Resize += panel3_Resize;
            panel4.Resize += panel4_Resize;
            panel5.Resize += panel5_Resize;
            panel6.Resize += panel6_Resize;
            panel7.Resize += panel7_Resize;
            panel3.Resize += panel3_Resize;
            panel4.Resize += panel4_Resize;
            panel5.Resize += panel5_Resize;
            panel12.Resize += panel12_Resize;
            panel12.Paint += panel12_Paint;

            filter.Click += btnFilter_Click;
            reset.Click += btnReset_Click;

        }


        private void FormatTransactionsGrid()
        {
            if (dgvTransactions.Columns.Contains("Type"))
            {
                dgvTransactions.Columns["Type"].HeaderText = "Type";
                dgvTransactions.Columns["Type"].FillWeight = 22;
            }

            if (dgvTransactions.Columns.Contains("Amount"))
            {
                dgvTransactions.Columns["Amount"].HeaderText = "Amount";
                dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTransactions.Columns["Amount"].FillWeight = 18;
            }

            if (dgvTransactions.Columns.Contains("Currency"))
            {
                dgvTransactions.Columns["Currency"].HeaderText = "Currency";
                dgvTransactions.Columns["Currency"].FillWeight = 14;
            }

            if (dgvTransactions.Columns.Contains("Description"))
            {
                dgvTransactions.Columns["Description"].HeaderText = "Details";
                dgvTransactions.Columns["Description"].FillWeight = 30;
            }

            if (dgvTransactions.Columns.Contains("Tran_Date"))
            {
                dgvTransactions.Columns["Tran_Date"].HeaderText = "Date";
                dgvTransactions.Columns["Tran_Date"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvTransactions.Columns["Tran_Date"].FillWeight = 18;
            }

            if (dgvTransactions.Columns.Contains("Transaction_ID"))
                dgvTransactions.Columns["Transaction_ID"].Visible = false;

            if (dgvTransactions.Columns.Contains("Account_Number"))
                dgvTransactions.Columns["Account_Number"].Visible = false;

            if (dgvTransactions.Columns.Contains("ID_Transactions"))
                dgvTransactions.Columns["ID_Transactions"].Visible = false;

            if (dgvTransactions.Columns.Contains("ID_Account"))
                dgvTransactions.Columns["ID_Account"].Visible = false;
        }
        private void SetupTransactionsGrid()
        {
            dgvTransactions.Columns.Clear();
            dgvTransactions.AutoGenerateColumns = true;

            dgvTransactions.BorderStyle = BorderStyle.None;
            dgvTransactions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTransactions.GridColor = Color.FromArgb(235, 235, 235);
            dgvTransactions.BackgroundColor = Color.White;
            dgvTransactions.EnableHeadersVisualStyles = false;
            dgvTransactions.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvTransactions.EnableHeadersVisualStyles = false;
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTransactions.ColumnHeadersHeight = 42;

            dgvTransactions.DefaultCellStyle.BackColor = Color.White;
            dgvTransactions.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            dgvTransactions.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);

            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.AllowUserToResizeRows = false;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.MultiSelect = false;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTransactions.RowTemplate.Height = 38;
            dgvTransactions.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        }
        private void LoadExchangeRatesForChart(int fromCurrencyId, int toCurrencyId, string pairText)
        {
            exchangeRates.Clear();
            chartPairText = pairText;

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                SELECT Rate
                FROM (
                    SELECT Rate
                    FROM Exchange_Rate
                    WHERE Currency_type_from = :fromId
                      AND Currency_type_to = :toId
                    ORDER BY Rate_Date DESC
                )
                WHERE ROWNUM <= 6";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.BindByName = true;
                        cmd.Parameters.Add(":fromId", OracleDbType.Int32).Value = fromCurrencyId;
                        cmd.Parameters.Add(":toId", OracleDbType.Int32).Value = toCurrencyId;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                exchangeRates.Add(reader.GetDecimal(0));
                            }
                        }
                    }
                }

                exchangeRates.Reverse();

                if (exchangeRates.Count < 2)
                {
                    exchangeRates.Clear();
                    exchangeRates.Add(0.1910m);
                    exchangeRates.Add(0.1935m);
                    exchangeRates.Add(0.1942m);
                    exchangeRates.Add(0.1951m);
                    exchangeRates.Add(0.1962m);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading exchange rates: " + ex.Message);
            }
        }
        private void LoadUserAccountsForFilter()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                SELECT ID_Account, Account_NO
                FROM Account
                WHERE ID_Client = :clientId
                ORDER BY ID_Account";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":clientId", OracleDbType.Int32).Value = currentUserId;

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            cmbAccount.Items.Clear();
                            cmbAccount.Items.Add("All");

                            while (reader.Read())
                            {
                                cmbAccount.Items.Add(
                                    new ComboBoxItem(
                                        reader["Account_NO"].ToString(),
                                        Convert.ToInt32(reader["ID_Account"])
                                    )
                                );
                            }
                        }
                    }
                }

                cmbAccount.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message);
            }
        }
        private void SetupTransactionFilters()
        {
            cmbType.Items.Clear();
            cmbType.Items.Add("All");
            cmbType.Items.Add("Deposit");
            cmbType.Items.Add("Withdrawal");
            cmbType.Items.Add("Transfer");
            cmbType.Items.Add("Fee");
            cmbType.Items.Add("Interest");
            cmbType.SelectedIndex = 0;

            dtpFrom.Value = DateTime.Today.AddMonths(-3);
            dtpTo.Value = DateTime.Today;

            LoadUserAccountsForFilter();
        }

        private void ColorTransactionsGrid()
        {
            if (!dgvTransactions.Columns.Contains("Type"))
                return;

            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                if (row.IsNewRow) continue;

                string type = row.Cells["Type"].Value?.ToString()?.Trim().ToLower() ?? "";

                if (type == "deposit")
                {
                    row.Cells["Type"].Style.ForeColor = Color.FromArgb(34, 139, 34);
                    row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    if (dgvTransactions.Columns.Contains("Amount"))
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(34, 139, 34);
                        row.Cells["Amount"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    }
                }
                else if (type == "withdrawal")
                {
                    row.Cells["Type"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                    row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    if (dgvTransactions.Columns.Contains("Amount"))
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(220, 53, 69);
                        row.Cells["Amount"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    }
                }
                else if (type == "transfer")
                {
                    row.Cells["Type"].Style.ForeColor = Color.FromArgb(0, 102, 204);
                    row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    if (dgvTransactions.Columns.Contains("Amount"))
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(0, 102, 204);
                        row.Cells["Amount"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    }
                }
                else if (type == "interest")
                {
                    row.Cells["Type"].Style.ForeColor = Color.FromArgb(255, 140, 0);
                    row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    if (dgvTransactions.Columns.Contains("Amount"))
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(255, 140, 0);
                        row.Cells["Amount"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    }
                }
                else if (type == "fee")
                {
                    row.Cells["Type"].Style.ForeColor = Color.FromArgb(128, 128, 128);
                    row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);

                    if (dgvTransactions.Columns.Contains("Amount"))
                    {
                        row.Cells["Amount"].Style.ForeColor = Color.FromArgb(128, 128, 128);
                        row.Cells["Amount"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                    }
                }

                
            }
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

        private void panel1_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel1, 30);
        }

        private void panel2_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel2, 30);
        }

        private void panel3_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel3, 30);
        }

        private void panel4_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel4, 30);
        }

        private void panel5_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel5, 30);
        }

        private void panel6_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel6, 30);
        }

        private void panel7_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel7, 30);
        }
        private void panel12_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel12, 30);
            panel12.Invalidate();
        }
        private void StyleTransactionsGrid()
        {
            dgvTransactions.BorderStyle = BorderStyle.None;
            dgvTransactions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTransactions.GridColor = Color.FromArgb(235, 235, 235);

            dgvTransactions.BackgroundColor = Color.White;
            dgvTransactions.EnableHeadersVisualStyles = false;

            dgvTransactions.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgvTransactions.ColumnHeadersHeight = 42;

            dgvTransactions.DefaultCellStyle.BackColor = Color.White;
            dgvTransactions.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            dgvTransactions.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);

            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.AllowUserToResizeRows = false;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.MultiSelect = false;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTransactions.RowTemplate.Height = 38;

            dgvTransactions.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
        }
        private void panel13_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel14, 30);
            SetRoundedPanel(panel13, 30);
            SetRoundedPanel(panel15, 30);
            panel13.Invalidate();
        }
        private void LoadRecentTransactions()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                SELECT *
                FROM (
                    SELECT 
                        tp.Type AS Type,
                        t.Sum_Amount AS Amount,
                        c.Currency_type_Name AS Currency,
                        t.Description AS Description,
                        t.Date_Tran AS Tran_Date
                    FROM Transactions t
                    JOIN Account a ON t.ID_Account = a.ID_Account
                    JOIN Type tp ON t.ID_Type = tp.ID_Type
                    JOIN Currency_type c ON t.ID_Currency_Type = c.ID_Currency_type
                    WHERE a.ID_Client = :clientId
                    ORDER BY t.Date_Tran DESC
                )
                WHERE ROWNUM <= 10";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add(":clientId", OracleDbType.Int32).Value = currentUserId;

                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        System.Data.DataTable dt = new System.Data.DataTable();
                        da.Fill(dt);

                        dgvTransactions.DataSource = dt;
                        StyleTransactionsGrid();
                        FormatTransactionsGrid();
                        ColorTransactionsGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading recent transactions: " + ex.Message);
            }
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            if (dtpFrom.Value.Date > dtpTo.Value.Date)
            {
                MessageBox.Show("From date cannot be after To date.");
                return;
            }
            LoadFilteredTransactions();
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            dtpFrom.Value = DateTime.Today.AddMonths(-3);
            dtpTo.Value = DateTime.Today;
            cmbType.SelectedIndex = 0;
            cmbAccount.SelectedIndex = 0;

            LoadRecentTransactions();
        }

        private void LoadFilteredTransactions()
        {
            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    string query = @"
                SELECT 
                    tp.Type AS Type,
                    t.Sum_Amount AS Amount,
                    c.Currency_type_Name AS Currency,
                    t.Description AS Description,
                    t.Date_Tran AS Tran_Date
                FROM Transactions t
                JOIN Account a ON t.ID_Account = a.ID_Account
                JOIN Type tp ON t.ID_Type = tp.ID_Type
                JOIN Currency_type c ON t.ID_Currency_Type = c.ID_Currency_type
                WHERE a.ID_Client = :clientId
                  AND t.Date_Tran >= :dateFrom
                  AND t.Date_Tran < :dateTo";

                    if (cmbType.Text != "All")
                    {
                        query += " AND tp.Type = :type";
                    }

                    if (cmbAccount.SelectedItem != null && cmbAccount.Text != "All")
                    {
                        query += " AND a.ID_Account = :accountId";
                    }

                    query += " ORDER BY t.Date_Tran DESC";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.BindByName = true;

                        cmd.Parameters.Add(":clientId", OracleDbType.Int32).Value = currentUserId;
                        cmd.Parameters.Add(":dateFrom", OracleDbType.Date).Value = dtpFrom.Value.Date;
                        cmd.Parameters.Add(":dateTo", OracleDbType.Date).Value = dtpTo.Value.Date.AddDays(1);

                        if (cmbType.Text != "All")
                        {
                            cmd.Parameters.Add(":type", OracleDbType.Varchar2).Value = cmbType.Text;
                        }

                        if (cmbAccount.SelectedItem != null && cmbAccount.Text != "All")
                        {
                            ComboBoxItem selectedAccount = (ComboBoxItem)cmbAccount.SelectedItem;
                            cmd.Parameters.Add(":accountId", OracleDbType.Int32).Value = selectedAccount.Value;
                        }

                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvTransactions.DataSource = dt;
                        StyleTransactionsGrid();
                        FormatTransactionsGrid();
                        ColorTransactionsGrid();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering transactions: " + ex.Message);
            }
        }

        private void ApplyTransactionsGridStyle()
        {
            dgvTransactions.BorderStyle = BorderStyle.None;
            dgvTransactions.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvTransactions.GridColor = Color.FromArgb(235, 235, 235);

            dgvTransactions.BackgroundColor = Color.White;
            dgvTransactions.EnableHeadersVisualStyles = false;
            dgvTransactions.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;

            dgvTransactions.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(245, 247, 250);
            dgvTransactions.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(60, 60, 60);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvTransactions.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvTransactions.DefaultCellStyle.BackColor = Color.White;
            dgvTransactions.DefaultCellStyle.ForeColor = Color.FromArgb(45, 45, 45);
            dgvTransactions.DefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Regular);
            dgvTransactions.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            dgvTransactions.DefaultCellStyle.SelectionForeColor = Color.FromArgb(35, 35, 35);

            dgvTransactions.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(252, 252, 252);

            dgvTransactions.RowHeadersVisible = false;
            dgvTransactions.AllowUserToAddRows = false;
            dgvTransactions.AllowUserToDeleteRows = false;
            dgvTransactions.AllowUserToResizeRows = false;
            dgvTransactions.ReadOnly = true;
            dgvTransactions.MultiSelect = false;
            dgvTransactions.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTransactions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvTransactions.ColumnHeadersHeight = 42;
            dgvTransactions.RowTemplate.Height = 38;
            dgvTransactions.DefaultCellStyle.Padding = new Padding(6, 0, 6, 0);
            dgvTransactions.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(245, 247, 250);
            dgvTransactions.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
        }

        private void ApplyTransactionsGridColumns()
        {

            if (dgvTransactions.Columns.Contains("Type"))
            {
                dgvTransactions.Columns["Type"].HeaderText = "Type";
                dgvTransactions.Columns["Type"].FillWeight = 22;
            }

            if (dgvTransactions.Columns.Contains("Amount"))
            {
                dgvTransactions.Columns["Amount"].HeaderText = "Amount";
                dgvTransactions.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvTransactions.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvTransactions.Columns["Amount"].FillWeight = 18;
            }

            if (dgvTransactions.Columns.Contains("Currency"))
            {
                dgvTransactions.Columns["Currency"].HeaderText = "Currency";
                dgvTransactions.Columns["Currency"].FillWeight = 14;
            }

            if (dgvTransactions.Columns.Contains("Description"))
            {
                dgvTransactions.Columns["Description"].HeaderText = "Details";
                dgvTransactions.Columns["Description"].FillWeight = 30;
            }

            if (dgvTransactions.Columns.Contains("Tran_Date"))
            {
                dgvTransactions.Columns["Tran_Date"].HeaderText = "Date";
                dgvTransactions.Columns["Tran_Date"].DefaultCellStyle.Format = "dd.MM.yyyy";
                dgvTransactions.Columns["Tran_Date"].FillWeight = 18;
            }
        }


        private void mainForn_Load(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }
        private void panel2_Paint(object sender, PaintEventArgs e) { }
        private void panel3_Paint(object sender, PaintEventArgs e) { }
        private void panel4_Paint(object sender, PaintEventArgs e) { }
        private void panel5_Paint(object sender, PaintEventArgs e) { }
        private void panel6_Paint(object sender, PaintEventArgs e) { }
        private void panel7_Paint(object sender, PaintEventArgs e) { }

        private void label1_Click(object sender, EventArgs e) { }
        private void label2_Click(object sender, EventArgs e) { }
        private void label3_Click(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void label5_Click(object sender, EventArgs e) { }

        private void panel6_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void ValidThru(object sender, EventArgs e)
        {

        }

        private void cardHldrName_Click(object sender, EventArgs e)
        {

        }

        private void cardNum_Click(object sender, EventArgs e)
        {

        }

        private void panel12_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = panel12.ClientRectangle;
            g.Clear(panel12.BackColor);

            using (Font titleFont = new Font("Segoe UI", 11, FontStyle.Bold))
            using (Font smallFont = new Font("Segoe UI", 9, FontStyle.Bold))
            using (Font axisFont = new Font("Segoe UI", 8))
            using (Brush textBrush = new SolidBrush(Color.FromArgb(80, 80, 80)))
            using (Pen linePen = new Pen(Color.FromArgb(98, 70, 255), 3))
            using (Brush pointFill = new SolidBrush(Color.White))
            using (Pen pointBorder = new Pen(Color.FromArgb(98, 70, 255), 2))
            {
                g.DrawString("Exchange Rates", titleFont, textBrush, 20, 15);
                g.DrawString(chartPairText, smallFont, textBrush, rect.Width - 100, 18);

                if (exchangeRates == null || exchangeRates.Count < 2)
                    return;

                int leftMargin = 55;
                int rightMargin = 20;
                int topMargin = 55;
                int bottomMargin = 30;

                int chartWidth = rect.Width - leftMargin - rightMargin;
                int chartHeight = rect.Height - topMargin - bottomMargin;

                decimal minRate = decimal.MaxValue;
                decimal maxRate = decimal.MinValue;

                foreach (decimal rate in exchangeRates)
                {
                    if (rate < minRate) minRate = rate;
                    if (rate > maxRate) maxRate = rate;
                }

                decimal padding = (maxRate - minRate) * 0.2m;
                if (padding == 0) padding = 0.01m;

                minRate -= padding;
                maxRate += padding;

                for (int i = 0; i < 4; i++)
                {
                    decimal value = minRate + ((maxRate - minRate) / 3m) * i;
                    float y = topMargin + chartHeight - ((float)(value - minRate) / (float)(maxRate - minRate) * chartHeight);
                    g.DrawString(Math.Round(value, 3).ToString(), axisFont, textBrush, 8, y - 8);
                }

                List<PointF> points = new List<PointF>();

                for (int i = 0; i < exchangeRates.Count; i++)
                {
                    float x = leftMargin + (chartWidth / (float)(exchangeRates.Count - 1)) * i;
                    float y = topMargin + chartHeight -
                              ((float)(exchangeRates[i] - minRate) / (float)(maxRate - minRate) * chartHeight);

                    points.Add(new PointF(x, y));
                }

                if (points.Count >= 2)
                {
                    GraphicsPath path = new GraphicsPath();
                    path.AddCurve(points.ToArray(), 0.5f);
                    g.DrawPath(linePen, path);
                }

                foreach (PointF p in points)
                {
                    g.FillEllipse(pointFill, p.X - 5, p.Y - 5, 10, 10);
                    g.DrawEllipse(pointBorder, p.X - 5, p.Y - 5, 10, 10);
                }
            }
        }

        private string GetUserNameByUsername(string username)
        {
            currentUsername = "";

            using (OracleConnection conn = new OracleConnection(connStr))
            {
                conn.Open();

                string query = "SELECT NAME FROM APP_USER WHERE USERNAME = :username";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    cmd.Parameters.Add(":username", OracleDbType.Varchar2).Value = username;

                    object result = cmd.ExecuteScalar();

                    if (result != null)
                        currentUsername = result.ToString().ToUpper();
                }
            }

            return currentUsername;
        }
        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void usrName_Click(object sender, EventArgs e)
        {


        }

        private void panel13_Paint(object sender, PaintEventArgs e)
        {

        }

        private void panel14_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void panel15_Paint(object sender, PaintEventArgs e)
        {

        }
    }
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public int Value { get; set; }

        public ComboBoxItem(string text, int value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }
}