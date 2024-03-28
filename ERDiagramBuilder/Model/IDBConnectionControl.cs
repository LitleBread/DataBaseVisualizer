using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERDiagramBuilder
{
    internal interface IDBConnectionControl
    {
        string GetConnectionString();
        DatabaseConnector DBConnector { get;}
    }
}
