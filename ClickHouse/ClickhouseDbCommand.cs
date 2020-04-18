using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClickHouse.Ado;

namespace ClickHouse.Ado
{
	class ClickhouseDbCommand : DbCommand
	{
		private readonly ClickHouseCommand dbCommand;
		private readonly ClickHouseDbConnection dbConnection;

		public ClickhouseDbCommand(ClickHouseDbConnection connection)
		{
			dbCommand = new ClickHouseCommand(new ClickHouseConnection(connection.ConnectionString));
			Connection = connection;
		}

		public ClickhouseDbCommand(ClickHouseDbConnection connection, string commandText)
		{
			dbCommand = new ClickHouseCommand(new ClickHouseConnection(connection.ConnectionString), commandText);
			CommandText = commandText;
		}
		
		public override string CommandText 
		{ 
			get => dbCommand.CommandText; 
			set => value = dbCommand.CommandText; 
		}

		public override int CommandTimeout { get; set; }

		public override CommandType CommandType { get; set; }

		public override bool DesignTimeVisible { get; set; }

		public override UpdateRowSource UpdatedRowSource { get; set; }

		protected override DbConnection DbConnection
		{
			get => dbConnection;
			set => throw new NotSupportedException();
		}

		protected override DbParameterCollection DbParameterCollection { get; } = new ClickHouseDbParameterCollection();

		protected override DbTransaction DbTransaction { get; set; }

		protected override bool CanRaiseEvents => base.CanRaiseEvents;

		public override void Cancel() => throw new NotImplementedException();

		public override int ExecuteNonQuery() => throw new NotImplementedException();

		public override object ExecuteScalar() => throw new NotImplementedException();

		public override void Prepare() => throw new NotImplementedException();

		protected override DbParameter CreateDbParameter()
		{

			var parameter = new ClickHouseDbParameter();
			DbParameterCollection.Add(parameter);
			return parameter;
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotImplementedException();

	}
}
