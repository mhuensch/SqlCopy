using Run00.DynamicEf;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class Context : DynamicDbContext, IDbContext
	{
		public Context(IEnumerable<Type> entityTypes) : base(entityTypes) { }

		IQueryable<T> IDbContext.GetEntities<T>()
		{
			return this.Set<T>();
		}

		IQueryable IDbContext.GetEntities(Type type)
		{
			return this.Set(type);
		}
	}
}
