using Run00.SqlCopy;
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
			if (string.Equals(entityName, "dbo.Samples", StringComparison.InvariantCultureIgnoreCase) || string.Equals(entityName, "dbo.SampleChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity) };

			if (string.Equals(entityName, "dbo.SampleGrandChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IChildOwnedEntity) };

			return Enumerable.Empty<Type>();
		}
	}
}
