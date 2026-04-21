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
        private AccountService accountService = new AccountService();
        private TransactionService transactionService = new TransactionService();
        private ExchangeRateService exchangeRateService = new ExchangeRateService();
        private TransferRequestService transferRequestService = new TransferRequestService();

        private bool isCvvVisible = false;
        private List<decimal> exchangeRates = new List<decimal>();
        private string chartPairText = "Lei = EUR";
        private string currentUsername;
        private int? selectedAccountId = null;
        private int currentClientId;
        private int currentAppUserId;
        private string realCVV = "";

        private bool isCardVisible = false;
        private string fullCardNumber = "";
        private string fullCardHolderName = "";
        private string fullValidThru = "";

        private bool isBalanceVisible = false;
        private decimal currentBalance = 0m;
        private int currentCurrencyId = 0;
        private string currentAccountNo = "";

        public mainForn(int clientId, int userId, string username)
        {
            InitializeComponent();

            currentClientId = clientId;
            currentAppUserId = userId;
            currentUsername = username;
            panelAccountDetails.Resize += panelAccountDetails_Resize;
            ThemeManager.ApplyTheme(this);
            ApplyModernUi();
            panel2.Tag = "KeepColor";
            panelAccountDetails.Tag = "KeepColor";
            panel3.Tag = "KeepColor";
            panel4.Tag = "KeepColor";
            panel5.Tag = "KeepColor";
            panel6.Tag = "KeepColor";
            panel12.Tag = "KeepColor";
            panel13.Tag = "KeepColor";
            FormStateHelper.Attach(this);

            LanguageManager.LoadLanguage();
            ApplyTranslations();

            usrName.Text = string.IsNullOrWhiteSpace(currentUsername)
                ? "USER"
                : currentUsername.ToUpper();

            WireEvents();

            SetupApprovalsBell();
            SetupCvvButton();
            SetupCvvShow();

            LoadUserCVV();
            LoadCardInfo();
            LoadExchangeRatesForChart(1, 2, "EUR -> Lei");
            lblBalanceAmount.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            SetupTransactionsGrid();
            SetupTransactionFilters();
            LoadRecentTransactions();
            LoadUserAccountsList();
            LoadUserStatus();
            LoadApprovalBadge();
        }
        private void MakeCircularButton(Button btn)
        {
            System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
            path.AddEllipse(0, 0, btn.Width, btn.Height);
            btn.Region = new Region(path);
        }
        private void ApplyModernUi()
        {
            this.BackColor = Color.FromArgb(245, 247, 251);

            panel2.BackColor = Color.White;
            panel3.BackColor = Color.White;
            panel4.BackColor = Color.White;
            panel5.BackColor = Color.White;
            panel6.BackColor = Color.White;
            panel12.BackColor = Color.White;

            SetRoundedPanel(panel2, 30);
            SetRoundedPanel(panel3, 30);
            SetRoundedPanel(panel4, 40);
            SetRoundedPanel(panel5, 40);
            SetRoundedPanel(panel6, 30);
            SetRoundedPanel(panel12, 30);
            SetRoundedPanel(panel13, 30);

            UiStyle.StylePrimaryButton(AddMoney);
            UiStyle.StylePrimaryButton(sendMoney);

            UiStyle.StyleSecondaryButton(filter);
            UiStyle.StyleSecondaryButton(reset);

            UiStyle.StyleGrid(dgvTransactions);

            UiStyle.StyleSectionTitle(label4);
            UiStyle.StyleSectionTitle(label10);
            UiStyle.StyleSectionTitle(label15);

            panel12.Invalidate();
            panel13.Invalidate();
        }

        private void WireEvents()
        {
            dgvTransactions.DataBindingComplete += dgvTransactions_DataBindingComplete;
            listAccounts.SelectedIndexChanged += listAccounts_SelectedIndexChanged;
            listAccounts.DrawItem += listAccounts_DrawItem;
            panelAccountDetails.Resize += panelAccountDetails_Resize;
            panel2.Resize += panel2_Resize;
            panel3.Resize += panel3_Resize;
            panel4.Resize += panel4_Resize;
            panel5.Resize += panel5_Resize;
            panel6.Resize += panel6_Resize;
            panel12.Resize += panel12_Resize;
            panel13.Resize += panel13_Resize;

            panel12.Paint += panel12_Paint;
            panel13.Paint += panel13_Paint;

            filter.Click += btnFilter_Click;
            reset.Click += btnReset_Click;
        }
        private void panelAccountDetails_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panelAccountDetails, 30);

            btnCopyIBAN.Left = panelAccountDetails.Width - btnCopyIBAN.Width - 15;
            lblAccountNumber.Width = btnCopyIBAN.Left - lblAccountNumber.Left - 8;
        }
        private void SetActiveMenu(Button activeButton)
        {
            Button[] buttons = { homeBtn, secrty, profl, button1 };

            foreach (Button btn in buttons)
                btn.BackColor = Color.FromArgb(67, 97, 238);

            activeButton.BackColor = Color.FromArgb(49, 76, 194);
        }

        private void AddHoverEffect(Button btn, Color normalColor, Color hoverColor)
        {
            btn.MouseEnter += (s, e) => btn.BackColor = hoverColor;
            btn.MouseLeave += (s, e) => btn.BackColor = normalColor;
        }

        private void StyleSidebarButton(Button btn)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.BackColor = Color.FromArgb(67, 97, 238);
            btn.ForeColor = Color.White;
            btn.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(18, 0, 0, 0);
            btn.Cursor = Cursors.Hand;
        }

        private bool CheckUserPassword(string password)
        {
            try
            {
                string query = @"
            SELECT User_Password
            FROM App_User
            WHERE ID_User = :userId";

                DataTable dt = DatabaseHelper.ExecuteDataTable(
                    query,
                    new OracleParameter("userId", OracleDbType.Int32) { Value = currentAppUserId }
                );

                if (dt.Rows.Count == 0)
                    return false;

                string storedHash = dt.Rows[0]["User_Password"].ToString();

                return PasswordHelper.VerifyPassword(password, storedHash);
            }
            catch
            {
                return false;
            }
        }


        private void cvvShow_Click_1(object sender, EventArgs e)
        {
            isCardVisible = !isCardVisible;
            cvvShow.Text = isCardVisible ? "🙈" : "👁";
            RefreshCardDisplay();

        }

        private void LoadUserCVV()
        {
            try
            {
                realCVV = accountService.LoadUserCVV(currentAppUserId);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading CVV: " + ex.Message);
            }
        }

        private void LoadCardInfo()
        {
            try
            {
                string fullName = accountService.LoadCardHolderName(currentClientId);
                DataTable dt = accountService.LoadCardData(currentAppUserId);

                string cardNumber = "";
                string validThru = "";

                if (dt.Rows.Count > 0)
                {
                    cardNumber = dt.Rows[0]["CARD_NUMBER"] == DBNull.Value ? "" : dt.Rows[0]["CARD_NUMBER"].ToString().Trim();
                    validThru = dt.Rows[0]["VALID_THRU"] == DBNull.Value ? "" : dt.Rows[0]["VALID_THRU"].ToString().Trim();
                }

                fullCardHolderName = string.IsNullOrWhiteSpace(fullName) ? "NO NAME" : fullName.ToUpper();
                fullCardNumber = string.IsNullOrWhiteSpace(cardNumber) ? "NO CARD" : cardNumber;
                fullValidThru = string.IsNullOrWhiteSpace(validThru) ? "--/--" : validThru;

                RefreshCardDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading card info: " + ex.Message);
            }
        }

        private string MaskCardNumber(string cardNumber)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                return "**** **** **** ****";

            string clean = cardNumber.Replace(" ", "").Trim();
            if (clean.Length < 8)
                return "**** **** **** ****";

            string first4 = clean.Substring(0, 4);
            string last4 = clean.Substring(clean.Length - 4);
            return $"{first4} •••• •••• {last4}";
        }
        private string MaskCardHolder(string holder)
        {
            if (string.IsNullOrWhiteSpace(holder))
                return "CARD HOLDER";

            string[] parts = holder.Trim().Split(' ');
            if (parts.Length == 1)
                return parts[0].Substring(0, Math.Min(2, parts[0].Length)) + "••••";

            string first = parts[0];
            string last = parts[parts.Length - 1];
            return first.Substring(0, Math.Min(2, first.Length)) + "••• " + last.Substring(0, Math.Min(2, last.Length)) + "•••";
        }
        private void RefreshCardDisplay()
        {
            cardNum.Text = isCardVisible ? fullCardNumber : MaskCardNumber(fullCardNumber);
            cardHldrName.Text = isCardVisible ? fullCardHolderName : MaskCardHolder(fullCardHolderName);
            label11.Text = fullValidThru;
            panel13.Invalidate();
        }

        private void ColorTypeColumn()
        {
            if (!dgvTransactions.Columns.Contains("Type"))
                return;

            foreach (DataGridViewRow row in dgvTransactions.Rows)
            {
                if (row.IsNewRow || row.Cells["Type"].Value == null)
                    continue;

                string type = row.Cells["Type"].Value.ToString().Trim().ToLower();

                if (type == "deposit")
                {
                    row.Cells["Type"].Style.ForeColor = Color.Green;
                    row.Cells["Type"].Style.SelectionForeColor = Color.Green;
                }
                else if (type == "withdrawal")
                {
                    row.Cells["Type"].Style.ForeColor = Color.Red;
                    row.Cells["Type"].Style.SelectionForeColor = Color.Red;
                }
                else if (type == "transfer")
                {
                    row.Cells["Type"].Style.ForeColor = Color.RoyalBlue;
                    row.Cells["Type"].Style.SelectionForeColor = Color.RoyalBlue;
                }
                else if (type == "fee")
                {
                    row.Cells["Type"].Style.ForeColor = Color.Gray;
                    row.Cells["Type"].Style.SelectionForeColor = Color.Gray;
                }
                else if (type == "interest")
                {
                    row.Cells["Type"].Style.ForeColor = Color.DarkOrange;
                    row.Cells["Type"].Style.SelectionForeColor = Color.DarkOrange;
                }

                row.Cells["Type"].Style.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            }
        }

        private void listAccounts_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;

            e.DrawBackground();
            ListBox lb = (ListBox)sender;
            string text = lb.Items[e.Index].ToString();
            bool isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;

            Color backColor = isSelected ? Color.FromArgb(98, 70, 255) : Color.White;
            Color textColor = isSelected ? Color.White : Color.FromArgb(45, 45, 45);

            using (SolidBrush backBrush = new SolidBrush(backColor))
            using (SolidBrush textBrush = new SolidBrush(textColor))
            {
                e.Graphics.FillRectangle(backBrush, e.Bounds);
                Rectangle textRect = new Rectangle(e.Bounds.X + 12, e.Bounds.Y + 9, e.Bounds.Width - 12, e.Bounds.Height - 9);
                e.Graphics.DrawString(text, new Font("Segoe UI", 10, FontStyle.Bold), textBrush, textRect);
            }

            e.DrawFocusRectangle();
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

        private void listAccounts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listAccounts.SelectedItem != null)
            {
                ComboBoxItem selected = (ComboBoxItem)listAccounts.SelectedItem;
                selectedAccountId = selected.Value;
                LoadSelectedAccountBalance();
                LoadRecentTransactions();
            }
        }

        private void LoadUserAccountsList()
        {
            try
            {
                DataTable dt = accountService.LoadUserAccounts(currentClientId);

                listAccounts.Items.Clear();

                foreach (DataRow row in dt.Rows)
                {
                    listAccounts.Items.Add(
                        new ComboBoxItem(
                            row["Account_NO"].ToString(),
                            Convert.ToInt32(row["ID_Account"])
                        )
                    );
                }

                if (listAccounts.Items.Count > 0)
                    listAccounts.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading accounts: " + ex.Message);
            }
        }
        private void LoadExchangeRatesForChart(int fromCurrencyId, int toCurrencyId, string pairText)
        {
            exchangeRates.Clear();
            chartPairText = pairText;

            try
            {
                exchangeRates = exchangeRateService.LoadRates(fromCurrencyId, toCurrencyId, 6);

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
        }
        private void dgvTransactions_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            ColorTypeColumn();
        }
        private void LoadRecentTransactions()
        {
            try
            {
                DataTable dt = transactionService.LoadRecentTransactions(
                    currentClientId,
                    selectedAccountId
                );

                dgvTransactions.DataSource = dt;
                FormatTransactionsGrid();
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
            LoadRecentTransactions();
        }
        private void LoadFilteredTransactions()
        {
            try
            {
                DataTable dt = transactionService.LoadFilteredTransactions(
                    currentClientId,
                    dtpFrom.Value,
                    dtpTo.Value,
                    cmbType.Text,
                    selectedAccountId
                );

                dgvTransactions.DataSource = dt;
                FormatTransactionsGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error filtering transactions: " + ex.Message);
            }
        }
        private void panel12_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = panel12.ClientRectangle;
            g.Clear(panel12.BackColor);

            using (Pen borderPen = new Pen(Color.FromArgb(230, 233, 240), 1))
            {
                Rectangle borderRect = new Rectangle(0, 0, panel12.Width - 1, panel12.Height - 1);
                g.DrawRectangle(borderPen, borderRect);
            }

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

                using (Pen gridPen = new Pen(Color.FromArgb(240, 242, 247), 1))
                {
                    for (int i = 0; i < 4; i++)
                    {
                        float y = topMargin + (chartHeight / 3f) * i;
                        g.DrawLine(gridPen, leftMargin, y, leftMargin + chartWidth, y);
                    }
                }

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
                    float y = topMargin + chartHeight - ((float)(exchangeRates[i] - minRate) / (float)(maxRate - minRate) * chartHeight);
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
        private void panel13_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rect = panel13.ClientRectangle;

            using (LinearGradientBrush brush = new LinearGradientBrush(rect, Color.FromArgb(22, 78, 255), Color.FromArgb(0, 186, 255), 25f))
            {
                g.FillRectangle(brush, rect);
            }

            using (Pen softPen = new Pen(Color.FromArgb(255, 255, 255, 50), 1))
            {
                g.DrawEllipse(softPen, rect.Width - 90, rect.Height - 90, 120, 120);
                g.DrawEllipse(softPen, rect.Width - 50, -20, 80, 80);
            }

            using (Brush whiteBrush = new SolidBrush(Color.White))
            using (Brush lightBrush = new SolidBrush(Color.FromArgb(230, 240, 255)))
            using (Font brandFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Font chipFont = new Font("Segoe UI", 18, FontStyle.Bold))
            using (Font numFont = new Font("Consolas", 16, FontStyle.Bold))
            using (Font labelFont = new Font("Segoe UI", 8, FontStyle.Bold))
            using (Font valueFont = new Font("Segoe UI", 11, FontStyle.Bold))
            {
                g.DrawString("VISA", brandFont, whiteBrush, 22, 18);
                g.DrawString("MeliPay", brandFont, whiteBrush, rect.Width - 125, 18);

                using (Brush chipBrush = new SolidBrush(Color.FromArgb(255, 215, 90)))
                using (Brush chipTextBrush = new SolidBrush(Color.FromArgb(140, 100, 10)))
                {
                    g.FillRectangle(chipBrush, 24, 58, 42, 28);
                    g.DrawString("✦", chipFont, chipTextBrush, 32, 55);
                }

                g.DrawString(cardNum.Text, numFont, whiteBrush, 22, 105);
                g.DrawString("VALID THRU", labelFont, lightBrush, 22, 150);
                g.DrawString(label11.Text, valueFont, whiteBrush, 22, 168);
                g.DrawString("CARD HOLDER", labelFont, lightBrush, 150, 150);
                g.DrawString(cardHldrName.Text, valueFont, whiteBrush, 150, 168);
            }
        }
        private void SetRoundedPanel(Panel panel, int radius)
        {
            if (panel.Width <= 1 || panel.Height <= 1)
                return;

            using (GraphicsPath path = new GraphicsPath())
            {
                int d = radius * 2;

                path.StartFigure();
                path.AddArc(0, 0, d, d, 180, 90);
                path.AddArc(panel.Width - d - 1, 0, d, d, 270, 90);
                path.AddArc(panel.Width - d - 1, panel.Height - d - 1, d, d, 0, 90);
                path.AddArc(0, panel.Height - d - 1, d, d, 90, 90);
                path.CloseFigure();

                if (panel.Region != null)
                    panel.Region.Dispose();

                panel.Region = new Region(path);
            }
        }
        private void panel2_Resize(object sender, EventArgs e) => SetRoundedPanel(panel2, 30);
        private void panel3_Resize(object sender, EventArgs e) => SetRoundedPanel(panel3, 30);
        private void panel4_Resize(object sender, EventArgs e) => SetRoundedPanel(panel4, 40);
        private void panel5_Resize(object sender, EventArgs e) => SetRoundedPanel(panel5, 40);
        private void panel6_Resize(object sender, EventArgs e) => SetRoundedPanel(panel6, 30);
        private void panel7_Resize(object sender, EventArgs e) => SetRoundedPanel(panel3, 30);
        private void panel12_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel12, 30);
            panel12.Invalidate();
        }
        private void panel13_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel13, 30);
            panel13.Invalidate();
        }
        private void profl_Click(object sender, EventArgs e)
        {
            ProfileForm form = new ProfileForm(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            Hide();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            settings form = new settings(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            this.Hide();
        }
        private void homeBtn_Click(object sender, EventArgs e)
        {
            SetActiveMenu(homeBtn);
        }
        private void secrty_Click(object sender, EventArgs e)
        {
            securityForm form = new securityForm(currentClientId, currentAppUserId, currentUsername);
            form.Show();
            Hide();
        }
        private void cvvBtn_Click(object sender, EventArgs e)
        {
            if (!isCvvVisible)
            {
                string enteredPassword = Prompt.ShowDialog("Enter your password:", "CVV Verification");

                if (string.IsNullOrWhiteSpace(enteredPassword))
                    return;

                if (CheckUserPassword(enteredPassword))
                {
                    cvvBtn.Text = string.IsNullOrWhiteSpace(realCVV) ? "N/A" : realCVV;
                    isCvvVisible = true;
                }
                else
                {
                    MessageBox.Show("Incorrect password.");
                }
            }
            else
            {
                cvvBtn.Text = "CVV 👁";
                isCvvVisible = false;
            }
        }
        private void SetupCvvButton()
        {
            cvvBtn.Text = "CVV 👁";
            cvvBtn.Width = 50;
            cvvBtn.Height = 50;
            cvvBtn.FlatStyle = FlatStyle.Flat;
            cvvBtn.FlatAppearance.BorderSize = 0;
            cvvBtn.BackColor = Color.RoyalBlue;
            cvvBtn.ForeColor = Color.White;

            MakeCircularButton(cvvBtn);

            cvvBtn.Click -= cvvBtn_Click;
            cvvBtn.Click += cvvBtn_Click;
        }
        private void cvvBtn_Resize(object sender, EventArgs e)
        {
            MakeCircularButton(cvvBtn);
        }
        private void cvvShow_Resize(object sender, EventArgs e)
        {
            MakeCircularButton(cvvShow);
        }
        private void SetupCvvShow()
        {
            cvvShow.Text = "👁 Show";
            cvvShow.Width = 70;
            cvvShow.Height = 42;
            cvvShow.FlatStyle = FlatStyle.Flat;
            cvvShow.FlatAppearance.BorderSize = 0;
            cvvShow.BackColor = Color.RoyalBlue;
            cvvShow.ForeColor = Color.White;

            MakeCircularButton(cvvShow);
            cvvShow.Resize -= cvvShow_Resize;
            cvvShow.Resize += cvvShow_Resize;
        }
        private void AddMoney_Click(object sender, EventArgs e)
        {
            AddMoneyForm form = new AddMoneyForm(currentClientId, currentAppUserId, currentUsername);
            Hide();

            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshDashboardData();
            }
        }
        private void RefreshDashboardData()
        {
            LoadUserCVV();
            LoadCardInfo();
            LoadRecentTransactions();
            LoadUserAccountsList();
            LoadApprovalBadge();

        }
        private void sendMoney_Click(object sender, EventArgs e)
        {
            SendMoneyForm form = new SendMoneyForm (currentClientId, currentAppUserId, currentUsername);
            Hide();

            if (form.ShowDialog() == DialogResult.OK)
            {
                RefreshDashboardData();
            }
        }
        private void btnApprovalsBell_Click(object sender, EventArgs e)
        {
            ApprovalsForm form = new ApprovalsForm(currentClientId, currentAppUserId, currentUsername);
            form.ShowDialog();
            RefreshDashboardData();
        }
        private void LoadApprovalBadge()
        {
            try
            {
                int count = transferRequestService.GetPendingRequestsCount(currentClientId);

                if (count > 0)
                {
                    lblApprovalCount.Visible = true;
                    lblApprovalCount.Text = count > 99 ? "99+" : count.ToString();
                }
                else
                {
                    lblApprovalCount.Visible = false;
                }
            }
            catch
            {
                lblApprovalCount.Visible = false;
            }
        }
        private void SetupApprovalsBell()
        {
            btnApprovalsBell.Text = "🔔";
            btnApprovalsBell.Width = 48;
            btnApprovalsBell.Height = 48;
            btnApprovalsBell.FlatStyle = FlatStyle.Flat;
            btnApprovalsBell.FlatAppearance.BorderSize = 0;
            btnApprovalsBell.BackColor = Color.White;
            btnApprovalsBell.ForeColor = Color.FromArgb(0, 120, 215);
            btnApprovalsBell.Cursor = Cursors.Hand;
            btnApprovalsBell.Font = new Font("Segoe UI Emoji", 18, FontStyle.Regular);

            MakeCircularButton(btnApprovalsBell);

            lblApprovalCount.AutoSize = false;
            lblApprovalCount.Width = 22;
            lblApprovalCount.Height = 22;
            lblApprovalCount.TextAlign = ContentAlignment.MiddleCenter;
            lblApprovalCount.Font = new Font("Segoe UI", 8, FontStyle.Bold);
            lblApprovalCount.BackColor = Color.FromArgb(220, 53, 69);
            lblApprovalCount.ForeColor = Color.White;

            MakeCircularLabel(lblApprovalCount);

            lblApprovalCount.BringToFront();
            lblApprovalCount.Location = new Point(
                btnApprovalsBell.Left + btnApprovalsBell.Width - 12,
                btnApprovalsBell.Top - 4
            );
        }
        private void MakeCircularLabel(Label lbl)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, lbl.Width, lbl.Height);
            lbl.Region = new Region(path);
        }
        private void LoadSelectedAccountBalance()
        {
            if (!selectedAccountId.HasValue)
                return;

            try
            {
                int accountId = selectedAccountId.Value;
                AccountBalanceInfo info = accountService.GetAccountBalanceInfo(accountId);

                if (info == null)
                {
                    lblBalanceAmount.Text = "N/A";
                    lblAccountNumber.Text = "No account";
                    return;
                }

                currentBalance = info.Balance;
                currentCurrencyId = info.CurrencyId;
                currentAccountNo = info.AccountNo;

                lblAccountNumber.Text = info.AccountNo;
                RefreshBalanceDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading balance: " + ex.Message);
            }
        }
        private string GetCurrencyText(int currencyId)
        {
            switch (currencyId)
            {
                case 1: return "Lei";
                case 2: return "€";
                case 3: return "$";
                case 4: return "£";
                case 5: return "CHF";
                case 6: return "AED";
                case 7: return "₺";
                default: return "";
            }
        }
        private void RefreshBalanceDisplay()
        {
            string currencyText = GetCurrencyText(currentCurrencyId);

            if (isBalanceVisible)
                lblBalanceAmount.Text = $"{currencyText} {currentBalance:N2}";
            else
                lblBalanceAmount.Text = $"{currencyText} ****";
        }
        private void btnToggleBalance_Click(object sender, EventArgs e)
        {
            isBalanceVisible = !isBalanceVisible;

            if (isBalanceVisible)
                btnToggleBalance.Text = "🙈";
            else
                btnToggleBalance.Text = "👁";

            RefreshBalanceDisplay();
        }
        private void btnCopyIBAN_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(currentAccountNo))
            {
                Clipboard.SetText(currentAccountNo);
                MessageBox.Show("IBAN copied!");
            }
        }
        private void ApplyTranslations()
        {
            homeBtn.Text = LanguageManager.GetText("home");
            profl.Text = LanguageManager.GetText("profile");
            secrty.Text = LanguageManager.GetText("security");
            button1.Text = LanguageManager.GetText("settings");

            AddMoney.Text = LanguageManager.GetText("add_money");
            sendMoney.Text = LanguageManager.GetText("send_money");

            filter.Text = LanguageManager.GetText("filter");
            reset.Text = LanguageManager.GetText("reset");

            label15.Text = LanguageManager.GetText("welcome_back");
            label14.Text = LanguageManager.GetText("hi");
            label4.Text = LanguageManager.GetText("overview");
            label10.Text = LanguageManager.GetText("accounts");
            label16.Text = LanguageManager.GetText("from");
            label17.Text = LanguageManager.GetText("to");
            label18.Text = LanguageManager.GetText("type");
            lblBalanceAmount.Text = LanguageManager.GetText("amount");

        }
        private void LoadUserStatus()
        {
            try
            {
                using (OracleConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    string query = "SELECT Is_Active FROM Client WHERE Client_ID = :clientId";

                    using (OracleCommand cmd = new OracleCommand(query, conn))
                    {
                        cmd.Parameters.Add("clientId", OracleDbType.Int32).Value = currentClientId;

                        object result = cmd.ExecuteScalar();

                        if (result == null || result == DBNull.Value)
                        {
                            status.Text = "Unknown";
                            status.ForeColor = Color.Gray;
                            return;
                        }

                        int isActive = Convert.ToInt32(result);

                        if (isActive == 1)
                        {
                            status.Text = "Active";
                            status.ForeColor = Color.Green;
                        }
                        else
                        {
                            status.Text = "Blocked";
                            status.ForeColor = Color.Red;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading status: " + ex.Message);
            }
        }
    }
}
