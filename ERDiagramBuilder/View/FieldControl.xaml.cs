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

namespace ERDiagramBuilder.View
{
    /// <summary>
    /// Interaction logic for FieldControl.xaml
    /// </summary>
    public partial class FieldControl : UserControl
    {
        public Field Field { get; set; }
        public TableControl ParentTable { get; set; }
        public FieldControl ConnectedField { get; set; }
        public SolidColorBrush BackgroundColor
        {
            get
            {
                if (IsPrimaryKey) return Brushes.Gold;
                else if (IsForeignKey) return Brushes.Orange;
                else return Brushes.Transparent;
            }
        }
        public bool IsForeignKey { get => Field.IsForeignKey; }
        public bool IsPrimaryKey { get => Field.IsPrimaryKey; }

        public FieldControl(Field field, TableControl parentTable = null, FieldControl connectedField = null )
        {
            this.Field = field;
            this.ParentTable = parentTable;
            this.ConnectedField = connectedField;
            InitializeComponent();
        }
    }
}
