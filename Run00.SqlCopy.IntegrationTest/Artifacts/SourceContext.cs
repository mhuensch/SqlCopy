using System;
using System.Collections.Generic;
using System.Data.Entity;

namespace Run00.SqlCopy.IntegrationTest
{
	public class SourceContext : DbContext
	{
		public SourceContext(string connection) : base(connection) { }

		public DbSet<Sample> Samples { get; set; }
		public DbSet<SampleChild> SampleChildren { get; set; }
		public DbSet<SampleGrandChild> SampleGrandChilds { get; set; }
		public DbSet<Tenant> Tenant { get; set; }
		public DbSet<Library> Library { get; set; }
		public DbSet<LibraryPermission> LibraryPermission { get; set; }
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
		public Guid OwnerId { get; set; }
		public Sample Parent { get; set; }
		public ICollection<SampleGrandChild> Children { get; set; }
	}

	public class SampleGrandChild
	{
		public Guid Id { get; set; }
		public string Value { get; set; }
		public SampleChild Parent { get; set; }
	}

	public class Tenant
	{
		public int Id { get; set; }
		public ICollection<Library> Libraries { get; set; }
	}

	public class Library
	{
		public int Id { get; set; }
		public Tenant Tenant { get; set; }
		public ICollection<LibraryPermission> LibraryPermissions { get; set; }
	}

	public class LibraryPermission
	{
		public int Id { get; set; }
		public Library Library { get; set; }
	}
}
