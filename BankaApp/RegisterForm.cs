using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class RegisterForm : Form
    {
        string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";
        private static readonly Random rnd = new Random();

        public RegisterForm()
        {
            InitializeComponent();
            LoadCountries();

            pass.UseSystemPasswordChar = true;
            label4.Text = "";

            SetRoundedPanel(panel1, 30);
            panel1.Resize += panel1_Resize;


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
            panel.Region = new System.Drawing.Region(path);
        }

        private void panel1_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel1, 30);
        }

        private bool IsValidEGN(string egnValue)
        {
            return Regex.IsMatch(egnValue, @"^[0-9]{10}$");
        }

        private string GenerateRandomIban()
        {
            string countryCode = "BG";
            string checkDigits = rnd.Next(10, 99).ToString();
            string bankCode = "BANK";
            string branchCode = rnd.Next(1000, 9999).ToString();
            string accountPart = rnd.Next(10000000, 99999999).ToString();

            return countryCode + checkDigits + bankCode + branchCode + accountPart;
        }

        private string GenerateRandomCardNumber()
        {
            return $"{rnd.Next(4000, 5000)} {rnd.Next(1000, 10000)} {rnd.Next(1000, 10000)} {rnd.Next(1000, 10000)}";
        }

        private string GenerateValidThru()
        {
            return DateTime.Now.AddYears(10).ToString("MM/yy");
        }

        private void LoadCountries()
        {
            cmbCountry.Items.AddRange(new string[]
            {
                "Afghanistan","Albania","Algeria","Andorra","Angola","Argentina","Armenia","Australia","Austria","Azerbaijan",
                "Bahamas","Bahrain","Bangladesh","Barbados","Belarus","Belgium","Belize","Benin","Bhutan","Bolivia",
                "Bosnia and Herzegovina","Botswana","Brazil","Brunei","Bulgaria","Burkina Faso","Burundi","Cambodia","Cameroon","Canada",
                "Cape Verde","Central African Republic","Chad","Chile","China","Colombia","Comoros","Congo","Costa Rica","Croatia",
                "Cuba","Cyprus","Czech Republic","Denmark","Djibouti","Dominica","Dominican Republic","Ecuador","Egypt","El Salvador",
                "Equatorial Guinea","Eritrea","Estonia","Eswatini","Ethiopia","Fiji","Finland","France","Gabon","Gambia",
                "Georgia","Germany","Ghana","Greece","Grenada","Guatemala","Guinea","Guinea-Bissau","Guyana","Haiti",
                "Honduras","Hungary","Iceland","India","Indonesia","Iran","Iraq","Ireland","Israel","Italy",
                "Jamaica","Japan","Jordan","Kazakhstan","Kenya","Kiribati","Kuwait","Kyrgyzstan","Laos","Latvia",
                "Lebanon","Lesotho","Liberia","Libya","Liechtenstein","Lithuania","Luxembourg","Madagascar","Malawi","Malaysia",
                "Maldives","Mali","Malta","Marshall Islands","Mauritania","Mauritius","Mexico","Micronesia","Moldova","Monaco",
                "Mongolia","Montenegro","Morocco","Mozambique","Myanmar","Namibia","Nauru","Nepal","Netherlands","New Zealand",
                "Nicaragua","Niger","Nigeria","North Korea","North Macedonia","Norway","Oman","Pakistan","Palau","Panama",
                "Papua New Guinea","Paraguay","Peru","Philippines","Poland","Portugal","Qatar","Romania","Russia","Rwanda",
                "Saint Kitts and Nevis","Saint Lucia","Saint Vincent and the Grenadines","Samoa","San Marino","Sao Tome and Principe",
                "Saudi Arabia","Senegal","Serbia","Seychelles","Sierra Leone","Singapore","Slovakia","Slovenia","Solomon Islands","Somalia",
                "South Africa","South Korea","South Sudan","Spain","Sri Lanka","Sudan","Suriname","Sweden","Switzerland","Syria",
                "Taiwan","Tajikistan","Tanzania","Thailand","Timor-Leste","Togo","Tonga","Trinidad and Tobago","Tunisia","Turkey",
                "Turkmenistan","Tuvalu","Uganda","Ukraine","United Arab Emirates","United Kingdom","United States","Uruguay","Uzbekistan","Vanuatu",
                "Vatican City","Venezuela","Vietnam","Yemen","Zambia","Zimbabwe"
            });

            cmbCountry.SelectedIndex = 0;
        }

        private bool IsValidEmail(string emailValue)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(emailValue);
                return addr.Address == emailValue;
            }
            catch
            {
                return false;
            }
        }

        private bool IsStrongPassword(string password)
        {
            if (password.Length < 8)
                return false;

            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            foreach (char c in password)
            {
                if (char.IsUpper(c))
                    hasUpper = true;
                else if (char.IsLower(c))
                    hasLower = true;
                else if (char.IsDigit(c))
                    hasDigit = true;
                else
                    hasSpecial = true;
            }

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        private bool IsValidPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return false;

            return Regex.IsMatch(phone, @"^\+?[0-9]{8,15}$");
        }

        private bool RecordExists(OracleConnection conn, OracleTransaction transaction, string query, string paramName, object value)
        {
            using (OracleCommand cmd = new OracleCommand(query, conn))
            {
                cmd.Transaction = transaction;
                cmd.BindByName = true;
                cmd.Parameters.Add(paramName, value);

                object result = cmd.ExecuteScalar();
                return Convert.ToInt32(result) > 0;
            }
        }

        private bool StreetExists(OracleConnection conn, OracleTransaction transaction, int streetId)
        {
            return RecordExists(
                conn,
                transaction,
                "SELECT COUNT(*) FROM Street WHERE ID_Street = :streetId",
                ":streetId",
                streetId
            );
        }

        private void crtAccount_Click(object sender, EventArgs e)
        {
            label4.ForeColor = System.Drawing.Color.Red;
            label4.Text = "";

            string usernameValue = username.Text.Trim();
            string egnValue = EGN.Text.Trim();
            string addressValue = adress.Text.Trim();
            string streetIdValue = streetId.Text.Trim();
            string passwordValue = pass.Text.Trim();
            string emailValue = email.Text.Trim();
            string phoneValue = phoneNum.Text.Trim();
            string countryValue = cmbCountry.SelectedItem != null ? cmbCountry.SelectedItem.ToString() : "";
            string genderValue = "";

            int birthYear;
            int streetIdInt;
            int newClientId = 0;

            if (radioBtnMan.Checked)
                genderValue = "Male";
            else if (radioBtnWoman.Checked)
                genderValue = "Female";

            if (string.IsNullOrWhiteSpace(usernameValue) ||
                string.IsNullOrWhiteSpace(egnValue) ||
                string.IsNullOrWhiteSpace(addressValue) ||
                string.IsNullOrWhiteSpace(streetIdValue) ||
                string.IsNullOrWhiteSpace(passwordValue) ||
                string.IsNullOrWhiteSpace(emailValue) ||
                string.IsNullOrWhiteSpace(phoneValue) ||
                string.IsNullOrWhiteSpace(age.Text.Trim()) ||
                string.IsNullOrWhiteSpace(genderValue))
            {
                label4.Text = "Please fill in all required fields.";
                return;
            }

            if (!int.TryParse(age.Text.Trim(), out birthYear))
            {
                label4.Text = "Birth year must be a valid number.";
                return;
            }

            if (birthYear < 1900 || birthYear > DateTime.Now.Year)
            {
                label4.Text = "Birth year is invalid.";
                return;
            }

            if (!int.TryParse(streetIdValue, out streetIdInt))
            {
                label4.Text = "Street ID must be a valid number.";
                return;
            }

            if (!IsValidEGN(egnValue))
            {
                label4.Text = "EGN must contain exactly 10 digits.";
                return;
            }

            if (!IsValidEmail(emailValue))
            {
                label4.Text = "Please enter a valid email address.";
                return;
            }

            if (!IsValidPhone(phoneValue))
            {
                label4.Text = "Please enter a valid phone number.";
                return;
            }

            if (!IsStrongPassword(passwordValue))
            {
                label4.Text = "Password must be at least 8 characters and include uppercase, lowercase, number and symbol.";
                return;
            }

            try
            {
                using (OracleConnection conn = new OracleConnection(connStr))
                {
                    conn.Open();

                    using (OracleTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            if (!StreetExists(conn, transaction, streetIdInt))
                            {
                                label4.Text = "Invalid street ID. Please enter an existing street ID.";
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Email = :val", ":val", emailValue) ||
                                RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Email = :val", ":val", emailValue))
                            {
                                label4.Text = "This email is already registered.";
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Phone_number = :val", ":val", phoneValue) ||
                                RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Phone_number = :val", ":val", phoneValue))
                            {
                                label4.Text = "This phone number is already registered.";
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE EGN = :val", ":val", egnValue))
                            {
                                label4.Text = "This EGN is already registered.";
                                return;
                            }
                            string generatedCardNumber = GenerateRandomCardNumber();
                            string generatedValidThru = GenerateValidThru();
                            string appUserQuery = @"
                                                          INSERT INTO App_User
                               (ID_User, Username, User_Password, Email, User_Role, Phone_number, Gender, Birth_Year, Country, Card_Number, Valid_Thru)
                                VALUES
                               (SEQ_APP_USER.NEXTVAL, :username, :password, :email, :role, :phone, :gender, :birthyear, :country, :cardNumber, :validThru)";

                            using (OracleCommand cmdUser = new OracleCommand(appUserQuery, conn))
                            {
                                cmdUser.Transaction = transaction;
                                cmdUser.BindByName = true;

                                cmdUser.Parameters.Add(":username", OracleDbType.Varchar2).Value = usernameValue;
                                cmdUser.Parameters.Add(":password", OracleDbType.Varchar2).Value = passwordValue;
                                cmdUser.Parameters.Add(":email", OracleDbType.Varchar2).Value = emailValue;
                                cmdUser.Parameters.Add(":role", OracleDbType.Varchar2).Value = "Client";
                                cmdUser.Parameters.Add(":phone", OracleDbType.Varchar2).Value = phoneValue;
                                cmdUser.Parameters.Add(":gender", OracleDbType.Varchar2).Value = genderValue;
                                cmdUser.Parameters.Add(":birthyear", OracleDbType.Int32).Value = birthYear;
                                cmdUser.Parameters.Add(":country", OracleDbType.Varchar2).Value = countryValue;
                                cmdUser.Parameters.Add(":cardNumber", OracleDbType.Varchar2).Value = generatedCardNumber;
                                cmdUser.Parameters.Add(":validThru", OracleDbType.Varchar2).Value = generatedValidThru;

                                cmdUser.ExecuteNonQuery();
                            }

                            string clientQuery = @"
                                INSERT INTO Client
                                (Name, EGN, ID_Street, Adress, Phone_number, Is_Active, Email, Gender, Birth_Year, Country)
                                VALUES
                                (:name, :egn, :street, :adress, :phone, 1, :email, :gender, :birthyear, :country)
                                RETURNING Client_ID INTO :newClientId";

                            using (OracleCommand cmdClient = new OracleCommand(clientQuery, conn))
                            {
                                cmdClient.Transaction = transaction;
                                cmdClient.BindByName = true;

                                cmdClient.Parameters.Add(":name", OracleDbType.Varchar2).Value = usernameValue;
                                cmdClient.Parameters.Add(":egn", OracleDbType.Varchar2).Value = egnValue;
                                cmdClient.Parameters.Add(":street", OracleDbType.Int32).Value = streetIdInt;
                                cmdClient.Parameters.Add(":adress", OracleDbType.Varchar2).Value = addressValue;
                                cmdClient.Parameters.Add(":phone", OracleDbType.Varchar2).Value = phoneValue;
                                cmdClient.Parameters.Add(":email", OracleDbType.Varchar2).Value = emailValue;
                                cmdClient.Parameters.Add(":gender", OracleDbType.Varchar2).Value = genderValue;
                                cmdClient.Parameters.Add(":birthyear", OracleDbType.Int32).Value = birthYear;
                                cmdClient.Parameters.Add(":country", OracleDbType.Varchar2).Value = countryValue;

                                OracleParameter outClientId = new OracleParameter(":newClientId", OracleDbType.Int32);
                                outClientId.Direction = ParameterDirection.Output;
                                cmdClient.Parameters.Add(outClientId);

                                cmdClient.ExecuteNonQuery();
                                newClientId = Convert.ToInt32(((OracleDecimal)outClientId.Value).Value);
                            }

                            bool accountCreated = false;
                            int ibanAttempts = 0;

                            while (!accountCreated && ibanAttempts < 20)
                            {
                                try
                                {
                                    string generatedIban = GenerateRandomIban();

                                    string accountQuery = @"
                                        INSERT INTO Account
                                        (Account_NO, ID_Client, ID_Currency_type, Interest, Availibility)
                                        VALUES
                                        (:iban, :clientId, :currencyType, :interest, :availibility)";

                                    using (OracleCommand cmdAccount = new OracleCommand(accountQuery, conn))
                                    {
                                        cmdAccount.Transaction = transaction;
                                        cmdAccount.BindByName = true;

                                        cmdAccount.Parameters.Add(":iban", OracleDbType.Varchar2).Value = generatedIban;
                                        cmdAccount.Parameters.Add(":clientId", OracleDbType.Int32).Value = newClientId;
                                        cmdAccount.Parameters.Add(":currencyType", OracleDbType.Int32).Value = 1;
                                        cmdAccount.Parameters.Add(":interest", OracleDbType.Int32).Value = 1;
                                        cmdAccount.Parameters.Add(":availibility", OracleDbType.Int32).Value = 0;

                                        cmdAccount.ExecuteNonQuery();
                                        accountCreated = true;
                                    }
                                }
                                catch (OracleException ex)
                                {
                                    if (ex.Number == 1 && ex.Message.ToLower().Contains("uq_account_no"))
                                    {
                                        ibanAttempts++;
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                            }

                            if (!accountCreated)
                            {
                                throw new Exception("Could not create bank account automatically.");
                            }

                            transaction.Commit();

                            MessageBox.Show("Account created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            LoginForm loginForm = new LoginForm();
                            loginForm.Show();
                            this.Hide();
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number == 1)
                {
                    string errorText = ex.Message.ToLower();

                    if (errorText.Contains("uq_app_user_email") || errorText.Contains("uq_client_email"))
                        label4.Text = "This email is already registered.";
                    else if (errorText.Contains("uq_app_user_phone") || errorText.Contains("uq_client_phone"))
                        label4.Text = "This phone number is already registered.";
                    else if (errorText.Contains("uq_client_egn"))
                        label4.Text = "This EGN is already registered.";
                    else
                        label4.Text = "Duplicate value detected. Please check your input.";
                }
                else if (ex.Number == 2291)
                {
                    label4.Text = "Invalid street ID. Please enter an existing street ID.";
                }
                else if (ex.Number == 2290)
                {
                    label4.Text = "Invalid data. Please check your input values.";
                }
                else
                {
                    label4.Text = "Database error: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                label4.Text = "Error: " + ex.Message;
            }
        }

        private void RegisterForm_Load(object sender, EventArgs e) { }
        private void groupBox1_Enter(object sender, EventArgs e) { }
        private void cmbCountry_SelectedIndexChanged(object sender, EventArgs e) { }
        private void pass_TextChanged(object sender, EventArgs e) { }
        private void username_TextChanged(object sender, EventArgs e) { }
        private void email_TextChanged(object sender, EventArgs e) { }
        private void phoneNum_TextChanged(object sender, EventArgs e) { }
        private void age_TextChanged(object sender, EventArgs e) { }
        private void radioBtnWoman_CheckedChanged(object sender, EventArgs e) { }
        private void radioBtnMan_CheckedChanged(object sender, EventArgs e) { }
        private void label4_Click(object sender, EventArgs e) { }
        private void adress_TextChanged(object sender, EventArgs e) { }
        private void EGN_TextChanged(object sender, EventArgs e) { }
        private void streetId_TextChanged(object sender, EventArgs e) { }
        private void city_TextChanged(object sender, EventArgs e) { }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
            this.Hide();
        }
    }
}