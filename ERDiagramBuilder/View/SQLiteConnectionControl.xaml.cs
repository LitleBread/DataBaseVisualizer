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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ERDiagramBuilder
{
    /// <summary>
    /// Interaction logic for SQLiteConnectionControl.xaml
    /// </summary>
    public partial class SQLiteConnectionControl : UserControl, IDBConnectionControl
    {
        public SQLiteConnectionControl()
        {
            InitializeComponent();
        }
        private string connectionString;

        public DatabaseConnector DBConnector { get; set; }

        //public DatabaseConnector DBConnector => new SQLiteDBConnector(GetConnectionString());

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {

            if (!((string[])e.Data.GetData(DataFormats.FileDrop))[0].Contains(".db"))
            {
                hintTB.Text = "Неверный формат файла";
                return;
            }
            hintTB.Text = "Путь указан выше";
            connectionString = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            filePathTB.Text = connectionString;
            DBConnector = new SQLiteDBConnector($"Data Source={connectionString};Version=3;");
        }

        public string GetConnectionString()
        {
            return $"Data Source={connectionString};";
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            hintTB.Text = "Отпустите";
        }

        private void Grid_DragLeave(object sender, DragEventArgs e)
        {
            hintTB.Text = "Или перетащите файл сюда";
        }
    }
}
