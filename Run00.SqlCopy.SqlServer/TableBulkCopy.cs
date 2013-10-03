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
			System.Diagnostics.Debug.WriteLine("Copying table " + dataTable.TableName);
			var copy = new SqlBulkCopy(targetDatabase.ConnectionString);
			copy.BulkCopyTimeout = 60000;
			if (dataTable.TableName.Equals("dbo.user"))
			{
				dataTable.TableName = "[dbo].[user]";
			}
			copy.DestinationTableName = dataTable.TableName;
			copy.WriteToServer(dataTable);
		}
	}
}