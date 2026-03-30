using System.IO;

namespace BankaApp
{
    public static class LanguageManager
    {
        private static readonly string filePath = "language.txt";

        public static string CurrentLanguage { get; private set; } = "EN";

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

        private static void SaveLanguage()
        {
            File.WriteAllText(filePath, CurrentLanguage);
        }
    }
}