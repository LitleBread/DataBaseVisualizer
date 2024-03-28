using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ERDiagramBuilder.View;
using MySql;
using MySql.Data;
using MySql.Data.MySqlClient;
using MySql.Data.Types;


namespace ERDiagramBuilder
{
    internal class MySqlConnector : DatabaseConnector
    {
        private MySqlConnection connection;
        public MySqlConnector(string connectionString) : base(connectionString)
        {
            
            Tables = GetTables();
        }

        public override void ChangeColumnName(string tableName, string oldColumnName, string newColumnName)
        {
            connection.Open();
            //получение типа поля
            MySqlCommand getTypeCommand = new MySqlCommand($"show columns from {tableName} ;", connection);
            int rows = getTypeCommand.ExecuteNonQuery();
            var reader = getTypeCommand.ExecuteReader();
            string type = "";
            
            while (reader.Read())
            {
                type = reader.GetString(1);
                if (reader.GetString(0) == oldColumnName) break;
            }
            reader.Close();
            //команда для изменения поля
            MySqlCommand changeNameCommand = new MySqlCommand($"ALTER TABLE `{connection.Database}`.`{tableName}` CHANGE COLUMN `{oldColumnName}` `{newColumnName}` {type} NULL DEFAULT NULL ;", connection);
            changeNameCommand.ExecuteNonQuery();
            connection.Close();
        }

        class TableField
        {
            public TableField(string tableName, string fieldName)
            {
                TableName = tableName;
                FieldName = fieldName;
            }

            public string TableName { get; set; }
            public string FieldName { get; set; }
        }
        override protected List<Table> GetTables()
        {
            
            try
            {
                connection = new MySqlConnection(connectionString);
                connection.Open();
            }
            catch { }
            //получение списка таблиц
            MySqlCommand getAllTables = new MySqlCommand("SHOW TABLES;", connection);
            var tablesNameReader = getAllTables.ExecuteReader();
            List<Table> result = new List<Table>();
            while (tablesNameReader.Read())
            {
                string tableName = tablesNameReader.GetString(0);
                result.Add(new Table(tableName, this));
            }
            tablesNameReader.Close();

            Dictionary<TableField, TableField> relations = new Dictionary<TableField, TableField>();
            //получение полей для каждой таблицы
            foreach (var tbl in result)
            {
                MySqlCommand getTableInfo = new MySqlCommand($"SHOW CREATE TABLE {tbl.Name}", connection);
                var _ = getTableInfo.ExecuteReader();
                _.Read();
                string tblInfo = _.GetString(1);
                _.Close();

                Regex tblFields = new Regex("`([a-zA-Z_0-9]+)` ([a-z]+)");
                foreach (Match item in tblFields.Matches(tblInfo))
                {
                    Field field = new Field(item.Groups[1].Value, item.Groups[2].Value, tbl);
                    tbl.Fields.Add(field);
                }

                Regex tblPrimaryKey = new Regex("PRIMARY KEY \\(`([a-zA-Z_0-9]+)`\\)");
                string pKey = tblPrimaryKey.Match(tblInfo).Groups[1].Value;
                var keyField = tbl.Fields.First(e => e.Name == pKey);
                keyField.IsPrimaryKey = true;

                Regex foreignKeys = new Regex("FOREIGN KEY \\(`([a-zA-Z_0-9]+)`\\) REFERENCES `([a-zA-Z_0-9]+)` \\(`([a-zA-Z_0-9]+)`\\)");
                foreach (Match item in foreignKeys.Matches(tblInfo))
                {
                    relations.Add(new TableField(item.Groups[2].Value, item.Groups[3].Value), new TableField(tbl.Name, item.Groups[1].Value));
                }
            }

            foreach (var item in relations)
            {
                var sourceTbl = result.First(e => e.Name == item.Key.TableName);
                var sourceField = sourceTbl.Fields.First(e => e.Name == item.Key.FieldName);
                
                var foreignTbl = result.First(e => e.Name == item.Value.TableName);
                var foreignField = foreignTbl.Fields.First(e => e.Name == item.Value.FieldName);
                foreignField.ForeignKeySourse = sourceField;
                foreignField.IsForeignKey = true;
            }
           

            connection.Close();
            return result;
        }
    }
}
