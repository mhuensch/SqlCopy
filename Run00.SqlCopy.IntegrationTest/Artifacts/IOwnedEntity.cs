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
}
