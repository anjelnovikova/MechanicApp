using MaterialDesignThemes.Wpf;
using DataBaseControllerLibrary;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MechanicApp.Core;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientRatingPage.xaml
    /// </summary>
    public partial class ClientRatingPage : Page
    {
        DB_Handler DB;
        DialogHost Host;
        User TableUser;

        public ClientRatingPage(DialogHost host, User usr)
        {
            Host = host;
            TableUser = usr;
            DB = new DB_Handler();
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            SqlParameter[] param = [
                new SqlParameter("@ClientID", TableUser.Personnel_Num),
                new SqlParameter("@Text", AdditionalText.Text),
                new SqlParameter("@Rating", Rating.Value),
                new SqlParameter("@Date", DateTime.Now),
            ];
            DB.ExecuteWriteQuery($"INSERT INTO ClientRatings VALUES (@ClientID,@Rating, @Text, @Date); ", param);
            await CustomDialogBox.ShowCustomDialog("Успешно", "Спасибо за ваш отзыв", PackIconKind.CheckCircle, Brushes.Green, Host);
        }
    }
}
