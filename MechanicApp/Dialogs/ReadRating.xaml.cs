using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для ReadRating.xaml
    /// </summary>
    public partial class ReadRating : UserControl
    {
        DialogHost Host;
        public ReadRating(DialogHost host, string fio, int rating, string text, DateTime? ratingTime)
        {
            InitializeComponent();
            Host = host;
            LetterText.Text = $"{text}\n\n" +
            $"Письмо от: {fio}\n" +
            $"Дата написания: {ratingTime}";
            Rating.Value = rating;
        }

        private void del_Click(object sender, RoutedEventArgs e)
        {
            Host.CurrentSession.Close(false);
        }
    }
}
