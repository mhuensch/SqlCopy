using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;

namespace Run00.SqlCopySqlServer
{
	public class SchemaCopy : ISchemaCopy
	{
		void ISchemaCopy.CopySchema(DatabaseInfo source, DatabaseInfo target)
		{
			DropTargetDatabseIfExists(target);
			CopySourceDatabaseToTarget(source, target);
		}

		private static void DropTargetDatabseIfExists(DatabaseInfo location)
		{
			//Manually creating and opening connection because SMO with LocalDB has 
			//a bug in opening the connection when querying the database.
			using (var connection = new SqlConnection(location.ConnectionString))
			{
				try
				{		
					connection.Open();
					var server = new Server(new ServerConnection(connection));
					var db = server.Databases[location.Database];

					if (db == null)
						return;

					db.Drop();
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
					connection.Close();
				}
			}
		}

		private static void CopySourceDatabaseToTarget(DatabaseInfo source, DatabaseInfo target)
		{
			//Manually creating and opening connection because SMO with LocalDB has 
			//a bug in opening the connection when querying the database.
			using (var connection = new SqlConnection(source.ConnectionString))
			{
				try
				{
					var server = new Server(new ServerConnection(connection));
					var transfer = new Transfer(server.Databases[source.Database]);

					transfer.DestinationServer = target.Server;
					transfer.DestinationDatabase = target.Database;

					transfer.CopyData = false;
					transfer.CopyAllObjects = false;

					transfer.CopyAllSchemas = true;
					transfer.CopyAllTables = true;
					transfer.Options.DriAll = true;
					transfer.CopyAllDefaults = true;
					transfer.CopyAllViews = false;
					transfer.CopyAllSynonyms = true;
					transfer.CreateTargetDatabase = true;
					transfer.DropDestinationObjectsFirst = true;

					//transfer.CopyAllUserDefinedFunctions = true;
					//transfer.CopyAllUserDefinedTypes = true;
					//transfer.TargetDatabaseFilePath = dataFolder;

					var dataFiles = server.Databases[source.Database].FileGroups.Cast<FileGroup>().SelectMany(fg => fg.Files.Cast<DataFile>()).ToList();
					var d = dataFiles.Select(f => f.FileName);
					MapDatabaseFiles(transfer.DatabaseFileMappings, d, source.Database, target.Database);

					var logFiles = server.Databases[source.Database].LogFiles.Cast<LogFile>().ToList();
					var df = logFiles.Select(f => f.FileName);
					MapDatabaseFiles(transfer.DatabaseFileMappings, df, source.Database, target.Database);

					transfer.TransferData();
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private static void MapDatabaseFiles(DatabaseFileMappingsDictionary dictionary, IEnumerable<string> sourceFiles, string sourceDb, string targetDb)
		{
			foreach (var file in sourceFiles)
				dictionary.Add(file, file.Replace(sourceDb, targetDb));
		}
	}
}
