using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace ERDiagramBuilder
{
    /// <summary>
    /// Interaction logic for DatabaseConnectionWindow.xaml
    /// </summary>
    public partial class DatabaseConnectionWindow : Window
    {
        private Dictionary<string, UserControl> connectionViews;
        private UserControl connetionInterface;
        //названия существующих коннекторов
        public List<string> ConnetionTypes
        {
            get => connectionViews.Select(e => e.Key).ToList();
        }
        public DatabaseConnectionWindow()
        {
            InitializeComponent();
            //список интерфейсов соединения, коннекторов
            connectionViews = new Dictionary<string, UserControl>()
            {
                {"MySql", new MySqlConnectionControl() },
                {"SQLite", new SQLiteConnectionControl() }
            };
            //установка элементов выпадающего списка и обновление интерфейса при выборе
            connectionType.ItemsSource = ConnetionTypes;
            //обновление интефеса при выборе
            connectionType.SelectionChanged += (a, b) =>
            {
                connetionInterface = connectionViews[connectionType.SelectedItem.ToString()];
                connetionInterface.Visibility = Visibility.Visible;
                connetionInterface.Margin = new Thickness(4);
                connetionInterface.SetValue(Grid.RowProperty, 1);
                gridForUserControl.Children.Clear();
                gridForUserControl.Children.Add(connetionInterface);
            };
        }
        public DatabaseConnector DBConnector { get; private set; }
        private void connectBTN_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //все интерфейсы(внешние) соединения должны реализовать интерфейс(программный) IDBConnectionControl
                DBConnector = (connetionInterface as IDBConnectionControl).DBConnector;
                this.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }
    }
}
