using Run00.SqlCopySchema;
using System;
using System.Collections.Generic;

namespace Run00.SqlCopySchema
{
	public interface ISchemaConverter
	{
		IEnumerable<Type> ToEntityTypes(Database database);
	}
}
