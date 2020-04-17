using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using FastReport.Data.ConnectionEditors;
using FastReport.Forms;
using FastReport.Utils;
using ClickHouse.Ado;

namespace FastReport.Data
{
	public partial class ClickHouseConnectionEditor : ConnectionEditorBase
	{
		private string FConnectionString;

		private void btnAdvanced_Click(object sender, EventArgs e)
		{
			using (AdvancedConnectionPropertiesForm form = new AdvancedConnectionPropertiesForm())
			{
				ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
				builder.ConnectionString = ConnectionString;
				form.AdvancedProperties = builder;
				if (form.ShowDialog() == DialogResult.OK)
				ConnectionString = form.AdvancedProperties.ToString();
			}
		}

		private void Localize()
		{
			//MyRes res = new MyRes("ConnectionEditors,Common");

			//gbServer.Text = res.Get("ServerLogon");
			//lblServer.Text = res.Get("Server");
			//lblUserName.Text = res.Get("UserName");
			//lblPassword.Text = res.Get("Password");

			//gbDatabase.Text = res.Get("Database");
			//lblDatabase.Text = res.Get("DatabaseName");
			//btnAdvanced.Text = Res.Get("Buttons,Advanced");
		}

		protected override string GetConnectionString()
		{
			ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
			builder.ConnectionString = FConnectionString;

			builder.Host = tbServer.Text;
			builder.SessionId = tbUserName.Text;
			builder.Password = tbPassword.Text;
			builder.Database = tbDatabase.Text;
	  
			return builder.ToString();
		}

		protected override void SetConnectionString(string value)
		{
			FConnectionString = value;

			ClickHouseConnectionStringBuilder builder = new ClickHouseConnectionStringBuilder();
			builder.ConnectionString = value;

			tbServer.Text = builder.Host;
			tbUserName.Text = builder.User;
			tbPassword.Text = builder.Password;
			tbDatabase.Text = builder.Database;
		}

		public ClickHouseConnectionEditor()
		{
			InitializeComponent();
			Localize();
		}
	}
}
