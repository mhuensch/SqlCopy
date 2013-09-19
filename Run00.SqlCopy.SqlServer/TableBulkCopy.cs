using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Run00.SqlCopySqlServer
{
	public class TableBulkCopy : ITableBulkCopy
	{
		void ITableBulkCopy.Copy(DatabaseInfo targetDatabase, DataTable dataTable)
		{
			var copy = new SqlBulkCopy(targetDatabase.ConnectionString);
			copy.DestinationTableName = dataTable.TableName;
			copy.WriteToServer(dataTable);
		}
	}
}