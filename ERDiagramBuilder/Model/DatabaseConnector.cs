using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERDiagramBuilder
{
    //базовый класс для соединения с бд
    public abstract class DatabaseConnector
    {
        //строка соединения, нужна везде
        protected virtual string connectionString { get; set; }
        //таблицы
        public List<Table> Tables { get; set; }
        //отношения таблиц, какая с какой связана
        public Dictionary<string, List<string>> Relations { get; set; }
        //метод для изменения названия поля
        public abstract void ChangeColumnName(string tableName, string oldColumnName, string newColumnName);
        protected abstract List<Table> GetTables();
        protected DatabaseConnector(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
