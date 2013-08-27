using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public interface IDbRepository
	{
		IQueryable<T> GetEntities<T>() where T : class;
		IQueryable GetEntities(Type type);
	}
}
