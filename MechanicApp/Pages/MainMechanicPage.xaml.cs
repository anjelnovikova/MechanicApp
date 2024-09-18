using DataBaseControllerLibrary;
using System.Windows.Controls;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainMechanicPage.xaml
    /// </summary>
    public partial class MainMechanicPage : Page
    {
        DB_Handler db = new DB_Handler();
        string[] Headers;
        string[] Bindings;
        User TableUser;
        public MainMechanicPage(User user)
        {
            TableUser = user;
            InitializeComponent();
            CreateOrdersInfo();
            LoadRecentOrders();
        }

        private void LoadRecentOrders()
        {
            var Dict = db.ExecuteReadQuery($@"SELECT
				R.requestID,
				U1.fio AS ClientName,
				VM.modelName AS CarModel,
				R.startDate,
				R.requestStatus
			FROM
				Requests R
			LEFT JOIN Users U1 ON R.clientID = U1.userID
			LEFT JOIN Users U2 ON R.masterID = U2.userID
			LEFT JOIN VehicleModels VM ON R.modelID = VM.modelID  -- Join to get car model
			WHERE
				U2.userID = {TableUser.Personnel_Num}
			ORDER BY
				R.startDate DESC");
            if (Dict.Any())
            {
                var Entry = (Dict[0] as IDictionary<string, object>);
                Headers = new string[] { "Номер заказа", "ФИО Клиента", "Модель машины", "Дата", "Статус" };
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
            var Dict = db.ExecuteReadQuery("SELECT COUNT(*) AS OrderCount FROM Requests");
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
