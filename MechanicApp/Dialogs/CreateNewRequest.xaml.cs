using MaterialDesignThemes.Wpf;
using DataBaseControllerLibrary;
using Microsoft.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace MechanicApp.Dialogs
{
    /// <summary>
    /// Логика взаимодействия для CreateNewRequest.xaml
    /// </summary>
    public partial class CreateNewRequest : UserControl
    {

        DB_Handler db_h;
        DialogHost Host;
        User TableUser;

        public CreateNewRequest(DialogHost host, User user)
        {
            InitializeComponent();
            db_h = new DB_Handler();
            TableUser = user;
            Host = host;
            var carModels = db_h.ExecuteReadQuery("SELECT modelName as carModel FROM VehicleModels", null)
            .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
            .ToList();
            CarName.ItemsSource = carModels;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CarName.SelectedItem != null && ProblemDesc.Text != "")
            {
                SqlParameter[] param = [
                new SqlParameter("@ClientID", TableUser.Personnel_Num),
                    new SqlParameter("@Problem", ProblemDesc.Text),
                    new SqlParameter($"@carNameID", db_h.ExecuteReadQuery($"SELECT modelID FROM VehicleModels WHERE modelName = '{CarName.SelectedItem}'", null)
                                .Select(x => (x as IDictionary<string, object>).Values.ToArray()[0].ToString())
                                .ToList()[0]
                                ),
                    new SqlParameter("@StartDate", DateTime.Now),
                    new SqlParameter("@Status", "Новая заявка")
                ];
                db_h.ExecuteWriteQuery($"INSERT INTO Requests (startDate, problemDescription, clientID, modelID, requestStatus) VALUES (@StartDate, @Problem, @ClientID, @carNameID, @Status); ", param);
                Host.CurrentSession.Close(false);
            }
        }
    }
}
