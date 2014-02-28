using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class SchemaReader : ISchemaReader
	{
		Run00.SqlCopySchema.Database ISchemaReader.GetSchema(DatabaseInfo location)
		{
			var server = new Server(location.Server);
			var result = new Run00.SqlCopySchema.Database()
			{
				Name = location.Database,
				Tables = GetTables(server.Databases[location.Database])
			};

			return result;
		}

		private IEnumerable<Run00.SqlCopySchema.Table> GetTables(Microsoft.SqlServer.Management.Smo.Database database)
		{
			return database.Tables.Cast<Microsoft.SqlServer.Management.Smo.Table>().Select(t => new Run00.SqlCopySchema.Table()
			{
				Name = t.Name,
				Database = database.Name,
				Schema = t.Schema,
				IsSystemObject = t.IsSystemObject,
				Columns = GetColumns(t)
			});
		}

		private IEnumerable<Run00.SqlCopySchema.Column> GetColumns(Microsoft.SqlServer.Management.Smo.Table table)
		{
			return table.Columns.Cast<Microsoft.SqlServer.Management.Smo.Column>().Select(c => new Run00.SqlCopySchema.Column()
			{
				Name = c.Name,
				//Nullable = c.Nullable,
				Type = GetClrType(c.DataType.SqlDataType, c.Nullable),
				InPrimaryKey = c.InPrimaryKey,
				Table = table.Name
			});
		}

		private static Type GetClrType(SqlDataType sqlType, bool isNullable)
		{
			switch (sqlType)
			{
				case SqlDataType.BigInt:
					return typeof(long);
					//return isNullable ? typeof(long?) : typeof(long);

				case SqlDataType.Binary:
				case SqlDataType.Image:
				case SqlDataType.Timestamp:
				case SqlDataType.VarBinary:
				case SqlDataType.VarBinaryMax:
					return typeof(byte[]);

				case SqlDataType.Bit:
					return typeof(bool);
					//return isNullable ? typeof(bool?) : typeof(bool);

				case SqlDataType.Char:
				case SqlDataType.NChar:
				case SqlDataType.NText:
				case SqlDataType.NVarChar:
				case SqlDataType.NVarCharMax:
				case SqlDataType.Text:
				case SqlDataType.VarChar:
				case SqlDataType.VarCharMax:
				case SqlDataType.Xml:
				case SqlDataType.SysName:
					return typeof(string);

				case SqlDataType.DateTime:
				case SqlDataType.SmallDateTime:
				case SqlDataType.Date:
				case SqlDataType.Time:
				case SqlDataType.DateTime2:
					//return typeof(DateTime);
					return isNullable ? typeof(DateTime?) : typeof(DateTime);

				case SqlDataType.Numeric:
				case SqlDataType.Decimal:
				case SqlDataType.Money:
				case SqlDataType.SmallMoney:
					return typeof(decimal);
					//return isNullable ? typeof(decimal?) : typeof(decimal);

				case SqlDataType.Float:
					return typeof(double);
					//return isNullable ? typeof(double?) : typeof(double);

				case SqlDataType.Int:
					//return typeof(int);
					return isNullable ? typeof(int?) : typeof(int);

				case SqlDataType.Real:
					return typeof(float);
					//return isNullable ? typeof(float?) : typeof(float);

				case SqlDataType.UniqueIdentifier:
					return typeof(Guid);
					//return isNullable ? typeof(Guid?) : typeof(Guid);

				case SqlDataType.SmallInt:
					return isNullable ? typeof(short?) : typeof(short);

				case SqlDataType.TinyInt:
					return isNullable ? typeof(byte?) : typeof(byte);

				case SqlDataType.Variant:
				case SqlDataType.UserDefinedDataType:
					return typeof(object);

				case SqlDataType.DateTimeOffset:
					return typeof(DateTimeOffset);
					//return isNullable ? typeof(DateTimeOffset?) : typeof(DateTimeOffset);

				default:
					throw new ArgumentOutOfRangeException("sqlType");
			}
		}
	}
}
