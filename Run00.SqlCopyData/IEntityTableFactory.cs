using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Run00.SqlCopyData
{
	public interface IEntityTableFactory
	{
		DataTable Create(IQueryable source);
	}
}
