using System.Data;
using System.Linq;

namespace Run00.SqlCopyData
{
	public interface IEntityTableFactory
	{
		DataTable Create(IQueryable source);
	}
}