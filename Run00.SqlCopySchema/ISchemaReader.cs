using Run00.SqlCopy;

namespace Run00.SqlCopySchema
{
	public interface ISchemaReader
	{
		Database GetSchema(DatabaseInfo info);
	}
}