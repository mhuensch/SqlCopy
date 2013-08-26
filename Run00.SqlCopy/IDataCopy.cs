using System.Collections.Generic;

namespace Run00.SqlCopy
{
	public interface IDataCopy
	{
		void CopyData(DatabaseLocation source, DatabaseLocation target, IEnumerable<CopyParameter> copyParams);
	}
}
