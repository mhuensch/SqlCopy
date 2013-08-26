using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy
{
	public interface ISchemaCopy
	{
		void CopySchema(DatabaseLocation source, DatabaseLocation target);
	}
}
