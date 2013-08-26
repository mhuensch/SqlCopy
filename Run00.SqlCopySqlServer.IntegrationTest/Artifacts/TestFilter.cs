using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer.IntegrationTest
{
	public class TestFilter<T> : BaseEntityQueryFilter<T>
	{
		public override IEnumerable<T> Filter(IQueryable<T> query, IDbContext context)
		{
			return query;
		}
	}
}
