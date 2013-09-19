using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
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

					transfer.TargetDatabaseFilePath = GetServerDirectory(target);
					RemapServerFiles(server, transfer, source, target);

					transfer.TransferData();
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private static void RemapServerFiles(Server server, Transfer transfer, DatabaseInfo source, DatabaseInfo target)
		{
			var dir = GetServerDirectory(target);
			var dataFiles = server.Databases[source.Database].FileGroups.Cast<FileGroup>().SelectMany(fg => fg.Files.Cast<DataFile>()).ToList();
			var dataFile = dataFiles.Select(f => f.FileName);
			foreach (var file in dataFile)
				transfer.DatabaseFileMappings.Add(file, Path.Combine(dir, Path.GetFileName(file).Replace(source.Database, target.Database)));

			var logFiles = server.Databases[source.Database].LogFiles.Cast<LogFile>().ToList();
			var logFile = logFiles.Select(f => f.FileName);
			foreach (var file in logFile)
				transfer.DatabaseFileMappings.Add(file, Path.Combine(dir, Path.GetFileName(file).Replace(source.Database, target.Database)));
		}

		private static string GetServerDirectory(DatabaseInfo target)
		{
			var result = string.Empty;
			using (var connection = new SqlConnection(target.ConnectionString))
			{
				var server = new Server(new ServerConnection(connection));				
				try
				{
					result = server.Information.MasterDBPath;
				}
				catch
				{
					result = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				}
			}
			return result;
		}
		
	}
}
