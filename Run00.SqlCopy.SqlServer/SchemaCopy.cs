using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Microsoft.SqlServer.Management.Smo.Broker;

namespace Run00.SqlCopySqlServer
{
	public class SchemaCopy : Transfer, ISchemaCopy
	{
		private static void Log(string message)
		{
			File.AppendAllText(@"c:\temp\schemacopy.txt", message + Environment.NewLine);
		}

		void ISchemaCopy.CopySchema(DatabaseInfo source, DatabaseInfo target)
		{
			DropTargetDatabseIfExists(target);
			CreateDatabase(target);
			CopySourceDatabaseToTarget(source, target);
		}

		void ISchemaCopy.CopySchema(DatabaseInfo source, DatabaseInfo target, SchemaRewriteInfo rewriteInfo)
		{
			this._rewriteInfo = rewriteInfo;
			DropTargetDatabseIfExists(target);
			CreateDatabase(target);
			CopySourceDatabaseToTarget(source, target);
		}

		private static void CreateDatabase(DatabaseInfo location)
		{
			Log("creating target");
			var cb = new SqlConnectionStringBuilder(location.ConnectionString);
			cb.InitialCatalog = string.Empty;
			var connStr = cb.ToString();
			using (var connection = new SqlConnection(connStr))
			{
				try
				{		
					connection.Open();
					Log("Connected");
					var server = new Server(new ServerConnection(connection));
					server.Refresh();
					var newDb = new Database(server, location.Database);
					newDb.Create();
					server.Refresh();
					Log("Created database");
				}
				catch (SqlException ex)
				{
					Log(String.Format("Caught exception creating target {0}", ex.Message));
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
			Log("done creating target");
		}

		private static void DropTargetDatabseIfExists(DatabaseInfo location)
		{
			Log("dropping target");
			//Manually creating and opening connection because SMO with LocalDB has 
			//a bug in opening the connection when querying the database.
			using (var connection = new SqlConnection(location.ConnectionString))
			{
				try
				{
					connection.Open();
					Log("Opened connection");
					var server = new Server(new ServerConnection(connection));
					var db = server.Databases[location.Database];


					if (db == null)
						return;

					// db.Drop() doens't seem to close existing connections
					server.KillDatabase(location.Database);
				}
				catch (SqlException ex)
				{
					Log(String.Format("Caught exception {0}", ex.Message));
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
			Log("Done dropping target");
		}

		private void CopySourceDatabaseToTarget(DatabaseInfo source, DatabaseInfo target)
		{
			Log("copying source schema to target");
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

					var script = this.ScriptTransfer().Cast<string>();
					script = script.Except(script.Where(s => s.Contains("NOC1GENPRPT03")));
					File.WriteAllLines(@"C:\temp\xfer.sql", script);

					// script service broker stuff
					var messageTypes = new List<string>();
					foreach(var m in this.Database.ServiceBroker.MessageTypes)
					{
						var mt = (MessageType)m;
						if (mt.IsSystemObject)
							continue;
						var messageScript = mt.Script().Cast<string>();
						messageTypes.AddRange(messageScript);
					}
					var serviceContracts = new List<string>();
					foreach (var sc in this.Database.ServiceBroker.ServiceContracts)
					{
						var c = (ServiceContract)sc;
						if (c.IsSystemObject)
							continue;
						var contractScript = c.Script().Cast<string>();
						serviceContracts.AddRange(contractScript);
					}
					var queues = new List<string>();
					foreach(var q in this.Database.ServiceBroker.Queues)
					{
						var queue = (ServiceQueue)q;
						if (queue.IsSystemObject)
							continue;
						var queueScript = queue.Script().Cast<string>();
						queues.AddRange(queueScript);
					}
					var services = new List<string>();
					foreach (var sc in this.Database.ServiceBroker.Services)
					{
						var c = (BrokerService)sc;
						if (c.IsSystemObject)
							continue;
						var contractScript = c.Script().Cast<string>();
						services.AddRange(contractScript);
					}
					//var serviceContracts = (from s in this.Database.ServiceBroker.ServiceContracts.Cast<ServiceContract>()
					//		  select s.Script()).Cast<string>();
					//var services = (from s in this.Database.ServiceBroker.Services.Cast<BrokerService>()
					//			   select s.Script()).Cast<string>();
					script = script.Concat(messageTypes).Concat(serviceContracts).Concat(queues).Concat(services);

					using (var targetConnection = new SqlConnection(target.ConnectionString))
					{
						var targetServer = new Server(new ServerConnection(targetConnection));
						var db = targetServer.Databases[target.Database];
						foreach(var sql in script)
						{
							try
							{
								db.ExecuteNonQuery(RewriteSql(sql, source.Database, target.Database));
							}
							catch (Exception e)
							{
								Console.WriteLine(e.Message);
								Log("Exception executing sql");
								Log(e.Message);
								Log(e.InnerException.InnerException.Message);
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
			Log("done copying source to target");
		}

		private string RewriteSql(string sql, string sourceDatabase, string targetDatabase)
		{
            // hack
            if (sql.Contains("CREATE FUNCTION [dbo].[fn_SplitIntsFromXml](@input varchar(max))"))
            {
                sql = "SET QUOTED_IDENTIFIER ON\r\nGO\r\n" + sql;
            }
			//if (_rewriteInfo == null)
			//	return sql;
			//return sql.ToLower().Replace(_rewriteInfo.OldPrefix.ToLower(), _rewriteInfo.NewPrefix.ToLower());
			//return sql.Replace(_rewriteInfo.OldPrefix, _rewriteInfo.NewPrefix);
			// replace any references to the source database name with the target
			return sql.Replace(sourceDatabase, targetDatabase);
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

		private SchemaRewriteInfo _rewriteInfo;
	}
}
