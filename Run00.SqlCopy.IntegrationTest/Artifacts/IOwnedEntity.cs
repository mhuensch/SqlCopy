using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy.IntegrationTest
{
	public interface IOwnedEntity
	{
		Guid OwnerId { get; set; }
	}

	public interface IIdEntity
	{
		int id { get; set; }
	}

	public interface ITenantEntity
	{
		int tenant_id { get; set; }
	}

	public interface IIgnoreTable
	{
	}

	public interface IGuidTenantIdEntity
	{
		Guid TenantId { get; set; }
	}
}
