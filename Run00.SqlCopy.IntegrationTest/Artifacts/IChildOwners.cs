using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Run00.SqlCopy.IntegrationTest.Artifacts
{
	public interface IChildOwners
	{
		Guid Id { get; set; }
		Guid OwnerId { get; set; }
	}
}
