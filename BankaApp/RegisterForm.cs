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
        string connStr = "User Id=banka;Password=1234;Data Source=localhost:1521/XE;";
        private static readonly Random rnd = new Random();
        private bool isEnglish = true;
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
            button2.BackColor = System.Drawing.Color.RoyalBlue;
            button2.ForeColor = System.Drawing.Color.White;
            MakeButtonRound(button2);
            button2.Resize += button2_Resize;

            ApplyEnglishLanguage();
        }

        private void ApplyEnglishLanguage()
        {
            label1.Text = "Registration Form";

            fullNAme.Text = "Full Name :";
            label5.Text = "Email :";
            label6.Text = "Phone :";
            label7.Text = "Password :";
            label9.Text = "Year of Birth :";
            label10.Text = "PIN/IDN";

            label11.Text = "Street :";
            label8.Text = "Adress :";
            label12.Text = "City :";
            label3.Text = "Country *";
            label2.Text = "Gender *";

            radioBtnMan.Text = "Man";
            radioBtnWoman.Text = "Woman";

            crtAccount.Text = "CREATE ACCOUNT";
            button1.Text = "Cancel";

            username.PlaceholderText = "Full Name";
            email.PlaceholderText = "email adress";
            phoneNum.PlaceholderText = "phone";
            pass.PlaceholderText = "password";
            age.PlaceholderText = "Year of Birth";
            EGN.PlaceholderText = "PIN/IDN";
            streetId.PlaceholderText = "street";
            adress.PlaceholderText = "adress";
            city.PlaceholderText = "City";

            button2.Text = "BG";
            isEnglish = true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            if (isEnglish)
                ApplyBulgarianLanguage();
            else
                ApplyEnglishLanguage();
        }
        private void ApplyBulgarianLanguage()
        {
            label1.Text = "Форма за регистрация";

            fullNAme.Text = "Име и фамилия :";
            label5.Text = "Имейл :";
            label6.Text = "Телефон :";
            label7.Text = "Парола :";
            label9.Text = "Година на раждане :";
            label10.Text = "ЕГН/ЛНЧ";

            label11.Text = "Улица :";
            label8.Text = "Адрес :";
            label12.Text = "Град :";
            label3.Text = "Държава *";
            label2.Text = "Пол *";

            radioBtnMan.Text = "Мъж";
            radioBtnWoman.Text = "Жена";

            crtAccount.Text = "СЪЗДАЙ АКАУНТ";
            button1.Text = "Назад";

            username.PlaceholderText = "Име и фамилия";
            email.PlaceholderText = "имейл адрес";
            phoneNum.PlaceholderText = "телефон";
            pass.PlaceholderText = "парола";
            age.PlaceholderText = "Година на раждане";
            EGN.PlaceholderText = "ЕГН/ЛНЧ";
            streetId.PlaceholderText = "улица";
            adress.PlaceholderText = "адрес";
            city.PlaceholderText = "град";

            button2.Text = "EN";
            isEnglish = false;
        }

        private void button2_Resize(object sender, EventArgs e)
        {
            MakeButtonRound(button2);
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
        private string GenerateRandomCVV()
        {
            Random rnd = new Random();
            return rnd.Next(0, 1000).ToString("D3");
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
            string streetNameValue = streetId.Text.Trim(); // textbox за улица
            string addressValue = adress.Text.Trim();      // номер
            string cityValue = city.Text.Trim();           // ако искаш да го ползваш после
            string passwordValue = pass.Text.Trim();
            string emailValue = email.Text.Trim();
            string phoneValue = phoneNum.Text.Trim();
            string countryValue = cmbCountry.SelectedItem != null ? cmbCountry.SelectedItem.ToString() : "";
            string genderValue = "";
            string generatedCVV = GenerateRandomCVV();

            int birthYear;
            int newClientId = 0;

            if (radioBtnMan.Checked)
                genderValue = "Male";
            else if (radioBtnWoman.Checked)
                genderValue = "Female";

            if (string.IsNullOrWhiteSpace(usernameValue) ||
                string.IsNullOrWhiteSpace(egnValue) ||
                string.IsNullOrWhiteSpace(streetNameValue) ||
                string.IsNullOrWhiteSpace(addressValue) ||
                string.IsNullOrWhiteSpace(passwordValue) ||
                string.IsNullOrWhiteSpace(emailValue) ||
                string.IsNullOrWhiteSpace(phoneValue) ||
                string.IsNullOrWhiteSpace(age.Text.Trim()) ||
                string.IsNullOrWhiteSpace(genderValue))
            {
                label4.Text = isEnglish
                    ? "Please fill in all required fields."
                    : "Моля, попълнете всички задължителни полета.";
                return;
            }

            if (!int.TryParse(age.Text.Trim(), out birthYear))
            {
                label4.Text = isEnglish
                    ? "Birth year must be a valid number."
                    : "Годината на раждане трябва да е валидно число.";
                return;
            }

            if (birthYear < 1900 || birthYear > DateTime.Now.Year)
            {
                label4.Text = isEnglish
                    ? "Birth year is invalid."
                    : "Невалидна година на раждане.";
                return;
            }

            if (!IsValidEGN(egnValue))
            {
                label4.Text = isEnglish
                    ? "EGN must contain exactly 10 digits."
                    : "ЕГН трябва да съдържа точно 10 цифри.";
                return;
            }

            if (!IsValidEmail(emailValue))
            {
                label4.Text = isEnglish
                    ? "Please enter a valid email address."
                    : "Моля, въведете валиден имейл адрес.";
                return;
            }

            if (!IsValidPhone(phoneValue))
            {
                label4.Text = isEnglish
                    ? "Please enter a valid phone number."
                    : "Моля, въведете валиден телефонен номер.";
                return;
            }

            if (!IsStrongPassword(passwordValue))
            {
                label4.Text = isEnglish
                    ? "Password must be at least 8 characters and include uppercase, lowercase, number and symbol."
                    : "Паролата трябва да е поне 8 символа и да съдържа главна буква, малка буква, число и символ.";
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
                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Email = :val", ":val", emailValue) ||
    RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Email = :val", ":val", emailValue))
                            {
                                label4.Text = isEnglish
                                    ? "This email is already registered."
                                    : "Този имейл вече е регистриран.";
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM App_User WHERE Phone_number = :val", ":val", phoneValue) ||
                                RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE Phone_number = :val", ":val", phoneValue))
                            {
                                label4.Text = isEnglish
                                    ? "This phone number is already registered."
                                    : "Този телефонен номер вече е регистриран.";
                                return;
                            }

                            if (RecordExists(conn, transaction, "SELECT COUNT(*) FROM Client WHERE EGN = :val", ":val", egnValue))
                            {
                                label4.Text = isEnglish
                                    ? "This EGN is already registered."
                                    : "Това ЕГН вече е регистрирано.";
                                return;
                            }

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
                                cmdUser.Parameters.Add(":password", OracleDbType.Varchar2).Value = passwordValue;
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
                        (Name, EGN, ID_Street, Street_Name, Adress, Phone_number, Is_Active, Email, Gender, Birth_Year, Country)
                        VALUES
                        (:name, :egn, :streetId, :streetName, :adress, :phone, 1, :email, :gender, :birthyear, :country)
                        RETURNING Client_ID INTO :newClientId";

                            using (OracleCommand cmdClient = new OracleCommand(clientQuery, conn))
                            {
                                cmdClient.Transaction = transaction;
                                cmdClient.BindByName = true;

                                cmdClient.Parameters.Add(":name", OracleDbType.Varchar2).Value = usernameValue;
                                cmdClient.Parameters.Add(":egn", OracleDbType.Varchar2).Value = egnValue;
                                cmdClient.Parameters.Add(":streetId", OracleDbType.Int32).Value = DBNull.Value;
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
                                        ibanAttempts++;
                                    else
                                        throw;
                                }
                            }

                            if (!accountCreated)
                                throw new Exception(isEnglish
                                    ? "Could not create bank account automatically."
                                    : "Банковата сметка не можа да бъде създадена автоматично.");

                            transaction.Commit();


                            MessageBox.Show(isEnglish ? "Account created successfully." : "Акаунтът беше създаден успешно.",
                                isEnglish ? "Success" : "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
                        label4.Text = isEnglish
                            ? "This email is already registered."
                            : "Този имейл вече е регистриран.";
                    else if (errorText.Contains("uq_app_user_phone") || errorText.Contains("uq_client_phone"))
                        label4.Text = isEnglish
                            ? "This phone number is already registered."
                            : "Този телефонен номер вече е регистриран.";
                    else if (errorText.Contains("uq_client_egn"))
                        label4.Text = isEnglish
                            ? "This EGN is already registered."
                            : "Това ЕГН вече е регистрирано.";
                    else
                        label4.Text = isEnglish
                            ? "Duplicate value detected. Please check your input."
                            : "Открита е повтаряща се стойност. Моля, проверете данните.";
                }
                else if (ex.Number == 2290)
                {
                    label4.Text = isEnglish
                        ? "Invalid data. Please check your input values."
                        : "Невалидни данни. Моля, проверете въведените стойности.";
                }
                else
                {
                    label4.Text = isEnglish
                        ? "Database error: " + ex.Message
                        : "Грешка в базата данни: " + ex.Message;
                }
            }
            catch (Exception ex)
            {
                label4.Text = isEnglish
                    ? "Error: " + ex.Message
                    : "Грешка: " + ex.Message;
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void fullNAme_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void label7_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void radioBtnMan_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void radioBtnWoman_CheckedChanged_1(object sender, EventArgs e)
        {

        }


    }
}