using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public interface ICommandBuilder
	{
		IDbCommand Build(DatabaseInfo database, IQueryable query);
	}
}
