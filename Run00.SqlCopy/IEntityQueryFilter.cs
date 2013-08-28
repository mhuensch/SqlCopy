using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	[ContractClass(typeof(EntityQueryFilterContractClass))]
	public interface IEntityQueryFilter
	{
		Type EntityType { get; }
		IQueryable Filter(IQueryable query, IDbRepository context);
	}

	[ContractClassFor(typeof(IEntityQueryFilter))]
	internal abstract class EntityQueryFilterContractClass : IEntityQueryFilter
	{

		Type IEntityQueryFilter.EntityType
		{
			get { throw new NotImplementedException(); }
		}

		IQueryable IEntityQueryFilter.Filter(IQueryable query, IDbRepository context)
		{
			Contract.Requires(query != null);
			Contract.Requires(context != null);

			throw new NotImplementedException();
		}
	}
}
