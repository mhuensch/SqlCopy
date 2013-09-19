using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Run00.SqlCopy
{
	public class QueryParameter
	{
		public string Name { get; set; }
		public Type Type { get; set; }
		public object Value { get; set; }
	}
}
