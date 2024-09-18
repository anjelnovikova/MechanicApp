using DataBaseControllerLibrary;
using MechanicApp.Pages;
using MechanicApp.Windows;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        User UserAcc;
        public MainWindow(User userAcc)
        {
            InitializeComponent();
            UserAcc = userAcc;
            ProfileChip.Content = userAcc.FName;
            switch (UserAcc.Security)
            {
                case "Менеджер":
                    PageBrowser.Content = new MainManagerPage();
                    LoadManagerPage();
                    break;
                case "Оператор":
                    PageBrowser.Content = new MainOperatorPage();
                    LoadOperatorPage();
                    break;
                case "Автомеханик":
                    PageBrowser.Content = new MainMechanicPage(userAcc);
                    LoadMechanicPage();
                    break;
                case "Заказчик":
                    PageBrowser.Content = new MainClientPage(userAcc);
                    LoadClientPage();
                    break;
            }
        }
        private void LoadOperatorPage()
        {
            Button1.Content = "Заявки";
            Button2.Content = "Отзывы";
            Button2.Visibility = Visibility.Visible;
        }

        private void LoadMechanicPage()
        {
            Button1.Content = "Заявки";
            Button2.Visibility = Visibility.Collapsed;
        }

        private void LoadManagerPage()
        {
            Button1.Content = "Пользователи";
            Button2.Visibility = Visibility.Collapsed;
        }
        private void LoadClientPage()
        {
            Button1.Content = "Мои заказы";
            Button2.Content = "Оставить отзыв";
            Button2.Visibility = Visibility.Visible;
        }

        private void MainBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (UserAcc.Security)
            {
                case "Менеджер":
                    PageBrowser.Content = new MainManagerPage();
                    break;
                case "Оператор":
                    PageBrowser.Content = new MainOperatorPage();
                    break;
                case "Автомеханик":
                    PageBrowser.Content = new MainMechanicPage(UserAcc);
                    break;
                case "Заказчик":
                    PageBrowser.Content = new MainClientPage(UserAcc);
                    break;
            }

        }

        private void ProfileChip_Click(object sender, RoutedEventArgs e)
        {
            Profile p = new Profile(this, UserAcc);
            p.Show();
            this.WindowState = WindowState.Minimized;
        }

        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (UserAcc.Security)
            {
                case "Менеджер":
                    PageBrowser.Content = new UsersPage(Host, UserAcc);
                    break;
                case "Оператор":
                    PageBrowser.Content = new OrdersPage(Host, UserAcc);
                    break;
                case "Автомеханик":
                    PageBrowser.Content = new OrdersPage(Host, UserAcc);
                    break;
                case "Заказчик":
                    PageBrowser.Content = new ClientOrdersPage(Host, UserAcc);
                    break;
            }

        }

        private void StarsBtn_Click(object sender, RoutedEventArgs e)
        {
            switch (UserAcc.Security)
            {
                case "Менеджер":

                    break;
                case "Оператор":
                    PageBrowser.Content = new RatingsPage(Host);
                    break;
                case "Автомеханик":

                    break;
                case "Заказчик":
                    PageBrowser.Content = new ClientRatingPage(Host, UserAcc);
                    break;
            }

        }

        private void PopupBox_Opened(object sender, RoutedEventArgs e)
        {
            EnterLogs.Children.Clear();
            DB_Handler db = new DB_Handler();
            var Dict = db.ExecuteReadQuery(@"SELECT TOP (10) [LoginTime], [LoginStatus]
            FROM [Login_logs]
            WHERE userID = @UID", [new SqlParameter("@UID", UserAcc.Personnel_Num)]);
            foreach (var LoginTry in Dict)
            {
                var b = (LoginTry as IDictionary<string, object>);
                var txt = new TextBlock()
                {
                    Text = $"Попытка входа в {b["LoginTime"]}, Результат: {b["LoginStatus"]}",
                    Margin = new Thickness(5),
                    FontSize = 12

                };
                EnterLogs.Children.Add(txt);
            }
        }
    }
}