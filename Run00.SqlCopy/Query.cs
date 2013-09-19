using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Run00.SqlCopy
{
	public class Query
	{
		public string Sql { get; set; }
		public IEnumerable<QueryParameter> Parameters { get; set; }
	}
}
