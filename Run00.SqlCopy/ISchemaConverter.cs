using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public interface ISchemaConverter
	{
		IEnumerable<Type> ToEntityTypes(Database database);
	}
}
