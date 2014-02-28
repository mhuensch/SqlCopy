using System;

namespace Run00.SqlCopySchema
{
	public class Column
	{
		public string Name { get; set; }
		public string Table { get; set; }
		public Type Type { get; set; }
		public bool InPrimaryKey { get; set; }
		//public bool Nullable { get; set; }
	}
}
