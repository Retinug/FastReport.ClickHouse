using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;
using ClickHouse.Ado;

namespace ClickHouse.Ado
{
    class ClickHouseDbParameter : DbParameter
    {
        private readonly ClickHouseParameter parameter;

        public ClickHouseDbParameter()
        {
        }

        public ClickHouseDbParameter(ClickHouseParameter parameter)
        {
            this.parameter = parameter;
        }

        public override DbType DbType
        {
            get => parameter.DbType;
            set => parameter.DbType = value;
        }

        public override ParameterDirection Direction { get; set; }

        public override bool IsNullable { get; set; }

        public override string ParameterName 
        { 
            get => parameter.ParameterName;
            set => parameter.ParameterName = value;
        }

        public override int Size { get; set; }

        public override string SourceColumn { get; set; }

        public override bool SourceColumnNullMapping { get; set; }

        public override object Value
        {
            get => parameter.Value;
            set => parameter.Value = value;
        }
        public override DataRowVersion SourceVersion { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void ResetDbType() { throw new NotImplementedException(); }
    }
}
