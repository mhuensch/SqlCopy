using Run00.SqlCopy;
using System;
using System.Linq;

namespace Run00.SqlCopySqlServer.IntegrationTest.Artifacts
{
	public class TestFilter : BaseEntityQueryFilter<IOwnedEntity>
	{
		public override IQueryable<IOwnedEntity> Filter(IQueryable<IOwnedEntity> query, IDbRepository context)
		{
			var guid = Guid.Parse("63BDDD01-D781-4064-83DE-18A3DDAAF178");
			var blah = Guid.NewGuid();
			var guidList = new[] { guid, blah };
			var result = query.Where(o => guidList.Contains(o.OwnerId));
			return result;
		}
	}
}
