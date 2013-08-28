using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopyDynamicContext
{
	public class DbRepository : IDbRepository
	{
		public DbRepository(DynamicDbContext context)
		{
			_context = context;
		}

		IQueryable<T> IDbRepository.GetEntities<T>()
		{
			return _context.Set<T>();
		}

		IQueryable IDbRepository.GetEntities(Type type)
		{
			return _context.Set(type);
		}

		private DynamicDbContext _context;
	}
}
