using ERDiagramBuilder.View;
using Google.Protobuf.Collections;
using System;
using System.Collections;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<TableControl> tables;
        private DatabaseConnector connector;
        public bool IsStraightLined
        {
            get => Graph.IsStraightLined;
            set => Graph.IsStraightLined = value;
        }
        public MainWindow()
        {
            InitializeComponent();
            connector = new MySqlConnector("Server=localhost;Database=test_schema;User=root;Password=123456;");
            tables = new List<TableControl>();
            
            mainField.PreviewMouseMove += (a, b) =>
            {
                if (dragElement == null)
                    return;
                var position = b.GetPosition(a as IInputElement);
                Canvas.SetTop(dragElement, position.Y - offset.Y);
                Canvas.SetLeft(dragElement, position.X - offset.X);
            };
            mainField.PreviewMouseUp += (a, b) =>
            {
                mainField.ReleaseMouseCapture();
                dragElement = null;
            };
            DataContext = this;
            IsStraightLined = true;
        }
        UIElement dragElement;
        Point offset;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        //добавление таблиц на поле при задании коннектора
        private void UpdateTables()
        {
            foreach (var item in connector.Tables)
            {
                tables.Add(new TableControl(item));
            }
            List<FieldControl> fields = new List<FieldControl>();
            foreach (var item in tables)
            {
                foreach (var field in item.Table.Fields)
                {
                    fields.Add(new FieldControl(field, item));
                }
            }
            foreach (var field in fields)
            {
                if (field.Field.IsForeignKey)
                {
                    field.ConnectedField = fields.First(a => a.Field == field.Field.ForeignKeySourse);
                }
            }

            bool[,] adjancencyMatrix = new bool[fields.Count, fields.Count];
            for (int i = 0; i < fields.Count; i++)
            {
                if (fields[i].IsForeignKey)
                {
                    adjancencyMatrix[i, fields.IndexOf(fields[i].ConnectedField)] = true;
                    //adjancencyMatrix[fields.IndexOf(fields[i].ConnectedField), i] = true;
                }
            }
            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].Fields = fields.Where(a => a.ParentTable.Equals(tables[i])).ToList();
            }
            Graph graph = new Graph(adjancencyMatrix, fields.Select(a => a as UIElement).ToArray(), mainField);

            Random rng = new Random();
            mainField.Children.Clear();
            foreach (var item in tables)
            {
                Point position = new Point(rng.Next(10, (int)(mainField.ActualWidth - 200)), rng.Next(10, (int)(mainField.ActualHeight - 200)));
                Canvas.SetLeft(item, position.X);
                Canvas.SetTop(item, position.Y);
                Canvas.SetZIndex(item, 1);
                mainField.Children.Add(item);
            }
            for (int i = 0; i < tables.Count; i++)
            {
                tables[i].PreviewMouseDoubleClick += TablePreviewMouseDown;
            }
        }

        private void TablePreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is TableControl)
            {
                dragElement = sender as UIElement;
                offset = e.GetPosition(mainField);
                Canvas.SetZIndex((UIElement)dragElement, 2);
                foreach (var item in mainField.Children.OfType<TableControl>())
                {
                    if (item.Equals(dragElement)) continue;
                    Canvas.SetZIndex(item, 1);
                }

                if (!double.IsNaN(Canvas.GetTop(dragElement)))
                {
                    offset.Y -= Canvas.GetTop(dragElement);
                }
                if (!double.IsNaN(Canvas.GetLeft(dragElement)))
                {
                    offset.X -= Canvas.GetLeft(dragElement);
                }
                mainField.CaptureMouse();
            }

        }

        private void ConnectDBButtonClick(object sender, RoutedEventArgs e)
        {
            DatabaseConnectionWindow dbcw = new DatabaseConnectionWindow();
            dbcw.Closed += (a, b) =>
            {
                if (dbcw.DBConnector != null)
                {
                    connector = dbcw.DBConnector;
                    UpdateTables();
                }
                
            };
            dbcw.Show();
        }
    }


    

   
}
