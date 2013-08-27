using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Run00.SqlCopy
{
	public interface IEntityInterfaceLocator
	{
		IEnumerable<Type> GetInterfacesForEntity(string entityName);
	}
}
