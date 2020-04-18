using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;

namespace ClickHouse.Ado
{
    class ClickHouseDataAdapter : DbDataAdapter
    {
        public ClickHouseDataAdapter()
        {
        }
        public ClickHouseDataAdapter(string selectCommand, ClickHouseDbConnection connection)
        {
            SelectCommand = new ClickhouseDbCommand(connection, selectCommand);
            //SelectCommand = selectCommand;
        }

        public ClickHouseDataAdapter(DbCommand selectCommand)
        {
            SelectCommand = selectCommand;
        }

        public override int Fill(DataSet dataSet)
        {
            return base.Fill(dataSet);
        }

        
    }
}
