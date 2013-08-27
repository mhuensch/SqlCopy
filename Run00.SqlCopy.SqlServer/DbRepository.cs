using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer
{
	public class DbRepository : DynamicDbContext, IDbRepository
	{
		public DbRepository(DatabaseInfo info, IEnumerable<Type> entityTypes) : base(info, entityTypes) { }

		IQueryable<T> IDbRepository.GetEntities<T>()
		{
			return this.Set<T>();
		}

		IQueryable IDbRepository.GetEntities(Type type)
		{
			return this.Set(type);
		}
	}
}
