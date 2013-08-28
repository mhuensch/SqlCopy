using Microsoft.Samples.EntityDataReader;
using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopyData
{
	public class DataReaderFactory : IDataReaderFactory
	{
		IDataReader IDataReaderFactory.Create(IQueryable queryable)
		{
			var type = queryable.ElementType;
			var constructor = typeof(EntityDataReader<>).MakeGenericType(new Type[] { type }).GetConstructors().First();
			return constructor.Invoke(new object[] { queryable }) as IDataReader;
		}
	}
}
