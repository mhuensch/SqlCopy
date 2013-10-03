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
	public class SchemaCopy : Transfer, ISchemaCopy
	{
		void ISchemaCopy.CopySchema(DatabaseInfo source, DatabaseInfo target)
		{
			DropTargetDatabseIfExists(target);
			CreateDatabase(target);
			CopySourceDatabaseToTarget(source, target);
		}

		private static void CreateDatabase(DatabaseInfo location)
		{
			var cb = new SqlConnectionStringBuilder(location.ConnectionString);
			cb.InitialCatalog = string.Empty;
			var connStr = cb.ToString();
			using (var connection = new SqlConnection(connStr))
			{
				try
				{		
					connection.Open();
					var server = new Server(new ServerConnection(connection));
					server.Refresh();
					var newDb = new Database(server, location.Database);
					newDb.Create();
					server.Refresh();
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

					// db.Drop() doens't seem to close existing connections
					server.KillDatabase(location.Database);

					//db.AutoClose = true;
					//db.Drop();
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

		private void CopySourceDatabaseToTarget(DatabaseInfo source, DatabaseInfo target)
		{
			//Manually creating and opening connection because SMO with LocalDB has 
			//a bug in opening the connection when querying the database.
			using (var connection = new SqlConnection(source.ConnectionString))
			{
				try
				{
					var server = new Server(new ServerConnection(connection));
					//var transfer = new Transfer(server.Databases[source.Database]);
					this.Database = server.Databases[source.Database];

					this.DestinationServer = target.Server;
					this.DestinationDatabase = target.Database;

					this.CopyData = false;
					this.CopyAllObjects = false;

					this.CopyAllSchemas = true;
					this.CopyAllTables = true;
					this.Options.DriAll = true;
					this.CopyAllDefaults = true;
					//transfer.CopyAllViews = false;
					this.CopyAllViews = true;
					this.CopyAllSynonyms = true;
					this.CreateTargetDatabase = false;
					this.DropDestinationObjectsFirst = true;
					this.CopyAllStoredProcedures = true;
					this.CopyAllUserDefinedTypes = true;
					this.CopyAllUserDefinedTableTypes = true;
					this.CopyAllUserDefinedFunctions = true;
					this.CopyAllUserDefinedDataTypes = true;
					this.CopyAllUserDefinedAggregates = true;
					//this.CopyAllSequences = true;
					this.CopyAllDatabaseTriggers = true;
					this.CopyAllXmlSchemaCollections = true;

					this.Options.ContinueScriptingOnError = true;

					//transfer.CopyAllUserDefinedFunctions = true;
					//transfer.CopyAllUserDefinedTypes = true;

					this.TargetDatabaseFilePath = GetServerDirectory(target, server);
					RemapServerFiles(server, this, source, target);
					//this.BulkCopyTimeout = 10000;

					//this.Scripter.FilterCallbackFunction = new ScriptingFilter(x =>
					//{
					//	Console.WriteLine(x.Value);
					//	return true;
					//});
					
					var script = this.ScriptTransfer().Cast<string>();
					script = script.Except(script.Where(s => s.Contains("NOC1GENPRPT03")));
					File.WriteAllLines(@"C:\temp\xfer.sql", script);

					using (var targetConnection = new SqlConnection(target.ConnectionString))
					{
						var targetServer = new Server(new ServerConnection(targetConnection));
						var db = targetServer.Databases[target.Database];
						foreach(var sql in script)
						{
							Console.WriteLine("Executing {0}", sql);
							try
							{
								db.ExecuteNonQuery(sql);
							}
							catch (Exception e)
							{
								Console.WriteLine(e.Message);
							}
						}
					}

					//this.TransferData();
				}
				finally
				{
					connection.Close();
				}
			}
		}

		private static void RemapServerFiles(Server server, Transfer transfer, DatabaseInfo source, DatabaseInfo target)
		{
			var dir = GetServerDirectory(target, server);
			var dataFiles = server.Databases[source.Database].FileGroups.Cast<FileGroup>().SelectMany(fg => fg.Files.Cast<DataFile>()).ToList();
			var dataFile = dataFiles.Select(f => f.FileName);
			foreach (var file in dataFile)
				transfer.DatabaseFileMappings.Add(file, Path.Combine(dir, Path.GetFileName(file).Replace(source.Database, target.Database)));

			var logFiles = server.Databases[source.Database].LogFiles.Cast<LogFile>().ToList();
			var logFile = logFiles.Select(f => f.FileName);
			foreach (var file in logFile)
				transfer.DatabaseFileMappings.Add(file, Path.Combine(dir, Path.GetFileName(file).Replace(source.Database, target.Database)));
		}

		private static string GetServerDirectory(DatabaseInfo target, Server server)
		{
			var result = string.Empty;
			//using (var connection = new SqlConnection(target.ConnectionString))
			//{
			//	var server = new Server(new ServerConnection(connection));				
				try
				{
					result = server.Information.MasterDBPath;
				}
				catch
				{
					result = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				}
			//}
			return result;
		}

	}
}
