using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Run00.SqlCopy
{
	public abstract class BaseEntityQueryFilter<T> : IEntityQueryFilter
	{
		public abstract IQueryable<T> Filter(IQueryable<T> query);

		Type IEntityQueryFilter.EntityType { get { return typeof(T); } }

		IQueryable IEntityQueryFilter.Filter(IQueryable query)
		{
			var origElementType = query.ElementType;

			if (typeof(T).IsAssignableFrom(origElementType) == false)
				return query;

			var result = Filter((IQueryable<T>)query);
			var recastResult = ReCast(result, origElementType);
			return recastResult;
		}

		private static IQueryable ReCast(IQueryable source, Type toType)
		{
			if (castMethod == null)
				castMethod = typeof(Queryable).GetMethod("Cast");

			var typeCast = castMethod.MakeGenericMethod(new Type[] { toType });
			return typeCast.Invoke(null, new object[] { source }) as IQueryable;
		}

		private static MethodInfo castMethod;

		
	}
}
