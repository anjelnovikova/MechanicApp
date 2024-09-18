using MaterialDesignThemes.Wpf;
using System.Windows.Media;


namespace MechanicApp.Core
{
    internal class CustomDialogBox
    {
        public static async Task<object> ShowCustomDialog(string Header, string Text, PackIconKind Icon, Brush IconColor, DialogHost Host)
        {
            var view = new DialogBox(Header, Text, Icon, IconColor, Host);
            return await DialogHost.Show(view);
        }
    }
}
