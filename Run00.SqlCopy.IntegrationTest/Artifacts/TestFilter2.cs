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

	public class LinkTableFilter : BaseReleatedEntityFilter<ILinkEntity, ITenantAndIdEntity>
	{
		// NOTE: this is the _Type_ name, not the table name
		public override string ReleatedEntityName { get { return "dbo.user"; } }

		public override IQueryable<ILinkEntity> Filter(IQueryable<ILinkEntity> entities, IQueryable<ITenantAndIdEntity> releatedEntities)
		{
			/*
				select * from user_group_user ugu
				inner join [user] u
				on u.id = ugu.user_id
				where u.tenant_id = 335
			 */
			var result = from x in entities
						 join y in releatedEntities on x.user_id equals y.id
						 where y.tenant_id == 334
						 select x;
			return result;
		}
	}

	public interface ITenantAndIdEntity
	{
		int id { get; set; }
		int tenant_id { get; set; }
	}

	public interface ILinkEntity
	{
		int user_id { get; set; }
	}

}
