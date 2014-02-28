using Run00.SqlCopy;
using Run00.SqlCopy.IntegrationTest.Artifacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopy.IntegrationTest
{
	public class EntityInterfaceLocator : IEntityInterfaceLocator
	{
		IEnumerable<Type> IEntityInterfaceLocator.GetInterfacesForEntity(string entityName)
		{
			if (string.Equals(entityName, "dbo.Samples", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity) };

			if (string.Equals(entityName, "dbo.SampleChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity), typeof(IChildOwners) };

			if (string.Equals(entityName, "dbo.SampleGrandChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IChildOwnedEntity) };

			if (string.Equals(entityName, "dbo.tenant", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IIdEntity) };

			if (string.Equals(entityName, "dbo.Libraries"))
				return new[] { typeof(ITenantLibraryEntity) };

			//if(string.Equals(entityName, "dbo.user", StringComparison.InvariantCultureIgnoreCase))
			//if(_tenantTables.Any(x => x.Equals(entityName.Replace("dbo.", string.Empty))))
			//	return new[] { typeof(ITenantEntity) };

			//if (_platformTenantTables.Any(x => x.Equals(entityName)))
			//if(_platformGuidNonNull.Any(x => x.Equals(entityName)))
			//	return new[] { typeof(IGuidTenantIdEntity) };

			//if (string.Equals(entityName, "dbo.user_group_user"))
			//	return new[] { typeof(ILinkEntity) };
			if (string.Equals(entityName, "dbo.Libraries"))
				return new[] { typeof(ITenantLibraryEntity) };

			if(string.Equals(entityName, "dbo.LibraryPermissions"))
				return new[] { typeof(ILibraryEntity) };

			return new[] { typeof(IIgnoreTable) };
		}
	}
}