using Run00.DynamicEf;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace Run00.SqlCopySqlServer
{
	public class ContextFactory : IContextFactory
	{
		IDbContext IContextFactory.CreateContext(IEnumerable<Type> entityTypes)
		{
			return new Context(entityTypes);
		}
	}
}
