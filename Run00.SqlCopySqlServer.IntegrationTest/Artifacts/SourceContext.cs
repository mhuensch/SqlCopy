﻿using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Run00.SqlCopySqlServer.IntegrationTest.Artifacts
{
	public class SourceContext : DbContext
	{
		public SourceContext(string connection) : base(connection) { }

		public DbSet<Sample> Samples { get; set; }
		public DbSet<SampleChild> SampleChildren { get; set; }
	}

	public class Sample
	{
		public Guid Id { get; set; }
		public string Value { get; set; }
		public ICollection<SampleChild> Children { get; set; }
		public Guid OwnerId { get; set; }
	}

	public class SampleChild
	{
		public Guid Id { get; set; }
		public string Value { get; set; }
		public Sample Parent { get; set; }
		public Guid OwnerId { get; set; }
	}
}