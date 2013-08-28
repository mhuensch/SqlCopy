using System;
using System.Collections.Generic;

namespace Run00.SqlCopy
{
	public interface IDbRepositoryFactory
	{
		IDbRepository Create(DatabaseInfo info, IEnumerable<Type> entityTypes);
	}
}
