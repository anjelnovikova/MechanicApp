using MaterialDesignThemes.Wpf;
using DataBaseControllerLibrary;
using MechanicApp.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ClientOrdersPage.xaml
    /// </summary>
    public partial class ClientOrdersPage : Page
    {
        DB_Handler DB_h;
        DialogHost Host;
        User TableUser;
        string[] Headers;
        string[] Bindings;

        public ClientOrdersPage(DialogHost host, User usr)
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
            string query = $@"SELECT 
                    r.requestID,
                    r.startDate,
                    vt.vehicleTypeName AS carType,
                    vm.modelName AS carModel,
                    r.problemDescription,
                    r.requestStatus,
                    r.completionDate,
                    m.fio AS masterName,
                    cm.message AS comment
                FROM 
                    Requests r
                    -- Join with VehicleModels to get the vehicle type and model
                    INNER JOIN VehicleModels vm ON r.modelID = vm.modelID
                    INNER JOIN VehicleTypes vt ON vm.vehicleTypeID = vt.vehicleTypeID
                    -- Join with Users to get the details of the master and client
                    LEFT JOIN Users m ON r.masterID = m.userID
                    LEFT JOIN Users c ON r.clientID = c.userID
                    -- Join with Comments to get related comments
                    LEFT JOIN Comments cm ON r.requestID = cm.requestID
                WHERE
                    c.userID = {TableUser.Personnel_Num}
                ORDER BY 
                    r.requestID, cm.commentID;";
            var Dict = DB_h.ExecuteReadQuery(query);
            if (Dict.Any())
            {
                var b = (Dict[0] as IDictionary<string, object>);
                Headers = [
                    "Номер заказа",      // requestID
                    "Дата начала",       // startDate
                    "Тип автомобиля",    // vehicleTypeName (carType)
                    "Модель автомобиля", // modelName (carModel)
                    "Описание проблемы",// problemDescription
                    "Статус заявки",     // requestStatus
                    "Дата завершения",   // completionDate
                    "Механик",           // masterName
                    "Комментарий"        // comment
                ];
                Bindings = b.Keys.ToArray();
                OrdersGrid.Columns.Clear();
                foreach (var a in Enumerable.Zip(Headers, Bindings, (header, binding) => new { First = header, Second = binding }))
                {
                    OrdersGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                    {
                        Binding = new System.Windows.Data.Binding(a.Second)
                        {
                            StringFormat = b[a.Second] is DateTime ? "{0:dd/MM/yyyy}" : ""
                        },
                        Header = a.First,

                    });
                }
                OrdersGrid.ItemsSource = Dict;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var EditObj = new CreateNewRequest(Host, TableUser);
            await Host.ShowDialog(EditObj);
            RefreshTable();
        }
    }
}
