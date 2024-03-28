using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Text.RegularExpressions;

namespace ERDiagramBuilder
{
    //коннектор для SQLIte баз данных
    public class SQLiteDBConnector : DatabaseConnector
    {
        public SQLiteDBConnector(string connectionString) : base(connectionString)
        {
            Relations = new Dictionary<string, List<string>>();
            Tables = GetTables();

        }
        //составляется команда sql для базы данных по иземенению названия таблицы и исполняется
        public override void ChangeColumnName(string tableName, string oldColumnName, string newColumnName)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                SQLiteCommand command = new SQLiteCommand($"ALTER TABLE {tableName} RENAME COLUMN {oldColumnName} TO {newColumnName}", connection);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        protected override List<Table> GetTables()
        {
            List<Table> result = new List<Table>();
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                //получение списка таблиц
                connection.Open();
                SQLiteCommand tablesCommand = new SQLiteCommand("SELECT sql FROM sqlite_master WHERE type = 'table'", connection);

                var res = tablesCommand.ExecuteReader();
                List<string> tables = new List<string>();
                while (res.Read())
                {
                    tables.Add(res.GetString(0));
                }

                foreach (var tableCreate in tables)
                {
                    //получение названия таблицы
                    Regex tableNameRegex = new Regex("TABLE \"([#-z]+)\"");
                    Table table = new Table(tableNameRegex.Match(tableCreate).Groups[1].Value, this);
                    if (table.Name == string.Empty) continue;
                    //получение полей
                    Regex fieldsRegex = new Regex("\\t\"([#-z]+)\"\\t([A-Z]+)");
                    List<Field> fields = new List<Field>();
                    foreach (Match field in fieldsRegex.Matches(tableCreate))
                    {
                        fields.Add(new Field(field.Groups[1].Value, field.Groups[2].Value, table));
                    }
                    Regex primaryKey = new Regex("PRIMARY KEY\\(\"([#-z]+)\"");
                    string pkeyName = primaryKey.Match(tableCreate).Groups[1].Value;
                    var pKeyField = fields.First(a => a.Name == pkeyName);
                    pKeyField.IsPrimaryKey = true;
                    table.Fields = fields;
                    //добавление таблицы в итоговый список
                    result.Add(table);
                }
                foreach (var item in tables)
                {
                    Regex tableNameRegex = new Regex("TABLE \"([#-z]+)\"");
                    string tableName = tableNameRegex.Match(item).Groups[1].Value;
                    Regex relationsRegex = new Regex("FOREIGN KEY\\(\"([#-z]+)\"\\) REFERENCES \"([#-z]+)\"\\(\"([#-z]+)\"\\)");
                    List<string> relations = new List<string>();
                    //создание отношений для текущей талицы
                    foreach (Match relation in relationsRegex.Matches(item))
                    {
                        var currentTable = result.First(a => a.Name == tableName);
                        var connectedTable = result.First(a => a.Name == relation.Groups[2].Value);
                        var foreignKeyField = currentTable.Fields.First(a => a.Name == relation.Groups[1].Value);
                        var sourceField = connectedTable.Fields.First(a => a.Name == relation.Groups[3].Value);
                        foreignKeyField.IsForeignKey= true;
                        foreignKeyField.ForeignKeySourse = sourceField;
                    }
                }
                connection.Close();
            }
            //возвращение списка таблиц

            return result;
        }
    }
}
