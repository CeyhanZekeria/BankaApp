using System.Windows.Forms;

namespace BankaApp
{
    public static class FormStateHelper
    {
        public static void Attach(Form form)
        {
            AppState.ApplyFormState(form);

            form.Resize += (s, e) => AppState.SaveFormState(form);
            form.Move += (s, e) => AppState.SaveFormState(form);
            form.FormClosing += (s, e) => AppState.SaveFormState(form);
        }
    }
}