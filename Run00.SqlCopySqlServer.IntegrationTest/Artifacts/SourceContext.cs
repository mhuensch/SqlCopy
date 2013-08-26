using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Run00.SqlCopySqlServer.IntegrationTest.Artifacts
{
	public class SourceContext : DbContext
	{
		public SourceContext() : base("SourceContext") { }

		public DbSet<Sample> Samples { get; set; }
		public DbSet<SampleChild> SampleChildren { get; set; }
	}

	public class Sample
	{
		public Guid Id { get; set; }
		public string Value { get; set; }
		public ICollection<SampleChild> Children { get; set; }
	}

	public class SampleChild
	{
		public Guid Id { get; set; }
		public string Value { get; set; }
		public Sample Parent { get; set; }
	}
}
