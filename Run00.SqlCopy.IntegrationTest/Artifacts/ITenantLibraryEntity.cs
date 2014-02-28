using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy.IntegrationTest.Artifacts
{
	public interface ITenantLibraryEntity
	{
		int Id { get; set; }
		int Tenant_Id { get; set; }
	}

	public interface ILibraryEntity
	{
		int Library_Id { get; set; }
	}
}
