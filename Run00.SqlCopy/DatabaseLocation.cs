using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public class DatabaseLocation
	{
		public string Server { get; private set; }
		public string Database { get; private set; }

		public DatabaseLocation(string server, string database)
		{
			Contract.Requires(string.IsNullOrWhiteSpace(server) == false);
			Contract.Requires(string.IsNullOrWhiteSpace(database) == false);

			Contract.Ensures(string.IsNullOrWhiteSpace(server) == false);
			Contract.Ensures(string.IsNullOrWhiteSpace(database) == false);

			Server = server;
			Database = database;
		}
	}
}
