using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	// TODO: come up with something much more general for this
	public class SchemaRewriteInfo
	{
		public string OldPrefix { get; set; }
		public string NewPrefix { get; set; }
	}
}
