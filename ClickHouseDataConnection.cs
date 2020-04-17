using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using System.Data;
using ClickHouse.Ado;

namespace FastReport.Data
{
    public class ClickHouseDataConnection : DataConnectionBase
    {
        private void GetDBObjectNames(string name, List<string> list)
        {
            DataTable schema = null;
            string databaseName = "";
            DbConnection connection = GetConnection();
            try
            {
                OpenConnection(connection);
                ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
                builder.ConnectionString = ConnectionString;
                schema = connection.GetSchema(name);
                databaseName = builder.Database;
            }
            finally
            {
                DisposeConnection(connection);
            }
            foreach (DataRow row in schema.Rows)
            {
                if(String.IsNullOrEmpty(databaseName) || String.Compare(row["TABLE_SCHEMA"].ToString(), databaseName) == 0)
                list.Add(row["TABLE_NAME"].ToString());
            }
        }

        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Tables", list);
            GetDBObjectNames("Views", list);
            return list.ToArray();
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "`" + value + "`";
        }

        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;

            builder.User = userName;
            builder.Password = password;

            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
            return typeof(ClickHouseConnection);
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection,
          CommandParameterCollection parameters)
        {
            ClickHouseDataAdapter adapter = new ClickHouseDataAdapter();

            DataSet data = new DataSet();

            adapter.Fill(data);
            //MySqlDataAdapter adapter = new MySqlDataAdapter(selectCommand, connection as MySqlConnection);
            //foreach (CommandParameter p in parameters)
            //{
            //    //MySqlParameter parameter = adapter.SelectCommand.Parameters.Add(p.Name, (MySqlDbType)p.DataType, p.Size);
            //    ClickHouseParameter parameter.;
            //    parameter.Value = p.Value;
            //}
            return adapter;
        }

        public override Type GetParameterType()
        {
            //return typeof(MySqlDbType);
            return null;
        }

        public override string GetConnectionId()
        {
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;
            string info = "";
            try
            {
                info = builder.Database;
            }
            catch
            {
            }
            return "MySQL: " + info;
        }

        public override ConnectionEditorBase GetEditor()
        {
            return new ClickHouseConnectionEditor();
        }

    }
}
