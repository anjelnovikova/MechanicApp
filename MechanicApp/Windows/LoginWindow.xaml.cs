using MaterialDesignThemes.Wpf;
using MechanicApp.Core;
using MechanicApp.Dialogs;
using DataBaseControllerLibrary;
using System.Windows;
using System.Windows.Controls;


namespace MechanicApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        int ErrorCount = 0;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = loginTextBox.Text;
            string password = passwordBox.Password;

            var res = await OpenCaptcha();

            if (res == false)
            {
                ShowValidationError("Неверно введена капча");
                ErrorCount++;
                CheckError();
                return;
            }

            DB_Handler db = new DB_Handler();
            (string text, string header, User user) = db.ExecLogin(login, password);

            if (user == null)
            {
                ErrorCount++;
                CheckError();
                // Вызов метода для отображения ошибки
                ShowValidationError("Неверный логин или пароль");
            }
            else
            {
                //MessageBox.Show("Вход успешен");
                ClearValidationErrors();
                MainWindow mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
        }

        private async void CheckError()
        {
            if (ErrorCount == 2)
            {
                loginTextBox.IsEnabled = false;
                passwordBox.IsEnabled = false;
                await Task.Delay(10000);
                loginTextBox.IsEnabled = true;
                passwordBox.IsEnabled = true;
            }
            if (ErrorCount == 3)
            {
                loginTextBox.IsEnabled = false;
                passwordBox.IsEnabled = false;
            }
        }

        private async Task<bool> OpenCaptcha()
        {
            var view = new CaptchaDialog(Host);
            return Convert.ToBoolean(await DialogHost.Show(view));
        }

        // Метод для отображения ошибки на полях логина и пароля
        private void ShowValidationError(string errorMessage)
        {

            Validation.MarkInvalid(loginTextBox.GetBindingExpression(TextBox.TextProperty),
                new ValidationError(new ExceptionValidationRule(), loginTextBox.GetBindingExpression(TextBox.TextProperty))
                {
                    ErrorContent = errorMessage
                });


            Validation.MarkInvalid(passwordBox.GetBindingExpression(PasswordBoxAssist.PasswordProperty),
                new ValidationError(new ExceptionValidationRule(), passwordBox)
                {
                    ErrorContent = errorMessage
                });
        }

        // Метод для очистки ошибок валидации
        private void ClearValidationErrors()
        {
            Validation.ClearInvalid(loginTextBox.GetBindingExpression(TextBox.TextProperty));
            Validation.ClearInvalid(passwordBox.GetBindingExpression(PasswordBoxAssist.PasswordProperty));
        }

        private void Button_Click_CloseWd(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_MinimizedWd(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }
}