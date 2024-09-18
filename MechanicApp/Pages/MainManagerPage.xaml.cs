using MaterialDesignThemes.Wpf;
using DataBaseControllerLibrary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MechanicApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainManagerPage.xaml
    /// </summary>
    public partial class MainManagerPage : Page
    {
        DB_Handler db = new DB_Handler();
        public MainManagerPage()
        {
            InitializeComponent();
            CreateOrdersInfo();
            CreateInfoBlocks();
        }
        private void CreateInfoBlocks()
        {
            var Dict = db.ExecuteReadQuery(@"SELECT TOP 4 
				U.fio AS ClientName,        
				CR.comment AS Comment,
				CR.rating AS Stars,       
				CR.ratingDate AS RatingDate
			FROM 
				ClientRatings CR
			JOIN 
				Users U ON CR.clientID = U.userID
			ORDER BY 
				CR.ratingDate DESC; ");
            if (Dict.Any())
            {
                for (int i = 0; i < Dict.Count; i++) // Generate 4 blocks as an example
                {
                    var Entry = (Dict[i] as IDictionary<string, object>);

                    // Create the Border that will act as the container for each block
                    Border border = new Border
                    {
                        BorderBrush = Brushes.LightGray,
                        BorderThickness = new Thickness(1),
                        CornerRadius = new CornerRadius(5),
                        Padding = new Thickness(10),
                        Margin = new Thickness(5),
                        Width = 200
                    };

                    // Create a StackPanel to hold the content
                    StackPanel stackPanel = new StackPanel();

                    // Create a horizontal StackPanel for the icon and name
                    StackPanel headerPanel = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };

                    // Create the Ellipse for the user profile picture (simulated with a circle)
                    var profilePic = new PackIcon
                    {
                        Kind = PackIconKind.AccountCircle,
                        Width = 30,
                        Height = 30,
                        Foreground = (Brush)Application.Current.TryFindResource("MaterialDesign.Brush.Secondary")
                    };
                    var res = Application.Current.Resources;
                    //Ellipse profilePic = new Ellipse
                    //{
                    //    Width = 30,
                    //    Height = 30,
                    //    Fill = Brushes.Orange
                    //};

                    // Create the TextBlock for the user name
                    TextBlock userName = new TextBlock
                    {
                        Text = Entry["ClientName"].ToString(),
                        FontWeight = FontWeights.Bold,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };

                    // Add profile picture and user name to the header panel
                    headerPanel.Children.Add(profilePic);
                    headerPanel.Children.Add(userName);

                    // Create the TextBlock for the description text
                    TextBlock description = new TextBlock
                    {
                        Text = Entry["Comment"].ToString(),
                        TextWrapping = TextWrapping.Wrap
                    };

                    RatingBar rating = new RatingBar
                    {
                        Value = Convert.ToInt32(Entry["Stars"]),
                        IsReadOnly = true,
                        Margin = new Thickness(0, 5, 0, 0)
                    };

                    // Add the header panel and description to the main stack panel
                    stackPanel.Children.Add(headerPanel);
                    stackPanel.Children.Add(description);
                    stackPanel.Children.Add(rating);

                    // Add the stack panel to the border
                    border.Child = stackPanel;

                    // Add the border to the WrapPanel container
                    WrapPanelContainer.Children.Add(border);
                }
            }
        }
        private void CreateOrdersInfo()
        {
            var Dict = db.ExecuteReadQuery("SELECT COUNT(*) AS UsersCount FROM Users");
            var Entry = (Dict[0] as IDictionary<string, object>);
            if (Dict.Any())
            {
                OrderAmountTB.Text = $"Кол-во пользователей: {Entry["UsersCount"]}";
            }
            else
            {
                OrderAmountTB.Text = $"Нет пользователей";
            }
        }
    }
}
