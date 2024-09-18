using DataBaseControllerLibrary;
using System.Windows.Controls;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainClientPage.xaml
    /// </summary>
    public partial class MainClientPage : Page
    {
        DB_Handler db = new DB_Handler();
        string[] Headers;
        string[] Bindings;
        User TableUser;
        public MainClientPage(User user)
        {
            TableUser = user;
            InitializeComponent();
            CreateOrdersInfo();
            LoadRecentOrders();
        }

        private void LoadRecentOrders()
        {
            var Dict = db.ExecuteReadQuery($@"SELECT TOP 5
				R.requestID,
				U2.fio AS MasterName,
				VM.modelName AS CarModel,
				R.startDate,
				R.requestStatus
			FROM
				Requests R
			LEFT JOIN Users U1 ON R.clientID = U1.userID
            LEFT JOIN Users U2 ON R.masterID = U2.userID
			LEFT JOIN VehicleModels VM ON R.modelID = VM.modelID  -- Join to get car model
			WHERE
				U1.userID = {TableUser.Personnel_Num}
			ORDER BY
				R.startDate DESC");
            if (Dict.Any())
            {
                var Entry = (Dict[0] as IDictionary<string, object>);
                Headers = new string[] { "Номер заказа", "ФИО мастера", "Модель машины", "Дата", "Статус" };//b.Keys.ToArray();
                Bindings = Entry.Keys.ToArray();
                RecentOrdersGrid.Columns.Clear();
                foreach (var a in Enumerable.Zip(Headers, Bindings, (header, binding) => new { First = header, Second = binding }))
                {
                    RecentOrdersGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                    {
                        Binding = new System.Windows.Data.Binding(a.Second)
                        {
                            StringFormat = Entry[a.Second] is DateTime ? "{0:dd/MM/yyyy}" : ""
                        },
                        Header = a.First,

                    });
                }
                RecentOrdersGrid.ItemsSource = Dict;
            }
        }
        private void CreateOrdersInfo()
        {
            var Dict = db.ExecuteReadQuery("SELECT COUNT(*) AS OrderCount FROM Requests WHERE ClientID = @A0", [new Microsoft.Data.SqlClient.SqlParameter("@A0", TableUser.Personnel_Num)]);
            var Entry = (Dict[0] as IDictionary<string, object>);
            if (Dict.Any())
            {
                OrderAmountTB.Text = $"Кол-во заказов: {Entry["OrderCount"]}";
            }
            else
            {
                OrderAmountTB.Text = $"Заказы отсутсвуют";
            }
        }
    }
}
