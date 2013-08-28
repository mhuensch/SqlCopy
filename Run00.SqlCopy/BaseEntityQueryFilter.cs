using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	/// <summary>
	/// Abstract class used to create query filters for entities. This class is intended to 
	/// be used with the implementation of <seealso cref="Run00.SqlCopy.IDataCopy"/>
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public abstract class BaseEntityQueryFilter<T> : IEntityQueryFilter
	{
		Type IEntityQueryFilter.EntityType { get { return typeof(T); } }

		IQueryable IEntityQueryFilter.Filter(IQueryable query, IDbRepository context)
		{
			if (typeof(T).IsAssignableFrom(query.ElementType))
				return Filter(query.Cast<T>(), context);

			return query;
		}

		public abstract IQueryable<T> Filter(IQueryable<T> query, IDbRepository context);
	}
}
