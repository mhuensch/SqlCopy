using Run00.SqlCopy;
using System;
using System.Linq;

namespace Run00.SqlCopy.IntegrationTest
{
	public class TestFilter : BaseEntityFilter<IOwnedEntity>
	{
		public override IQueryable<IOwnedEntity> Filter(IQueryable<IOwnedEntity> entities)
		{
			var guid = Guid.Parse("63BDDD01-D781-4064-83DE-18A3DDAAF178");
			var result = entities.Where(o => o.OwnerId == guid);
			return result;
		}
	}
}
