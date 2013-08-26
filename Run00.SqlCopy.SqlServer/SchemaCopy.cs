using Microsoft.SqlServer.Management.Smo;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class SchemaCopy : ISchemaCopy
	{
		void ISchemaCopy.CopySchema(DatabaseLocation source, DatabaseLocation target)
		{
			DropTargetDatabseIfExists(target);

			var server = new Server(source.Server);
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

			var dataFiles = server.Databases[source.Database].FileGroups.Cast<FileGroup>().SelectMany(fg => fg.Files.Cast<DataFile>()).Select(f => f.FileName);
			MapDatabaseFiles(transfer.DatabaseFileMappings, dataFiles, source.Database, target.Database);

			var logFiles = server.Databases[source.Database].LogFiles.Cast<LogFile>().Select(f => f.FileName);
			MapDatabaseFiles(transfer.DatabaseFileMappings, logFiles, source.Database, target.Database);			

			transfer.TransferData();
		}

		private static void DropTargetDatabseIfExists(DatabaseLocation location)
		{
			var server = new Server(location.Server);
			var db = server.Databases[location.Database];
			
			if (db == null)
				return;

			db.Drop();
		}

		private static void MapDatabaseFiles(DatabaseFileMappingsDictionary dictionary, IEnumerable<string> sourceFiles, string sourceDb, string targetDb)
		{
			foreach (var file in sourceFiles)
				dictionary.Add(file, file.Replace(sourceDb, targetDb));
		}
	}
}
