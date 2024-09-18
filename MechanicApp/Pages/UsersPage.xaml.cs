using MaterialDesignThemes.Wpf;
using MechanicApp.Core;
using DataBaseControllerLibrary;
using MechanicApp.Dialogs;
using Microsoft.Win32;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для UsersPage.xaml
    /// </summary>
    public partial class UsersPage : Page
    {
        DB_Handler DB_h;
        DialogHost Host;
        User TableUser;
        string[] Headers;
        string[] Bindings;

        public UsersPage(DialogHost host, User usr)
        {
            Host = host;
            TableUser = usr;
            InitializeComponent();
        }

        private void Page_Initialized(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            DB_h = new DB_Handler();
            var query = @"SELECT * FROM Users";
            var Dict = DB_h.ExecuteReadQuery(query);
            if (Dict.Any())
            {
                var b = (Dict[0] as IDictionary<string, object>);
                Headers = [
                    "Номер пользователя",
                    "ФИО",
                    "телефон",
                    "логин",
                    "пароль",
                    "тип",
                ];
                Bindings = b.Keys.ToArray();
                UsersGrid.Columns.Clear();
                foreach (var a in Enumerable.Zip(Headers, Bindings, (header, binding) => new { First = header, Second = binding }))
                {
                    UsersGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                    {
                        Binding = new System.Windows.Data.Binding(a.Second)
                        {
                            StringFormat = b[a.Second] is DateTime ? "{0:dd/MM/yyyy}" : ""
                        },
                        Header = a.First,

                    });
                }
                UsersGrid.ItemsSource = Dict;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var d = UsersGrid.SelectedItem as ExpandoObject;
            if (d != null)
            {
                var EditObj = new AddEditTable(d, true, "Users", Headers, Host, TableUser);
                await Host.ShowDialog(EditObj);
                RefreshTable();
            }
            else
            {
                await CustomDialogBox.ShowCustomDialog("Ошибка", "Не выбрана запись для редактирования", PackIconKind.AlertCircle, System.Windows.Media.Brushes.Red, Host);
            }
        }

        private void SaveAsPdfButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                ExportClass.ExportDataGridToPdf(UsersGrid, saveFileDialog.FileName, Headers.Skip(1).ToArray(), Bindings.Skip(1).ToArray(), "Список сотрудников");
            }
        }

        private void ExportToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                ExportClass.ExportDataGridToCsv(UsersGrid, saveFileDialog.FileName, Headers.Skip(1).ToArray(), Bindings.Skip(1).ToArray());
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var d = UsersGrid.Items[0] as ExpandoObject;
            if (d != null)
            {
                var EditObj = new AddEditTable(d, false, "Users", Headers, Host, TableUser);
                await Host.ShowDialog(EditObj);
                RefreshTable();
            }
        }
    }
}
