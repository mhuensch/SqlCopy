using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopySqlServer
{
	public class ContextFactory : IContextFactory
	{
		IDbRepository IContextFactory.CreateContext(DatabaseInfo info, IEnumerable<Type> entityTypes)
		{
			System.Data.Entity.Database.SetInitializer<DbRepository>(null);
			return new DbRepository(info, entityTypes);
		}
	}
}
