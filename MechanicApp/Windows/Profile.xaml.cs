using System.Windows;
using DataBaseControllerLibrary;

namespace MechanicApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        Window Window;
        User UserAcc;
        public Profile(Window window, User usr)
        {
            InitializeComponent();
            Window = window;
            UserAcc = usr;
            Fname.Text = usr.FName;
            Sname.Text = usr.SName;
            Tname.Text = usr.TName;

            UserType.Text = usr.Security.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Window.WindowState = WindowState.Normal;
            Close();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Window.Close();
            LoginWindow lw = new LoginWindow();
            lw.Show();
            this.Close();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
        }
    }
}
