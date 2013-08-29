using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopy
{
	public interface IDbRepositoryFactory
	{
		IDbRepository Create(DatabaseInfo info, IEnumerable<Type> entityTypes);
	}
}
