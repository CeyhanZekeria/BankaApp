using System.Collections.Generic;
using System.IO;

namespace BankaApp
{
    public static class LanguageManager
    {
        private static readonly string filePath = "language.txt";

        public static string CurrentLanguage { get; private set; } = "EN";

        private static readonly Dictionary<string, Dictionary<string, string>> translations
            = new Dictionary<string, Dictionary<string, string>>
            {
                ["EN"] = new Dictionary<string, string>
                {
                    ["to"] = "To:",
                    ["save"] = "Save Changes",
                    ["type"] = "Type:",
                    ["from"] = "From:",
                    ["hi"] = "Hi",
                    ["home"] = "Home 🏠",
                    ["profile"] = "Profile 👤",
                    ["security"] = "Security 🪪",
                    ["settings"] = "Settings ⚙️",
                    ["theme"] = "Theme",
                    ["language"] = "Language",
                    ["logout"] = "Logout",
                    ["change_password"] = "Change Password",
                    ["notifications"] = "Notifications",
                    ["welcome_back"] = "Welcome Back!",
                    ["overview"] = "Overview",
                    ["register_form"] = "Registration Form",
                    ["full_name"] = "Full name :",
                    ["email"] = "Email :",
                    ["phone"] = "Phone :",
                    ["password"] = "Password :",
                    ["birth_year"] = "Birth year :",
                    ["egn_lnc"] = "EGN/LNC",
                    ["street"] = "Street :",
                    ["address"] = "Address :",
                    ["city"] = "City :",
                    ["country_required"] = "Country *",
                    ["gender_required"] = "Gender *",
                    ["male"] = "Male",
                    ["female"] = "Female",
                    ["create_account"] = "CREATE ACCOUNT",
                    ["back"] = "Back",
                    ["ph_full_name"] = "Full name",
                    ["ph_email"] = "email address",
                    ["ph_phone"] = "phone",
                    ["ph_password"] = "password",
                    ["ph_birth_year"] = "Birth year",
                    ["ph_egn_lnc"] = "EGN/LNC",
                    ["ph_street"] = "street",
                    ["ph_address"] = "address",
                    ["ph_city"] = "city",
                    ["login_title"] = "Login",
                    ["username_email"] = "Username / Email",
                    ["password"] = "Password",
                    ["no_account"] = "Don't have an account?",
                    ["log_in"] = "LOG IN",
                    ["create_account"] = "Create account",
                    ["ph_login_user"] = "Enter username or email",
                    ["ph_login_pass"] = "Enter password",
                    ["add_money"] = "Add Money",
                    ["send_money"] = "Send Money",
                    ["exchange"] = "Exchange",
                    ["filter"] = "Filter",
                    ["reset"] = "Reset",
                    ["welcome_back"] = "Welcome Back!",
                    ["overview"] = "Overview",
                    ["accounts"] = "Selected Account",
                    ["recent_transactions"] = "Recent Transactions",
                    ["ammount"] = "Total Balance:",
                },

                ["BG"] = new Dictionary<string, string>
                {
                    ["to"] = "До:",
                    ["ammount"] = "Наличност:",
                    ["save"] = "Запази",
                    ["type"] = "Вид:",
                    ["from"] = "От:",
                    ["hi"] = "",
                    ["home"] = "Начало 🏠",
                    ["profile"] = "Профил 👤",
                    ["security"] = "Сигурност 🪪",
                    ["settings"] = "Настройки ⚙️",
                    ["theme"] = "Тема",
                    ["language"] = "Език",
                    ["logout"] = "Изход",
                    ["change_password"] = "Смени парола",
                    ["notifications"] = "Известия",
                    ["welcome_back"] = "Добре дошъл!",
                    ["overview"] = "Преглед",
                    ["register_form"] = "Форма за регистрация",
                    ["full_name"] = "Име и фамилия :",
                    ["email"] = "Имейл :",
                    ["phone"] = "Телефон :",
                    ["password"] = "Парола :",
                    ["birth_year"] = "Година на раждане :",
                    ["egn_lnc"] = "ЕГН/ЛНЧ",
                    ["street"] = "Улица :",
                    ["address"] = "Адрес :",
                    ["city"] = "Град :",
                    ["country_required"] = "Държава *",
                    ["gender_required"] = "Пол *",
                    ["male"] = "Мъж",
                    ["female"] = "Жена",
                    ["create_account"] = "СЪЗДАЙ АКАУНТ",
                    ["back"] = "Назад",
                    ["ph_full_name"] = "Име и фамилия",
                    ["ph_email"] = "имейл адрес",
                    ["ph_phone"] = "телефон",
                    ["ph_password"] = "парола",
                    ["ph_birth_year"] = "Година на раждане",
                    ["ph_egn_lnc"] = "ЕГН/ЛНЧ",
                    ["ph_street"] = "улица",
                    ["ph_address"] = "адрес",
                    ["ph_city"] = "град",
                    ["login_title"] = "Добре Дошли",
                    ["username_email"] = "Потребител / Имейл",
                    ["password"] = "Парола",
                    ["no_account"] = "Нямате още профил?",
                    ["log_in"] = "ВХОД",
                    ["create_account"] = "Създай акаунт",
                    ["ph_login_user"] = "Въведи потребител или имейл",
                    ["ph_login_pass"] = "Въведи парола",
                    ["add_money"] = "Добави пари",
                    ["send_money"] = "Изпрати пари",
                    ["exchange"] = "Обмяна",
                    ["filter"] = "Фил.",
                    ["reset"] = "Изчис.",
                    ["welcome_back"] = "Добре дошъл!",
                    ["overview"] = "Преглед",
                    ["accounts"] = "Сметки",
                    ["recent_transactions"] = "Последни транзакции",
                }
            };

        public static void SetLanguage(string language)
        {
            if (language == "BG" || language == "EN")
            {
                CurrentLanguage = language;
                SaveLanguage();
            }
        }

        public static bool IsEnglish()
        {
            return CurrentLanguage == "EN";
        }

        public static bool IsBulgarian()
        {
            return CurrentLanguage == "BG";
        }

        public static void ToggleLanguage()
        {
            SetLanguage(CurrentLanguage == "EN" ? "BG" : "EN");
        }

        public static void LoadLanguage()
        {
            if (File.Exists(filePath))
            {
                string savedLanguage = File.ReadAllText(filePath).Trim();
                if (savedLanguage == "BG" || savedLanguage == "EN")
                    CurrentLanguage = savedLanguage;
            }
        }

        public static string GetText(string key)
        {
            if (translations.ContainsKey(CurrentLanguage) &&
                translations[CurrentLanguage].ContainsKey(key))
            {
                return translations[CurrentLanguage][key];
            }

            return key;
        }

        private static void SaveLanguage()
        {
            File.WriteAllText(filePath, CurrentLanguage);
        }
    }
}