using System.Collections.Generic;

namespace Run00.SqlCopySchema
{
	public sealed class Table
	{
		public string Name { get; set; }
		public string Schema { get; set; }
		public string Database { get; set; }
		public bool IsSystemObject { get; set; }
		public IEnumerable<Column> Columns { get; set; }
	}
}
