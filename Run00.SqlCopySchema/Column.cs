using System;

namespace Run00.SqlCopySchema
{
	public sealed class Column
	{
		public string Name { get; set; }
		public string Table { get; set; }
		public Type Type { get; set; }
	}
}
