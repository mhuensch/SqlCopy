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
		public TableBulkCopy (IQueryBuilder queryBuilder)
		{
			_queryBuilder = queryBuilder;
		}

		void ITableBulkCopy.Copy(IDbConnection connection, IQueryable query)
		{
			var sqlConnection = connection as SqlConnection;
			if (sqlConnection == null)
				throw new InvalidOperationException();

			var q = _queryBuilder.Build(query);
			var command = new SqlCommand(q.Sql, sqlConnection);
			foreach(var p in q.Parameters)
				command.Parameters.Add(new SqlParameter(p.Name, p.Value));

			var copy = new SqlBulkCopy(sqlConnection);
			copy.DestinationTableName = query.ElementType.FullName;
			copy.WriteToServer(command.ExecuteReader());
		}

		private readonly IQueryBuilder _queryBuilder;
	}
}
