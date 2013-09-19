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

	public class TestTenantFilter : BaseEntityFilter<ITenantEntity>
	{
		public override IQueryable<ITenantEntity> Filter(IQueryable<ITenantEntity> entities)
		{
			var result = entities.Where(o => o.tenant_id == 334);
			return result;
		}
	}

	public class IdFilter : BaseEntityFilter<IIdEntity>
	{
		public override IQueryable<IIdEntity> Filter(IQueryable<IIdEntity> entities)
		{
			var result = entities.Where(x => x.id == 334);
			return result;
		}
	}

	public class IgnoreTableFilter : BaseEntityFilter<IIgnoreTable>
	{
		public override IQueryable<IIgnoreTable> Filter(IQueryable<IIgnoreTable> entities)
		{
			return entities.Where(x => 1 == 0);
		}
	}

	public class PlatformTenantFilter : BaseEntityFilter<IGuidTenantIdEntity>
	{
		public override IQueryable<IGuidTenantIdEntity> Filter(IQueryable<IGuidTenantIdEntity> entities)
		{
			return entities.Where(e => e.TenantId.Equals(new Guid("45C5926B-E190-E111-83A6-00215A5BDA1C")));
		}
	}
}
