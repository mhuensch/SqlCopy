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
		public EntityInterfaceLocator()
		{
			_filterMap = new Dictionary<string, IEnumerable<Type>>();
			_filterMap.Add("Samples", new[] { typeof(IOwnedEntity) });
			_filterMap.Add("SampleChilds", new[] { typeof(IOwnedEntity) });
		}

		IEnumerable<Type> IEntityInterfaceLocator.GetInterfacesForEntity(string entityName)
		{
			if (_filterMap.ContainsKey(entityName) == false)
				return Enumerable.Empty<Type>();

			return _filterMap[entityName];
		}

		private Dictionary<string, IEnumerable<Type>> _filterMap;
	}
}
