using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PermDynamics
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Classes.PointInfo> pointsInfo = new List<Classes.PointInfo>();
        public List<Classes.PointInfo> pointsInfoSecond = new List<Classes.PointInfo>();
        public enum pages{
            main,
            chart
        }

        public void OpenPages(pages _pages)
        {
            if (_pages == pages.main)
                frame.Navigate(new Pages.Main(this));
            else if(_pages == pages.chart)
                frame.Navigate(new Pages.Chart(this));
        }

        public MainWindow()
        {
            InitializeComponent();
            OpenPages(pages.main);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = "server=localhost;user=root;password=;database=chart";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string tableName = "chart_" + DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string createTableQuery = $"CREATE TABLE {tableName} (id INT AUTO_INCREMENT PRIMARY KEY, value DOUBLE)";

                using (MySqlCommand createTableCommand = new MySqlCommand(createTableQuery, connection))
                {
                    createTableCommand.ExecuteNonQuery();
                }

                Console.WriteLine($"Таблица {tableName} успешно создана.");

                string insertDataQuery = $"INSERT INTO {tableName} (value) VALUES ";

                List<MySqlParameter> parameters = new List<MySqlParameter>();
                for (int i = 0; i < pointsInfo.Count; i++)
                {
                    string paramName = $"@value{i}";
                    insertDataQuery += $"({paramName})";

                    if (i < pointsInfo.Count - 1)
                        insertDataQuery += ", ";

                    parameters.Add(new MySqlParameter(paramName, pointsInfo[i].value));
                }

                using (MySqlCommand insertDataCommand = new MySqlCommand(insertDataQuery, connection))
                {
                    insertDataCommand.Parameters.AddRange(parameters.ToArray());
                    insertDataCommand.ExecuteNonQuery();
                }

                connection.Close();
            }
        }
    }
}
