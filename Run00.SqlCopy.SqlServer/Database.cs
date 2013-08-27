using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class Database
	{
		public string Server { get; set; }
		public string Name { get; set; }
		public IEnumerable<Table> Tables { get; set; }
	}
}
