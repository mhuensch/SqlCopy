using Run00.SqlCopy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer.IntegrationTest.Artifacts
{
	public class EntityInterfaceLocator : IEntityInterfaceLocator
	{
		IEnumerable<Type> IEntityInterfaceLocator.GetInterfacesForEntity(string entityName)
		{
			if (string.Equals(entityName, "Samples", StringComparison.InvariantCultureIgnoreCase) || string.Equals(entityName, "SampleChilds", StringComparison.InvariantCultureIgnoreCase))
				return new[] { typeof(IOwnedEntity) };

			return Enumerable.Empty<Type>();
		}
	}
}
