using Run00.SqlCopy;
using System;
using System.Linq;
using Run00.SqlCopy.IntegrationTest.Artifacts;

namespace Run00.SqlCopy.IntegrationTest
{
	public class TestFilter2 : BaseEntityQueryFilter<IChildOwnedEntity>
	{
		public override IQueryable<IChildOwnedEntity> Filter(IQueryable<IChildOwnedEntity> query)
		{
			var guid = Guid.Parse("63BDDD01-D781-4064-83DE-18A3DDAAF178");
			IQueryable<IChildOwners> owners = default(IQueryable<IChildOwners>);
			query.Where(i => owners.Any(o => o.Id.Equals(i.Parent_Id) && o.Id.Equals(guid) ));
			return query;
		}
	}
}
