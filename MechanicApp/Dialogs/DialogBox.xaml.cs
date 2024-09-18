using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MechanicApp
{
    /// <summary>
    /// Логика взаимодействия для UserControl1.xaml
    /// </summary>
    public partial class DialogBox : UserControl
    {
        DialogHost Host;
        public DialogBox(string header, string content, PackIconKind icon, Brush iconColor, DialogHost host)
        {
            InitializeComponent();
            HeaderTextBlock.Text = header;
            ContentTextBlock.Text = content;
            Icon.Kind = icon;
            Icon.Foreground = iconColor;
            Host = host;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Host.CurrentSession.Close(false);
        }
    }
}
