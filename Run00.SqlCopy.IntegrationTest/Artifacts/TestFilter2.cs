using Run00.SqlCopy;
using System;
using System.Linq;
using Run00.SqlCopy.IntegrationTest.Artifacts;

namespace Run00.SqlCopy.IntegrationTest
{
	public class TestFilter2 : BaseReleatedEntityFilter<IChildOwnedEntity, IChildOwners>
	{
		public override string ReleatedEntityName { get { return "dbo.SampleChilds"; } }

		public override IQueryable<IChildOwnedEntity> Filter(IQueryable<IChildOwnedEntity> entities, IQueryable<IChildOwners> releatedEntities)
		{
			var guid = Guid.Parse("63bddd01-d781-4064-83de-18a3ddaaf178");
			entities =
				from x in entities
				join y in releatedEntities on x.Parent_Id equals y.Id
				where y.OwnerId.Equals(guid)
				select x;
			return entities;
		}
	}
}
