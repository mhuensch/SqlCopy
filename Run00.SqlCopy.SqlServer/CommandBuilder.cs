using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class CommandBuilder : ICommandBuilder
	{
		public CommandBuilder(IQueryBuilder queryBuilder)
		{
			_queryBuilder = queryBuilder;
		}

		IDbCommand ICommandBuilder.Build(DatabaseInfo database, IQueryable query)
		{
			var connection = new SqlConnection(database.ConnectionString);
			connection.Open();

			var q = _queryBuilder.Build(query);
			var command = new SqlCommand(q.Sql, connection);
			foreach (var p in q.Parameters)
				command.Parameters.Add(new SqlParameter(p.Name, p.Value));

			return command;
		}

		private readonly IQueryBuilder _queryBuilder;
	}
}
