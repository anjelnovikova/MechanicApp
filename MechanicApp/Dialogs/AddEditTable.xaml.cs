using MaterialDesignThemes.Wpf;
using DataBaseControllerLibrary;
using Microsoft.Data.SqlClient;
using System.Dynamic;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для AddEditWorkingSchedule.xaml
    /// </summary>
    public partial class AddEditTable : UserControl
    {
        DB_Handler db_h;
        bool isEdit;
        string TableName;
        ExpandoObject Obj;
        DialogHost Host;
        User TableUser;
        string[] Headers;

        string[] readOnly = ["clientName", "clientPhone", "carModel", "carType", "problemDescription"];

        public AddEditTable(ExpandoObject obj, bool isedit, string tableName, string[] headers, DialogHost host, User user)
        {
            InitializeComponent();
            db_h = new DB_Handler();
            isEdit = isedit;
            TableName = tableName;
            Obj = obj;
            Headers = headers;
            TableUser = user;
            Host = host;
            switch (user.Security)
            {
                case "Менеджер":

                    break;
                case "Оператор":
                    readOnly = ["clientName", "clientPhone", "carModel", "carType", "problemDescription"];
                    break;
                case "Автомеханик":
                    readOnly = ["clientName", "clientPhone", "carModel", "carType", "masterName"];
                    break;
                case "Заказчик":

                    break;
            }
            DrawForObj(obj, isEdit);
        }

        public void DrawForObj(ExpandoObject obj, bool isEdit)
        {
            IDictionary<string, object> dictionary_object = obj;
            UniGrid.Rows = dictionary_object.Keys.Count;
            UniGrid.Columns = 2;
            int counter = 0;
            try
            {
                foreach (string key in dictionary_object.Keys)
                {
                    if (dictionary_object[key] is DateTime || key.ToLower().Contains("date"))
                    {
                        UniGrid.Children.Add(new TextBlock()
                        {
                            Text = Headers[counter],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 18,
                        });
                        UniGrid.Children.Add(new DatePicker()
                        {
                            Width = 215,
                            SelectedDate = isEdit ? dictionary_object[key] as DateTime? : null,
                            Uid = key,
                        });
                    }
                    else if (key == "masterName")
                    {
                        var fios = db_h.ExecuteReadQuery("SELECT fio as masterName FROM Users WHERE type = 'Автомеханик'", null)
                            .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                            .ToList();
                        UniGrid.Children.Add(new TextBlock()
                        {
                            Text = Headers[counter],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 18,
                        });
                        UniGrid.Children.Add(new ComboBox
                        {
                            ItemsSource = fios,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Width = 215,
                            IsReadOnly = TableUser.Security != "Автомеханик",
                            SelectedIndex = isEdit ? fios.IndexOf(dictionary_object[key] as string) : 0,
                            Uid = key
                        });
                    }
                    else if (key == "carModel" && TableUser.Security != "Автомеханик" && TableUser.Security != "Оператор")
                    {
                        var fios = db_h.ExecuteReadQuery("SELECT modelName as carModel FROM VehicleModels", null)
                        .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                        .ToList();
                        UniGrid.Children.Add(new TextBlock()
                        {
                            Text = Headers[counter],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 18,
                        });
                        UniGrid.Children.Add(new ComboBox
                        {
                            ItemsSource = fios,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Width = 215,
                            SelectedIndex = isEdit ? fios.IndexOf(dictionary_object[key] as string) : 0,
                            Uid = key
                        });
                    }
                    else if (key == "carType" && TableUser.Security != "Автомеханик" && TableUser.Security != "Оператор")
                    {
                        var fios = db_h.ExecuteReadQuery("SELECT vehicleTypeName as carType FROM VehicleTypes", null)
                        .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                        .ToList();
                        UniGrid.Children.Add(new TextBlock()
                        {
                            Text = Headers[counter],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 18,
                        });
                        UniGrid.Children.Add(new ComboBox
                        {
                            ItemsSource = fios,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Width = 215,
                            SelectedIndex = isEdit ? fios.IndexOf(dictionary_object[key] as string) : 0,
                            Uid = key
                        });
                    }
                    else
                    {
                        UniGrid.Children.Add(new TextBlock()
                        {
                            Text = Headers[counter],
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 18,
                            Visibility = key.Contains("ID") ? Visibility.Collapsed : Visibility.Visible
                        });
                        var tb = new TextBox()
                        {
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            FontSize = 14,
                            Width = 215,
                            Text = isEdit ? TryParseIntOrString(dictionary_object[key]) : "",
                            Uid = key,
                            IsEnabled = readOnly.Contains(key) ? false : true,
                            Visibility = key.Contains("ID") ? Visibility.Collapsed : Visibility.Visible
                        };
                        UniGrid.Children.Add(tb);
                    }
                    counter++;
                }
            }
            catch (IndexOutOfRangeException)
            {

            }
        }

        public string TryParseIntOrString(object obj)
        {
            if (obj is int)
                return (obj as int?).ToString();

            return obj as string;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var emptyRes = CheckEmptyTextBoxes();
            if (!emptyRes.Item1)
            {
                MessageBox.Show(emptyRes.Item2, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var res = CheckInvalidTextBoxes();
            if (!res.Item1)
            {
                MessageBox.Show(res.Item2, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var Keys = (Obj as IDictionary<string, object>).Keys.ToArray();
            var Values = UniGrid.Children.Cast<UIElement>().Where(x => x.Uid != "").ToArray();
            DifferentiateTables(TableName, Keys, Values);
            Host.CurrentSession.Close(false);
        }

        private (bool, string) CheckEmptyTextBoxes()
        {
            string[] Checks = ["login", "password", "fio"];
            var Blocks = UniGrid.Children.Cast<UIElement>().Where(x => Checks.Contains(x.Uid)).Select(x => x as TextBox).ToArray();
            var EmptyBlocks = Blocks.Where(x => x.Text == "");
            if (!EmptyBlocks.Any())
            {
                return (true, "");
            }
            return (false, "Имеются незаполненные данные. Проверьте корректность ввода");
        }

        private (bool, string) CheckInvalidTextBoxes()
        {
            string[] Checks = ["login", "password"];
            var allowedAcc = new int[] { 0, 1 };
            var Blocks = UniGrid.Children.Cast<UIElement>().Where(x => Checks.Contains(x.Uid)).Select(x => x as TextBox).ToArray();
            foreach (var Block in Blocks)
            {
                if (Block.Uid == "login" || Block.Uid == "password")
                {
                    var ln = Block.Text.Length;
                    if (ln < 5 || ln > 55)
                    {
                        return (false, "Логин или пароль должны быть не меньше 5 знаков и не больше 55");
                    }
                }
            }
            return (true, "");
        }

        private void DifferentiateTables(string TableName, string[] keys, UIElement[] Values)
        {
            if (TableName == "Requests")
            {
                var param = ParseUIElements(Values);
                if (isEdit)
                    db_h.ExecuteWriteQuery($"UPDATE {TableName} SET problemDescription = @A3, startDate = @Date0, requestStatus = @A4, completionDate=@Date1, masterID=@masterID WHERE requestID = @A0", param);
                else
                    db_h.ExecuteWriteQuery($"INSERT INTO {TableName} VALUES (@A0, @Date0, @Date1, @A1, @A2, @Date1, @A3, @A4, @A5, @A6)", param);
            }
            else if (TableName == "Users")
            {
                var param = ParseUIElements(Values);
                if (isEdit)
                    db_h.ExecuteWriteQuery($"UPDATE {TableName} SET fio = @A1, phone = @A2, login = @A3, password = @A4, type = @A5 WHERE userID = @A0", param);
                else
                    db_h.ExecuteWriteQuery($"INSERT INTO {TableName} VALUES (@A1, @A2, @A3, @A4, @A5)", param);
            }
        }

        private SqlParameter[] ParseUIElements(UIElement[] Values)
        {
            var Res = new List<SqlParameter>();
            int FieldCounter = 0;
            int DateCounter = 0;
            foreach (UIElement element in Values)
            {
                if (element is DatePicker)
                {
                    var date = (element as DatePicker).SelectedDate;
                    if (date == null)
                    {
                        Res.Add(new SqlParameter($"@Date{DateCounter}", DBNull.Value));
                        DateCounter += 1;
                        continue;
                    }
                    Res.Add(new SqlParameter($"@Date{DateCounter}", date));
                    DateCounter += 1;
                }
                else if (element is ComboBox)
                {
                    var cbx = (ComboBox)element;
                    switch (cbx.Uid) //Замена значения из комбобокса на ID записи в БД (пример: ФИО -> userID)
                    {
                        case "masterName":
                            Res.Add(new SqlParameter($"@masterFIO", (element as ComboBox).SelectedItem));
                            Res.Add(new SqlParameter($"@masterID", db_h.ExecuteReadQuery($"SELECT Users.userID FROM Users WHERE Users.fio = '{(element as ComboBox).SelectedItem}'", null)
                                .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                                .ToList()[0]
                                ));
                            break;
                        case "carName":
                            Res.Add(new SqlParameter($"@carName", (element as ComboBox).SelectedItem));
                            Res.Add(new SqlParameter($"@carNameID", db_h.ExecuteReadQuery($"SELECT modelID FROM VehicleModels WHERE modelName = '{(element as ComboBox).SelectedItem}'", null)
                                .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                                .ToList()[0]
                                ));
                            break;
                        case "carType":
                            Res.Add(new SqlParameter($"@carType", (element as ComboBox).SelectedItem));
                            Res.Add(new SqlParameter($"@carTypeID", db_h.ExecuteReadQuery($"SELECT vehicleTypeID FROM VehicleTypes WHERE vehicleTypeName = '{(element as ComboBox).SelectedItem}'", null)
                                .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                                .ToList()[0]
                                ));
                            break;
                    }
                }
                else if (element is CheckBox)
                {
                    Res.Add(new SqlParameter("@IsTrue", (element as CheckBox).IsChecked));
                }
                else
                {
                    Res.Add(new SqlParameter($"@A{FieldCounter}", (element as TextBox).Text));
                    FieldCounter++;
                }
            }
            return Res.ToArray();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Host.CurrentSession.Close(false);
        }
    }

}
