using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ERDiagramBuilder
{
    public class Field
    {
        public Field(string name, string type, Table parent)
        {
            Name = name;
            Type = type;
            this.Parent = parent;
            
        }
        private string name;
        public string Name
        {
            get => name;
            set
            {
                if (Parent != null)
                {
                    Parent.Connector.ChangeColumnName(Parent.Name, name, value);
                }
                name = value;
            }
        }
        public string Type { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public Field ForeignKeySourse { get; set; }
        public Table Parent { get; private set; }
        public Brush Brush
        {
            get
            {
                if (IsPrimaryKey) return Brushes.Gold;
                if (IsForeignKey) return Brushes.Coral;
                return Brushes.AntiqueWhite;
            }
        }
        
        
    }
}
