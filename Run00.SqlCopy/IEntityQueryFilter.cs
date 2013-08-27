using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public interface IEntityQueryFilter
	{
		Type EntityType { get; }
		IQueryable Filter(IQueryable query, IDbRepository context);
	}
}
