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

		IQueryable IEntityQueryFilter.Filter(IQueryable query, IDbRepository context)
		{
			if (typeof(T).IsAssignableFrom(query.ElementType))
				return Filter((IQueryable<T>)query, context);

			return query;
		}

		public abstract IQueryable<T> Filter(IQueryable<T> query, IDbRepository context);
	}
}
