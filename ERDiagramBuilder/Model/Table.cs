using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERDiagramBuilder
{
    public class Table
    {
        private string name;

        public List<Field> Fields { get; set; }
        public string Name 
        { 
            get => name; 
            set 
            { 
               
                name= value;
               
            } 
        }
        public DatabaseConnector Connector { get; private set; }

        public Table(string name, DatabaseConnector connector)
        {
            Name = name;
            this.Connector = connector;
            Fields = new List<Field>();
        }
        public Table(List<Field> fields, string name, DatabaseConnector connector) : this(name, connector)
        {
            Fields = fields;
        }
    }
}
