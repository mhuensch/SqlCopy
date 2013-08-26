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
			var server = new Server(source.Server);
			var transfer = new Transfer(server.Databases[source.Database]);
			var dataFolder = Path.GetDirectoryName(server.Databases[source.Database].FileGroups[0].Files[0].FileName);
			var targetFileName = Path.Combine(dataFolder, target.Database + ".mdf");

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
			//transfer.CopyAllUserDefinedFunctions = true;
			//transfer.CopyAllUserDefinedTypes = true;
			transfer.CreateTargetDatabase = true;
			transfer.TargetDatabaseFilePath = targetFileName;
			transfer.DropDestinationObjectsFirst = true;

			transfer.TransferData();
		}
	}
}
