using Run00.SqlCopy;
using System;
using System.Linq;

namespace Run00.SqlCopy.IntegrationTest
{
	public class TestFilter : BaseEntityQueryFilter<IOwnedEntity>
	{
		public override IQueryable<IOwnedEntity> Filter(IQueryable<IOwnedEntity> query)
		{
			var guid = Guid.Parse("63BDDD01-D781-4064-83DE-18A3DDAAF178");
			var result = query.Where(o => o.OwnerId == guid);
			return result;
		}
	}
}
