using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Run00.SqlCopy;
using Run00.SqlCopySqlServer.IntegrationTest.Artifacts;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Run00.SqlCopySqlServer.IntegrationTest
{
	[TestClass]
	public class CopyDatabase
	{
		[TestInitialize]
		public void CreateSourceDatabase()
		{
			System.Data.Entity.Database.SetInitializer(new SourceContextInitializer());

			//Force database to initialize
			var context = new SourceContext(_sourceConnection);
			var samples = context.Samples.Where(s => s.Id == Guid.NewGuid());

			Locator.Initialize();
		}

		[TestMethod]
		public void TestMethod1()
		{
			var source = new DatabaseInfo(_sourceConnection);
			var target = new DatabaseInfo(_targetConnection);

			Locator.Test<ISchemaCopy>(a =>
				a.CopySchema(source, target)
			);

			Locator.Test<IDataCopy>(dc =>
				dc.CopyData(source, target, new[] { new CopyParameter() { Name = "OwnerId", Value = "63BDDD01-D781-4064-83DE-18A3DDAAF178" } })
			);

		}

		public const string _sourceConnection = @"Data Source=(localdb)\v11.0;Initial Catalog=SourceContext;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
		public const string _targetConnection = @"Data Source=(localdb)\v11.0;Initial Catalog=SourceContextToo;Integrated Security=True;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
	}


	public static class Locator
	{
		public static void Initialize()
		{
			var currentDir = Directory.GetCurrentDirectory();
			_container = new WindsorContainer();
			_container.Install(FromAssembly.InDirectory(new AssemblyFilter(currentDir)));
		}

		public static void Test<T>(Action<T> action)
		{
			var service = _container.Resolve<T>();
			action.Invoke(service);
			_container.Release(service);
		}

		private static WindsorContainer _container;
	}
}
