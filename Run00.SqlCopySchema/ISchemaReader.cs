using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySchema
{
	public interface ISchemaReader
	{
		Database GetSchema(DatabaseInfo location);
	}
}