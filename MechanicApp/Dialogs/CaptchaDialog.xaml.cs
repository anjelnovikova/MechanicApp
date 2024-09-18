using EasyCaptcha.Wpf;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class CaptchaDialog : UserControl
    {
        DialogHost Host;
        public CaptchaDialog(DialogHost host)
        {
            InitializeComponent();
            Host = host;
        }

        private void UserControl_Initialized(object sender, EventArgs e)
        {
            GenerateCaptcha();
        }
        private void GenerateCaptcha()
        {
            CaptchaImage.CreateCaptcha(Captcha.LetterOption.Alphanumeric, 5);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool result = false;
            if (CaptchaImage.CaptchaText == CaptchaInput.Text)
                result = true;

            Host.CurrentSession.Close(result);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            GenerateCaptcha();
        }
    }
}
