using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SmoLiteApi;

namespace Run00.SqlCopy.SqlCopySchema.SmoLite
{
	public class SchemaCopy : ISchemaCopy
	{
		void ISchemaCopy.CopySchema(DatabaseInfo source, DatabaseInfo target)
		{
			DropTargetDatabseIfExists(target);
		}

		private static void DropTargetDatabseIfExists(DatabaseInfo location)
		{
			using (var connection = new SqlConnection(location.ConnectionString))
			{
				var server = new Server(connection);
				try
				{
					connection.Open();
					server.ConnectionContext.ServerInstance = location.Database;
					var db = server.Databases[location.Database];


					if (db == null)
						return;

					// db.Drop() doens't seem to close existing connections
					server.KillDatabase(location.Database);
				}
				catch (SqlException ex)
				{
					//Throw the error if a problem occurred that was not connecting to the database.
					var error = ex.Errors.Cast<SqlError>().FirstOrDefault();
					if (error == null || error.Number != 4060)
						throw;
				}
				finally
				{
					server.ConnectionContext.Disconnect();
				}
			}
		}
	}
}
