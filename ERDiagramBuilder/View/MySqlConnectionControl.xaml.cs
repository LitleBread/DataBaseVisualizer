using System;
using System.Collections.Generic;
using System.Data.Common;
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

namespace ERDiagramBuilder
{
    /// <summary>
    /// Interaction logic for MySqlConnectionControl.xaml
    /// </summary>
    public partial class MySqlConnectionControl : UserControl, IDBConnectionControl
    {
        
        public MySqlConnectionControl()
        {
            InitializeComponent();
            
        }

        public DatabaseConnector DBConnector { get => new MySqlConnector(GetConnectionString()); }

        public string GetConnectionString()
        {
            return $"Server={ipTB.Text};Database={dbTB.Text};Uid={usernameTB.Text};Pwd={passwordTB.Text};";
        }
    }
}
