using ERDiagramBuilder.View;
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
    /// Interaction logic for TableControl.xaml
    /// </summary>
    public partial class TableControl : UserControl
    {
       
        public Table Table
        {
            get { return (Table)GetValue(TableProperty); }
            set { SetValue(TableProperty, value); }
        }
        public string TableName
        {
            get { return (string)GetValue(NameProperty); }
            set 
            { 
                SetValue(NameProperty, value);

            }
        }
        public List<FieldControl> Fields
        {
            get { return (List<FieldControl>)GetValue(FieldsProperty); }
            set { SetValue(FieldsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Table.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableProperty =
            DependencyProperty.Register("TableProperty", typeof(Table), typeof(TableControl), new PropertyMetadata());
        // Using a DependencyProperty as the backing store for Name.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TableNameProperty =
            DependencyProperty.Register("TableName", typeof(string), typeof(TableControl), new PropertyMetadata());
        // Using a DependencyProperty as the backing store for Fields.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FieldsProperty =
            DependencyProperty.Register("Fields", typeof(List<FieldControl>), typeof(TableControl), new PropertyMetadata());
        
        public TableControl(Table table, List<FieldControl> fields = null)
        {
            InitializeComponent();
            this.Table = table;
            this.TableName = table.Name;
            this.Fields = fields == null ? new List<FieldControl>() : fields;
            DataContext = this;
            
        }
    }
}
