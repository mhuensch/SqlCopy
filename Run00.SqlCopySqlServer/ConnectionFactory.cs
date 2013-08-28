using Run00.SqlCopy;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Run00.SqlCopySqlServer
{
	public class ConnectionFactory : IConnectionFactory
	{
		IDbConnection IConnectionFactory.Create(DatabaseInfo info)
		{
			return new SqlConnection(info.ConnectionString);
		}
	}
}
