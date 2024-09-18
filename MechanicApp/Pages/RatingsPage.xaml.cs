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
    /// Логика взаимодействия для RatingsPage.xaml
    /// </summary>
    public partial class RatingsPage : Page
    {
        DB_Handler DB_h;
        DialogHost Host;
        string[] Headers;
        string[] Bindings;

        public RatingsPage(DialogHost host)
        {
            InitializeComponent();
            Host = host;
        }
        private void Page_Initialized(object sender, EventArgs e)
        {
            RefreshTable();
        }

        private void RefreshTable()
        {
            DB_h = new DB_Handler();
            var Dict = DB_h.ExecuteReadQuery(@"SELECT

				U.fio AS ClientName,        
				CR.comment AS Comment,
				CR.rating AS Stars,       
				CR.ratingDate AS RatingDate
			FROM 
				ClientRatings CR
			JOIN 
				Users U ON CR.clientID = U.userID
			ORDER BY 
				CR.ratingDate DESC;");
            if (Dict.Any())
            {
                var b = (Dict[0] as IDictionary<string, object>);
                Headers = [
                    "Номер рейтинга",
                    "ФИО",
                    "Текст",
                    "Рейтинг",
                    "Дата",
                ];
                Bindings = b.Keys.ToArray();
                RatingsGrid.Columns.Clear();
                foreach (var a in Enumerable.Zip(Headers, Bindings, (header, binding) => new { First = header, Second = binding }))
                {
                    if (a.First.ToLower().Contains("номер"))
                    {
                        continue;
                    }
                    RatingsGrid.Columns.Add(new System.Windows.Controls.DataGridTextColumn
                    {
                        Binding = new System.Windows.Data.Binding(a.Second)
                        {
                            StringFormat = b[a.Second] is DateTime ? "{0:dd/MM/yyyy}" : ""
                        },
                        Header = a.First,

                    });
                }
                AddButtonColumn();
                RatingsGrid.ItemsSource = Dict;
            }
        }

        private void AddButtonColumn()
        {
            // Create a DataGridTemplateColumn
            DataGridTemplateColumn buttonColumn = new DataGridTemplateColumn();
            buttonColumn.Header = "Чтение";

            // Create a DataTemplate that contains a button
            FrameworkElementFactory buttonFactory = new FrameworkElementFactory(typeof(Button));
            buttonFactory.SetValue(Button.ContentProperty, "Прочитать");
            buttonFactory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.White);

            // Set the event handler for the button
            buttonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(Button_Click));

            DataTemplate buttonTemplate = new DataTemplate();
            buttonTemplate.VisualTree = buttonFactory;

            // Set the CellTemplate to the button template
            buttonColumn.CellTemplate = buttonTemplate;

            // Add the column to the DataGrid
            RatingsGrid.Columns.Add(buttonColumn);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            var rowData = (button.DataContext as ExpandoObject) as IDictionary<string, object>;
            var rd = new ReadRating(Host, rowData["ClientName"].ToString(), Convert.ToInt32(rowData["Stars"]), rowData["Comment"].ToString(), rowData["RatingDate"] as DateTime?);
            Host.ShowDialog(rd);
        }

        private void SaveAsPdfButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF files (*.pdf)|*.pdf";
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                ExportClass.ExportDataGridToPdf(RatingsGrid, saveFileDialog.FileName, Headers.Skip(1).ToArray(), Bindings.Skip(1).ToArray(), "Список сотрудников");
            }
        }

        private void ExportToCsvButton_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            Nullable<bool> result = saveFileDialog.ShowDialog();
            if (result == true)
            {
                ExportClass.ExportDataGridToCsv(RatingsGrid, saveFileDialog.FileName, Headers.Skip(1).ToArray(), Bindings.Skip(1).ToArray());
            }
        }
    }
}
