using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace BankaApp
{
    public partial class RegisterForm : Form
    {
        private static readonly Random rnd = new Random();
        public RegisterForm()
        {
            InitializeComponent();
            LoadCountries();

            pass.UseSystemPasswordChar = true;
            label4.Text = "";

            SetRoundedPanel(panel2, 40);
            panel2.Resize += panel2_Resize;

            button2.Width = 50;
            button2.Height = 50;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.BackColor = Color.RoyalBlue;
            button2.ForeColor = Color.White;
            MakeButtonRound(button2);
            button2.Resize += button2_Resize;
            button2.Resize += button2_Resize;

            FormStateHelper.Attach(this);
            ThemeManager.ApplyTheme(this);



            LanguageManager.LoadLanguage();
            ApplyTranslations();


        }
        private int GetOrCreateCountryId(OracleConnection conn, OracleTransaction transaction, string countryName)
        {
            string selectQuery = @"
        SELECT ID_Country
        FROM Country
        WHERE UPPER(TRIM(Country)) = UPPER(TRIM(:countryName))";

            using (OracleCommand cmdSelect = new OracleCommand(selectQuery, conn))
            {
                cmdSelect.Transaction = transaction;
                cmdSelect.BindByName = true;
                cmdSelect.Parameters.Add(":countryName", OracleDbType.Varchar2).Value = countryName.Trim();

                object result = cmdSelect.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
            }

            string insertQuery = @"
        INSERT INTO Country (ID_Country, Country)
        VALUES (seq_country.NEXTVAL, :countryName)
        RETURNING ID_Country INTO :newCountryId";

            using (OracleCommand cmdInsert = new OracleCommand(insertQuery, conn))
            {
                cmdInsert.Transaction = transaction;
                cmdInsert.BindByName = true;
                cmdInsert.Parameters.Add(":countryName", OracleDbType.Varchar2).Value = countryName.Trim();

                OracleParameter outParam = new OracleParameter(":newCountryId", OracleDbType.Int32);
                outParam.Direction = ParameterDirection.Output;
                cmdInsert.Parameters.Add(outParam);

                cmdInsert.ExecuteNonQuery();
                return Convert.ToInt32(((OracleDecimal)outParam.Value).Value);
            }
        }

        private int GetOrCreateCityId(OracleConnection conn, OracleTransaction transaction, string cityName, int countryId)
        {
            string selectQuery = @"
        SELECT ID_City
        FROM City
        WHERE UPPER(TRIM(City)) = UPPER(TRIM(:cityName))
          AND ID_Country = :countryId";

            using (OracleCommand cmdSelect = new OracleCommand(selectQuery, conn))
            {
                cmdSelect.Transaction = transaction;
                cmdSelect.BindByName = true;
                cmdSelect.Parameters.Add(":cityName", OracleDbType.Varchar2).Value = cityName.Trim();
                cmdSelect.Parameters.Add(":countryId", OracleDbType.Int32).Value = countryId;

                object result = cmdSelect.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
            }

            string insertQuery = @"
        INSERT INTO City (ID_City, City, ID_Country, Postal_code)
        VALUES (seq_city.NEXTVAL, :cityName, :countryId, NULL)
        RETURNING ID_City INTO :newCityId";

            using (OracleCommand cmdInsert = new OracleCommand(insertQuery, conn))
            {
                cmdInsert.Transaction = transaction;
                cmdInsert.BindByName = true;
                cmdInsert.Parameters.Add(":cityName", OracleDbType.Varchar2).Value = cityName.Trim();
                cmdInsert.Parameters.Add(":countryId", OracleDbType.Int32).Value = countryId;

                OracleParameter outParam = new OracleParameter(":newCityId", OracleDbType.Int32);
                outParam.Direction = ParameterDirection.Output;
                cmdInsert.Parameters.Add(outParam);

                cmdInsert.ExecuteNonQuery();
                return Convert.ToInt32(((OracleDecimal)outParam.Value).Value);
            }
        }

        private int GetOrCreateStreetId(OracleConnection conn, OracleTransaction transaction, string streetName, int cityId)
        {
            string selectQuery = @"
        SELECT ID_Street
        FROM Street
        WHERE UPPER(TRIM(Street)) = UPPER(TRIM(:streetName))
          AND ID_City = :cityId";

            using (OracleCommand cmdSelect = new OracleCommand(selectQuery, conn))
            {
                cmdSelect.Transaction = transaction;
                cmdSelect.BindByName = true;
                cmdSelect.Parameters.Add(":streetName", OracleDbType.Varchar2).Value = streetName.Trim();
                cmdSelect.Parameters.Add(":cityId", OracleDbType.Int32).Value = cityId;

                object result = cmdSelect.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    return Convert.ToInt32(result);
            }

            string insertQuery = @"
        INSERT INTO Street (ID_Street, Street, ID_City)
        VALUES (seq_street.NEXTVAL, :streetName, :cityId)
        RETURNING ID_Street INTO :newStreetId";

            using (OracleCommand cmdInsert = new OracleCommand(insertQuery, conn))
            {
                cmdInsert.Transaction = transaction;
                cmdInsert.BindByName = true;
                cmdInsert.Parameters.Add(":streetName", OracleDbType.Varchar2).Value = streetName.Trim();
                cmdInsert.Parameters.Add(":cityId", OracleDbType.Int32).Value = cityId;

                OracleParameter outParam = new OracleParameter(":newStreetId", OracleDbType.Int32);
                outParam.Direction = ParameterDirection.Output;
                cmdInsert.Parameters.Add(outParam);

                cmdInsert.ExecuteNonQuery();
                return Convert.ToInt32(((OracleDecimal)outParam.Value).Value);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            LanguageManager.ToggleLanguage();
            ApplyTranslations();
        }

        private void ApplyTranslations()
        {
            label1.Text = LanguageManager.GetText("register_form");

            fullNAme.Text = LanguageManager.GetText("full_name");
            label5.Text = LanguageManager.GetText("email");
            label6.Text = LanguageManager.GetText("phone");
            label7.Text = LanguageManager.GetText("password");
            label9.Text = LanguageManager.GetText("birth_year");
            label10.Text = LanguageManager.GetText("egn_lnc");

            label11.Text = LanguageManager.GetText("street");
            label8.Text = LanguageManager.GetText("address");
            label12.Text = LanguageManager.GetText("city");
            label3.Text = LanguageManager.GetText("country_required");
            label2.Text = LanguageManager.GetText("gender_required");

            radioBtnMan.Text = LanguageManager.GetText("male");
            radioBtnWoman.Text = LanguageManager.GetText("female");
            rbEGN.Text = LanguageManager.GetText("egn");
            rbLNC.Text = LanguageManager.GetText("idn");

            crtAccount.Text = LanguageManager.GetText("create_account");
            button1.Text = LanguageManager.GetText("back");

            username.PlaceholderText = LanguageManager.GetText("ph_full_name");
            email.PlaceholderText = LanguageManager.GetText("ph_email");
            phoneNum.PlaceholderText = LanguageManager.GetText("ph_phone");
            pass.PlaceholderText = LanguageManager.GetText("ph_password");
            age.PlaceholderText = LanguageManager.GetText("ph_birth_year");
            EGN.PlaceholderText = LanguageManager.GetText("ph_egn_lnc");
            streetId.PlaceholderText = LanguageManager.GetText("ph_street");
            adress.PlaceholderText = LanguageManager.GetText("ph_address");
            city.PlaceholderText = LanguageManager.GetText("ph_city");

            button2.Text = LanguageManager.IsBulgarian() ? "EN" : "BG";
        }

        private void button2_Resize(object sender, EventArgs e)
        {
            MakeButtonRound(button2);
            button2.Width = 50;
            button2.Height = 50;
            button2.FlatStyle = FlatStyle.Flat;
            button2.FlatAppearance.BorderSize = 0;
            button2.BackColor = System.Drawing.Color.RoyalBlue;
            button2.ForeColor = System.Drawing.Color.White;
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

        private void panel2_Resize(object sender, EventArgs e)
        {
            SetRoundedPanel(panel2, 40);
        }
        private void MakeButtonRound(Button btn)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddEllipse(0, 0, btn.Width, btn.Height);
            btn.Region = new Region(path);
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
        private string GenerateRandomCVV()
        {
            return rnd.Next(0, 1000).ToString("D3");
        }
        private string GenerateValidThru()
        {
            return DateTime.Now.AddYears(10).ToString(@"MM\/yy");
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
        private void crtAccount_Click(object sender, EventArgs e)
        {
            label4.ForeColor = System.Drawing.Color.Red;
            label4.Text = "";
            label4.Visible = false;

            string usernameValue = username.Text.Trim();
            string egnValue = EGN.Text.Trim();
            string streetNameValue = streetId.Text.Trim();
            string addressValue = adress.Text.Trim();
            string cityValue = city.Text.Trim();
            string passwordValue = pass.Text.Trim();
            string hashedPassword = PasswordHelper.HashPassword(passwordValue);
            string emailValue = email.Text.Trim();
            string phoneValue = phoneNum.Text.Trim();
            string countryValue = cmbCountry.SelectedItem != null ? cmbCountry.SelectedItem.ToString().Trim() : "";
            string genderValue = "";
            string generatedCVV = GenerateRandomCVV();
            string identityType = rbLNC.Checked ? "LNC" : "EGN";

            int birthYear;
            int newClientId = 0;
            int countryId = 0;
            int cityId = 0;
            int streetIdDb = 0;

            if (radioBtnMan.Checked)
                genderValue = "Male";
            else if (radioBtnWoman.Checked)
                genderValue = "Female";

            if (string.IsNullOrWhiteSpace(usernameValue) ||
                string.IsNullOrWhiteSpace(egnValue) ||
                string.IsNullOrWhiteSpace(streetNameValue) ||
                string.IsNullOrWhiteSpace(addressValue) ||
                string.IsNullOrWhiteSpace(cityValue) ||
                string.IsNullOrWhiteSpace(countryValue) ||
                string.IsNullOrWhiteSpace(passwordValue) ||
                string.IsNullOrWhiteSpace(emailValue) ||
                string.IsNullOrWhiteSpace(phoneValue) ||
                string.IsNullOrWhiteSpace(age.Text.Trim()) ||
                string.IsNullOrWhiteSpace(genderValue))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Please fill in all required fields."
                    : "Моля, попълнете всички задължителни полета.";
                label4.Visible = true;
                return;
            }

            if (!int.TryParse(age.Text.Trim(), out birthYear))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Birth year must be a valid number."
                    : "Годината на раждане трябва да е валидно число.";
                label4.Visible = true;
                return;
            }

            if (birthYear < 1900 || birthYear > DateTime.Now.Year)
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Birth year is invalid."
                    : "Невалидна година на раждане.";
                label4.Visible = true;
                return;
            }

            if (!IsValidEGN(egnValue))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? (identityType == "LNC"
                        ? "LNC must contain exactly 10 digits."
                        : "EGN must contain exactly 10 digits.")
                    : (identityType == "LNC"
                        ? "ЛНЧ трябва да съдържа точно 10 цифри."
                        : "ЕГН трябва да съдържа точно 10 цифри.");
                label4.Visible = true;
                return;
            }

            if (!IsValidEmail(emailValue))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Please enter a valid email address."
                    : "Моля, въведете валиден имейл адрес.";
                label4.Visible = true;
                return;
            }

            if (!IsValidPhone(phoneValue))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Please enter a valid phone number."
                    : "Моля, въведете валиден телефонен номер.";
                label4.Visible = true;
                return;
            }

            if (!IsStrongPassword(passwordValue))
            {
                label4.Text = LanguageManager.IsEnglish()
                    ? "Password must be at least 8 characters and include uppercase, lowercase, number and symbol."
                    : "Паролата трябва да е поне 8 символа и да съдържа главна буква, малка буква, число и символ.";
                label4.Visible = true;
                return;
            }

            try
            {
                using (OracleConnection conn = DatabaseHelper.GetConnection())
                {
                    conn.Open();

                    using (OracleTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Email = :val", ":val", emailValue) ||
                                RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Email = :val", ":val", emailValue))
                            {
                                label4.Text = LanguageManager.IsEnglish()
                                    ? "This email is already registered."
                                    : "Този имейл вече е регистриран.";
                                label4.Visible = true;
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Phone_number = :val", ":val", phoneValue) ||
                                RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Phone_number = :val", ":val", phoneValue))
                            {
                                label4.Text = LanguageManager.IsEnglish()
                                    ? "This phone number is already registered."
                                    : "Този телефонен номер вече е регистриран.";
                                label4.Visible = true;
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE EGN = :val", ":val", egnValue))
                            {
                                label4.Text = LanguageManager.IsEnglish()
                                    ? (identityType == "LNC"
                                        ? "This LNC is already registered."
                                        : "This EGN is already registered.")
                                    : (identityType == "LNC"
                                        ? "Това ЛНЧ вече е регистрирано."
                                        : "Това ЕГН вече е регистрирано.");
                                label4.Visible = true;
                                return;
                            }

                            countryId = GetOrCreateCountryId(conn, transaction, countryValue);
                            cityId = GetOrCreateCityId(conn, transaction, cityValue, countryId);
                            streetIdDb = GetOrCreateStreetId(conn, transaction, streetNameValue, cityId);

                            string generatedCardNumber = GenerateRandomCardNumber();
                            string generatedValidThru = GenerateValidThru();

                            string appUserQuery = @"
                        INSERT INTO App_User
                        (ID_User, Username, User_Password, Email, User_Role, Phone_number, Gender, Birth_Year, Country, Card_Number, Valid_Thru, CVV)
                        VALUES
                        (SEQ_APP_USER.NEXTVAL, :username, :password, :email, :role, :phone, :gender, :birthyear, :country, :cardNumber, :validThru, :cvv)";

                            using (OracleCommand cmdUser = new OracleCommand(appUserQuery, conn))
                            {
                                cmdUser.Transaction = transaction;
                                cmdUser.BindByName = true;

                                cmdUser.Parameters.Add(":username", OracleDbType.Varchar2).Value = usernameValue;
                                cmdUser.Parameters.Add(":password", OracleDbType.Varchar2).Value = hashedPassword;
                                cmdUser.Parameters.Add(":email", OracleDbType.Varchar2).Value = emailValue;
                                cmdUser.Parameters.Add(":role", OracleDbType.Varchar2).Value = "Client";
                                cmdUser.Parameters.Add(":phone", OracleDbType.Varchar2).Value = phoneValue;
                                cmdUser.Parameters.Add(":gender", OracleDbType.Varchar2).Value = genderValue;
                                cmdUser.Parameters.Add(":birthyear", OracleDbType.Int32).Value = birthYear;
                                cmdUser.Parameters.Add(":country", OracleDbType.Varchar2).Value = countryValue;
                                cmdUser.Parameters.Add(":cardNumber", OracleDbType.Varchar2).Value = generatedCardNumber;
                                cmdUser.Parameters.Add(":validThru", OracleDbType.Varchar2).Value = generatedValidThru;
                                cmdUser.Parameters.Add(":cvv", OracleDbType.Varchar2).Value = generatedCVV;

                                cmdUser.ExecuteNonQuery();
                            }
                            string clientQuery = @"
    INSERT INTO Client
    (Name, EGN, Identity_Type, ID_Street, Street_Name, Adress, Phone_number, Is_Active, Email, Gender, Birth_Year, Country)
    VALUES
    (:name, :egn, :identityType, :streetId, :streetName, :adress, :phone, 1, :email, :gender, :birthyear, :country)
    RETURNING Client_ID INTO :newClientId";

                            using (OracleCommand cmdClient = new OracleCommand(clientQuery, conn))
                            {
                                cmdClient.Transaction = transaction;
                                cmdClient.BindByName = true;

                                cmdClient.Parameters.Add(":name", OracleDbType.Varchar2).Value = usernameValue;
                                cmdClient.Parameters.Add(":egn", OracleDbType.Varchar2).Value = egnValue;
                                cmdClient.Parameters.Add(":identityType", OracleDbType.Varchar2).Value = identityType;
                                cmdClient.Parameters.Add(":streetId", OracleDbType.Int32).Value = streetIdDb;
                                cmdClient.Parameters.Add(":streetName", OracleDbType.Varchar2).Value = streetNameValue;
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
                                        cmdAccount.Parameters.Add(":currencyType", OracleDbType.Int32).Value = 2;
                                        cmdAccount.Parameters.Add(":interest", OracleDbType.Int32).Value = 1;
                                        cmdAccount.Parameters.Add(":availibility", OracleDbType.Int32).Value = 0;

                                        cmdAccount.ExecuteNonQuery();
                                        accountCreated = true;
                                    }
                                }
                                catch (OracleException ex)
                                {
                                    if (ex.Number == 1 && ex.Message.ToLower().Contains("uq_account_no"))
                                        ibanAttempts++;
                                    else
                                        throw;
                                }
                            }

                            if (!accountCreated)
                                throw new Exception(LanguageManager.IsEnglish()
                                    ? "Could not create bank account automatically."
                                    : "Банковата сметка не можа да бъде създадена автоматично.");

                            transaction.Commit();

                            MessageBox.Show(
                                LanguageManager.IsEnglish() ? "Account created successfully." : "Акаунтът беше създаден успешно.",
                                LanguageManager.IsEnglish() ? "Success" : "Успех",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );

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
                label4.Visible = true;

                if (ex.Number == 1)
                {
                    string errorText = ex.Message.ToLower();

                    if (errorText.Contains("uq_app_user_email") || errorText.Contains("uq_client_email"))
                        label4.Text = LanguageManager.IsEnglish()
                            ? "This email is already registered."
                            : "Този имейл вече е регистриран.";
                    else if (errorText.Contains("uq_app_user_phone") || errorText.Contains("uq_client_phone"))
                        label4.Text = LanguageManager.IsEnglish()
                            ? "This phone number is already registered."
                            : "Този телефонен номер вече е регистриран.";
                    else if (errorText.Contains("uq_client_egn"))
                        label4.Text = LanguageManager.IsEnglish()
                            ? "This EGN is already registered."
                            : "Това ЕГН вече е регистрирано.";
                    else
                        label4.Text = LanguageManager.IsEnglish()
                            ? "Duplicate value detected. Please check your input."
                            : "Открита е повтаряща се стойност. Моля, проверете данните.";
                }
                else if (ex.Number == 2290)
                {
                    label4.Text = LanguageManager.IsEnglish()
                        ? "Invalid data. Please check your input values."
                        : "Невалидни данни. Моля, проверете въведените стойности.";
                }
                else
                {
                    label4.Text = LanguageManager.IsEnglish()
                        ? "Database error: " + ex.Message
                        : "Грешка в базата данни: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                label4.Visible = true;
                label4.Text = LanguageManager.IsEnglish()
                    ? "Error: " + ex.Message
                    : "Грешка: " + ex.Message;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoginForm form = new LoginForm();
            form.Show();
            this.Hide();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
    }
}