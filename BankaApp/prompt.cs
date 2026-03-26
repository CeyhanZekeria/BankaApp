using System;
using System.Windows.Forms;

namespace BankaApp
{
    public static class Prompt
    {
        public static string ShowDialog(string text, string caption)
        {
            Form prompt = new Form()
            {
                Width = 350,
                Height = 170,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };

            Label textLabel = new Label()
            {
                Left = 20,
                Top = 20,
                Width = 290,
                Text = text
            };

            TextBox inputBox = new TextBox()
            {
                Left = 20,
                Top = 50,
                Width = 290,
                UseSystemPasswordChar = true
            };

            Button confirmation = new Button()
            {
                Text = "OK",
                Left = 120,
                Width = 90,
                Top = 85,
                DialogResult = DialogResult.OK
            };

            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(inputBox);
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? inputBox.Text : "";
        }
    }
}