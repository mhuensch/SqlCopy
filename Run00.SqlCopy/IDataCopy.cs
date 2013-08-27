
namespace Run00.SqlCopy
{
	public interface IDataCopy
	{
		void CopyData(DatabaseInfo source, DatabaseInfo target);
	}
}
