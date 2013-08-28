using System.Collections.Generic;

namespace Run00.SqlCopySchema
{
	public sealed class Database
	{
		public string Server { get; set; }
		public string Name { get; set; }
		public IEnumerable<Table> Tables { get; set; }
	}
}
