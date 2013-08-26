using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public abstract class BaseEntityQueryFilter<T> : IEntityQueryFilter
	{
		Type IEntityQueryFilter.EntityType { get { return typeof(T); } }

		IQueryable IEntityQueryFilter.Filter(IQueryable query, IDbContext context)
		{
			return query;
		}

		public abstract IEnumerable<T> Filter(IQueryable<T> query, IDbContext context);
	}
}
