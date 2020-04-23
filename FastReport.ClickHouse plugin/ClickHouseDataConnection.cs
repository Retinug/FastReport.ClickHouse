using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Common;
using FastReport.Data.ConnectionEditors;
using System.Data;
using ClickHouse.Client.ADO;
using ClickHouse.Client.ADO.Adapters;
using ClickHouse.Client.ADO.Parameters;
using ClickHouse.Client.Types;

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
                if ((!list.Contains(row[1].ToString()) && (String.Compare(row[0].ToString(), databaseName) == 0)))
                    list.Add(row[1].ToString());
            }
        }

        public override string[] GetTableNames()
        {
            List<string> list = new List<string>();
            GetDBObjectNames("Columns", list);
            return list.ToArray();
        }

        public override string QuoteIdentifier(string value, DbConnection connection)
        {
            return "\"" + value + "\"";
        }

        protected override string GetConnectionStringWithLoginInfo(string userName, string password)
        {
            ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
            builder.ConnectionString = ConnectionString;

            builder.Username = userName;
            builder.Password = password;

            return builder.ToString();
        }

        public override Type GetConnectionType()
        {
            return typeof(ClickHouseConnection);
        }

        public override DbDataAdapter GetAdapter(string selectCommand, DbConnection connection, CommandParameterCollection parameters)
        {
            ClickHouseDataAdapter adapter = new ClickHouseDataAdapter();
            adapter.SelectCommand = new ClickHouseCommand(connection as ClickHouseConnection);
            adapter.SelectCommand.CommandText = selectCommand;

            foreach (CommandParameter p in parameters)
            {
                ClickHouseDbParameter parameter = new ClickHouseDbParameter();
                adapter.SelectCommand.Parameters.Add(p.Name);
                parameter.Value = p.Value;
            }
            return adapter;
        }

        public override Type GetParameterType()
        {
            return typeof(ClickHouseTypeCode);
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
            return "ClickHouse: " + info;
        }

        //public override void FillTableSchema(DataTable table, string selectCommand, CommandParameterCollection parameters)
        //{
        //    using (DbConnection conn = GetConnection())
        //    {
        //        OpenConnection(conn);
        //        // prepare select command
        //        //ClickHouseCommand selectClickHouseCommand = new ClickHouseCommand(conn as ClickHouseConnection);
        //        //selectCommand = PrepareSelectCommand(selectCommand, table.TableName, conn);
        //        // read the table schema
        //        using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
        //        {
        //            adapter.SelectCommand.CommandTimeout = CommandTimeout;
        //            //adapter.FillSchema(table, SchemaType.Source);
        //        }
        //    }
        //}

        public override void FillTableData(DataTable table, string selectCommand, CommandParameterCollection parameters)
        {
            using (DbConnection conn = GetConnection())
            {
                OpenConnection(conn);
                using (DbDataAdapter adapter = GetAdapter(selectCommand, conn, parameters))
                {
                    adapter.SelectCommand.CommandTimeout = CommandTimeout;
                    table.Clear();
                    adapter.Fill(table);
                }
            }
        }


        public override ConnectionEditorBase GetEditor()
        {
            return new ClickHouseConnectionEditor();
        }

    }
}
