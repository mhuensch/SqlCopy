using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopyDynamicContext
{
	public class DbRepositoryFactory : IDbRepositoryFactory
	{
		public DbRepositoryFactory(IQueryProviderFactory queryProviderFactory)
		{
			_queryProviderFactory = queryProviderFactory;
		}

		IDbRepository IDbRepositoryFactory.Create(DatabaseInfo info, IEnumerable<Type> entityTypes)
		{
			return new DbRepository(_queryProviderFactory.Create(info), entityTypes);
		}

		private readonly IQueryProviderFactory _queryProviderFactory;
	}

}
