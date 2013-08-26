using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class SchemaReader : ISchemaReader
	{
		Run00.SqlCopy.Database ISchemaReader.GetSchema(DatabaseLocation location)
		{
			var server = new Server(location.Server);
			var result = new Run00.SqlCopy.Database()
			{
				Name = location.Database,
				Tables = GetTables(server.Databases[location.Database])
			};

			return result;
		}

		private IEnumerable<Run00.SqlCopy.Table> GetTables(Microsoft.SqlServer.Management.Smo.Database database)
		{
			return database.Tables.Cast<Microsoft.SqlServer.Management.Smo.Table>().Select(t => new Run00.SqlCopy.Table()
			{
				Name = t.Name,
				Database = database.Name,
				Schema = t.Schema,
				Columns = GetColumns(t)
			});
		}

		private IEnumerable<Run00.SqlCopy.Column> GetColumns(Microsoft.SqlServer.Management.Smo.Table table)
		{
			return table.Columns.Cast<Microsoft.SqlServer.Management.Smo.Column>().Select(c => new Run00.SqlCopy.Column()
			{
				Name = c.Name,
				Table = table.Name
			});
		}
	}
}
