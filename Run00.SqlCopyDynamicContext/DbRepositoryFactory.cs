using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopyDynamicContext
{
	public class DbRepositoryFactory : IDbRepositoryFactory
	{
		IDbRepository IDbRepositoryFactory.Create(DatabaseInfo info, IEnumerable<Type> entityTypes)
		{
			System.Data.Entity.Database.SetInitializer<DynamicDbContext>(null);
			var context = new DynamicDbContext(info, entityTypes);
			return new DbRepository(context);
		}
	}
}
