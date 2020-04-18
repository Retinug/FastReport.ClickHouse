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
			set => dbCommand.CommandText = value; 
		}

		public override int CommandTimeout
		{
			get => dbCommand.CommandTimeout;
			set => dbCommand.CommandTimeout = value;
		}

		public override CommandType CommandType 
		{ 
			get => dbCommand.CommandType;
			set => dbCommand.CommandType = value;
		}

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

		public override void Cancel() => dbCommand.Cancel();

		public override int ExecuteNonQuery() => dbCommand.ExecuteNonQuery();

		public override object ExecuteScalar() => dbCommand.ExecuteScalar();

		public override void Prepare() => dbCommand.Prepare();

		protected override DbParameter CreateDbParameter()
		{
			var parameter = new ClickHouseDbParameter();
			DbParameterCollection.Add(parameter);
			return parameter;
		}

		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => throw new NotImplementedException();

	}
}
